﻿using OA.Bae.Components.Records;
using OA.Bae.Esm;
using OA.Bae.Formats;
using OA.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OA.Bae
{
    public class InRangeCellInfo
    {
        public GameObject gameObject;
        public GameObject objectsContainerGameObject;
        public CELLRecord cellRecord;
        public IEnumerator objectsCreationCoroutine;

        public InRangeCellInfo(GameObject gameObject, GameObject objectsContainerGameObject, CELLRecord cellRecord, IEnumerator objectsCreationCoroutine)
        {
            this.gameObject = gameObject;
            this.objectsContainerGameObject = objectsContainerGameObject;
            this.cellRecord = cellRecord;
            this.objectsCreationCoroutine = objectsCreationCoroutine;
        }
    }

    public class RefCellObjInfo
    {
        public CELLRecord.RefObjDataGroup refObjDataGroup;
        public Record referencedRecord;
        public string modelFilePath;
    }

    public class CellManager
    {
        const int cellRadius = 4;
        const int detailRadius = 3;
        const string defaultLandTextureFilePath = "textures/_land_default.dds";

        MorrowindDataReader r;
        TextureManager textureManager;
        NifManager nifManager;
        TemporalLoadBalancer temporalLoadBalancer;
        Dictionary<Vector2i, InRangeCellInfo> cellObjects = new Dictionary<Vector2i, InRangeCellInfo>();

        public CellManager(MorrowindDataReader dataReader, TextureManager textureManager, NifManager nifManager, TemporalLoadBalancer temporalLoadBalancer)
        {
            this.r = dataReader;
            this.textureManager = textureManager;
            this.nifManager = nifManager;
            this.temporalLoadBalancer = temporalLoadBalancer;
        }

        public Vector2i GetExteriorCellIndices(Vector3 point)
        {
            return new Vector2i(Mathf.FloorToInt(point.x / Convert.exteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / Convert.exteriorCellSideLengthInMeters));
        }

        public InRangeCellInfo StartCreatingExteriorCell(Vector2i cellIndices)
        {
            var cell = r.FindExteriorCellRecord(cellIndices);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                cellObjects[cellIndices] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public void UpdateExteriorCells(Vector3 currentPosition, bool immediate = false, int cellRadiusOverride = -1)
        {
            var cameraCellIndices = GetExteriorCellIndices(currentPosition);

            var cellRadius = (cellRadiusOverride >= 0) ? cellRadiusOverride : CellManager.cellRadius;
            var minCellX = cameraCellIndices.x - cellRadius;
            var maxCellX = cameraCellIndices.x + cellRadius;
            var minCellY = cameraCellIndices.y - cellRadius;
            var maxCellY = cameraCellIndices.y + cellRadius;

            // Destroy out of range cells.
            var outOfRangeCellIndices = new List<Vector2i>();

            foreach (var KVPair in cellObjects)
                if (KVPair.Key.x < minCellX || KVPair.Key.x > maxCellX || KVPair.Key.y < minCellY || KVPair.Key.y > maxCellY)
                    outOfRangeCellIndices.Add(KVPair.Key);

            foreach (var cellIndices in outOfRangeCellIndices)
                DestroyExteriorCell(cellIndices);

            // Create new cells.
            for (var r = 0; r <= cellRadius; r++)
                for (var x = minCellX; x <= maxCellX; x++)
                    for (var y = minCellY; y <= maxCellY; y++)
                    {
                        var cellIndices = new Vector2i(x, y);
                        var cellXDistance = Mathf.Abs(cameraCellIndices.x - cellIndices.x);
                        var cellYDistance = Mathf.Abs(cameraCellIndices.y - cellIndices.y);
                        var cellDistance = Mathf.Max(cellXDistance, cellYDistance);
                        if (cellDistance == r && !cellObjects.ContainsKey(cellIndices))
                        {
                            var cellInfo = StartCreatingExteriorCell(cellIndices);
                            if (cellInfo != null && immediate)
                                temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
                        }
                    }

            // Update LODs.
            foreach (var keyValuePair in cellObjects)
            {
                var cellIndices = keyValuePair.Key;
                var cellInfo = keyValuePair.Value;
                var cellXDistance = Mathf.Abs(cameraCellIndices.x - cellIndices.x);
                var cellYDistance = Mathf.Abs(cameraCellIndices.y - cellIndices.y);
                var cellDistance = Mathf.Max(cellXDistance, cellYDistance);
                if (cellDistance <= detailRadius)
                {
                    if (!cellInfo.objectsContainerGameObject.activeSelf)
                        cellInfo.objectsContainerGameObject.SetActive(true);
                }
                else
                {
                    if (cellInfo.objectsContainerGameObject.activeSelf)
                        cellInfo.objectsContainerGameObject.SetActive(false);
                }
            }
        }

        public InRangeCellInfo StartCreatingInteriorCell(string cellName)
        {
            var cell = r.FindInteriorCellRecord(cellName);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                cellObjects[Vector2i.zero] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public InRangeCellInfo StartCreatingInteriorCell(Vector2i gridCoords)
        {
            var cell = r.FindInteriorCellRecord(gridCoords);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                cellObjects[Vector2i.zero] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public InRangeCellInfo StartInstantiatingCell(CELLRecord cell)
        {
            Debug.Assert(cell != null);
            string cellObjName = null;
            LANDRecord land = null;
            if (!cell.isInterior)
            {
                cellObjName = "cell " + cell.gridCoords.ToString();
                land = r.FindLANDRecord(cell.gridCoords);
            }
            else
                cellObjName = cell.NAME.value;
            var cellObj = new GameObject(cellObjName);
            cellObj.tag = "Cell";
            var cellObjectsContainer = new GameObject("objects");
            cellObjectsContainer.transform.parent = cellObj.transform;
            var cellObjectsCreationCoroutine = InstantiateCellObjectsCoroutine(cell, land, cellObj, cellObjectsContainer);
            temporalLoadBalancer.AddTask(cellObjectsCreationCoroutine);
            return new InRangeCellInfo(cellObj, cellObjectsContainer, cell, cellObjectsCreationCoroutine);
        }

        public void DestroyAllCells()
        {
            foreach (var keyValuePair in cellObjects)
            {
                temporalLoadBalancer.CancelTask(keyValuePair.Value.objectsCreationCoroutine);
                Object.Destroy(keyValuePair.Value.gameObject);
            }
            cellObjects.Clear();
        }

        /// <summary>
        /// A coroutine that instantiates the terrain for, and all objects in, a cell.
        /// </summary>
        private IEnumerator InstantiateCellObjectsCoroutine(CELLRecord cell, LANDRecord land, GameObject cellObj, GameObject cellObjectsContainer)
        {
            // Start pre-loading all required textures for the terrain.
            if (land != null)
            {
                var landTextureFilePaths = GetLANDTextureFilePaths(land);
                if (landTextureFilePaths != null)
                    foreach (var landTextureFilePath in landTextureFilePaths)
                        textureManager.PreloadTextureFileAsync(landTextureFilePath);
                yield return null;
            }
            // Extract information about referenced objects.
            var refCellObjInfos = GetRefCellObjInfos(cell);
            yield return null;
            // Start pre-loading all required files for referenced objects. The NIF manager will load the textures as well.
            foreach (var refCellObjInfo in refCellObjInfos)
                if (refCellObjInfo.modelFilePath != null)
                    nifManager.PreloadNifFileAsync(refCellObjInfo.modelFilePath);
            yield return null;
            // Instantiate terrain.
            if (land != null)
            {
                var instantiateLANDTaskEnumerator = InstantiateLANDCoroutine(land, cellObj);
                // Run the LAND instantiation coroutine.
                while (instantiateLANDTaskEnumerator.MoveNext())
                    // Yield every time InstantiateLANDCoroutine does to avoid doing too much work in one frame.
                    yield return null;
                // Yield after InstantiateLANDCoroutine has finished to avoid doing too much work in one frame.
                yield return null;
            }
            // Instantiate objects.
            foreach (var refCellObjInfo in refCellObjInfos)
            {
                InstantiateCellObject(cell, cellObjectsContainer, refCellObjInfo);
                yield return null;
            }
        }

        private RefCellObjInfo[] GetRefCellObjInfos(CELLRecord cell)
        {
            var refCellObjInfos = new RefCellObjInfo[cell.refObjDataGroups.Count];
            for (var i = 0; i < cell.refObjDataGroups.Count; i++)
            {
                var refObjInfo = new RefCellObjInfo();
                refObjInfo.refObjDataGroup = cell.refObjDataGroups[i];
                // Get the record the RefObjDataGroup references.
                r.MorrowindESMFile.objectsByIDString.TryGetValue(refObjInfo.refObjDataGroup.NAME.value, out refObjInfo.referencedRecord);
                if (refObjInfo.referencedRecord != null)
                {
                    var modelFileName = RecordUtils.GetModelFileName(refObjInfo.referencedRecord);
                    // If the model file name is valid, store the model file path.
                    if (!string.IsNullOrEmpty(modelFileName))
                        refObjInfo.modelFilePath = "meshes\\" + modelFileName;
                }
                refCellObjInfos[i] = refObjInfo;
            }
            return refCellObjInfos;
        }

        /// <summary>
        /// Instantiates an object in a cell. Called by InstantiateCellObjectsCoroutine after the object's assets have been pre-loaded.
        /// </summary>
        private void InstantiateCellObject(CELLRecord cell, GameObject parent, RefCellObjInfo refCellObjInfo)
        {
            if (refCellObjInfo.referencedRecord != null)
            {
                GameObject modelObj = null;
                // If the object has a model, instantiate it.
                if (refCellObjInfo.modelFilePath != null)
                {
                    modelObj = nifManager.InstantiateNIF(refCellObjInfo.modelFilePath);
                    PostProcessInstantiatedCellObject(modelObj, refCellObjInfo);
                    modelObj.transform.parent = parent.transform;
                }
                // If the object has a light, instantiate it.
                if (refCellObjInfo.referencedRecord is LIGHRecord)
                {
                    var lightObj = InstantiateLight((LIGHRecord)refCellObjInfo.referencedRecord, cell.isInterior);
                    // If the object also has a model, parent the model to the light.
                    if (modelObj != null)
                    {
                        // Some NIF files have nodes named "AttachLight". Parent it to the light if it exists.
                        var attachLightObj = GameObjectUtils.FindChildRecursively(modelObj, "AttachLight");
                        if (attachLightObj == null)
                        {
                            //attachLightObj = GameObjectUtils.FindChildWithNameSubstringRecursively(modelObj, "Emitter");
                            attachLightObj = modelObj;
                        }
                        if (attachLightObj != null)
                        {
                            lightObj.transform.position = attachLightObj.transform.position;
                            lightObj.transform.rotation = attachLightObj.transform.rotation;
                            lightObj.transform.parent = attachLightObj.transform;
                        }
                        else // If there is no "AttachLight", center the light in the model's bounds.
                        {
                            lightObj.transform.position = GameObjectUtils.CalcVisualBoundsRecursive(modelObj).center;
                            lightObj.transform.rotation = modelObj.transform.rotation;
                            lightObj.transform.parent = modelObj.transform;
                        }
                    }
                    else // If the light has no associated model, instantiate the light as a standalone object.
                    {
                        PostProcessInstantiatedCellObject(lightObj, refCellObjInfo);
                        lightObj.transform.parent = parent.transform;
                    }
                }
            }
            else Debug.Log("Unknown Object: " + refCellObjInfo.refObjDataGroup.NAME.value);
        }

        private GameObject InstantiateLight(LIGHRecord LIGH, bool indoors)
        {
            var lightObj = new GameObject("Light");
            lightObj.isStatic = true;
            var lightComponent = lightObj.AddComponent<Light>();
            lightComponent.range = 3 * (LIGH.LHDT.radius / Convert.meterInMWUnits);
            lightComponent.color = new Color32(LIGH.LHDT.red, LIGH.LHDT.green, LIGH.LHDT.blue, 255);
            lightComponent.intensity = 1.5f;
            lightComponent.bounceIntensity = 0f;
            lightComponent.shadows = BaeSettings.renderLightShadows ? LightShadows.Soft : LightShadows.None;
            if (!indoors && !BaeSettings.renderExteriorCellLights) // disabling exterior cell lights because there is no day/night cycle
                lightComponent.enabled = false;
            return lightObj;
        }

        /// <summary>
        /// Finishes initializing an instantiated cell object.
        /// </summary>
        private void PostProcessInstantiatedCellObject(GameObject gameObject, RefCellObjInfo refCellObjInfo)
        {
            var refObjDataGroup = refCellObjInfo.refObjDataGroup;
            // Handle object transforms.
            if (refObjDataGroup.XSCL != null)
                gameObject.transform.localScale = Vector3.one * refObjDataGroup.XSCL.value;
            gameObject.transform.position += NifUtils.NifPointToUnityPoint(refObjDataGroup.DATA.position);
            gameObject.transform.rotation *= NifUtils.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DATA.eulerAngles);
            var tagTarget = gameObject;
            var coll = gameObject.GetComponentInChildren<Collider>(); // if the collider is on a child object and not on the object with the component, we need to set that object's tag instead.
            if (coll != null)
                tagTarget = coll.gameObject;
            ProcessObjectType<DOORRecord>(tagTarget, refCellObjInfo, "Door");
            ProcessObjectType<ACTIRecord>(tagTarget, refCellObjInfo, "Activator");
            ProcessObjectType<CONTRecord>(tagTarget, refCellObjInfo, "Container");
            ProcessObjectType<LIGHRecord>(tagTarget, refCellObjInfo, "Light");
            ProcessObjectType<LOCKRecord>(tagTarget, refCellObjInfo, "Lock");
            ProcessObjectType<PROBRecord>(tagTarget, refCellObjInfo, "Probe");
            ProcessObjectType<REPARecord>(tagTarget, refCellObjInfo, "RepairTool");
            ProcessObjectType<WEAPRecord>(tagTarget, refCellObjInfo, "Weapon");
            ProcessObjectType<CLOTRecord>(tagTarget, refCellObjInfo, "Clothing");
            ProcessObjectType<ARMORecord>(tagTarget, refCellObjInfo, "Armor");
            ProcessObjectType<INGRRecord>(tagTarget, refCellObjInfo, "Ingredient");
            ProcessObjectType<ALCHRecord>(tagTarget, refCellObjInfo, "Alchemical");
            ProcessObjectType<APPARecord>(tagTarget, refCellObjInfo, "Apparatus");
            ProcessObjectType<BOOKRecord>(tagTarget, refCellObjInfo, "Book");
            ProcessObjectType<MISCRecord>(tagTarget, refCellObjInfo, "MiscObj");
            ProcessObjectType<CREARecord>(tagTarget, refCellObjInfo, "Creature");
            ProcessObjectType<NPC_Record>(tagTarget, refCellObjInfo, "NPC");
        }

        private void ProcessObjectType<RecordType>(GameObject gameObject, RefCellObjInfo info, string tag) where RecordType : Record
        {
            var record = info.referencedRecord;
            if (record is RecordType)
            {
                var obj = GameObjectUtils.FindTopLevelObject(gameObject);
                if (obj == null) { return; }
                var component = GenericObjectComponent.Create(obj, record, tag);
                //only door records need access to the cell object data group so far
                if (record is DOORRecord)
                    ((DoorComponent)component).refObjDataGroup = info.refObjDataGroup;
            }
        }

        private List<string> GetLANDTextureFilePaths(LANDRecord land)
        {
            // Don't return anything if the LAND doesn't have height data or texture data.
            if (land.VHGT == null || land.VTEX == null) { return null; }
            var textureFilePaths = new List<string>();
            var distinctTextureIndices = land.VTEX.textureIndices.Distinct().ToList();
            for (var i = 0; i < distinctTextureIndices.Count; i++)
            {
                var textureIndex = (short)((short)distinctTextureIndices[i] - 1);
                if (textureIndex < 0)
                {
                    textureFilePaths.Add(defaultLandTextureFilePath);
                    continue;
                }
                var ltex = r.FindLTEXRecord(textureIndex);
                var textureFilePath = ltex.DATA.value;
                textureFilePaths.Add(textureFilePath);
            }
            return textureFilePaths;
        }

        /// <summary>
        /// Creates terrain representing a LAND record.
        /// </summary>
        private IEnumerator InstantiateLANDCoroutine(LANDRecord land, GameObject parent)
        {
            Debug.Assert(land != null);
            // Don't create anything if the LAND doesn't have height data.
            if (land.VHGT == null)
                yield break;
            // Return before doing any work to provide an IEnumerator handle to the coroutine.
            yield return null;
            const int LAND_SIDE_LENGTH_IN_SAMPLES = 65;
            var heights = new float[LAND_SIDE_LENGTH_IN_SAMPLES, LAND_SIDE_LENGTH_IN_SAMPLES];
            // Read in the heights in Morrowind units.
            const int VHGTIncrementToMWUnits = 8;
            float rowOffset = land.VHGT.referenceHeight;
            for (var y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
            {
                rowOffset += land.VHGT.heightOffsets[y * LAND_SIDE_LENGTH_IN_SAMPLES];
                heights[y, 0] = VHGTIncrementToMWUnits * rowOffset;
                float colOffset = rowOffset;
                for (var x = 1; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
                {
                    colOffset += land.VHGT.heightOffsets[(y * LAND_SIDE_LENGTH_IN_SAMPLES) + x];
                    heights[y, x] = VHGTIncrementToMWUnits * colOffset;
                }
            }
            // Change the heights to percentages.
            float minHeight, maxHeight;
            ArrayUtils.GetExtrema(heights, out minHeight, out maxHeight);
            for (var y = 0; y < LAND_SIDE_LENGTH_IN_SAMPLES; y++)
                for (var x = 0; x < LAND_SIDE_LENGTH_IN_SAMPLES; x++)
                    heights[y, x] = Utils.ChangeRange(heights[y, x], minHeight, maxHeight, 0, 1);
            // Texture the terrain.
            SplatPrototype[] splatPrototypes = null;
            float[,,] alphaMap = null;
            const int LAND_TEXTURE_INDICES_COUNT = 256;
            var textureIndices = land.VTEX != null ? land.VTEX.textureIndices : new ushort[LAND_TEXTURE_INDICES_COUNT];
            // Create splat prototypes.
            var splatPrototypeList = new List<SplatPrototype>();
            var texInd2splatInd = new Dictionary<ushort, int>();
            for (var i = 0; i < textureIndices.Length; i++)
            {
                var textureIndex = (short)((short)textureIndices[i] - 1);
                if (!texInd2splatInd.ContainsKey((ushort)textureIndex))
                {
                    // Load terrain texture.
                    string textureFilePath;
                    if (textureIndex < 0)
                        textureFilePath = defaultLandTextureFilePath;
                    else
                    {
                        var LTEX = r.FindLTEXRecord(textureIndex);
                        textureFilePath = LTEX.DATA.value;
                    }
                    var texture = textureManager.LoadTexture(textureFilePath);
                    // Yield after loading each texture to avoid doing too much work on one frame.
                    yield return null;
                    // Create the splat prototype.
                    var splat = new SplatPrototype();
                    splat.texture = texture;
                    splat.smoothness = 0;
                    splat.metallic = 0;
                    splat.tileSize = new Vector2(6, 6);
                    // Update collections.
                    var splatIndex = splatPrototypeList.Count;
                    splatPrototypeList.Add(splat);
                    texInd2splatInd.Add((ushort)textureIndex, splatIndex);
                }
            }
            splatPrototypes = splatPrototypeList.ToArray();
            // Create the alpha map.
            var VTEX_ROWS = 16;
            var VTEX_COLUMNS = VTEX_ROWS;
            alphaMap = new float[VTEX_ROWS, VTEX_COLUMNS, splatPrototypes.Length];
            for (var y = 0; y < VTEX_ROWS; y++)
            {
                var yMajor = y / 4;
                var yMinor = y - (yMajor * 4);
                for (var x = 0; x < VTEX_COLUMNS; x++)
                {
                    var xMajor = x / 4;
                    var xMinor = x - (xMajor * 4);
                    var texIndex = (short)((short)textureIndices[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor] - 1);
                    if (texIndex >= 0) { var splatIndex = texInd2splatInd[(ushort)texIndex]; alphaMap[y, x, splatIndex] = 1; }
                    else alphaMap[y, x, 0] = 1;
                }
            }
            // Yield before creating the terrain GameObject because it takes a while.
            yield return null;
            // Create the terrain.
            var heightRange = maxHeight - minHeight;
            var terrainPosition = new Vector3(Convert.exteriorCellSideLengthInMeters * land.gridCoords.x, minHeight / Convert.meterInMWUnits, Convert.exteriorCellSideLengthInMeters * land.gridCoords.y);
            var heightSampleDistance = Convert.exteriorCellSideLengthInMeters / (LAND_SIDE_LENGTH_IN_SAMPLES - 1);
            var terrain = GameObjectUtils.CreateTerrain(heights, heightRange / Convert.meterInMWUnits, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
            terrain.GetComponent<Terrain>().materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
            terrain.transform.parent = parent.transform;
            terrain.isStatic = true;
        }

        private void DestroyExteriorCell(Vector2i indices)
        {
            InRangeCellInfo cellInfo;
            if (cellObjects.TryGetValue(indices, out cellInfo))
            {
                temporalLoadBalancer.CancelTask(cellInfo.objectsCreationCoroutine);
                Object.Destroy(cellInfo.gameObject);
                cellObjects.Remove(indices);
            }
            else Debug.LogError("Tried to destroy a cell that isn't created.");
        }
    }
}
