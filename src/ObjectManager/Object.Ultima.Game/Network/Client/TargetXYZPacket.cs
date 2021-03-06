﻿using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class TargetXYZPacket : SendPacket
    {
        public TargetXYZPacket(short x, short y, short z, ushort modelNumber, int cursorID, byte targetType)
            : base(0x6C, "Target XYZ", 19)
        {
            Stream.Write((byte)0x01); // BYTE[1] type: 0x00 = Select Object; 0x01 = Select X, Y, Z
            Stream.Write(cursorID); // BYTE[4] cursorID 
            Stream.Write(targetType); // BYTE[1] Cursor Type; 3 to cancel.
            Stream.Write((int)0x00); // BYTE[4] Clicked On ID. Not used in this packet.
            Stream.Write(x); // BYTE[2] click xLoc
            Stream.Write(y); // BYTE[2] click yLoc
            Stream.Write(z); // BYTE click zLoc
            Stream.Write(modelNumber); // BYTE[2] model # (if a static tile, 0 if a map/landscape tile)
        }
    }
}
