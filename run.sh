#!/bin/bash
# Run the Stride3D F# demo game.
# Usage: ./run.sh [--build-assets] [--background]
#
# Prerequisites (Arch Linux):
#   sudo pacman -S freetype2 ttf-liberation mesa libgl sdl2 openal
#
# Options:
#   --build-assets  Build Stride assets (requires freetype2, fonts)
#   --background    Run game in background, return immediately

set -e
cd "$(dirname "$0")/Stride3dFSharp"

BUILD_ARGS="-p:StrideCompilerSkipBuild=true"
BG=""

for arg in "$@"; do
    case "$arg" in
        --build-assets) BUILD_ARGS="" ;;
        --background)   BG="&" ;;
    esac
done

echo "Building Stride3D F# project..."
dotnet build $BUILD_ARGS 2>&1 | grep -E "succeeded|FAILED|error"

echo "Running game..."
if [ -n "$BG" ]; then
    dotnet bin/Debug/net10.0/linux-x64/Stride3dFSharp.dll &
    echo "Game PID: $!"
else
    dotnet bin/Debug/net10.0/linux-x64/Stride3dFSharp.dll
fi
