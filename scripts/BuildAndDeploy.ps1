$BUCKET_NAME = 'jwscully.uk'

# call our build-script
./BuildScript.ps1

if(!$?) {
	echo "Error occured during build phase"
}

# move to the output of our C# script
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