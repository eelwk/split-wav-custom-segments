# split-wav-custom-segments
Split a wav file into custom segments

This sample comes with two files to test with:

1. sample.json
2. sample.wav

The json file is an example of a file that describes how a wav file might be segmented into sentences. The file contains an array of "Segments," each of which has 2 properties:

1. Offset in seconds : segment begins at this offset in unit of seconds from beginning of audio
2. Duration in seconds : segment has this duration in unit of seconds

# running the sample

Open solution file using Visual Studio (2017 or higher) 

Restore all packages (open package manager console and 
run "dotnet restore")

Find all instances of "replaceme" and replace with your local settings.

Run sample application (.net core console app)

Inspect your working directory.

You should see a wav folder with 5 wav files, each one a segment of the wav file.
