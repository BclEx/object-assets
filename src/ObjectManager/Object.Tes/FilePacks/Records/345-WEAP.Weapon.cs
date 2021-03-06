﻿using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class WEAPRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct DATAField
        {
            public enum WEAPType
            {
                ShortBladeOneHand = 0, LongBladeOneHand, LongBladeTwoClose, BluntOneHand, BluntTwoClose, BluntTwoWide, SpearTwoWide, AxeOneHand, AxeTwoHand, MarksmanBow, MarksmanCrossbow, MarksmanThrown, Arrow, Bolt,
            }

            public float Weight;
            public int Value;
            public ushort Type;
            public short Health;
            public float Speed;
            public float Reach;
            public short Damage; //: EnchantPts;
            public byte ChopMin;
            public byte ChopMax;
            public byte SlashMin;
            public byte SlashMax;
            public byte ThrustMin;
            public byte ThrustMax;
            public int Flags; // 0 = ?, 1 = Ignore Normal Weapon Resistance?

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                if (format == GameFormatId.TES3)
                {
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEInt32();
                    Type = r.ReadLEUInt16();
                    Health = r.ReadLEInt16();
                    Speed = r.ReadLESingle();
                    Reach = r.ReadLESingle();
                    Damage = r.ReadLEInt16();
                    ChopMin = r.ReadByte();
                    ChopMax = r.ReadByte();
                    SlashMin = r.ReadByte();
                    SlashMax = r.ReadByte();
                    ThrustMin = r.ReadByte();
                    ThrustMax = r.ReadByte();
                    Flags = r.ReadLEInt32();
                    return;
                }
                Type = (ushort)r.ReadLEUInt32();
                Speed = r.ReadLESingle();
                Reach = r.ReadLESingle();
                Flags = r.ReadLEInt32();
                Value = r.ReadLEInt32();
                Health = (short)r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Damage = r.ReadLEInt16();
                ChopMin = ChopMax = SlashMin = SlashMax = ThrustMin = ThrustMax = 0;
            }
        }

        public override string ToString() => $"WEAP: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField FULL; // Item Name
        public DATAField DATA; // Weapon Data
        public FILEField ICON; // Male Icon (optional)
        public FMIDField<ENCHRecord> ENAM; // Enchantment ID
        public FMIDField<SCPTRecord> SCRI; // Script (optional)
        // TES4
        public IN16Field? ANAM; // Enchantment points (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = r.ReadSTRV(dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL":
                case "FNAM": FULL = r.ReadSTRV(dataSize); return true;
                case "DATA":
                case "WPDT": DATA = new DATAField(r, dataSize, format); return true;
                case "ICON":
                case "ITEX": ICON = r.ReadFILE(dataSize); return true;
                case "ENAM": ENAM = new FMIDField<ENCHRecord>(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "ANAM": ANAM = r.ReadT<IN16Field>(dataSize); return true;
                default: return false;
            }
        }
    }
}