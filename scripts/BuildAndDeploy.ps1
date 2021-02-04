# This file will build the project and our compile our GenerateUpdateScript
# GenerateUpdateScript will generate the .XML and .ZIP files needed for the auto-updater

$REF_FS = "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.IO.Compression.FileSystem.dll"
$BUCKET_NAME = 'jwscully.uk'

$XML_PATH = 'C:\Users\yames\source\repos\TarkovAssistantWPF\updates\update_info.xml'
$ZIP_PATH = '../updates/update.zip'


echo "##### Compiling TarkovAssistantWPF.csproj"
# Compile our project
MSBuild.exe ../TarkovAssistantWPF.csproj

echo "##### Compiling GenerateUpdateFiles.cs"
# Compile and run our update files script
csc.exe -reference:"$REF_FS" .\GenerateUpdateFiles.cs
if(!$?) {
	echo "Error occured during compilation of script"
	exit
}

echo "##### Running GenerateUpdateFiles.exe"
.\GenerateUpdateFiles.exe
if(!$?) {
	echo "Error occurred when running GenerateUpdateFiles.exe"
	Exit
}

echo "##### Moving to ../updates/"
# move to the output of our C# script
cd ../updates/


# for each of the files (zip, xml) we want to move these to AWS
$files = Get-ChildItem "." -Filter *.*

echo "Found files: $files"

# Upload files
foreach ($f in $files) {
	
	# If on my computer, use AWS tokens from ~/.aws/
	if($env:UserName -eq 'yames') {
		Write-S3Object -BucketName "$BUCKET_NAME" -Key "tarkov-assistant/$f" -File "$f"
	} else {
		Write-S3Object -AccessKey $env:ACCESS_KEY -SecretKey = $env:SECRET_KEY -BucketName "$BUCKET_NAME" -Key "tarkov-assistant/$f" -File "$f"
	}


	


	if($?) {
		echo "Successfully uploaded $f to AWS"
	}
}

# return to original place of execution
cd ../scripts/