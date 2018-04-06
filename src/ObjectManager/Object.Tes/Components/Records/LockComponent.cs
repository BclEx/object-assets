﻿using OA.Bae.Esm;

namespace OA.Bae.Components.Records
{
    public class LockComponent : GenericObjectComponent
    {
        void Start()
        {
            usable = true;
            pickable = false;
            var LOCK = (LOCKRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = LOCK.FNAM.value;
            objData.weight = LOCK.LKDT.weight.ToString();
            objData.value = LOCK.LKDT.value.ToString();
        }
    }
}