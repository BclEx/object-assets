﻿using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ACRERecord : Record
    {
        public override string ToString() => $"GMST: {EDID.Value}";
        public STRVField EDID { get; set; }
        public REFRRecord.XESPField? XESP; // Enable Parent (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}