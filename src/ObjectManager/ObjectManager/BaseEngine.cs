﻿using OA.Components;
using OA.Configuration;
using OA.Core;
using OA.UI;
using System;
using System.IO;
using UnityEngine;

namespace OA
{
    public class BaseEngine
    {
        const float DesiredWorkTimePerFrame = 1.0f / 200;
        protected const int CellRadiusOnLoad = 2;
        public static BaseEngine Instance;

        public readonly IAssetManager AssetManager;
        public readonly IAssetPack Asset;
        public readonly IDataPack Data;
        public readonly ICellManager CellManager;
        public readonly TemporalLoadBalancer LoadBalancer;
        readonly GameObject _sunObj;

        public BaseEngine(IAssetManager assetManager, Uri asset, Uri data)
            : this(assetManager, assetManager.GetAssetPack(asset).Result, assetManager.GetDataPack(data).Result) { }
        public BaseEngine(IAssetManager assetManager, IAssetPack asset, IDataPack data)
        {
            AssetManager = assetManager;
            Asset = asset;
            Data = data;

            LoadBalancer = new TemporalLoadBalancer();
            CellManager = assetManager.GetCellManager(asset, data, LoadBalancer);
            var game = BaseSettings.Game;

            // ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = game.AmbientIntensity;

            // sun
            _sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            _sunObj.GetComponent<Light>().shadows = game.RenderSunShadows ? LightShadows.Soft : LightShadows.None;
            _sunObj.SetActive(false);
            if (game.DayNightCycle)
                _sunObj.AddComponent<DayNightCycle>();

            //// water
            //_waterObj = GameObject.Instantiate(TesGame.instance.WaterPrefab);
            //_waterObj.SetActive(false);
            //var water = _waterObj.GetComponent<Water>();
            //water.waterMode = game.instance.WaterQuality;
            //if (!TesGame.instance.WaterBackSideTransparent)
            //{
            //    var side = _waterObj.transform.GetChild(0);
            //    var sideMaterial = side.GetComponent<Renderer>().sharedMaterial;
            //    sideMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            //    sideMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            //    sideMaterial.SetInt("_ZWrite", 1);
            //    sideMaterial.DisableKeyword("_ALPHATEST_ON");
            //    sideMaterial.DisableKeyword("_ALPHABLEND_ON");
            //    sideMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //    sideMaterial.renderQueue = -1;
            //}

            //Cursor.SetCursor(Asset.LoadTexture("tx_cursor", 1), Vector2.zero, CursorMode.Auto);
        }

        #region Player Spawn

        protected int _currentWorld;
        protected ICellRecord _currentCell;
        protected Transform _playerTransform;
        protected PlayerComponent _playerComponent;
        protected GameObject _playerCameraObj;

        Color32 _defaultAmbientColor = new Color32(137, 140, 160, 255);
        //GameObject _waterObj;
        //UnderwaterEffect _underwaterEffect;

        protected virtual GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }
            player.transform.position = position;
            _playerTransform = player.GetComponent<Transform>();
            var cameraInPlayer = player.GetComponentInChildren<Camera>();
            if (cameraInPlayer == null)
                throw new InvalidOperationException("Player:Camera missing");
            playerCamera = cameraInPlayer.gameObject;
            _playerComponent = player.GetComponent<PlayerComponent>();
            //_underwaterEffect = playerCamera.GetComponent<UnderwaterEffect>();
            return player;
        }

        /// <summary>
        /// Spawns the player inside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayer(GameObject playerPrefab, Vector3 position)
        {
            var cellId = CellManager.GetCellId(position, _currentWorld);
            _currentCell = Data.FindCellRecord(cellId);
            Debug.Assert(_currentCell != null);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            var cellInfo = CellManager.StartCreatingCell(cellId);
            LoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
            if (cellId.z != -1) OnExteriorCell(_currentCell);
            else OnInteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player outside using the position of the player.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerAndUpdate(GameObject playerPrefab, Vector3 position)
        {
            var cellId = CellManager.GetCellId(position, _currentWorld);
            _currentCell = Data.FindCellRecord(cellId);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            CellManager.UpdateCells(_playerCameraObj.transform.position, _currentWorld, true, CellRadiusOnLoad);
            OnExteriorCell(_currentCell);
        }

        protected virtual void OnExteriorCell(ICellRecord CELL)
        {
            RenderSettings.ambientLight = _defaultAmbientColor;
            _sunObj.SetActive(true);
            //_waterObj.transform.position = Vector3.zero;
            //_waterObj.SetActive(true);
            //_underwaterEffect.enabled = true;
            //_underwaterEffect.Level = 0.0f;
        }

        protected virtual void OnInteriorCell(ICellRecord CELL)
        {
            var cellAmbientLight = CELL.AmbientLight;
            if (cellAmbientLight != null)
                RenderSettings.ambientLight = cellAmbientLight.Value;
            _sunObj.SetActive(false);
            //_underwaterEffect.enabled = CELL.WHGT != null;
            //if (CELL.WHGT != null)
            //{
            //    var offset = 1.6f; // Interiors cells needs this offset to render at the correct location.
            //    _waterObj.transform.position = new Vector3(0, (CELL.WHGT.value / Convert.meterInMWUnits) - offset, 0);
            //    _waterObj.SetActive(true);
            //    _underwaterEffect.Level = _waterObj.transform.position.y;
            //}
            //else _waterObj.SetActive(false);
        }

        #endregion

        public virtual void Update()
        {
            // The current cell can be null if the player is outside of the defined game world.
            if (_currentCell == null || !_currentCell.IsInterior)
                CellManager.UpdateCells(_playerCameraObj.transform.position, _currentWorld);
            LoadBalancer.RunTasks(DesiredWorkTimePerFrame);
        }
    }
}
