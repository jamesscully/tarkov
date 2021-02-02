$REF_FS = '-reference:"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\System.IO.Compression.FileSystem.dll"'

$BUCKET_NAME = 'jwscully.uk'

$XML_PATH = 'C:\Users\yames\source\repos\TarkovAssistantWPF\updates\update_info.xml'
$ZIP_PATH = '../updates/update.zip'

# Compile and run our update files script
csc -reference:"$REF_FS" .\GenerateUpdateFiles.cs; .\GenerateUpdateFiles.exe

echo "Moving to AWS..."

# move into the output of our C# script
cd ../updates/


# for each of the files (zip, xml) we want to move these to AWS
$files = Get-ChildItem "." -Filter *.*

echo "Found files: $files"

foreach ($f in $files) {
	
	# Upload file (requires my AWS tokens)
	Write-S3Object -BucketName "$BUCKET_NAME" -Key "tarkov-assistant/$f" -File "$f"
	
	if($?) {
		echo "Successfully uploaded $f to AWS"
	}
}

# return to original place of execution
cd ../scripts/




