@echo off

:: Start the View.Webapi project in a new terminal window
start cmd /k "cd src\View.Webapi && dotnet run --launch-profile https"

:: Start the View.React project in a new terminal window
start cmd /k "cd src\View.React && dotnet run"

:: Add more projects as needed

echo Projects have been started in separate terminal windows.
