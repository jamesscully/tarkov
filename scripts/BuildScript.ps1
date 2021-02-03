# This file will build the project and our compile our GenerateUpdateScript
# GenerateUpdateScript will generate the .XML and .ZIP files needed for the auto-updater

$REF_FS = '-reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.IO.Compression.FileSystem.dll"'

$XML_PATH = 'C:\Users\yames\source\repos\TarkovAssistantWPF\updates\update_info.xml'
$ZIP_PATH = '../updates/update.zip'


# Compile our project
MSBuild ../TarkovAssistantWPF.csproj

# Compile and run our update files script
csc -reference:"$REF_FS" .\GenerateUpdateFiles.cs; .\GenerateUpdateFiles.exe





