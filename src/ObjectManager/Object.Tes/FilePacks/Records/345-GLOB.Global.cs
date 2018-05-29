﻿using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class GLOBRecord : Record, IHaveEDID
    {
        public override string ToString() => $"GLOB: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public BYTEField? FNAM; // Type of global (s, l, f)
        public FLTVField? FLTV; // Float data

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new BYTEField(r, dataSize); return true;
                case "FLTV": FLTV = new FLTVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}