
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace netcore3_simple_game_engine
{
    public static class Sound
    {
        public static void SoundThreadMethod()
        {
            
            string filename = "61609__metamorphmuses__plock.wav";

            using (AudioContext context = new AudioContext())
            {
                int buffer = AL.GenBuffer();
                int source = AL.GenSource();
                int state;

                int channels, bits_per_sample, sample_rate;
                byte[] sound_data = LoadWave(File.Open(filename, FileMode.Open), out channels, out bits_per_sample, out sample_rate);
                AL.BufferData(buffer, GetSoundFormat(channels, bits_per_sample), sound_data, sound_data.Length, sample_rate);

                AL.Source(source, ALSourcei.Buffer, buffer);
                AL.SourcePlay(source);

                AL.GetSource(source, ALGetSourcei.SourceState, out state);

                Trace.Write("Playing");

                // Query the source to find out when it stops playing.
                do
                {
                    Thread.Sleep(250);
                    Trace.Write(".");
                    AL.GetSource(source, ALGetSourcei.SourceState, out state);
                }
                while ((ALSourceState)state == ALSourceState.Playing);

                Trace.WriteLine("");

                AL.SourceStop(source);
                AL.DeleteSource(source);
                AL.DeleteBuffer(buffer);
            }
        }

        // Loads a wave/riff audio file.
        public static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        public static ALFormat GetSoundFormat(int channels, int bits)
        {
            switch (channels)
            {
                case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                default: throw new NotSupportedException("The specified sound format is not supported.");
            }
        }
    }
}