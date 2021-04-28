- [Description](#description)
  - [Features](#features)
- [Usage](#usage)
  - [Installation](#installation)
    - [Windows service](#windows-service)
    - [Desktop app](#desktop-app)
    - [Demo mode](#demo-mode)
- [Development](#development)

# Description
ValheimBackup is a Windows application to automatically backup world files for the [Valheim](https://store.steampowered.com/app/892970/Valheim/) game. For a quick demo, see the [Demo mode](#demo-mode) section.

## Features
- Continuous and automatic backup of world files to local disk
- Automatic cleanup (rolling backups)
- Configure multiple Servers for backup:
  - FTP Server
  - Start/end date
  - Backup frequency
  - Cleanup frequency (optional)
  - World directory
  - World selection (all or only specific)
- View list of locally saved backups
- Restore backup to server (planned)
- Desktop notifications (planned)

**To request a new feature or notify of a bug, submit a new [issue](https://github.com/slimnate/ValheimBackup/issues/new).**
# Usage

## Installation

### Windows service

### Desktop app

### Demo mode

A demo mode is provided, that includes some sample save game files, and a local FTP server that serves those files. The demo application comes pre-configured with a server entry that connects to the local FTP server.

# Development

The ValheimBackup solution has three projects that each work together to provide all the required functionality:

- **WPF application** - Provides the GUI to manage servers and view backups
- **Windows Service** - Runs in the background to perform backup and cleanup operations
- **Shared Class Library** - Provides core functionality that is shared among the GUI and the service (business logic, FTP connectivity, some application settings)
