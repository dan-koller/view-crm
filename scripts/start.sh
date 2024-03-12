#!/bin/bash
# This script is used to start the View.Webapi and View.Web projects.
# It will also modify the dependencies in the .csproj files, so that
# the correct dependencies are used for the selected database provider.

#################################################
# Print a help message if specified             #
#################################################

if [ "$1" = "-h" ] || [ "$1" = "--help" ]; then
    echo "Usage: ./start.sh [mssql|sqlite]"
    echo "If no argument is specified, the default is mssql."
    exit 0
fi

#################################################
# Set the View_DB_PROVIDER environment variable #
#################################################

# Get the arguments (can either be "mssql" or "sqlite" or nothing)
# If nothing, then default to "mssql"
arg1=$1
# make arg1 lowercase using POSIX-compliant tr
arg1=$(echo "$arg1" | tr '[:upper:]' '[:lower:]')

if [ "$arg1" = "-mssql" ]; then
    # Set the View_DB_PROVIDER environment variable to "mssql"
    export View_DB_PROVIDER="mssql"
elif [ "$arg1" = "-sqlite" ]; then
    # Set the View_DB_PROVIDER environment variable to "sqlite"
    export View_DB_PROVIDER="sqlite"
else
    # Set the View_DB_PROVIDER environment variable to "mssql"
    echo "No argument specified. Defaulting to mssql."
    export View_DB_PROVIDER="mssql"
fi

#################################################
# Check if prerequisites are installed          #
#################################################

# If sqlite is specified, check if it is installed
if [ "$View_DB_PROVIDER" = "sqlite" ]; then
    if ! command -v sqlite3 &> /dev/null; then
        echo "sqlite3 could not be found."
        echo "Please install sqlite3 and try again."
        exit 1
    fi
fi

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "dotnet could not be found."
    echo "Please install dotnet and try again."
    exit 1
fi

#################################################
# Modify the dependencies in the .csproj files   #
#################################################

SQLITE_DEPENDENCY='<ProjectReference Include=\"..\\View.Common.DataContext.Sqlite\\View.Common.DataContext.Sqlite.csproj\" \/>'
SQLSERVER_DEPENDENCY='<ProjectReference Include=\"..\\View.Common.DataContext.SqlServer\\View.Common.DataContext.SqlServer.csproj\" \/>'
PROJECTS=( src/View.Webapi/View.Webapi.csproj src/View.Service/View.Service.csproj src/View.UnitTests/View.UnitTests.csproj )

# If the View_DB_PROVIDER environment variable is set to "mssql"
# we don't need to do anything, since the default dependencies 
# are already set to "mssql". However, if the previous value was
# "sqlite", we need to modify the dependencies in the .csproj files.
if [ "$View_DB_PROVIDER" = "mssql" ]; then
    for project in "${PROJECTS[@]}"
    do
        echo "Modifying $project file..."

        # Check if the comments are already present before removing them
        if grep -q "<!-- $SQLITE_DEPENDENCY -->" "$project"; then
            # Works with both GNU and BSD/macOS Sed, due to a *non-empty* option-argument:
            # Create a backup file *temporarily* and remove it on success.
            sed -i.bak "s/<!-- $SQLITE_DEPENDENCY -->/$SQLITE_DEPENDENCY/g" "$project" && rm "$project.bak"
        fi

        if grep -q "<!-- $SQLSERVER_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/<!-- $SQLSERVER_DEPENDENCY -->/$SQLSERVER_DEPENDENCY/g" "$project" && rm "$project.bak"
        fi

        # Uncomment the sqlserver dependency and comment out the sqlite dependency
        if ! grep -q "<!-- $SQLITE_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/$SQLITE_DEPENDENCY/<!-- $SQLITE_DEPENDENCY -->/g" "$project" && rm "$project.bak"
        fi

        if grep -q "<!-- $SQLSERVER_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/$SQLSERVER_DEPENDENCY/<!-- $SQLSERVER_DEPENDENCY -->/g" "$project" && rm "$project.bak"
        fi
    done
fi

# If the View_DB_PROVIDER environment variable is set to "sqlite"
# we need to modify the dependencies in the .csproj files.
if [ "$View_DB_PROVIDER" = "sqlite" ]; then
    for project in "${PROJECTS[@]}"
    do
        echo "Modifying $project file..."

        # Check if the comments are already present before adding them
        if ! grep -q "<!-- $SQLITE_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/$SQLITE_DEPENDENCY/<!-- $SQLITE_DEPENDENCY -->/g" "$project" && rm "$project.bak"
        fi

        if ! grep -q "<!-- $SQLSERVER_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/$SQLSERVER_DEPENDENCY/<!-- $SQLSERVER_DEPENDENCY -->/g" "$project" && rm "$project.bak"
        fi

        # Uncomment the sqlite dependency and comment out the sqlserver dependency
        if grep -q "<!-- $SQLITE_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/<!-- $SQLITE_DEPENDENCY -->/$SQLITE_DEPENDENCY/g" "$project" && rm "$project.bak"
        fi

        if ! grep -q "<!-- $SQLSERVER_DEPENDENCY -->" "$project"; then
            sed -i.bak "s/$SQLSERVER_DEPENDENCY/<!-- $SQLSERVER_DEPENDENCY -->/g" "$project" && rm "$project.bak"
        fi
    done
fi

#################################################
# Build the startup projects with dotnet build  #
#################################################

# Build all projects in the solution
echo "Building all projects in the solution..."
dotnet build

#################################################
# Start the View.Webapi and View.React projects   #
#################################################

# Start the View.Webapi project
echo "Starting View.Webapi project..."
dotnet run --project src/View.Webapi/View.Webapi.csproj --launch-profile https &

# Wait for a moment (adjust the sleep duration as needed)
sleep 5

# Start the View.React project
echo "Starting View.React project..."
dotnet run --project src/View.React/View.React.csproj