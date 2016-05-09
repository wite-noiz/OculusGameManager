# OculusGameManager

## Changelog
v1.1 2015-05-06
* Replaced log trawling logic with OAF parsing
* Redistributables now included in backups

v1.0 2015-05-03
* Initial release

## Introduction
A simple application for managing your Oculus Home apps.

NOTE: This is a quick-and-dirty solution; there is minimal logging, error handling, and IOC.

## Guide
OGM has to run as local admin in order to be able to start/stop the Oculus services (required so that changes take effect) and to modify the file-system.  
On start, if Oculus Home is running, OGM will prompt to stop it and will prompt to restart it after closing.

### Display
The main window shows the list of installed applications and their true location on the file system (junctions resolved).

### Configuring
Only one thing to configure: use the "..." button in the top panel to select your default backup/restore location.

### Backing up
Assuming the app is currently installed, the "Backup" button will create a zip archive of all necessary files for restore at your default backup location.

### Restoring a backup
The "Restore Backup" button will prompt you to locate a zip archive to restore (defaults to your backup location). If the name of the archive is not a known app, it will fail to protect you from unwanted files.

### Relocating an installed app
If you want to gain more diskspace on your Oculus installation drive, you can relocate an installed app.  
This will move the app files to the new location and create a directory junction to the original path, so that Oculus Home isn't aware of the difference.

### Locating an app
If you know that you have an Oculus app located somewhere other than the default location (i.e., you've moved the folder yourself), you can choose to locate the folder by showing the not installed apps and choosing the "Locate" button.  
If all goes well and the app now shows as installed, you may be prompted to recreate the manifest file before it will appear in Oculus Home.

## Known limitations
Currently, the generated manifest files contain no version information, so Oculus Home asks you to update them, which will initiate a full download.

OGM has only been built against a single valid Oculus installation with additional testing on a PC without Oculus Home.  There are likely to be plenty of unexpected permutations that create interesting issues.

## Future features
* Query the Oculus graph servers for all app information
* Space estimatation / task timing
* Last backup date/time
* Clear all local profile data