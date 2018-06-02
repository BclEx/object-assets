﻿//
//  LSCRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LSCRRecord: Record {
    public struct LNAMField
    {
        public FormId<Record> Direct;
        public FormId<WRLDRecord> IndirectWorld;
        public short IndirectGridX;
        public short IndirectGridY;

        public LNAMField(UnityBinaryReader r, uint dataSize)
        {
            Direct = new FormId<Record>(r.ReadLEUInt32());
            //if (dataSize == 0)
            IndirectWorld = new FormId<WRLDRecord>(r.ReadLEUInt32());
            //if (dataSize == 0)
            IndirectGridX = r.ReadLEInt16();
            IndirectGridY = r.ReadLEInt16();
        }
    }

    public override string ToString() => $"LSCR: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public FILEField ICON; // Icon
    public STRVField DESC; // Description
    public List<LNAMField> LNAMs; // LoadForm

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "ICON": ICON = new FILEField(r, dataSize); return true;
            case "DESC": DESC = new STRVField(r, dataSize); return true;
            case "LNAM": if (LNAMs == null) LNAMs = new List<LNAMField>(); LNAMs.Add(new LNAMField(r, dataSize)); return true;
            default: return false;
        }
    }
}
