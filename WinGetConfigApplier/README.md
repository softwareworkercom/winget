# WinGet Configuration Applier

A C# console application that parses a JSON file containing application identifiers and installation options, then programmatically applies a WinGet configuration to perform unattended (silent) software installations.

## Features

- ✅ **JSON Configuration**: Define applications to install in a simple JSON format
- ✅ **Validation**: Built-in configuration validation with detailed error messages
- ✅ **Logging**: Comprehensive logging with configurable log levels (Debug, Information, Warning, Error)
- ✅ **Error Handling**: Robust error handling and failure reporting
- ✅ **WinGet Integration**: Generates WinGet DSC YAML configuration and applies it programmatically
- ✅ **Silent Installation**: Supports unattended installations with automatic package agreement acceptance
- ✅ **Developer Mode**: Optional Windows Developer Mode enablement

## Prerequisites

- Windows 10/11 (version 10.0.22631 or higher recommended)
- [WinGet](https://aka.ms/getwinget) installed
- .NET 9.0 SDK or runtime

## Usage

### Basic Usage

Run the application with the default configuration file (`apps-config.json`):

```bash
dotnet run
```

Or if you've built the executable:

```bash
WinGetConfigApplier.exe
```

### Custom Configuration

Specify a custom configuration file and output path:

```bash
dotnet run -- myconfig.json output.yaml
```

Or:

```bash
WinGetConfigApplier.exe myconfig.json output.yaml
```

### Command Line Arguments

```
WinGetConfigApplier [config-file] [output-file]

Arguments:
  config-file    Path to JSON configuration file (default: apps-config.json)
  output-file    Path to output YAML file (default: generated-winget-config.yaml)

Options:
  --help, -h     Display help message
```

## Configuration File Format

The JSON configuration file should follow this structure:

```json
{
  "applications": [
    {
      "id": "Microsoft.VisualStudioCode",
      "name": "Visual Studio Code",
      "source": "winget",
      "installOptions": {
        "silent": true,
        "acceptPackageAgreements": true
      },
      "description": "Install Visual Studio Code"
    }
  ],
  "configuration": {
    "minOsVersion": "10.0.22631",
    "enableDeveloperMode": true,
    "logLevel": "Information"
  }
}
```

### Configuration Properties

#### Application Object

- `id` (required): The WinGet package identifier (e.g., `Microsoft.VisualStudioCode`)
- `name` (required): Display name for the application
- `source` (required): Package source (typically `winget` or `msstore`)
- `installOptions`: Installation options
  - `silent`: Enable silent installation (default: `true`)
  - `acceptPackageAgreements`: Automatically accept package agreements (default: `true`)
- `description`: Description shown during installation

#### Configuration Settings

- `minOsVersion`: Minimum Windows OS version required (default: `10.0.22631`)
- `enableDeveloperMode`: Enable Windows Developer Mode (default: `false`)
- `logLevel`: Logging verbosity - `Debug`, `Information`, `Warning`, or `Error` (default: `Information`)

## Building the Application

### Debug Build

```bash
dotnet build
```

### Release Build

```bash
dotnet build -c Release
```

The compiled executable will be in:
- Debug: `bin/Debug/net9.0/`
- Release: `bin/Release/net9.0/`

### Publishing

To create a self-contained executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

The published application will be in `bin/Release/net9.0/win-x64/publish/`

## Logging

The application provides color-coded console logging:

- **White**: Informational messages
- **Yellow**: Warnings
- **Red**: Errors
- **Gray**: Debug messages (when log level is set to Debug)

Each log entry includes a timestamp in the format: `[LEVEL] yyyy-MM-dd HH:mm:ss - message`

## Error Handling

The application includes comprehensive error handling:

1. **Configuration Validation**: Validates JSON structure and required fields before processing
2. **WinGet Availability Check**: Verifies WinGet is installed before attempting installations
3. **Process Error Handling**: Captures and logs WinGet output and errors
4. **Exit Codes**:
   - `0`: Success
   - `1`: Error (configuration invalid, WinGet not available, installation failed, etc.)

## Example Workflow

1. **Create or modify** `apps-config.json` with desired applications
2. **Run the application**: `dotnet run`
3. **Review the summary** of applications to be installed
4. **Confirm** when prompted (y/N)
5. **Monitor progress** through console logs
6. **Check results** - successful installations or error messages

## Sample Output

```
=== WinGet Configuration Applier ===

Configuration file: apps-config.json
Output file: generated-winget-config.yaml

[INFO] 2025-12-13 00:47:29 - Validating configuration...
[INFO] 2025-12-13 00:47:29 - Configuration validation passed
[INFO] 2025-12-13 00:47:29 - Checking if WinGet is available...
[INFO] 2025-12-13 00:47:29 - WinGet version: v1.7.10514
[INFO] 2025-12-13 00:47:29 - Building WinGet configuration YAML...
[INFO] 2025-12-13 00:47:29 - WinGet configuration written to: generated-winget-config.yaml
[INFO] 2025-12-13 00:47:29 - === Configuration Summary ===
[INFO] 2025-12-13 00:47:29 - Total applications to install: 4
[INFO] 2025-12-13 00:47:29 -   - Visual Studio Code (Microsoft.VisualStudioCode)
[INFO] 2025-12-13 00:47:29 -   - Git (Git.Git)
[INFO] 2025-12-13 00:47:29 -   - .NET SDK 9 (Microsoft.DotNet.SDK.9)
[INFO] 2025-12-13 00:47:29 -   - PowerShell (Microsoft.PowerShell)

Do you want to apply this configuration? (y/N): y
[INFO] 2025-12-13 00:47:32 - Starting installation...
[INFO] 2025-12-13 00:47:32 - Applying WinGet configuration: generated-winget-config.yaml
...
[INFO] 2025-12-13 00:50:15 - === Installation completed successfully ===
```

## Troubleshooting

### WinGet Not Found

If you see "WinGet is not available on this system":
1. Install WinGet from https://aka.ms/getwinget
2. Restart your terminal/command prompt
3. Run `winget --version` to verify installation

### Configuration Validation Errors

The validator will provide specific error messages for:
- Missing required fields (id, name, source)
- Empty application lists
- Invalid configuration structure

### Installation Failures

If an installation fails:
- Check the console output for specific error messages
- Review the generated YAML file for correctness
- Verify the package ID exists in WinGet: `winget search <package-id>`
- Check WinGet logs for detailed error information

## License

MIT License - See repository root for details
