﻿using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using MP3Sharp;

namespace OA.Core
{
    // TODO: Handle long audio files.
    public static class AudioUtils
    {
        public static AudioClip CreateAudioClip(string name, PCMAudioBuffer audioBuffer)
        {
            var sampleData = audioBuffer.ToFloatArray();
            var audioClip = AudioClip.Create(name, sampleData.Length, audioBuffer.channelCount, audioBuffer.samplingRate, false);
            audioClip.SetData(sampleData, 0);
            return audioClip;
        }

        public static int SampleFramesToBytes(int sampleFrameCount, int channelCount, int bitDepth)
        {
            return sampleFrameCount * channelCount * (bitDepth / 8);
        }

        public static int BytesToSampleFrames(int byteCount, int channelCount, int bitDepth)
        {
            return byteCount / (bitDepth / 8) / channelCount;
        }

        public static GameObject Play2DAudioStream(MP3StreamReader audioStream)
        {
            var gameObject = new GameObject("tmp2DAudioStream");
            var audioSource = gameObject.AddComponent<AudioSource>();
            //audioSource.clip = CreateStreamingAudioClip("tmpAudioClip", audioStream);
            audioSource.loop = true;
            var audioStreamComponent = gameObject.AddComponent<OneShotAudioStreamComponent>();
            audioStreamComponent.audioStream = audioStream;
            return gameObject;
        }

        public static PCMAudioBuffer ReadAudioFile(string filePath)
        {
            switch (Path.GetExtension(filePath).ToLowerInvariant())
            {
                case ".wav": return ReadWAV(filePath);
                case ".mp3": return ReadMP3(filePath);
                default: throw new ArgumentOutOfRangeException("filePath", "Tried to read an unsupported audio file format.");
            }
        }

        // TODO: Endianness?
        public static PCMAudioBuffer ReadWAV(string filePath)
        {
            using (var r = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
            {
                var chunkID = r.ReadBytes(4);
                if (!StringUtils.Equals(chunkID, "RIFF"))
                    throw new FileFormatException("Invalid chunk ID.");
                var chunkSize = r.ReadLEUInt32(); // Size of the rest of the chunk after this number.
                var format = r.ReadBytes(4);
                if (!StringUtils.Equals(format, "WAVE"))
                    throw new FileFormatException("Invalid chunk format.");
                var subchunk1ID = r.ReadBytes(4);
                if (!StringUtils.Equals(subchunk1ID, "fmt "))
                    throw new FileFormatException("Invalid subchunk ID.");
                var subchunk1Size = r.ReadLEUInt32(); // Size of rest of subchunk.
                var audioFormat = r.ReadLEUInt16();
                if (audioFormat != 1) // 1 = PCM
                    throw new NotImplementedException("Unsupported audio format.");
                var numChannels = r.ReadLEUInt16();
                var samplingRate = r.ReadLEUInt32(); // # of samples per second (not including all channels).
                var byteRate = r.ReadLEUInt32(); // # of bytes per second (including all channels).
                var blockAlign = r.ReadLEUInt16(); // # of bytes for one sample (including all channels).
                var bitsPerSample = r.ReadLEUInt16(); // # of bits per sample (not including all channels).
                if (subchunk1Size == 18)
                {
                    // Read any extra values.
                    var subchunk1ExtraSize = r.ReadLEUInt16();
                    r.ReadBytes(subchunk1ExtraSize);
                }
                var subchunk2ID = r.ReadBytes(4); // "data"
                if (!StringUtils.Equals(subchunk2ID, "data"))
                    throw new FileFormatException("Invalid subchunk ID.");
                var subchunk2Size = r.ReadLEUInt32(); // Size of rest of subchunk.
                var audioData = r.ReadBytes((int)subchunk2Size);
                return new PCMAudioBuffer((int)numChannels, (int)bitsPerSample, (int)samplingRate, audioData);
            }
        }

        // TODO: Handle exceptions
        public static PCMAudioBuffer ReadMP3(string filePath)
        {
            using (var audioStream = new MP3StreamReader(filePath))
            {
                var audioData = new List<byte>(2 * (int)audioStream.compressedStreamLengthInBytes); // Allocate enough space for a 50% compression ratio.
                var streamBufferSizeInSampleFrames = 16384;
                var streamBuffer = new byte[SampleFramesToBytes(streamBufferSizeInSampleFrames, audioStream.channelCount, audioStream.bitDepth)];
                do
                {
                    var sampleFramesRead = audioStream.ReadSampleFrames(streamBuffer, 0, streamBufferSizeInSampleFrames);
                    if (sampleFramesRead > 0)
                    {
                        var bytesRead = SampleFramesToBytes(sampleFramesRead, audioStream.channelCount, audioStream.bitDepth);
                        audioData.AddRange(new ArrayRange<byte>(streamBuffer, 0, bytesRead));
                    }
                } while (!audioStream.isDoneStreaming);
                return new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, audioData.ToArray());
            }
        }

        /// <summary>
        /// Streams audio into a floating point sample buffer.
        /// </summary>
        /// <param name="unityBuffer"></param>
        /// <param name="intermediateBuffer">A PCM sample buffer to act as an intermediary between the raw audio stream and Unity.</param>
        /// <param name="audioStream"></param>
        /// <returns>Returns the number of samples that were read from the stream.</returns>
        public static int FillUnityStreamBuffer(float[] unityBuffer, PCMAudioBuffer intermediateBuffer, MP3StreamReader audioStream)
        {
            if (audioStream.isDoneStreaming)
            {
                // Fill the Unity sample buffer with zeros.
                Array.Clear(unityBuffer, 0, unityBuffer.Length);
                return 0;
            }
            var totalSampleFramesToRead = unityBuffer.Length / audioStream.channelCount;
            var sampleFramesRead = 0;
            while (sampleFramesRead < totalSampleFramesToRead)
            {
                // Read some sample frames.
                var sampleFramesLeftToRead = totalSampleFramesToRead - sampleFramesRead;
                var sampleFramesReturned = audioStream.ReadSampleFrames(intermediateBuffer.data, 0, Math.Min(sampleFramesLeftToRead, intermediateBuffer.sampleFrameCount));
                if (sampleFramesReturned > 0)
                {
                    // Convert the read samples to floats copy them to the output buffer.
                    intermediateBuffer.ToFloatArray(unityBuffer, sampleFramesRead, sampleFramesReturned);
                    sampleFramesRead += sampleFramesReturned;
                }
                else
                {
                    // Fill the rest of the Unity sample buffer with zeros.
                    var samplesRead = sampleFramesRead * audioStream.channelCount;
                    Array.Clear(unityBuffer, samplesRead, unityBuffer.Length - samplesRead);
                    break;
                }
            }
            return sampleFramesRead * audioStream.channelCount;
        }

        // Quick hack until Unity bugs are fixed.
        public static void ResampleHack(float[] srcSamples, float[] dstSamples)
        {
            var srcSampleFrameCount = srcSamples.Length / 2;
            var dstSampleFrameCount = dstSamples.Length / 2;
            var lastSrcSampleFrameIndex = srcSampleFrameCount - 1;
            var lastDstSampleFrameIndex = dstSampleFrameCount - 1;
            for (var channelIndex = 0; channelIndex < 2; channelIndex++)
                for (var dstSampleFrameIndex = 0; dstSampleFrameIndex <= lastDstSampleFrameIndex; dstSampleFrameIndex++)
                {
                    var dstSmpI = channelIndex + (2 * dstSampleFrameIndex);
                    float sample;
                    if (dstSampleFrameIndex == 0) sample = srcSamples[channelIndex];
                    else if (dstSampleFrameIndex == lastDstSampleFrameIndex) sample = srcSamples[channelIndex + (2 * lastSrcSampleFrameIndex)];
                    else
                    {
                        var iPercent = (float)dstSampleFrameIndex / lastDstSampleFrameIndex;
                        var srcSampleFrameIF = iPercent * lastSrcSampleFrameIndex;
                        var LSrcSampleFrameI = (int)Math.Floor(srcSampleFrameIF);
                        var RSrcSampleFrameI = LSrcSampleFrameI + 1;
                        var t = srcSampleFrameIF - LSrcSampleFrameI;
                        var srcSmp0I = channelIndex + (2 * LSrcSampleFrameI);
                        var srcSmp1I = channelIndex + (2 * RSrcSampleFrameI);
                        sample = Mathf.Lerp(srcSamples[srcSmp0I], srcSamples[srcSmp1I], t);
                    }
                    dstSamples[dstSmpI] = sample;
                }
        }

        public static void LowPassHack(float[] samples)
        {
            var sampleFrameCount = samples.Length / 2;
            for (var channelI = 0; channelI < 2; channelI++)
                for (var sampleFrameI = 0; sampleFrameI < sampleFrameCount - 1; sampleFrameI++)
                {
                    var sample0I = channelI + (2 * sampleFrameI);
                    var sample1I = sample0I + 2;
                    samples[sample0I] = (samples[sample0I] + samples[sample1I]) / 2;
                }
        }

        /// <summary>
        /// Create a Unity audio clip for an audio stream.
        /// </summary>
        private static AudioClip CreateStreamingAudioClip(string name, MP3StreamReader audioStream)
        {
            var streamBuffer = new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, 8192);
            var bufferAudioClipSampleFrameCount = audioStream.samplingRate;
            return AudioClip.Create(name, bufferAudioClipSampleFrameCount, audioStream.channelCount, audioStream.samplingRate, true, delegate (float[] samples)
            {
                var samplesReturned = FillUnityStreamBuffer(samples, streamBuffer, audioStream);
                if (audioStream.isOpen && audioStream.isDoneStreaming)
                    audioStream.Close();
            });
        }
    }

    /// <summary>
    /// Pulse-code modulation (uncompressed samples) audio buffer
    /// </summary>
    public struct PCMAudioBuffer
    {
        public int channelCount;
        public int bitDepth; // bits per sample
        public int samplingRate; // sample frames per second
        public byte[] data; // sample data (channels are interleaved)

        public int bytesPerSample
        {
            get { return bitDepth / 8; }
        }

        public int bytesPerSampleFrame
        {
            get { return channelCount * bytesPerSample; }
        }

        public int sampleFrameCount
        {
            get { return sampleCount / channelCount; }
        }

        public int sampleCount
        {
            get { return data.Length / bytesPerSample; }
        }

        public PCMAudioBuffer(int channelCount, int bitDepth, int samplingRate, int sampleFrameCount)
        {
            this.channelCount = channelCount;
            this.bitDepth = bitDepth;
            this.samplingRate = samplingRate;
            data = null; // Finish assigning values to all members so that properties can be used.
            data = new byte[sampleFrameCount * bytesPerSampleFrame];
        }

        public PCMAudioBuffer(int channelCount, int bitDepth, int samplingRate, byte[] data)
        {
            this.channelCount = channelCount;
            this.bitDepth = bitDepth;
            this.samplingRate = samplingRate;
            this.data = data;
            Debug.Assert((data.Length % bytesPerSampleFrame) == 0);
        }

        public float[] ToFloatArray()
        {
            var floatArray = new float[sampleCount];
            ToFloatArray(floatArray, 0, sampleFrameCount);
            return floatArray;
        }

        // TODO: assert numSampleFrames valid
        public void ToFloatArray(float[] floatArray, int offsetInSampleFrames, int numSampleFrames)
        {
            var offsetInSamples = offsetInSampleFrames * channelCount;
            var numSamples = numSampleFrames * channelCount;
            switch (bitDepth)
            {
                case 8:
                    for (var i = 0; i < numSamples; i++)
                        floatArray[offsetInSamples + i] = (float)(unchecked((sbyte)data[i])) / sbyte.MaxValue;
                    break;
                case 16:
                    for (var i = 0; i < numSamples; i++)
                        floatArray[offsetInSamples + i] = (float)BitConverter.ToInt16(data, 2 * i) / short.MaxValue;
                    break;
                case 32:
                    for (var i = 0; i < numSamples; i++)
                        floatArray[offsetInSamples + i] = BitConverter.ToSingle(data, 4 * i);
                    break;
                case 64:
                    for (var i = 0; i < numSamples; i++)
                        floatArray[offsetInSamples + i] = (float)BitConverter.ToDouble(data, 8 * i);
                    break;
                default: throw new NotImplementedException("Tried to convert a PCMAudioBuffer with an unsupported bit depth (" + bitDepth.ToString() + ") to a float array.");
            }
        }
    }

    // TODO: Handle exceptions
    // TODO: Change MP3 libraries to properly handle mono/stereo.
    public class MP3StreamReader : IDisposable
    {
        private MP3Stream audioStream;

        public readonly int channelCount = 2;
        public readonly int bitDepth = 16; // bits per sample
        public readonly int samplingRate; // sample frames per second
        public readonly long compressedStreamLengthInBytes;

        public bool isDoneStreaming
        {
            get { return !isOpen || audioStream.IsEOF; }
        }

        public bool isOpen
        {
            get { return audioStream != null; }
        }

        public MP3StreamReader(string filePath)
        {
            audioStream = new MP3Stream(filePath);
            samplingRate = audioStream.Frequency;
            compressedStreamLengthInBytes = audioStream.Length;
        }

        public void Close()
        {
            Debug.Assert(isOpen);
            audioStream.Close();
            audioStream = null;
        }

        public void Dispose()
        {
            if (isOpen)
                Close();
        }

        // Returns how many sample frames were actually read.
        public int ReadSampleFrames(byte[] buffer, int offsetInSampleFrames, int sampleFrameCount)
        {
            Debug.Assert(isOpen);
            var offsetInBytes = AudioUtils.SampleFramesToBytes(offsetInSampleFrames, channelCount, bitDepth);
            var requestedByteCount = AudioUtils.SampleFramesToBytes(sampleFrameCount, channelCount, bitDepth);
            var bytesRead = 0;
            int bytesReturned;
            do
            {
                bytesReturned = audioStream.Read(buffer, offsetInBytes + bytesRead, requestedByteCount - bytesRead);
                bytesRead += bytesReturned;
            } while (bytesReturned > 0);
            Debug.Assert((bytesRead % AudioUtils.SampleFramesToBytes(1, channelCount, bitDepth)) == 0);
            // Stereoize audio and fix MP3Sharp's strange behavior.
            if (audioStream.ChannelCount == 1)
            {
                var iEnd = offsetInBytes + bytesRead;
                for (var i = offsetInBytes; i < iEnd; i += 4)
                {
                    buffer[i + 2] = buffer[i];
                    buffer[i + 3] = buffer[i + 1];
                }
            }
            var sampleFramesRead = AudioUtils.BytesToSampleFrames(bytesRead, channelCount, bitDepth);
            return sampleFramesRead;
        }
    }
}