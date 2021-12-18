cd FindViewById.Library
dotnet build -c Release
cd ../FindViewById.CodeGen
dotnet pack -c Release
cd bin/Release