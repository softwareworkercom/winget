# Dev Container Guide

This guide explains how to use the Dev Container configuration to create a portable VS Code development environment.

## What is a Dev Container?

A Dev Container (Development Container) allows you to use a Docker container as a full-featured development environment. This ensures everyone on your team has the same tools, dependencies, and configuration, regardless of their operating system.

## Prerequisites

Before you can use this Dev Container, you need:

1. **Docker Desktop** (Windows/Mac) or **Docker Engine** (Linux)
   - [Download Docker Desktop for Windows](https://www.docker.com/products/docker-desktop/)
   - [Download Docker Desktop for Mac](https://www.docker.com/products/docker-desktop/)
   - [Install Docker Engine on Linux](https://docs.docker.com/engine/install/)

2. **Visual Studio Code**
   - [Download VS Code](https://code.visualstudio.com/)

3. **Dev Containers Extension**
   - Install from VS Code: Search for "Dev Containers" in the Extensions view (Ctrl+Shift+X)
   - Or install from [VS Code Marketplace](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

## What's Included

This Dev Container provides a complete development environment with:

### Base Environment
- **.NET 10.0 SDK** - Latest .NET development tools
- **Node.js LTS** - JavaScript runtime with npm, yarn support
- **Git** - Version control
- **PowerShell** - Cross-platform shell and scripting

### Development Tools
- **Docker Engine** (via docker-in-docker) - Build and run containers
- **Azure CLI** - Manage Azure resources
- **GitHub CLI** - Interact with GitHub from terminal
- **kubectl & Helm** - Kubernetes management tools

### Pre-installed Global Tools
After container creation, these are automatically installed:
- **Angular CLI** - Angular framework
- **Vite** - Next-generation frontend tooling
- **dotnet-ef** - Entity Framework Core tools
- **Data API Builder** - Microsoft Data API builder
- **SqlPackage** - SQL Server deployment tool
- **Stryker** - Mutation testing tool

### VS Code Extensions (30+ extensions)
All extensions from `post-install.ps1` are pre-installed:
- C# and .NET development tools
- Database tools (MSSQL, SQL Database Projects)
- GitHub Copilot and PR tools
- Docker and Kubernetes tools
- Linting, formatting, and debugging tools
- React and Angular snippets
- And many more...

### Port Forwarding
Pre-configured ports for common development scenarios:
- **3000** - React/Node.js applications
- **4200** - Angular applications
- **5000** - .NET HTTP
- **5001** - .NET HTTPS
- **8080, 8081** - General development servers

## How to Use

### Method 1: Open in Dev Container (Recommended)

1. **Clone this repository**
   ```bash
   git clone https://github.com/softwareworkercom/winget.git
   cd winget
   ```

2. **Open in VS Code**
   ```bash
   code .
   ```

3. **Reopen in Container**
   - VS Code will detect the `.devcontainer` configuration
   - A notification will appear: "Folder contains a Dev Container configuration file"
   - Click **"Reopen in Container"**
   
   Or manually:
   - Press `F1` or `Ctrl+Shift+P`
   - Type "Dev Containers: Reopen in Container"
   - Press Enter

4. **Wait for Container Build**
   - First time: 5-10 minutes (downloads image and installs tools)
   - Subsequent times: 30-60 seconds
   - You can see progress in the VS Code notification area

5. **Start Developing!**
   - All tools and extensions are ready
   - Your code is mounted and synced automatically
   - Terminal opens inside the container with all tools available

### Method 2: Use GitHub Codespaces

1. **Open in Codespaces**
   - Go to the repository on GitHub
   - Click the green **"Code"** button
   - Select **"Codespaces"** tab
   - Click **"Create codespace on main"**

2. **Wait for Environment Setup**
   - GitHub will build and start your dev environment in the cloud
   - All tools and extensions load automatically

3. **Develop in Browser or VS Code**
   - Edit code directly in the browser
   - Or click **"Open in VS Code Desktop"** for local experience

### Method 3: Clone Repository in Container Volume

For better performance on Windows/Mac:

1. Press `F1` or `Ctrl+Shift+P`
2. Type "Dev Containers: Clone Repository in Container Volume"
3. Enter repository URL: `https://github.com/softwareworkercom/winget.git`
4. Wait for clone and container setup

## Verifying Installation

Once the container is running, open a new terminal in VS Code and verify:

```bash
# Check .NET version
dotnet --version
# Should show .NET 10.x

# Check Node.js version
node --version
# Should show v20.x or v22.x (LTS)

# Check Docker
docker --version
# Should show Docker version

# Check Azure CLI
az --version

# Check kubectl
kubectl version --client

# Check global .NET tools
dotnet tool list --global
# Should list: dotnet-ef, microsoft.dataapibuilder, microsoft.sqlpackage, dotnet-stryker

# Check global npm packages
npm list -g --depth=0
# Should list: @angular/cli, vite
```

## Working with the Container

### File Access
- Your code is automatically synced between your host machine and the container
- Changes made in VS Code are immediately reflected in both locations
- SSH keys and Git config are mounted read-only for authentication

### Running Commands
All commands run inside the container:

```bash
# .NET commands
dotnet new webapi -n MyApi
dotnet run

# Node.js commands
npm install
npm start

# Angular commands
ng new my-app
ng serve

# Docker commands
docker build -t myapp .
docker run -p 8080:80 myapp

# Git commands (uses your host credentials)
git status
git commit -m "Your changes"
git push
```

### Opening Ports
Ports are automatically forwarded. When your app starts:
- VS Code detects the port
- Shows a notification
- Forwards it to your host machine
- Access via `localhost:PORT` in your host browser

## Troubleshooting

### Container Won't Start
- **Check Docker is running**: `docker ps` should work in host terminal
- **Check Docker resources**: Ensure Docker has enough memory (8GB+ recommended)
- **Rebuild container**: Command Palette → "Dev Containers: Rebuild Container"

### Extensions Not Loading
- Wait for post-create command to finish (check terminal output)
- Manually reload: Command Palette → "Developer: Reload Window"

### Slow Performance (Windows/Mac)
- Use "Clone Repository in Container Volume" method
- Or move code to WSL2 (Windows) for better performance

### Missing Tools
- Check terminal output for post-create command errors
- Manually run: `npm install -g @angular/cli vite`
- Or: `dotnet tool install --global dotnet-ef`

### Port Already in Use
- Stop other services using the same port on host
- Or change port in your application config

## Customizing the Container

You can customize `.devcontainer/devcontainer.json`:

### Add More Extensions
```json
"customizations": {
  "vscode": {
    "extensions": [
      "existing.extensions",
      "your.new-extension"
    ]
  }
}
```

### Add More Features
```json
"features": {
  "ghcr.io/devcontainers/features/python:1": {
    "version": "3.11"
  }
}
```

### Change Post-Create Commands
```json
"postCreateCommand": "your custom commands here"
```

## Additional Resources

- [VS Code Dev Containers Documentation](https://code.visualstudio.com/docs/devcontainers/containers)
- [Dev Container Specification](https://containers.dev/)
- [Available Dev Container Features](https://containers.dev/features)
- [GitHub Codespaces Documentation](https://docs.github.com/en/codespaces)
- [Docker Documentation](https://docs.docker.com/)

## Benefits

✅ **Consistent Environment** - Same setup across all machines and team members

✅ **No Local Installation** - No need to install .NET, Node.js, or other tools on host

✅ **Isolation** - Keep development dependencies separate from your system

✅ **Portable** - Take your environment anywhere (Windows, Mac, Linux, cloud)

✅ **Quick Onboarding** - New team members ready in minutes, not hours

✅ **Version Control** - Dev environment configuration is versioned with your code

---

**Need Help?** Open an issue in the repository or check the VS Code Dev Containers documentation.
