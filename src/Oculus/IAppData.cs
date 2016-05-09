using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OculusGameManager.Oculus
{
    /// <summary>
    /// App-defining data that may come from a manifest file or OAF record
    /// </summary>
    public interface IAppData
    {
        string ApplicationId { get; }
        string CanonicalName { get; }
        string DisplayName { get; }
        string SmallLandscapeImage { get; }
        int VersionCode { get; }
        string Version { get; }
        string LaunchFile { get; } // TODO
        long PackageSize { get; }
        string[] Redistributables { get; } // TODO
    }
}
