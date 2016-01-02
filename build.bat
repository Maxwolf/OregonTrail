SET build_path=%~dp0
PowerShell -NoProfile -ExecutionPolicy Bypass -Command "& '%build_path:~0%build.ps1'"
PAUSE