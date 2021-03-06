﻿using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.World.Entities.Mobiles;

namespace OA.Ultima.Network.Server
{
    public class MobileMovingPacket : RecvPacket
    {
        readonly Serial serial;
        readonly ushort bodyid;
        readonly short x;
        readonly short y;
        readonly sbyte z;
        readonly byte direction;
        readonly ushort hue;
        public readonly MobileFlags Flags;
        readonly byte notoriety;

        public Serial Serial 
        {
            get { return serial; }
        }

        public ushort BodyID 
        {
            get { return bodyid; }
        }

        public short X
        {
            get { return x; }
        }

        public short Y 
        {
            get { return y; } 
        }

        public sbyte Z 
        {
            get { return z; } 
        }

        public byte Direction
        {
            get { return direction; }
        }

        public ushort Hue
        {
            get { return hue; }
        }

        public byte Notoriety
        {
            get { return notoriety; }
        }

        public MobileMovingPacket(PacketReader reader)
            : base(0x77, "Mobile Moving")
        {
            serial = reader.ReadInt32();
            bodyid = reader.ReadUInt16();
            x = reader.ReadInt16();
            y = reader.ReadInt16();
            z = reader.ReadSByte();
            direction = reader.ReadByte();
            hue = reader.ReadUInt16();
            Flags = new MobileFlags((MobileFlag)reader.ReadByte());
            notoriety = reader.ReadByte();
        }
    }
}
