using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.Diagnostics;

namespace OculusGameManager.Utils
{
	/// <summary>
	/// Provides access to NTFS junction points in .Net.
	/// 
	/// Modified from: http://www.codeproject.com/Articles/15633/Manipulating-NTFS-Junction-Points-in-NET
	/// </summary>
	public class JunctionHandler
	{
		#region Constants
		/// <summary>
		/// The file or directory is not a reparse point.
		/// </summary>
		private const int ERROR_NOT_A_REPARSE_POINT = 4390;

		/// <summary>
		/// The reparse point attribute cannot be set because it conflicts with an existing attribute.
		/// </summary>
		private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;

		/// <summary>
		/// The data present in the reparse point buffer is invalid.
		/// </summary>
		private const int ERROR_INVALID_REPARSE_DATA = 4392;

		/// <summary>
		/// The tag present in the reparse point buffer is invalid.
		/// </summary>
		private const int ERROR_REPARSE_TAG_INVALID = 4393;

		/// <summary>
		/// There is a mismatch between the tag specified in the request and the tag present in the reparse point.
		/// </summary>
		private const int ERROR_REPARSE_TAG_MISMATCH = 4394;

		/// <summary>
		/// Command to set the reparse point data block.
		/// </summary>
		private const int FSCTL_SET_REPARSE_POINT = 0x000900A4;

		/// <summary>
		/// Command to get the reparse point data block.
		/// </summary>
		private const int FSCTL_GET_REPARSE_POINT = 0x000900A8;

		/// <summary>
		/// Command to delete the reparse point data base.
		/// </summary>
		private const int FSCTL_DELETE_REPARSE_POINT = 0x000900AC;

		/// <summary>
		/// Reparse point tag used to identify mount points and junction points.
		/// </summary>
		private const uint IO_REPARSE_TAG_MOUNT_POINT = 0xA0000003;

		/// <summary>
		/// This prefix indicates to NTFS that the path is to be treated as a non-interpreted
		/// path in the virtual file system.
		/// </summary>
		private const string NonInterpretedPathPrefix = @"\??\";

		[Flags]
		private enum EFileAccess : uint
		{
			GenericRead = 0x80000000,
			GenericWrite = 0x40000000,
			GenericExecute = 0x20000000,
			GenericAll = 0x10000000,
		}

		[Flags]
		private enum EFileShare : uint
		{
			None = 0x00000000,
			Read = 0x00000001,
			Write = 0x00000002,
			Delete = 0x00000004,
		}

		private enum ECreationDisposition : uint
		{
			New = 1,
			CreateAlways = 2,
			OpenExisting = 3,
			OpenAlways = 4,
			TruncateExisting = 5,
		}

		[Flags]
		private enum EFileAttributes : uint
		{
			Readonly = 0x00000001,
			Hidden = 0x00000002,
			System = 0x00000004,
			Directory = 0x00000010,
			Archive = 0x00000020,
			Device = 0x00000040,
			Normal = 0x00000080,
			Temporary = 0x00000100,
			SparseFile = 0x00000200,
			ReparsePoint = 0x00000400,
			Compressed = 0x00000800,
			Offline = 0x00001000,
			NotContentIndexed = 0x00002000,
			Encrypted = 0x00004000,
			Write_Through = 0x80000000,
			Overlapped = 0x40000000,
			NoBuffering = 0x20000000,
			RandomAccess = 0x10000000,
			SequentialScan = 0x08000000,
			DeleteOnClose = 0x04000000,
			BackupSemantics = 0x02000000,
			PosixSemantics = 0x01000000,
			OpenReparsePoint = 0x00200000,
			OpenNoRecall = 0x00100000,
			FirstPipeInstance = 0x00080000
		}
		#endregion Constants

		#region P/Invoke stuff
		[StructLayout(LayoutKind.Sequential)]
		private struct REPARSE_DATA_BUFFER
		{
			/// <summary>
			/// Reparse point tag. Must be a Microsoft reparse point tag.
			/// </summary>
			public uint ReparseTag;

			/// <summary>
			/// Size, in bytes, of the data after the Reserved member. This can be calculated by:
			/// (4 * sizeof(ushort)) + SubstituteNameLength + PrintNameLength + 
			/// (namesAreNullTerminated ? 2 * sizeof(char) : 0);
			/// </summary>
			public ushort ReparseDataLength;

			/// <summary>
			/// Reserved; do not use. 
			/// </summary>
			public ushort Reserved;

			/// <summary>
			/// Offset, in bytes, of the substitute name string in the PathBuffer array.
			/// </summary>
			public ushort SubstituteNameOffset;

			/// <summary>
			/// Length, in bytes, of the substitute name string. If this string is null-terminated,
			/// SubstituteNameLength does not include space for the null character.
			/// </summary>
			public ushort SubstituteNameLength;

			/// <summary>
			/// Offset, in bytes, of the print name string in the PathBuffer array.
			/// </summary>
			public ushort PrintNameOffset;

			/// <summary>
			/// Length, in bytes, of the print name string. If this string is null-terminated,
			/// PrintNameLength does not include space for the null character. 
			/// </summary>
			public ushort PrintNameLength;

			/// <summary>
			/// A buffer containing the unicode-encoded path string. The path string contains
			/// the substitute name string and print name string.
			/// </summary>
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x3FF0)]
			public byte[] PathBuffer;
		}

		private static class NativeMethods
		{
			[DllImport("kernel32.dll", SetLastError = true)]
			public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode,
				IntPtr InBuffer, int nInBufferSize,
				IntPtr OutBuffer, int nOutBufferSize,
				out int pBytesReturned, IntPtr lpOverlapped);

			[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern IntPtr CreateFile(
				string lpFileName,
				EFileAccess dwDesiredAccess,
				EFileShare dwShareMode,
				IntPtr lpSecurityAttributes,
				ECreationDisposition dwCreationDisposition,
				EFileAttributes dwFlagsAndAttributes,
				IntPtr hTemplateFile);
		}
		#endregion P/Invoke stuff

		/// <summary>
		/// Creates a junction point from the specified directory to the specified target directory.
		/// </summary>
		/// <remarks>
		/// Only works on NTFS.
		/// </remarks>
		/// <param name="junctionPoint">The junction point path</param>
		/// <param name="targetDir">The target directory</param>
		/// <param name="moveFiles">If true, moves any existing directory contents over the target (overwrites target)</param>
		/// <exception cref="IOException">Thrown when the junction point could not be created or when
		/// an existing directory was found and <paramref name="overwrite" /> if false</exception>
		public void Create(string junctionPoint, string targetDir, bool moveFiles = true)
		{
			targetDir = Path.GetFullPath(targetDir);

			if (Directory.Exists(junctionPoint))
			{
				// Fix false-positive
				if (Directory.GetDirectories(junctionPoint, "*.*", SearchOption.AllDirectories).Length + Directory.GetFiles(junctionPoint, "*.*", SearchOption.AllDirectories).Length == 0)
				{
					Directory.Delete(junctionPoint);
				}
			}
			if (Directory.Exists(junctionPoint))
			{
				if (moveFiles)
				{
					// Let it fail on IO error
					Directory.Move(junctionPoint, targetDir);
				}
				else
				{
					throw new IOException("Junction point already exists, without move command.");
				}
			}

			if (!Directory.Exists(targetDir))
			{
				// Let it fail on IO error
				Directory.CreateDirectory(targetDir);
			}

			// Try without admin
			var p = new Process();
			p.StartInfo.FileName = "cmd";
			p.StartInfo.Arguments = String.Format("/c mklink /j \"{0}\" \"{1}\"", junctionPoint, targetDir);
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.CreateNoWindow = true;
			p.Start();
			p.WaitForExit();

			if (!Directory.Exists(junctionPoint))
			{
				// Try again with admin
				p = new Process { StartInfo = p.StartInfo };
				p.StartInfo.Verb = "runas";
				p.Start();
				p.WaitForExit();
			}
		}

		/// <summary>
		/// Deletes a junction point at the specified source directory along with the directory itself.
		/// Does nothing if the junction point does not exist.
		/// </summary>
		/// <remarks>
		/// Only works on NTFS.
		/// </remarks>
		/// <param name="junctionPoint">The junction point path</param>
		/// <param name="moveFilesBack">If true, recreates junction target at junction point</param>
		public void Remove(string junctionPoint, bool moveFilesBack = true)
		{
			if (!Directory.Exists(junctionPoint))
			{
				if (File.Exists(junctionPoint))
					throw new IOException("Path is not a junction point.");

				return;
			}

			using (SafeFileHandle handle = openReparsePoint(junctionPoint, EFileAccess.GenericWrite))
			{
				REPARSE_DATA_BUFFER reparseDataBuffer = new REPARSE_DATA_BUFFER();

				reparseDataBuffer.ReparseTag = IO_REPARSE_TAG_MOUNT_POINT;
				reparseDataBuffer.ReparseDataLength = 0;
				reparseDataBuffer.PathBuffer = new byte[0x3ff0];

				int inBufferSize = Marshal.SizeOf(reparseDataBuffer);
				IntPtr inBuffer = Marshal.AllocHGlobal(inBufferSize);
				try
				{
					Marshal.StructureToPtr(reparseDataBuffer, inBuffer, false);

					int bytesReturned;
					bool result = NativeMethods.DeviceIoControl(handle.DangerousGetHandle(), FSCTL_DELETE_REPARSE_POINT,
						inBuffer, 8, IntPtr.Zero, 0, out bytesReturned, IntPtr.Zero);

					if (!result)
						throwLastWin32Error("Unable to delete junction point.");
				}
				finally
				{
					Marshal.FreeHGlobal(inBuffer);
				}

				try
				{
					Directory.Delete(junctionPoint);
				}
				catch (IOException ex)
				{
					throw new IOException("Unable to delete junction point.", ex);
				}
			}
		}

		/// <summary>
		/// Determines whether the specified path refers to a junction point.
		/// </summary>
		/// <param name="path">The junction point path</param>
		/// <returns>True if the specified path represents a junction point</returns>
		/// <exception cref="IOException">Thrown if the specified path is invalid
		/// or some other error occurs</exception>
		public bool IsJunction(string path)
		{
			if (!Directory.Exists(path))
			{
				return false;
			}

			using (var handle = openReparsePoint(path, EFileAccess.GenericRead))
			{
				var target = internalGetTarget(handle);
				return target != null;
			}
		}

		/// <summary>
		/// Gets the target of the specified junction point.
		/// </summary>
		/// <remarks>
		/// Only works on NTFS.
		/// </remarks>
		/// <param name="junctionPoint">The junction point path</param>
		/// <param name="pedantic">If true, returns null if path is not a junction point</param>
		/// <param name="supportDeepJunction">Check parents for junctions first</param>
		/// <returns>The target of the junction point</returns>
		/// <exception cref="IOException">Thrown when the specified path does not
		/// exist, is invalid, is not a junction point, or some other error occurs</exception>
		public string ResolveTarget(string junctionPoint, bool pedantic = true, bool supportDeepJunction = true)
		{
			bool changedParent = false;
			if (supportDeepJunction)
			{
				var parent = Path.GetDirectoryName(junctionPoint);
				if (parent != null)
				{
					var junc = ResolveTarget(parent, true, true);
					if (junc != null)
					{
						junctionPoint = junctionPoint.Replace(parent, junc);
						changedParent = true;
					}
				}
			}

			using (var handle = openReparsePoint(junctionPoint, EFileAccess.GenericRead))
			{
				var target = internalGetTarget(handle);
				if (target == null && (changedParent || !pedantic))
				{
					return junctionPoint;
				}
				return target;
			}
		}

		private static string internalGetTarget(SafeFileHandle handle)
		{
			var outBufferSize = Marshal.SizeOf(typeof(REPARSE_DATA_BUFFER));
			var outBuffer = Marshal.AllocHGlobal(outBufferSize);

			try
			{
				int bytesReturned;
				var result = NativeMethods.DeviceIoControl(handle.DangerousGetHandle(), FSCTL_GET_REPARSE_POINT,
					IntPtr.Zero, 0, outBuffer, outBufferSize, out bytesReturned, IntPtr.Zero);

				if (!result)
				{
					var error = Marshal.GetLastWin32Error();
					if (error == ERROR_NOT_A_REPARSE_POINT)
					{
						return null;
					}

					throwLastWin32Error("Unable to get information about junction point.");
				}

				var reparseDataBuffer = (REPARSE_DATA_BUFFER)
					Marshal.PtrToStructure(outBuffer, typeof(REPARSE_DATA_BUFFER));
				if (reparseDataBuffer.ReparseTag != IO_REPARSE_TAG_MOUNT_POINT)
				{
					return null;
				}

				var targetDir = Encoding.Unicode.GetString(reparseDataBuffer.PathBuffer,
					reparseDataBuffer.SubstituteNameOffset, reparseDataBuffer.SubstituteNameLength);
				if (targetDir.StartsWith(NonInterpretedPathPrefix))
				{
					targetDir = targetDir.Substring(NonInterpretedPathPrefix.Length);
				}

				return targetDir;
			}
			finally
			{
				Marshal.FreeHGlobal(outBuffer);
			}
		}

		private static SafeFileHandle openReparsePoint(string reparsePoint, EFileAccess accessMode)
		{
			var ptr = NativeMethods.CreateFile(reparsePoint, accessMode,
				EFileShare.Read | EFileShare.Write | EFileShare.Delete,
				IntPtr.Zero, ECreationDisposition.OpenExisting,
				EFileAttributes.BackupSemantics | EFileAttributes.OpenReparsePoint, IntPtr.Zero);
			if (Marshal.GetLastWin32Error() != 0)
			{
				throwLastWin32Error("Unable to open reparse point.");
			}

			var reparsePointHandle = new SafeFileHandle(ptr, true);
			return reparsePointHandle;
		}

		private static void throwLastWin32Error(string message)
		{
			throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
		}
	}
}
