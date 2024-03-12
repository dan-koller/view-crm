# This script is used to start the View.Webapi and View.Web projects.
# It will also modify the dependencies in the .csproj files, so that
# the correct dependencies are used for the selected database provider.

#################################################
# Print a help message if specified             #
#################################################

if ($args[0] -eq "-h" -or $args[0] -eq "--help") {
    Write-Host "Usage: start.ps1 [mssql|sqlite]"
    Write-Host "If no argument is specified, the default is mssql."
    exit 0
}

#################################################
# Set the VIEW_DB_PROVIDER environment variable #
#################################################

# Get the arguments (can either be "mssql" or "sqlite" or nothing)
# If nothing, then default to "mssql"
$arg1 = $args[0]

if ($arg1 -eq "-mssql") {
    # Set the VIEW_DB_PROVIDER environment variable to "mssql"
    $env:VIEW_DB_PROVIDER = "mssql"
} elseif ($arg1 -eq "-sqlite") {
    # Set the VIEW_DB_PROVIDER environment variable to "sqlite"
    $env:VIEW_DB_PROVIDER = "sqlite"
} else {
    # Set the VIEW_DB_PROVIDER environment variable to "mssql"
    Write-Host "No argument specified. Defaulting to mssql."
    $env:VIEW_DB_PROVIDER = "mssql"
}

#################################################
# Check if prerequisites are installed          #
#################################################

# If sqlite is specified, check if it is installed
if ($env:VIEW_DB_PROVIDER -eq "sqlite") {
    # Check if sqlite is installed
    if (-not (Get-Command sqlite3 -ErrorAction SilentlyContinue)) {
        Write-Host "sqlite3 could not be found. Please install sqlite3."
        exit 1
    }
}

# Check if dotnet is installed
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host "dotnet could not be found. Please install dotnet."
    exit 1
}

#################################################
# Modify the dependencies in the .csproj files  #
#################################################

$SQLITE_DEPENDENCY = '<ProjectReference Include="..\View.Common.DataContext.Sqlite\View.Common.DataContext.Sqlite.csproj" />'
$SQLSERVER_DEPENDENCY = '<ProjectReference Include="..\View.Common.DataContext.SqlServer\View.Common.DataContext.SqlServer.csproj" />'
$PROJECTS = @("src/View.Webapi/View.Webapi.csproj", "src/View.UnitTests/View.UnitTests.csproj")

# If the VIEW_DB_PROVIDER environment variable is set to "mssql"
# we don't need to do anything, since the default dependencies 
# are already set to "mssql". However, if the previous value was
# "sqlite", we need to modify the dependencies in the .csproj files.
if ($env:VIEW_DB_PROVIDER -eq "mssql") {
    foreach ($project in $PROJECTS) {
        Write-Host "Modifying $project file..."

        # Check if the comments are already present before removing them
        if (Select-String -Path $project -Pattern ([regex]::Escape("<!-- $SQLITE_DEPENDENCY -->"))) {
            (Get-Content $project) -replace [regex]::Escape("<!-- $SQLITE_DEPENDENCY -->"), $SQLITE_DEPENDENCY | Set-Content $project
        }

        if (Select-String -Path $project -Pattern ([regex]::Escape("<!-- $SQLSERVER_DEPENDENCY -->"))) {
            (Get-Content $project) -replace [regex]::Escape("<!-- $SQLSERVER_DEPENDENCY -->"), $SQLSERVER_DEPENDENCY | Set-Content $project
        }

        # Uncomment the sqlserver dependency and comment out the sqlite dependency
        if (-not (Select-String -Path $project -Pattern ([regex]::Escape($SQLSERVER_DEPENDENCY)))) {
            (Get-Content $project) -replace [regex]::Escape($SQLSERVER_DEPENDENCY), "<!-- $SQLSERVER_DEPENDENCY -->" | Set-Content $project
        }

        if (Select-String -Path $project -Pattern ([regex]::Escape($SQLITE_DEPENDENCY))) {
            (Get-Content $project) -replace [regex]::Escape($SQLITE_DEPENDENCY), "<!-- $SQLITE_DEPENDENCY -->" | Set-Content $project
        }
    }
}

# If the VIEW_DB_PROVIDER environment variable is set to "sqlite"
# we need to modify the dependencies in the .csproj files.
if ($env:VIEW_DB_PROVIDER -eq "sqlite") {
    foreach ($project in $PROJECTS) {
        Write-Host "Modifying $project file..."

        # Check if the comments are already present before adding them
        if (-not (Select-String -Path $project -Pattern ([regex]::Escape("<!-- $SQLITE_DEPENDENCY -->")))) {
            (Get-Content $project) -replace [regex]::Escape($SQLITE_DEPENDENCY), "<!-- $SQLITE_DEPENDENCY -->" | Set-Content $project
        }

        if (-not (Select-String -Path $project -Pattern ([regex]::Escape("<!-- $SQLSERVER_DEPENDENCY -->")))) {
            (Get-Content $project) -replace [regex]::Escape($SQLSERVER_DEPENDENCY), "<!-- $SQLSERVER_DEPENDENCY -->" | Set-Content $project
        }

        # Uncomment the sqlite dependency and comment out the sqlserver dependency
        if (Select-String -Path $project -Pattern ([regex]::Escape("<!-- $SQLITE_DEPENDENCY -->"))) {
            (Get-Content $project) -replace [regex]::Escape("<!-- $SQLITE_DEPENDENCY -->"), $SQLITE_DEPENDENCY | Set-Content $project
        }

        if (-not (Select-String -Path $project -Pattern ([regex]::Escape($SQLSERVER_DEPENDENCY)))) {
            (Get-Content $project) -replace [regex]::Escape($SQLSERVER_DEPENDENCY), "<!-- $SQLSERVER_DEPENDENCY -->" | Set-Content $project
        }
    }
}

#################################################
# Build the startup projects with dotnet build  #
#################################################

# Build all projects in the solution
Write-Host "Building all projects in the solution..."
dotnet build

#################################################
# Start the View.Webapi and View.Web projects   #
#################################################

# Start the View.Webapi project
Write-Host "Starting View.Webapi project..."
Start-Process "dotnet" -ArgumentList "run --project src/View.Webapi/View.Webapi.csproj --launch-profile https" -NoNewWindow

# Wait for a moment (adjust the sleep duration as needed)
Start-Sleep -Seconds 5

# Start the View.React project
Write-Host "Starting View.React project..."
Start-Process "dotnet" -ArgumentList "run --project src/View.React/View.React.csproj" -NoNewWindow