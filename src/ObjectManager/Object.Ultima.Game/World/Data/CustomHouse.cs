﻿using OA.Ultima.Resources;
using System.Collections.Generic;

namespace OA.Ultima.World.Data
{
    class CustomHouse
    {
        public Serial Serial;
        public int Hash;

        int _planeCount;
        CustomHousePlane[] _planes;

        public CustomHouse(Serial serial)
        {
            Serial = serial;
        }

        public void Update(int hash, int planecount, CustomHousePlane[] planes)
        {
            Hash = hash;
            _planeCount = planecount;
            _planes = planes;
        }

        public StaticTile[] GetStatics(int width, int height)
        {
            var statics = new List<StaticTile>();

            // Custom Houses are sent in 'planes' of four different types. We determine which type we're looking at the index and the size.
            var sizeFloor = ((width - 1) * (height - 1));
            var sizeWalls = (width * height);
            // There is no z data for most planes, so we have to determine their z by their relative position to preceeding planes of the same type.
            var numTilesInLastPlane = 0;
            var zIndex = 0;
            for (var plane = 0; plane < _planeCount; plane++)
            {
                var numTiles = _planes[plane].ItemData.Length >> 1;
                if (plane == _planeCount - 1 && numTiles != sizeFloor && numTiles != sizeWalls)
                {
                    numTiles = _planes[plane].ItemData.Length / 5;
                    var index = 0;
                    for (var j = 0; j < numTiles; j++)
                    {
                        var s = new StaticTile();
                        s.ID = (short)((_planes[plane].ItemData[index++] << 8) + _planes[plane].ItemData[index++]);
                        var x = (sbyte)_planes[plane].ItemData[index++];
                        var y = (sbyte)_planes[plane].ItemData[index++];
                        var z = (sbyte)_planes[plane].ItemData[index++];
                        s.X = (byte)((width >> 1) + x - 1);
                        s.Y = (byte)((height >> 1) + y);
                        s.Z = (sbyte)z;
                        statics.Add(s);
                    }
                }
                else
                {
                    int iWidth = width, iHeight = height;
                    int iX = 0, iY = 0;
                    int x = 0, y = 0, z = 0;
                    if (plane == 0)
                    {
                        zIndex = 0;
                        iWidth += 1;
                        iHeight += 1;
                    }
                    else if (numTiles == sizeFloor)
                    {
                        if (numTilesInLastPlane != sizeFloor) zIndex = 1;
                        else zIndex++;
                        iWidth -= 1;
                        iHeight -= 1;
                        iX = 1;
                        iY = 1;
                    }
                    else if (numTiles == sizeWalls)
                    {
                        if (numTilesInLastPlane != sizeWalls) zIndex = 1;
                        else zIndex++;
                    }
                    switch (zIndex)
                    {
                        case 0: z = 0; break;
                        case 1: z = 7; break;
                        case 2: z = 27; break;
                        case 3: z = 47; break;
                        case 4: z = 67; break;
                        default: continue;
                    }
                    var index = 0;
                    for (var j = 0; j < numTiles; j++)
                    {
                        var s = new StaticTile();
                        s.ID = (short)((_planes[plane].ItemData[index++] << 8) + _planes[plane].ItemData[index++]);
                        s.X = (byte)(x + iX);
                        s.Y = (byte)(y + iY);
                        s.Z = (sbyte)z;
                        y++;
                        if (y >= iHeight)
                        {
                            y = 0;
                            x++;
                        }
                        statics.Add(s);
                    }
                    numTilesInLastPlane = numTiles;
                }
            }
            return statics.ToArray();
        }
    }
}
