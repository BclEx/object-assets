﻿using System;
using UnityEngine;

namespace OA.Ultima
{
    public static class ConvertUtils
    {
        public const int yardInMWUnits = 64;
        public const float meterInYards = 1.09361f;
        public const float meterInMWUnits = meterInYards * yardInMWUnits;

        public const int exteriorCellSideLengthInMWUnits = 8192;
        public const float exteriorCellSideLengthInMeters = (float)exteriorCellSideLengthInMWUnits / meterInMWUnits;

        public static Quaternion RotationMatrixToQuaternion(Matrix4x4 matrix)
        {
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));
        }

        public static uint FromBGR555(ushort bgr555, bool addAlpha = true)
        {
            //return (uint)bgr555;
            byte a = addAlpha ? (byte)0xFF : (byte)0;
            byte r = (byte)Math.Min(((bgr555 & 0x7C00) >> 10) * 8, byte.MaxValue);
            byte g = (byte)Math.Min(((bgr555 & 0x03E0) >> 5) * 8, byte.MaxValue);
            byte b = (byte)Math.Min(((bgr555 & 0x001F) >> 0) * 8, byte.MaxValue);
            uint color =
                ((uint)(a << 24) & 0xFF000000) |
                ((uint)(r << 16) & 0x00FF0000) |
                ((uint)(g << 8) & 0x0000FF00) |
                ((uint)(b << 0) & 0x000000FF);
            return color;
        }
    }
}