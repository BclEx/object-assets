﻿using OA.Ultima.FilePacks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Ultima.Formats
{
    public class SifManager
    {
        readonly AssetFile _asset;
        readonly MaterialManager _materialManager;
        GameObject _prefabContainerObj;
        readonly Dictionary<string, Task<object>> _staPreloadTasks = new Dictionary<string, Task<object>>();
        readonly Dictionary<string, GameObject> _staPrefabs = new Dictionary<string, GameObject>();

        public SifManager(AssetFile asset, MaterialManager materialManager)
        {
            _asset = asset;
            _materialManager = materialManager;
        }

        public GameObject InstantiateSta(string filePath)
        {
            EnsurePrefabContainerObjectExists();
            // Get the prefab.
            if (!_staPrefabs.TryGetValue(filePath, out GameObject prefab))
            {
                // Load & cache the STA prefab.
                prefab = LoadStaPrefabDontAddToPrefabCache(filePath);
                _staPrefabs[filePath] = prefab;
            }
            // Instantiate the prefab.
            return GameObject.Instantiate(prefab);
        }

        public void PreloadStaFileAsync(string filePath)
        {
            // If the STA prefab has already been created we don't have to load the file again.
            if (_staPrefabs.ContainsKey(filePath)) return;
            // Start loading the STA asynchronously if we haven't already started.
            if (!_staPreloadTasks.TryGetValue(filePath, out Task<object> staLoadingTask))
            {
                staLoadingTask = _asset.LoadObjectInfoAsync(filePath);
                _staPreloadTasks[filePath] = staLoadingTask;
            }
        }

        private void EnsurePrefabContainerObjectExists()
        {
            if (_prefabContainerObj == null)
            {
                _prefabContainerObj = new GameObject("STA Prefabs");
                _prefabContainerObj.SetActive(false);
            }
        }

        private GameObject LoadStaPrefabDontAddToPrefabCache(string filePath)
        {
            Debug.Assert(!_staPrefabs.ContainsKey(filePath));
            PreloadStaFileAsync(filePath);
            var file = (SiFile)_staPreloadTasks[filePath].Result;
            _staPreloadTasks.Remove(filePath);
            // Start pre-loading all the STA's textures.
            foreach (var saObject in file.Blocks)
                if (saObject is SiSourceTexture stSourceTexture)
                    if (!string.IsNullOrEmpty(stSourceTexture.FilePath))
                        _materialManager.TextureManager.PreloadTextureFileAsync(stSourceTexture.FilePath);
            var objBuilder = new SifObjectBuilder(file, _materialManager, 0);
            var prefab = objBuilder.BuildObject();
            prefab.transform.parent = _prefabContainerObj.transform;
            // Add LOD support to the prefab.
            var LODComponent = prefab.AddComponent<LODGroup>();
            var LODs = new LOD[1]
            {
                //new LOD(0.015F, prefab.GetComponentsInChildren<Renderer>())
                new LOD(0F, prefab.GetComponentsInChildren<Renderer>())
            };
            LODComponent.SetLODs(LODs);
            return prefab;
        }
    }
}