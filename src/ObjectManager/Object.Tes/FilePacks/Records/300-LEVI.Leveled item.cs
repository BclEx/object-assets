﻿using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LEVIRecord : Record
    {
        public override string ToString() => $"LEVI: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public IN32Field DATA; // List data - 1 = Calc from all levels <= PC level, 2 = Calc for each item
        public BYTEField NNAM; // Chance None?
        public IN32Field INDX; // Number of items in list
        public List<STRVField> INAMs = new List<STRVField>(); // ID string of list item
        public List<IN16Field> INTVs = new List<IN16Field>(); // PC level for previous INAM
        // The CNAM/INTV can occur many times in pairs

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            if (format == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "DATA": DATA = new IN32Field(r, dataSize); return true;
                    case "NNAM": NNAM = new BYTEField(r, dataSize); return true;
                    case "INDX": INDX = new IN32Field(r, dataSize); return true;
                    case "INAM": INAMs.Add(new STRVField(r, dataSize)); return true;
                    case "INTV": INTVs.Add(new IN16Field(r, dataSize)); return true;
                    default: return false;
                }
            return false;
        }
    }
}