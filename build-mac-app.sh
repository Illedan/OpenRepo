#!/bin/bash
# Builds dist/OpenRepo.app, the desktop version of OpenRepo, for this Mac.
# Pass --install to replace /Applications/OpenRepo.app with it and open it.
set -euo pipefail
cd "$(dirname "$0")"

case "$(uname -m)" in
  arm64) runtime=osx-arm64 ;;
  *) runtime=osx-x64 ;;
esac
version=$(grep -o "[0-9]*\.[0-9]*" version.config)
bundle_id=com.illedan.openrepo
app=dist/OpenRepo.app

rm -rf "$app" dist/publish dist/OpenRepo.iconset
dotnet publish src/OpenRepo.Desktop -c Release -r "$runtime" --self-contained -o dist/publish

mkdir -p "$app/Contents/MacOS" "$app/Contents/Resources" dist/OpenRepo.iconset
cp -R dist/publish/ "$app/Contents/MacOS/"

for size in 16 32 128 256 512; do
  sips -z $size $size src/OpenRepo.Desktop/Assets/icon.png --out "dist/OpenRepo.iconset/icon_${size}x${size}.png" >/dev/null
  sips -z $((size * 2)) $((size * 2)) src/OpenRepo.Desktop/Assets/icon.png --out "dist/OpenRepo.iconset/icon_${size}x${size}@2x.png" >/dev/null
done
iconutil -c icns dist/OpenRepo.iconset -o "$app/Contents/Resources/OpenRepo.icns"

cat > "$app/Contents/Info.plist" <<PLIST
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
  <key>CFBundleName</key><string>OpenRepo</string>
  <key>CFBundleDisplayName</key><string>OpenRepo</string>
  <key>CFBundleIdentifier</key><string>$bundle_id</string>
  <key>CFBundleExecutable</key><string>OpenRepo.Desktop</string>
  <key>CFBundleIconFile</key><string>OpenRepo</string>
  <key>CFBundlePackageType</key><string>APPL</string>
  <key>CFBundleShortVersionString</key><string>$version</string>
  <key>CFBundleVersion</key><string>$version</string>
  <key>LSMinimumSystemVersion</key><string>12.0</string>
  <key>LSApplicationCategoryType</key><string>public.app-category.developer-tools</string>
  <key>NSHighResolutionCapable</key><true/>
</dict>
</plist>
PLIST

rm -rf dist/publish dist/OpenRepo.iconset
# Ad-hoc signing, which Apple Silicon requires to run the app. It is not notarized, so only use it on this Mac.
codesign --force --deep --sign - "$app"
echo "Built $app"

if [[ "${1:-}" == "--install" ]]; then
  osascript -e "if application id \"$bundle_id\" is running then tell application id \"$bundle_id\" to quit" >/dev/null 2>&1 || true
  while pgrep -qf "/Applications/OpenRepo.app/"; do sleep 0.2; done
  rm -rf /Applications/OpenRepo.app
  cp -R "$app" /Applications/
  open /Applications/OpenRepo.app
  echo "Installed /Applications/OpenRepo.app"
fi
