# DownloadSorter

A C# console application that automatically organizes and sorts files in your Downloads directory based on configurable rules.

## Overview

DownloadSorter is a utility application that helps keep your Downloads folder organized by automatically sorting files into different directories based on file extensions and custom rules. Define rules once, and the sorter handles the rest!

## Features

- 📁 **Custom Sort Rules** - Create sorting rules based on file extensions
- 🎯 **Flexible Configuration** - Store rules in a JSON configuration file
- 🔄 **Batch Processing** - Sort multiple files at once
- ⚙️ **Easy Management** - Create, edit, and remove sort rules through an interactive console UI
- 💾 **Persistent Storage** - Configuration is saved and automatically loaded on startup
- 🪟 **Windows Native** - Built with .NET 10 for Windows environments

## Requirements

- .NET 10 Runtime or later
- Windows operating system
- Access to read/write files in target directories

## Installation

1. Download the latest release from [GitHub Releases](https://github.com/Involts/DownloadSorter/releases)

2. Extract the downloaded ZIP file to your desired location

3. Run the application:
```bash
DownloadSorter.exe
```

Or double-click `DownloadSorter.exe` to launch the application.

## Usage

### Starting the Application

Launch the application with the `Console` argument to start the interactive menu:

```bash
DownloadSorter.exe
```

### Main Menu Options

1. **Create new Sort Rule** - Add a new sorting rule
   - Provide a name for the rule
   - Specify the destination directory
   - Enter the file extension to match (e.g., `.exe`, `.pdf`, `.mp4`)

2. **Manage Sort Rules** - Edit or delete existing rules
   - Select a rule to manage
   - Edit properties or remove the rule

3. **Show all sorts** - View all configured sort rules

4. **Start Sorter** - Execute the sorting process
   - Files matching the rules will be moved to their destinations

5. **Change Download Directory** - Update the source directory to monitor

6. **Exit** - Close the application

### Configuration File

The application stores its configuration in `SortConfig.json` in the application directory:

```json
{
  "DownloadLocation": "C:/Users/YourUsername/Downloads",
  "SortRule": [
    {
      "Name": "Executables",
      "Location": "C:/Users/YourUsername/Downloads/Programs",
      "FileExtension": ".exe"
    },
    {
      "Name": "Documents",
      "Location": "C:/Users/YourUsername/Downloads/Docs",
      "FileExtension": ".pdf"
    }
  ]
}
```

## Project Structure

```
DownloadSorter/
├── DownloadSorter/                 # Main application project
│   ├── Console-UI/
│   │   ├── ConsoleNavigation.cs   # Console menu navigation logic
│   │   ├── ConsoleMenu.cs         # Menu utilities and input handling
│   │   ├── ConsoleHelper.cs       # Console output formatting
│   │   └── UIManager.cs           # UI orchestration
│   ├── Services/
│   │   └── SortManager.cs         # Core sorting and configuration logic
│   ├── Data/
│   │   ├── SortConfiguration.cs   # Configuration model
│   │   └── SortRule.cs            # Individual sort rule model
│   └── Program.cs                 # Application entry point
├── DownloadSorterTests/            # Unit tests
│   └── Services/
│       └── SortManagmentTests.cs  # Sort manager tests
├── DownloadSorter.sln            # Solution file
└── README.md                       # This file
```

## Building

### Debug Build
```bash
dotnet build -c Debug
```

### Release Build
```bash
dotnet build -c Release
```

## Testing

Run the test suite:

```bash
dotnet test
```

## Technologies

- **Language**: C# 14.0
- **Framework**: .NET 10
- **JSON Serialization**: Newtonsoft.Json (NuGet)
- **Hosting**: Microsoft.Extensions.Hosting.WindowsServices

## Example Workflow

1. Launch the application
2. Set your Downloads directory (e.g., `C:/Users/John/Downloads`)
3. Create a sort rule:
   - Name: "Videos"
   - Destination: `C:/Users/John/Downloads/Videos`
   - Extension: `.mp4`
4. Create another rule for PDFs, images, etc.
5. Click "Start Sorter" to organize your files
6. Your files are now sorted into their respective folders!

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is open source. Check the repository for license details.

## Support

For issues, bug reports, or feature requests, please visit the [GitHub repository](https://github.com/Involts/DownloadSorter).

---

**Note**: Always ensure you have backups of important files before running automated file sorting operations.
