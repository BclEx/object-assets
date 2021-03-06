﻿using OA.Core;
using OA.Tes.FilePacks.Components;
using OA.Tes.FilePacks.Records;
using OA.Tes.UI;
using System;
using UnityEngine;

namespace OA.Tes
{
    public class TesEngine : BaseEngine
    {
        const float MaxInteractDistance = 3;
        public TesUIManager UIManager;

        public TesEngine(IAssetManager assetManager, Uri asset, Uri data, TesUIManager uiManager)
            : this(assetManager, assetManager.GetAssetPack(asset).Result, assetManager.GetDataPack(data).Result, uiManager) { }
        public TesEngine(IAssetManager assetManager, IAssetPack asset, IDataPack data, TesUIManager uiManager)
        : base(assetManager, asset, data)
        {
            UIManager = uiManager;
            UIManager.Active = true;
        }

        public override void Update()
        {
            base.Update();
            CastInteractRay();
        }

        #region Player Spawn

        PlayerInventory _playerInventory;

        protected override GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = base.CreatePlayer(playerPrefab, position, out playerCamera);
            _playerInventory = player.GetComponent<PlayerInventory>();
            return player;
        }

        #endregion

        #region Raycast

        RaycastHit[] _interactRaycastHitBuffer = new RaycastHit[32];
        protected void CastInteractRay()
        {
            // Cast a ray to see what the camera is looking at.
            var ray = new Ray(_playerCameraObj.transform.position, _playerCameraObj.transform.forward);
            var raycastHitCount = Physics.RaycastNonAlloc(ray, _interactRaycastHitBuffer, MaxInteractDistance);
            if (raycastHitCount > 0 && !_playerComponent.Paused)
                for (var i = 0; i < raycastHitCount; i++)
                {
                    var hitInfo = _interactRaycastHitBuffer[i];
                    var component = hitInfo.collider.GetComponentInParent<BASEComponent>();
                    if (component != null)
                    {
                        if (string.IsNullOrEmpty(component.objData.name))
                            return;
                        ShowInteractiveText(component);
                        if (InputManager.GetButtonDown("Use"))
                        {
                            if (component is DOORComponent) OpenDoor((DOORComponent)component);
                            else if (component.usable) component.Interact();
                            else if (component.pickable) _playerInventory?.Add(component);
                        }
                        break;
                    }
                    else CloseInteractiveText(); //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                }
            else CloseInteractiveText(); //deactivate text if nothing is raycasted against
        }

        public void ShowInteractiveText(BASEComponent component)
        {
            var data = component.objData;
            UIManager?.InteractiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
        }

        public void CloseInteractiveText()
        {
            UIManager?.InteractiveText.Close();
        }

        private void OpenDoor(DOORComponent component)
        {
            if (!component.doorData.leadsToAnotherCell)
                component.Interact();
            else
            {
                // The door leads to another cell, so destroy all currently loaded cells.
                CellManager.DestroyAllCells();
                // Move the player.
                _playerTransform.position = component.doorData.doorExitPos;
                _playerTransform.localEulerAngles = new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);
                // Load the new cell.
                CELLRecord newCell;
                if (component.doorData.leadsToInteriorCell)
                {
                    var cellInfo = CellManager.StartCreatingCellByName(-1, 0, component.doorData.doorExitName);
                    LoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
                    newCell = (CELLRecord)cellInfo.CellRecord;
                    OnInteriorCell(newCell);
                }
                else
                {
                    var cellId = CellManager.GetCellId(component.doorData.doorExitPos, _currentWorld);
                    newCell = (CELLRecord)Data.FindCellRecord(cellId);
                    CellManager.UpdateCells(_playerCameraObj.transform.position, _currentWorld, true, CellRadiusOnLoad);
                    OnExteriorCell(newCell);
                }
                _currentCell = newCell;
            }
        }
        #endregion

        //public void TestAllCells(string resultsFilePath)
        //{
        //    using (var w = new StreamWriter(resultsFilePath))
        //        foreach (var record in ((TesDataPack)Data).GetRecordsOfType<CELLRecord>())
        //        {
        //            var CELL = (CELLRecord)record;
        //            try
        //            {
        //                var cellInfo = ((TesCellManager)CellManager).StartInstantiatingCell(CELL);
        //                LoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
        //                //DestroyImmediate(cellInfo.gameObject);
        //                w.Write("Pass: ");
        //            }
        //            catch (Exception e) { w.Write($"Fail: {e.Message}"); }
        //            if (!CELL.IsInterior)
        //                w.WriteLine(CELL.GridCoords.ToString());
        //            else w.WriteLine(CELL.EDID.Value);
        //            w.Flush();
        //        }
        //}
    }
}
