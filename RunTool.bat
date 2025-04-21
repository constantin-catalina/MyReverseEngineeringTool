@echo off
echo Running MyReverseEngineeringTool for multiple files...

set "TOOL_PATH=C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\bin\Debug\net8.0\assignment_3.exe"
set "TEST_FILES_DIR=C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\test_files"
set "OUTPUT_DIR=C:\Users\Catalina\Desktop\Politehnica 2022 - 2026\Anul III\Sem II\PASSC\Lab\assignment_3\assignment_3\output"
set "OUTPUT_DIR_SHORT=.\output"

if not exist "%OUTPUT_DIR%" mkdir "%OUTPUT_DIR%"

echo.
echo Processing file 1: assignment_2_1.dll (Format: text)
"%TOOL_PATH%" "%TEST_FILES_DIR%\assignment_2_1.dll" --format text --output "%OUTPUT_DIR%"

echo.
echo Processing file 2: assignment_2_1.dll (Format: plantuml)
"%TOOL_PATH%" "%TEST_FILES_DIR%\assignment_2_1.dll" --format yuml --output "%OUTPUT_DIR%"

echo.
echo Processing file 3: assignment_2_1.dll (Format: plantuml)
"%TOOL_PATH%" "%TEST_FILES_DIR%\assignment_2_1.dll" --format plantuml --output "%OUTPUT_DIR%"

echo.
echo Processing file 4: SimpleEventBus.dll (Format: text)
"%TOOL_PATH%" "%TEST_FILES_DIR%\SimpleEventBus.dll" --format text --output "%OUTPUT_DIR%"

echo.
echo Processing file 5: SimpleEventBus.dll (Format: yuml)
"%TOOL_PATH%" "%TEST_FILES_DIR%\SimpleEventBus.dll" --format yuml --output "%OUTPUT_DIR%"

echo.
echo Processing file 6: SimpleEventBus.dll (Format: plantuml)
"%TOOL_PATH%" "%TEST_FILES_DIR%\SimpleEventBus.dll" --format plantuml --output "%OUTPUT_DIR%"

echo.
echo Processing file 7: SimpleEventBus2.dll (No methods, No attributes, Fully Qualified)
"%TOOL_PATH%" "%TEST_FILES_DIR%\SimpleEventBus2.dll" --format plantuml --output "%OUTPUT_DIR%" --no-methods --no-attributes --fully-qualified

echo.
echo Processing file 8: SimpleEventBus2.dll (Ignore option: Subscription, IBasicEventBus)
"%TOOL_PATH%" "%TEST_FILES_DIR%\SimpleEventBus2.dll" --format text --output "%OUTPUT_DIR%" --ignore "Subscription" --ignore "IBasicEventBus"

echo.
echo All files processed. Results are saved in: %OUTPUT_DIR%
pause
