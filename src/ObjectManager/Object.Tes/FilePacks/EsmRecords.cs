﻿using OA.Core;
using OA.Tes.FilePacks.Records;
using System;
using System.Collections.Generic;

//https://github.com/WrinklyNinja/esplugin/tree/master/src
//http://en.uesp.net/wiki/Tes3Mod:File_Format
//http://en.uesp.net/wiki/Tes4Mod:Mod_File_Format
//http://en.uesp.net/wiki/Tes5Mod:Mod_File_Format
//https://github.com/TES5Edit/TES5Edit/blob/dev/wbDefinitionsTES5.pas 
//http://en.uesp.net/morrow/tech/mw_esm.txt
namespace OA.Tes.FilePacks
{
    [Flags]
    public enum HeaderFlags : uint
    {
        EsmFile = 0x00000001,               // ESM file. (TES4.HEDR record only.)
        Deleted = 0x00000020,               // Deleted
        R00 = 0x00000040,                   // Constant / (REFR) Hidden From Local Map (Needs Confirmation: Related to shields)
        R01 = 0x00000100,                   // Must Update Anims / (REFR) Inaccessible
        R02 = 0x00000200,                   // (REFR) Hidden from local map / (ACHR) Starts dead / (REFR) MotionBlurCastsShadows
        R03 = 0x00000400,                   // Quest item / Persistent reference / (LSCR) Displays in Main Menu
        InitiallyDisabled = 0x00000800,     // Initially disabled
        Ignored = 0x00001000,               // Ignored
        VisibleWhenDistant = 0x00008000,    // Visible when distant
        R04 = 0x00010000,                   // (ACTI) Random Animation Start
        R05 = 0x00020000,                   // (ACTI) Dangerous / Off limits (Interior cell) Dangerous Can't be set withough Ignore Object Interaction
        Compressed = 0x00040000,            // Data is compressed
        CantWait = 0x00080000,              // Can't wait
        // tes5
        R06 = 0x00100000,                   // (ACTI) Ignore Object Interaction Ignore Object Interaction Sets Dangerous Automatically
        IsMarker = 0x00800000,              // Is Marker
        R07 = 0x02000000,                   // (ACTI) Obstacle / (REFR) No AI Acquire
        NavMesh01 = 0x04000000,             // NavMesh Gen - Filter
        NavMesh02 = 0x08000000,             // NavMesh Gen - Bounding Box
        R08 = 0x10000000,                   // (FURN) Must Exit to Talk / (REFR) Reflected By Auto Water
        R09 = 0x20000000,                   // (FURN/IDLM) Child Can Use / (REFR) Don't Havok Settle
        R10 = 0x40000000,                   // NavMesh Gen - Ground / (REFR) NoRespawn
        R11 = 0x80000000,                   // (REFR) MultiBound
    }

    public class Header
    {
        public string Type; // 4 bytes
        public uint DataSize;
        public HeaderFlags Flags;
        public bool Compressed => (Flags & HeaderFlags.Compressed) != 0;
        public uint FormId;
        // group
        public string Label;
        public int GroupType;

        public Header(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            if (Type == "GRUP")
            {
                if (formatId == GameFormatId.Tes4) DataSize = r.ReadLEUInt32() - 20;
                else if (formatId == GameFormatId.Tes5) DataSize = r.ReadLEUInt32() - 24;
                Label = r.ReadASCIIString(4);
                if (formatId == GameFormatId.Tes4) GroupType = r.ReadLEInt32();
                else if (formatId == GameFormatId.Tes5) GroupType = r.ReadLEInt32();
                r.ReadLEUInt32(); // stamp | stamp + uknown
                if (formatId == GameFormatId.Tes4)
                    return;
                r.ReadLEUInt32(); // version + uknown
                return;
            }
            DataSize = r.ReadLEUInt32();
            if (formatId == GameFormatId.Tes3)
                r.ReadLEUInt32(); // Unknown
            Flags = (HeaderFlags)r.ReadLEUInt32();
            if (formatId == GameFormatId.Tes3)
                return;
            // tes4
            FormId = r.ReadLEUInt32();
            r.ReadLEUInt32();
            if (formatId == GameFormatId.Tes4)
                return;
            // tes5
            r.ReadLEUInt32();
        }

        static Dictionary<string, Func<byte, Record>> Create = new Dictionary<string, Func<byte, Record>>
        {
            { "TES3", x => new TES3Record() },
            { "TES4", x => new TES4Record() },
            // 0      
            { "LTEX", x => x > 0 ? new LTEXRecord() : null },
            { "STAT", x => x > 0 ? new STATRecord() : null },
            { "CELL", x => x > 0 ? new CELLRecord() : null },
            { "LAND", x => x > 0 ? new LANDRecord() : null },
            // 1      
            { "DOOR", x => x > 1 ? new DOORRecord() : null },
            { "MISC", x => x > 1 ? new MISCRecord() : null },
            { "WEAP", x => x > 1 ? new WEAPRecord() : null },
            { "CONT", x => x > 1 ? new CONTRecord() : null },
            { "LIGH", x => x > 1 ? new LIGHRecord() : null },
            { "ARMO", x => x > 1 ? new ARMORecord() : null },
            { "CLOT", x => x > 1 ? new CLOTRecord() : null },
            { "REPA", x => x > 1 ? new REPARecord() : null },
            { "ACTI", x => x > 1 ? new ACTIRecord() : null },
            { "APPA", x => x > 1 ? new APPARecord() : null },
            { "LOCK", x => x > 1 ? new LOCKRecord() : null },
            { "PROB", x => x > 1 ? new PROBRecord() : null },
            { "INGR", x => x > 1 ? new INGRRecord() : null },
            { "BOOK", x => x > 1 ? new BOOKRecord() : null },
            { "ALCH", x => x > 1 ? new ALCHRecord() : null },
            { "CREA", x => x > 1 && TesSettings.Game.CreaturesEnabled ? new CREARecord() : null },
            { "NPC_", x => x > 1 && TesSettings.Game.NpcsEnabled ? new NPC_Record() : null },
            // 2      
            { "GMST", x => x > 2 ? new GMSTRecord() : null },
            { "GLOB", x => x > 2 ? new GLOBRecord() : null },
            { "SOUN", x => x > 2 ? new SOUNRecord() : null },
            { "REGN", x => x > 2 ? new REGNRecord() : null },
            // 3
            { "CLAS", x => x > 3 ? new CLASRecord() : null },
            { "SPEL", x => x > 3 ? new SPELRecord() : null },
            { "BODY", x => x > 3 ? new BODYRecord() : null },
            { "PGRD", x => x > 3 ? new PGRDRecord() : null },
            { "INFO", x => x > 3 ? new INFORecord() : null },
            { "DIAL", x => x > 3 ? new DIALRecord() : null },
            { "SNDG", x => x > 3 ? new SNDGRecord() : null },
            { "ENCH", x => x > 3 ? new ENCHRecord() : null },
            { "SCPT", x => x > 3 ? new SCPTRecord() : null },
            { "SKIL", x => x > 3 ? new SKILRecord() : null },
            { "RACE", x => x > 3 ? new RACERecord() : null },
            { "MGEF", x => x > 3 ? new MGEFRecord() : null },
            { "LEVI", x => x > 3 ? new LEVIRecord() : null },
            { "LEVC", x => x > 3 ? new LEVCRecord() : null },
            { "BSGN", x => x > 3 ? new BSGNRecord() : null },
            { "FACT", x => x > 3 ? new FACTRecord() : null },
            // 4 - Oblivion
            { "ACRE", x => x > 4 ? new ACRERecord() : null }, //*
            { "ACHR", x => x > 4 ? new ACHRRecord() : null },
            { "AMMO", x => x > 4 ? new AMMORecord() : null },
            { "ANIO", x => x > 4 ? new ANIORecord() : null },
            { "CLMT", x => x > 4 ? new CLMTRecord() : null },
            { "CSTY", x => x > 4 ? new CSTYRecord() : null },
            { "EFSH", x => x > 4 ? new EFSHRecord() : null },
            { "EYES", x => x > 4 ? new EYESRecord() : null },
            { "FLOR", x => x > 4 ? new FLORRecord() : null },
            { "FURN", x => x > 4 ? new FURNRecord() : null },
            { "GRAS", x => x > 4 ? new GRASRecord() : null },
            { "HAIR", x => x > 4 ? new HAIRRecord() : null }, //*
            { "IDLE", x => x > 4 ? new IDLERecord() : null },
            { "KEYM", x => x > 4 ? new KEYMRecord() : null }, //*
            { "LSCR", x => x > 4 ? new LSCRRecord() : null }, //?
            { "LVLC", x => x > 4 ? new LVLCRecord() : null },
            { "LVLI", x => x > 4 ? new LVLIRecord() : null },
            { "LVSP", x => x > 4 ? new LVSPRecord() : null },
            { "PACK", x => x > 4 ? new PACKRecord() : null },
            { "QUST", x => x > 4 ? new QUSTRecord() : null },
            { "REFR", x => x > 4 ? new REFRRecord() : null },
            { "ROAD", x => x > 4 ? new ROADRecord() : null }, //*
            { "SBSP", x => x > 4 ? new SBSPRecord() : null }, //*
            { "SGST", x => x > 4 ? new SGSTRecord() : null }, //*
            { "SLGM", x => x > 4 ? new SLGMRecord() : null },
            { "TREE", x => x > 4 ? new TREERecord() : null },
            { "WATR", x => x > 4 ? new WATRRecord() : null },
            { "WRLD", x => x > 4 ? new WRLDRecord() : null },
            { "WTHR", x => x > 4 ? new WTHRRecord() : null },

            // 5 - Skyrim
            { "AACT", x => x > 5 ? new AACTRecord() : null },
            { "ADDN", x => x > 5 ? new ADDNRecord() : null },
            { "ARMA", x => x > 5 ? new ARMARecord() : null },
            { "ARTO", x => x > 5 ? new ARTORecord() : null },
            { "ASPC", x => x > 5 ? new ASPCRecord() : null },
            { "ASTP", x => x > 5 ? new ASTPRecord() : null },
            { "AVIF", x => x > 5 ? new AVIFRecord() : null },
            { "DLBR", x => x > 5 ? new DLBRRecord() : null },
            { "DLVW", x => x > 5 ? new DLVWRecord() : null },
            { "SNDR", x => x > 5 ? new SNDRRecord() : null },
            //

        };

        public Record CreateRecord(long position, byte level)
        {
            if (Create.TryGetValue(Type, out var func))
            {
                var r = func(level);
                if (r != null)
                {
                    r.Position = position;
                    r.Header = this;
                }
                return r;
            }
            Utils.Warning($"Unsupported ESM record type: {Type}");
            return null;
        }
    }

    public abstract class Record : IRecord
    {
        internal long Position;
        internal Header Header;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract bool CreateField(UnityBinaryReader r, string type, uint dataSize);

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
        public abstract bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize);

        public void Read(UnityBinaryReader r, string filePath, GameFormatId formatId)
        {
            var endPosition = r.BaseStream.Position + Header.DataSize;
            while (r.BaseStream.Position < endPosition)
            {
                var header = new FieldHeader(r, formatId);
                var position = r.BaseStream.Position;
                var skipField = formatId != GameFormatId.Tes3 ? !CreateField(r, formatId, header.Type, header.DataSize) : !CreateField(r, header.Type, header.DataSize);
                if (skipField)
                {
                    Utils.Warning($"Unsupported ESM record type: {Header.Type} {header.Type}");
                    r.BaseStream.Position += header.DataSize;
                    continue;
                }
                // check full read
                if (r.BaseStream.Position != position + header.DataSize)
                    throw new FormatException($"Failed reading {header.Type} field data at offset {position} in {filePath}");
            }
            // check full read
            if (r.BaseStream.Position != Position + Header.DataSize)
                throw new FormatException($"Failed reading {Header.Type} record data at offset {Position} in {filePath}");
        }
    }

    public class FieldHeader
    {
        public string Type; // 4 bytes
        public uint DataSize;

        public FieldHeader(UnityBinaryReader r, GameFormatId formatId)
        {
            Type = r.ReadASCIIString(4);
            if (formatId == GameFormatId.Tes3) DataSize = r.ReadLEUInt32();
            else DataSize = r.ReadLEUInt16();
        }
    }
}
