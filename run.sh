#!/bin/bash
# Run the Stride3D F# demo game.
# Usage: ./run.sh [--build-assets] [--background] [--opengl]
#
# Prerequisites (Arch Linux):
#   sudo pacman -S freetype2 ttf-liberation mesa libgl sdl2 openal vulkan-icd-loader
#
#   FreeImage (AUR — required for asset compilation):
#     cd /tmp && git clone https://aur.archlinux.org/freeimage.git && cd freeimage && makepkg -si
#     sudo ln -sf /usr/lib/libfreeimage.so /usr/lib/freeimage.so
#
# Options:
#   --build-assets  Build Stride assets (requires freeimage)
#   --background    Run game in background, return immediately
#   --opengl        Use OpenGL instead of default graphics API (recommended for GPU passthrough)

set -e
cd "$(dirname "$0")/Stride3dFSharp"

BUILD_ARGS="-p:StrideCompilerSkipBuild=true"
BG=""
EXTRA_PROPS=""

for arg in "$@"; do
    case "$arg" in
        --build-assets) BUILD_ARGS="" ;;
        --background)   BG="&" ;;
        --opengl)       EXTRA_PROPS="-p:StrideGraphicsApi=OpenGL" ;;
    esac
done

# Ensure glslangValidator is available for runtime shader compilation
GLSLANG_SRC="$HOME/.nuget/packages/stride.shaders.compiler/4.3.0.2507/contentFiles/any/any/linux-x64/glslangValidator.bin"
GLSLANG_DST="linux-x64/glslangValidator.bin"
if [ ! -f "$GLSLANG_DST" ] && [ -f "$GLSLANG_SRC" ]; then
    echo "Copying glslangValidator.bin to linux-x64/..."
    mkdir -p linux-x64/
    cp "$GLSLANG_SRC" "$GLSLANG_DST"
    chmod +x "$GLSLANG_DST"
fi

echo "Building Stride3D F# project..."
dotnet build $BUILD_ARGS $EXTRA_PROPS 2>&1 | grep -E "succeeded|FAILED|error"

echo "Running game..."
if [ -n "$BG" ]; then
    dotnet bin/Debug/net10.0/linux-x64/Stride3dFSharp.dll &
    echo "Game PID: $!"
else
    dotnet bin/Debug/net10.0/linux-x64/Stride3dFSharp.dll
fi
