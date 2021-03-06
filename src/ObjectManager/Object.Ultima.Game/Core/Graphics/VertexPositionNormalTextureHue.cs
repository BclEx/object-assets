﻿using UnityEngine;

namespace OA.Ultima.Core.Graphics
{
    public struct VertexPositionNormalTextureHue // : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 TextureCoordinate;
        public Vector3 Hue;

        //public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        //(
        //    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), // position
        //    new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), // normal
        //    new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0), // tex coord
        //    new VertexElement(sizeof(float) * 9, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 1) // hue
        //);

        //VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionNormalTextureHue(Vector3 position, Vector3 normal, Vector3 textureCoordinate)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoordinate;
            Hue = Vector3.zero;
        }

        public static readonly VertexPositionNormalTextureHue[] PolyBuffer = {
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
        };

        public static readonly VertexPositionNormalTextureHue[] PolyBufferFlipped = {
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
        };

        public static int SizeInBytes { get { return sizeof(float) * 12; } }

        public override string ToString()
        {
            return string.Format("VPNTH: <{0}> <{1}>", Position.ToString(), TextureCoordinate.ToString());
        }
    }
}
