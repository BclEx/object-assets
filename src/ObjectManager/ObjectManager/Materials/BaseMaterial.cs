﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace OA.Materials
{
    /// <summary>
    /// An abstract class to describe a material.
    /// </summary>
    public abstract class BaseMaterial
    {
        protected Dictionary<MaterialProps, Material> _existingMaterials;
        protected TextureManager _textureManager;

        public BaseMaterial(TextureManager textureManager)
        {
            _textureManager = textureManager;
            _existingMaterials = new Dictionary<MaterialProps, Material>();
        }

        public abstract Material BuildMaterialFromProperties(MaterialProps mp);
        public abstract Material BuildMaterial();
        public abstract Material BuildMaterialBlended(BlendMode sourceBlendMode, BlendMode destinationBlendMode);
        public abstract Material BuildMaterialTested(float cutoff = 0.5f);

        // https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
        protected static Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 1.0F);
            Texture2D normalTexture;
            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;
            normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
            for (var y = 0; y < normalTexture.height; y++)
                for (var x = 0; x < normalTexture.width; x++)
                {
                    xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                    xRight = source.GetPixel(x + 1, y).grayscale * strength;
                    yUp = source.GetPixel(x, y - 1).grayscale * strength;
                    yDown = source.GetPixel(x, y + 1).grayscale * strength;
                    xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    yDelta = ((yUp - yDown) + 1) * 0.5f;
                    normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
                }
            normalTexture.Apply();
            return normalTexture;
        }
    }
}
