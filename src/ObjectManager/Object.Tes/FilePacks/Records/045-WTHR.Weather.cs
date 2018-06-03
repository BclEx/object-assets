﻿using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class WTHRRecord : Record
    {
        public struct FNAMField
        {
            public float DayNear;
            public float DayFar;
            public float NightNear;
            public float NightFar;

            public FNAMField(UnityBinaryReader r, int dataSize)
            {
                DayNear = r.ReadLESingle();
                DayFar = r.ReadLESingle();
                NightNear = r.ReadLESingle();
                NightFar = r.ReadLESingle();
            }
        }

        public struct HNAMField
        {
            public float EyeAdaptSpeed;
            public float BlurRadius;
            public float BlurPasses;
            public float EmissiveMult;
            public float TargetLUM;
            public float UpperLUMClamp;
            public float BrightScale;
            public float BrightClamp;
            public float LUMRampNoTex;
            public float LUMRampMin;
            public float LUMRampMax;
            public float SunlightDimmer;
            public float GrassDimmer;
            public float TreeDimmer;

            public HNAMField(UnityBinaryReader r, int dataSize)
            {
                EyeAdaptSpeed = r.ReadLESingle();
                BlurRadius = r.ReadLESingle();
                BlurPasses = r.ReadLESingle();
                EmissiveMult = r.ReadLESingle();
                TargetLUM = r.ReadLESingle();
                UpperLUMClamp = r.ReadLESingle();
                BrightScale = r.ReadLESingle();
                BrightClamp = r.ReadLESingle();
                LUMRampNoTex = r.ReadLESingle();
                LUMRampMin = r.ReadLESingle();
                LUMRampMax = r.ReadLESingle();
                SunlightDimmer = r.ReadLESingle();
                GrassDimmer = r.ReadLESingle();
                TreeDimmer = r.ReadLESingle();
            }
        }

        public struct DATAField
        {
            public byte WindSpeed;
            public byte CloudSpeed_Lower;
            public byte CloudSpeed_Upper;
            public byte TransDelta;
            public byte SunGlare;
            public byte SunDamage;
            public byte Precipitation_BeginFadeIn;
            public byte Precipitation_EndFadeOut;
            public byte ThunderLightning_BeginFadeIn;
            public byte ThunderLightning_EndFadeOut;
            public byte ThunderLightning_Frequency;
            public byte WeatherClassification;
            public ColorRef LightningColor;

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                WindSpeed = r.ReadByte();
                CloudSpeed_Lower = r.ReadByte();
                CloudSpeed_Upper = r.ReadByte();
                TransDelta = r.ReadByte();
                SunGlare = r.ReadByte();
                SunDamage = r.ReadByte();
                Precipitation_BeginFadeIn = r.ReadByte();
                Precipitation_EndFadeOut = r.ReadByte();
                ThunderLightning_BeginFadeIn = r.ReadByte();
                ThunderLightning_EndFadeOut = r.ReadByte();
                ThunderLightning_Frequency = r.ReadByte();
                WeatherClassification = r.ReadByte();
                LightningColor = new ColorRef { Red = r.ReadByte(), Green = r.ReadByte(), Blue = r.ReadByte(), NullByte = 255 };
            }
        }

        public struct SNAMField
        {
            public FormId<SOUNRecord> Sound; // Sound FormId
            public uint Type; // Sound Type - 0=Default, 1=Precipitation, 2=Wind, 3=Thunder

            public SNAMField(UnityBinaryReader r, int dataSize)
            {
                Sound = new FormId<SOUNRecord>(r.ReadLEUInt32());
                Type = r.ReadLEUInt32();
            }
        }

        public override string ToString() => $"WTHR: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public FILEField CNAM; // Lower Cloud Layer
        public FILEField DNAM; // Upper Cloud Layer
        public BYTVField NAM0; // Colors by Types/Times
        public FNAMField FNAM; // Fog Distance
        public HNAMField HNAM; // HDR Data
        public DATAField DATA; // Weather Data
        public List<SNAMField> SNAMs = new List<SNAMField>(); // Sounds

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "CNAM": CNAM = new FILEField(r, dataSize); return true;
                case "DNAM": DNAM = new FILEField(r, dataSize); return true;
                case "NAM0": NAM0 = new BYTVField(r, dataSize); return true;
                case "FNAM": FNAM = new FNAMField(r, dataSize); return true;
                case "HNAM": HNAM = new HNAMField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                case "SNAM": SNAMs.Add(new SNAMField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}