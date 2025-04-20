@echo off
echo Running MyReverseEngineeringTool for multiple files...

set "TOOL_PATH=C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\bin\Debug\net8.0\assignment_3.exe"
set "OUTPUT_DIR=C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\output"
set "OUTPUT_DIR_SHORT=.\output"

if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

echo.
echo Processing file 1: assignment_2_1.dll
"%TOOL_PATH%" "C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\test_files\assignment_2_1.dll" --format text --output "%OUTPUT_DIR%"

echo.
echo Processing file 2: assignment_2_1.dll
"%TOOL_PATH%" "C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\test_files\assignment_2_1.dll" --format yuml --output "%OUTPUT_DIR%"

echo.
echo Processing file 3: assignment_2_1.dll
"%TOOL_PATH%" "C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\test_files\assignment_2_1.dll" --format plantuml --output "%OUTPUT_DIR%"

echo.
echo All files processed. Results are saved in: %OUTPUT_DIR%
pause
