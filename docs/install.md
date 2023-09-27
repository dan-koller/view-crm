# Install instructions

View is available for Windows, Linux and macOS. However, the installation process is different for each operating system and there are some limitations.

It is recommended to install View on a Windows machine, since it is the only operating system that supports all features of View.

## Requirements

-   Windows 10, Linux or macOS
-   [.NET 7](https://dotnet.microsoft.com/download/dotnet/7.0)
-   [Node.js](https://nodejs.org/en/) 18
-   SQLServer 2019 or SQLite\*

\* _SQLServer is recommended, since it is the only database that supports all features of View. SQLite is only recommended for development purposes. Also, if you're on macOS, you will need to use SQLite._

## Preparations

By default, View requires a local instance of _SQLServer_. You can download the Developer Edition for free from [here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

If you're using _SQLite_, make sure it is available on your path.

## Installation

1. Clone the repository

```
git clone https://github.com/dan-koller/view-crm.git
```

2. Navigate to the project directory

```
cd view-crm
```

3. Build the project

If you are using a **local SQLServer** instance, you can directly build the project.

```
dotnet build
```

If you're using **SQLite** you will **need to update** the database provider in the references of the `View.Webapi.csproj` file:

```xml
	<ItemGroup>
		<!-- Change to Sqlite if you prefer -->
		<ProjectReference Include="..\View.Common.DataContext.Sqlite\View.Common.DataContext.Sqlite.csproj" />
		<ProjectReference Include="..\View.Common.UserDataContext.Sqlite\View.Common.UserDataContext.Sqlite.csproj" />
		<!-- <ProjectReference Include="..\View.Common.DataContext.SqlServer\View.Common.DataContext.SqlServer.csproj" />  -->
		<!-- <ProjectReference Include="..\View.Common.UserDataContext.SqlServer\View.Common.UserDataContext.SqlServer.csproj" />  -->
	</ItemGroup>
```

Rebuild the project to update the references using `dotnet build` in the `View.Webapi` directory.

4. Install the dependencies for the frontend

```
cd src/View.Webapp
npm i
```

5. Run the start script for your operating system from the root directory

-   Windows:

```bat
.\start.bat
```

-   Linux/macOS:

```sh
./start.sh
```

6. Open the application in your browser

```
http://localhost:5000
```
