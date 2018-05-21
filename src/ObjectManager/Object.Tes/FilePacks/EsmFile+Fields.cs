﻿using OA.Core;
using System;
using UnityEngine;

namespace OA.Tes.FilePacks
{
    public interface IHaveEDID
    {
        STRVField EDID { get; }
    }

    public interface IHaveMODL
    {
        MODLGroup MODL { get; }
    }

    public class MODLGroup
    {
        public override string ToString() => $"{Value}";
        public string Value;
        public float Bound;
        public byte[] Textures; // Texture Files Hashes

        public MODLGroup(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }

        public void MODBField(UnityBinaryReader r, uint dataSize)
        {
            Bound = r.ReadLESingle();
        }

        public void MODTField(UnityBinaryReader r, uint dataSize)
        {
            Textures = r.ReadBytes((int)dataSize);
        }

    }

    public struct STRVField
    {
        public override string ToString() => $"{Value}";
        public string Value;

        public STRVField(UnityBinaryReader r, uint dataSize, ASCIIFormat format = ASCIIFormat.PossiblyNullTerminated)
        {
            Value = r.ReadASCIIString((int)dataSize, format);
        }
    }

    public struct FILEField
    {
        public override string ToString() => $"{Value}";
        public string Value;

        public FILEField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }
    }

    public struct INTVField
    {
        public override string ToString() => $"{Value}";
        public long Value;

        public INTVField(UnityBinaryReader r, uint dataSize)
        {
            switch (dataSize)
            {
                case 1: Value = r.ReadByte(); break;
                case 2: Value = r.ReadLEInt16(); break;
                case 4: Value = r.ReadLEInt32(); break;
                case 8: Value = r.ReadLEInt64(); break;
                default: throw new NotImplementedException($"Tried to read an INTV subrecord with an unsupported size ({dataSize})");
            }
        }
        public UI16Field ToUI16Field() => new UI16Field { Value = (ushort)Value };
    }

    //[StructLayout(LayoutKind.Explicit)]
    public struct DATVField
    {
        //[FieldOffset(0)]
        public bool ValueB;
        //[FieldOffset(0)]
        public int ValueI;
        //[FieldOffset(0)]
        public float ValueF;
        //[FieldOffset(0)]
        public string ValueS;

        public static DATVField Create(UnityBinaryReader r, uint dataSize, char type)
        {
            switch (type)
            {
                case 'b': return new DATVField { ValueB = r.ReadLEInt32() != 0 };
                case 'i': return new DATVField { ValueI = r.ReadLEInt32() };
                case 'f': return new DATVField { ValueF = r.ReadLESingle() };
                case 's': return new DATVField { ValueS = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated) };
                default: throw new InvalidOperationException();
            }
        }
    }

    public struct FLTVField
    {
        public override string ToString() => $"{Value}";
        public float Value;

        public FLTVField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLESingle();
        }
    }

    public struct BYTEField
    {
        public override string ToString() => $"{Value}";
        public byte Value;

        public BYTEField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadByte();
        }
    }

    public struct IN16Field
    {
        public override string ToString() => $"{Value}";
        public short Value;

        public IN16Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt16();
        }
    }

    public struct UI16Field
    {
        public override string ToString() => $"{Value}";
        public ushort Value;

        public UI16Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEUInt16();
        }
    }

    public struct IN32Field
    {
        public override string ToString() => $"{Value}";
        public int Value;

        public IN32Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt32();
        }
    }

    public struct UI32Field
    {
        public override string ToString() => $"{Value}";
        public uint Value;

        public UI32Field(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEUInt32();
        }
    }

    public struct FormId<TRecord>
        where TRecord : Record
    {
        public override string ToString() => $"{Type}:{Id}";
        public readonly uint Id;
        public readonly string Name;
        public string Type => typeof(TRecord).Name.Substring(0, 4);

        public FormId(uint id) { Id = id; Name = null; }
        public FormId(string name) { Id = 0; Name = name; }
        FormId(uint id, string name) { Id = id; Name = name; }
        public FormId<TRecord> AddName(string name) => new FormId<TRecord>(Id, name);
    }

    public struct FMIDField<TRecord>
        where TRecord : Record
    {
        public override string ToString() => $"{Value}";
        public FormId<TRecord> Value;

        public FMIDField(UnityBinaryReader r, uint dataSize)
        {
            Value = dataSize == 4 ? new FormId<TRecord>(r.ReadLEUInt32()) : new FormId<TRecord>(r.ReadASCIIString((int)dataSize, ASCIIFormat.ZeroPadded));
        }

        public void AddName(string name)
        {
            Value = Value.AddName(name);
        }
    }

    public struct FMID2Field<TRecord>
       where TRecord : Record
    {
        public override string ToString() => $"{Value1}x{Value1}";
        public FormId<TRecord> Value1;
        public FormId<TRecord> Value2;

        public FMID2Field(UnityBinaryReader r, uint dataSize)
        {
            Value1 = new FormId<TRecord>(r.ReadLEUInt32());
            Value2 = new FormId<TRecord>(r.ReadLEUInt32());
        }
    }

    public struct ColorRef
    {
        public override string ToString() => $"{Red}:{Green}:{Blue}";
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte NullByte;

        public ColorRef(UnityBinaryReader r)
        {
            Red = r.ReadByte();
            Green = r.ReadByte();
            Blue = r.ReadByte();
            NullByte = r.ReadByte();
        }

        public Color32 ToColor32() => new Color32(Red, Green, Blue, 255);
    }

    public struct CREFField
    {
        public ColorRef Color;

        public CREFField(UnityBinaryReader r, uint dataSize)
        {
            Color = new ColorRef(r);
        }
    }

    public struct CNTOField
    {
        public override string ToString() => $"{Item.Name ?? Item.Id.ToString()}";
        public uint ItemCount; // Number of the item
        public FormId<Record> Item; // The ID of the item

        public CNTOField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.Tes3)
            {
                ItemCount = r.ReadLEUInt32();
                Item = new FormId<Record>(r.ReadASCIIString(32, ASCIIFormat.ZeroPadded));
                return;
            }
            Item = new FormId<Record>(r.ReadLEUInt32());
            ItemCount = r.ReadLEUInt32();
        }
    }

    public struct BYTVField
    {
        public override string ToString() => $"BYTS";
        public byte[] Value;

        public BYTVField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadBytes((int)dataSize);
        }
    }

    public struct UNKNField
    {
        public override string ToString() => $"UNKN";
        public byte[] Value;

        public UNKNField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadBytes((int)dataSize);
        }
    }
}