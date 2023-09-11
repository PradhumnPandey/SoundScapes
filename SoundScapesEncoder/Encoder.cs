using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace SoundScapesEncoder
{

    public class ColorEncoder
    {
        static int[] factor = { 100, 300, 500, 700 };
        static double[] gain = { 0.7, 0.5, 0.3, 0.1 };
        public void Encode(string path)
        {
            int height, width;
            string line = String.Empty;

            string[] pixel;
            using (StreamReader sr = new StreamReader(path))
            {
                line = sr.ReadLine();
                String[] dim = line.Split(" "); ;
                String[] pixelData;
                int A, R, G, B;
                height = Convert.ToInt32(dim[0]);
                width = Convert.ToInt32(dim[1]);
                for (int i = 0; i < width; i++)
                {
                    line = sr.ReadLine();
                    pixel = line.Split(",");
                    for (int j = 0; j < height; j++)
                    {
                        pixelData = pixel[j].Split(" ");
                        generateAudio(0, Convert.ToInt32(pixelData[0]));
                        generateAudio(1, Convert.ToInt32(pixelData[1]));
                        generateAudio(2, Convert.ToInt32(pixelData[2]));
                        generateAudio(3, Convert.ToInt32(pixelData[3]));
                    }
                }
            }
        }

        public void generateAudio(int color, int value)
        {
            var sineWaveProvider = new SignalGenerator()
            {
                Gain = gain[color],
                Frequency = factor[color] + (value * 0.5),
                Type = SignalGeneratorType.Sin
            };
            writeAudio(sineWaveProvider);
        }
        public void writeAudio(SignalGenerator signalGenerator)
        {
            using (var outputDevice = new WaveOutEvent())
            {
                // Set the audio output device
                outputDevice.Init(signalGenerator);

                // Start playing the signal
                outputDevice.Play();
                System.Threading.Thread.Sleep(20);
                // Record the signal and save it to a WAV file
                //string outputFilePath = "output.wav";

                //// Create a WaveFileWriter
                //using (var writer = new WaveFileWriter(outputFilePath, signalGenerator.WaveFormat))
                //{
                //    int bufferSize = 1024;
                //    float[] buffer = new float[bufferSize];
                //    int bytesRead;

                //    // Read audio data from the signal generator and write it to the file
                //    while ((bytesRead = signalGenerator.Read(buffer, 0, bufferSize)) > 0)
                //    {
                //        writer.Write(buffer, 0, bytesRead);
                //    }
                //}
                outputDevice.Stop();
            }

        }

    }
    public class BWEncoder
    {
        List<float[]> buffer;
        static int[] factor = { 100, 1000, 500 };
        static double[] gain = { 1, 0.1, 0.5 };
        public BWEncoder()
        {
            buffer = new List<float[]>();
        }

        public void EncodeBW(string path)
        {
            int height, width;
            string line = String.Empty;
            string[] stream;
            using (StreamReader sr = new StreamReader(path))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    stream = line.Split(" ");
                    foreach (String i in stream)
                    {
                        generateAudioBW(Convert.ToInt32(i));
                    }
                    generateAudioBW(2);
                }
            }
            writeAudioBW();
        }
        public void generateAudioBW(int pixel)
        {
           
            int sampleRate = 44100; // Adjust according to your requirements
            double signalsPerSecond = 3300;
            double durationPerSignal = 1.0 / signalsPerSecond; // Duration for each signal in seconds
            int totalSamplesPerSignal = (int)(sampleRate * durationPerSignal);
            float[] sample = new float[totalSamplesPerSignal];

            int value;
            if (pixel == 2) value = 10000;
            else value = pixel == 0 ? 1000 : 20000; // 0  aur 1 ki frequency yha set krni hai
            var sineWaveProvider = new SignalGenerator()
            {
                Gain = gain[pixel],
                Frequency = factor[pixel] + (value * 0.5),
                Type = SignalGeneratorType.Sin
            };


            sineWaveProvider.Read(sample, 0, sample.Length);
            buffer.Add(sample);
        }
        public void writeAudioBW()
        {
            int sampleRate = 44100;
                WaveFormat waveFormat = new WaveFormat(sampleRate, 16, 1);


                using (WaveFileWriter writer = new WaveFileWriter("output.wav", waveFormat))
                {
                    foreach (float[] buf in buffer)
                    {
                        byte[] byteBuffer = new byte[buf.Length * 2];

                        for (int i = 0; i < buf.Length; i++)
                        {
                            short sample16Bit = (short)(buf[i] * short.MaxValue);
                            BitConverter.GetBytes(sample16Bit).CopyTo(byteBuffer, i * 2);
                        }

                        writer.Write(byteBuffer, 0, byteBuffer.Length);
                    }
                }
        }

        public int[] readAudioBW(string audioFilePath)
        {
            List<double> freqeuncies = new List<double>();
            using (var reader = new AudioFileReader(audioFilePath))
            {
                int sampleRate = reader.WaveFormat.SampleRate;
                int blockSize = sampleRate / 3300; // 3300 signal per second process karre hai. isse zyada me freq sahi se read ni kr paa rha hai.

                float[] buffer = new float[blockSize];

                int bytesRead;
                int count = 0;
                while ((bytesRead = reader.Read(buffer, 0, blockSize)) > 0)
                {
                    // Apply FFT
                    Complex32[] complexSignal = new Complex32[blockSize];
                    for (int i = 0; i < blockSize; i++)
                    {
                        complexSignal[i] = new Complex32(buffer[i], 0);
                    }

                    Fourier.Forward(complexSignal, FourierOptions.Matlab);

                    // Get the magnitudes of the frequencies
                    double[] magnitudes = new double[blockSize / 2];
                    for (int i = 0; i < blockSize / 2; i++)
                    {
                        magnitudes[i] = complexSignal[i].Magnitude;
                    }
                    // Find the dominant frequency
                    double dominantFrequency = Array.IndexOf(magnitudes, magnitudes.Max()) * sampleRate / blockSize;

                    freqeuncies.Add(dominantFrequency);
                    Console.WriteLine("Dominant Frequency: " + dominantFrequency + " Hz " + count++);
                }
            }
            return writeImageBW(freqeuncies, Directory.GetCurrentDirectory() + "\\Output\\convertedData.txt");
        }

        public int[] writeImageBW(List<double> frequencies,string outPath)
        {
            double low, high, lineFreq;
            var freq = frequencies.Distinct().ToList();
            lineFreq = frequencies.Last();
            freq.Remove(lineFreq);
            low = freq.Min();
            high = freq.Max();
            int height = 0, width = 0;

            using (StreamWriter sw = new StreamWriter(outPath))
            {
                foreach (double frequency in frequencies)
                {
                    if (frequency == low)
                    {
                        sw.Write("0 ");
                        width++;
                    }
                    else if (frequency == high)
                    {
                        sw.Write("1 ");
                        width++;
                    }

                    else
                    {
                        sw.WriteLine();
                        height++;
                    }
                }
            }

            width /= height;
            return new int[] { width,height};
        }
    }
}
