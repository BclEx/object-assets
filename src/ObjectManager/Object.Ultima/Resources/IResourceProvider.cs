﻿using OA.Core;
using OA.Ultima.Core.UI;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public interface IResourceProvider
    {
        AAnimationFrame[] GetAnimation(int body, ref int hue, int action, int direction);
        Texture2DInfo GetUITexture(int textureID, bool replaceMask080808 = false);
        Texture2DInfo GetItemTexture(int textureID);
        Texture2DInfo GetLandTexture(int textureID);
        Texture2DInfo GetTexmapTexture(int textureID);

        bool IsPointInUITexture(int textureID, int x, int y);
        bool IsPointInItemTexture(int textureID, int x, int y, int extraRange = 0);
        void GetItemDimensions(int textureID, out int width, out int height);

        ushort GetWebSafeHue(Color32 color);
        IFont GetUnicodeFont(int fontIndex);
        IFont GetAsciiFont(int fontIndex);
        string GetString(int strIndex);

        void RegisterResource<T>(IResource<T> resource);
        T GetResource<T>(int resourceIndex);
    }
}
