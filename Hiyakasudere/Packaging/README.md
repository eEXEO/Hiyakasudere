# MS Store Publishing (MSIX Packaging)

This folder contains the manifest and assets needed to publish Hiyakasudere to the Microsoft Store as an MSIX package.

## Identity Information
- **Name:** 20890VertexTeam.Hiyakasudere
- **Publisher:** CN=B9F3B529-1E75-4D84-AE62-206726311D04
- **Publisher Display Name:** moeIT
- **Store ID:** 9MZ99G4SQ4ZD

## How to Build MSIX

1. First, publish the app as self-contained for Windows:
   ```
   dotnet publish -c Release -r win-x64 --self-contained -o ./publish/win
   ```

2. Use the Windows SDK `makeappx.exe` tool to package:
   ```
   makeappx pack /d ./publish/win /p Hiyakasudere.msix /l
   ```

3. Sign with your certificate:
   ```
   signtool sign /fd SHA256 /a /f YourCert.pfx /p YourPassword Hiyakasudere.msix
   ```

4. Upload the .msix to Partner Center.

## Alternative: Use Visual Studio
- Open the solution in VS
- Right-click project > Publish > Create App Packages
- Follow the wizard to generate Store-ready MSIX

## Assets Needed
Place the following PNG files in an `Assets/` subfolder:
- StoreLogo.png (50x50)
- Square44x44Logo.png (44x44)
- Square150x150Logo.png (150x150)
- Square310x310Logo.png (310x310)
- Wide310x150Logo.png (310x150)
- SplashScreen.png (620x300)
