using System;
using split_wav;

namespace sample
{
    class Program
    {
        // replace with where you would like your wav segment output files to land
        private const string WorkingDirectory = @"replaceme";
        // replace with where your test wav file is located locally
        private const string PathToJson = @"replaceme";
        // replace with where your test json file is located locally
        private const string PathToWav = @"replaceme";
        // replace with what you would like your output files to be prefixed with
        private const string OutputFileName = "replaceme";
        static void Main(string[] args)
        {
            WavHelper.SplitWave(PathToWav, PathToJson, OutputFileName, WorkingDirectory);
        }
    }
}
