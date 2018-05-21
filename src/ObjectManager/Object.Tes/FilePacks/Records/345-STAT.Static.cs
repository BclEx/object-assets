﻿using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class STATRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"STAT: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}