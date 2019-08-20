using System;
using System.IO;
using System.Linq;
using System.Text;
using NAudio.Wave;
using Newtonsoft.Json;
using split_wav.Models;

namespace split_wav
{
    // wav file splitting adapted with care from 
    // https://github.com/alexvab/WavSplitter
    public class WavHelper
    {
        public static void SplitWave(string wavFileIn, string jsonFile, string wavFileOut, string workingDirectory)
        {
            using (var waveFileReader = new WaveFileReader(wavFileIn))
            {
                SplitWave(jsonFile, wavFileOut, waveFileReader, workingDirectory);
            }
        }

        private static void SplitWave(string jsonFile, string wavFileOut, WaveFileReader waveFileReader, string workingDirectory)
        {
            RootObject root;
            using (StreamReader r = new StreamReader(jsonFile))
            {
                string json = r.ReadToEnd();
                root = JsonConvert.DeserializeObject<RootObject>(json);
            }
            if (root != null)
            {
                int totalMilliseconds = (int)GetWaveFileDurationInMilliseconds(waveFileReader);

                var totalDuration = TimeSpan.FromMilliseconds(totalMilliseconds);

                int numberOfParts = root.Segments.Count;

                const int minDigits = 4;
                var numberOfDigits = numberOfParts.ToString().Length < minDigits ? minDigits : numberOfParts.ToString().Length;
                string formatString = Enumerable.Repeat("0", numberOfDigits).Aggregate((s, d) => s + d);

                //for PCM AverageBytesPerSecond = sampleRate * (channels * (bits / 8));
                // need this
                int bytesPerMilliseconds = waveFileReader.WaveFormat.AverageBytesPerSecond / 1000;
                double startPos = 0.0;
                if (!Directory.Exists($"{workingDirectory}\\wav"))
                {
                    Directory.CreateDirectory($"{workingDirectory}\\wav");
                }
                var wavDirectory = $"{workingDirectory}\\wav";

                for (int i = 0; i < numberOfParts; i++)
                {
                    var segment = root.Segments[i];
                    startPos = ConvertSecondsToMilliseconds(segment.OffsetInSeconds) * bytesPerMilliseconds;
                    double endPos = startPos + (ConvertSecondsToMilliseconds(segment.DurationInSeconds) * bytesPerMilliseconds);

                    // part name would be something like this: output-0001.wav, output-0002.wav, output-0003.wav, etc
                    var partFileName = $"{wavFileOut}-{(i + 1).ToString(formatString)}.wav";
                    string path = $"{wavDirectory}\\{partFileName}";
                    using (var waveFileWriter = new WaveFileWriter(path, waveFileReader.WaveFormat))
                    {
                        WriteWavChunk(waveFileReader, waveFileWriter, Convert.ToInt64(startPos), Convert.ToInt64(endPos));
                    }
                    startPos = endPos;
                }
            }
        }

        private static long GetWaveFileDurationInMilliseconds(WaveFileReader waveFileReader)
        {
            return waveFileReader.SampleCount / waveFileReader.WaveFormat.SampleRate * 1000;
        }

        private static double ConvertSecondsToMilliseconds(double seconds)
        {
            return TimeSpan.FromSeconds(seconds).TotalMilliseconds;
        }

        private static void WriteWavChunk(WaveFileReader reader, WaveFileWriter writer, long startPos, long endPos)
        {
            reader.Position = startPos;

            // make sure that buffer is sized to a multiple of our WaveFormat.BlockAlign.
            // WaveFormat.BlockAlign = channels * (bits / 8), so for 16 bit stereo wav it will be 4096 bytes
            var buffer = new byte[reader.BlockAlign * 1024];
            while (reader.Position < endPos)
            {
                long bytesRequired = endPos - reader.Position;
                if (bytesRequired > 0)
                {
                    int bytesToRead = (int)Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesToRead);
                    }
                }
            }
        }
    }
}
