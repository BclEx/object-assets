﻿using OA.Core;
using OA.Ultima.FilePacks.Records;
using OA.Ultima.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.FilePacks
{
    public static class RecordUtils
    {
        public static string GetModelFileName(IRecord record)
        {
            if (record is STATRecord) return ((STATRecord)record).Name;
            else return null;
        }
    }

    public partial class DataFile : IDisposable
    {
        public const int CELL_PACK = 8;
        const int recordHeaderSizeInBytes = 16;
        TileMatrixData _tileData;
        public Record[] records;
        public Dictionary<Type, List<IRecord>> recordsByType;
        public Dictionary<string, IRecord> objectsByIDString;
        public Dictionary<Vector3Int, CELLRecord> _CELLsById;
        public Dictionary<Vector3Int, LANDRecord> _LANDsById;

        public DataFile(uint map)
        {
            _tileData = new TileMatrixData(map);
            ReadRecords();
            PostProcessRecords();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~DataFile()
        {
            Close();
        }

        public void Close()
        {
            if (_tileData != null)
            {
                _tileData.Dispose();
                _tileData = null;
            }
        }

        public List<IRecord> GetRecordsOfType<T>() where T : Record { return recordsByType.TryGetValue(typeof(T), out List<IRecord> records) ? records : null; }

        private void ReadRecords()
        {
            var recordList = new List<Record>();
            // add items
#if false
            for (short itemId = 0; itemId < TileData.LandData.Length; itemId++)
                recordList.Add(new STATRecord(true, itemId));
#endif
            for (short itemId = 0; itemId < TileData.ItemData.Length; itemId++)
                recordList.Add(new STATRecord(false, itemId));
            // add tiles
            for (uint chunkY = 0; chunkY < _tileData.ChunkHeight / CELL_PACK; chunkY++)
                for (uint chunkX = 0; chunkX < _tileData.ChunkWidth / CELL_PACK; chunkX++)
                {
                    var land = new LANDRecord(_tileData, chunkX, chunkY);
                    recordList.Add(land);
                    recordList.Add(new CELLRecord(_tileData, land, chunkX, chunkY));
                }
            records = recordList.ToArray();
        }

        private void PostProcessRecords()
        {
            recordsByType = new Dictionary<Type, List<IRecord>>();
            objectsByIDString = new Dictionary<string, IRecord>();
            _CELLsById = new Dictionary<Vector3Int, CELLRecord>();
            _LANDsById = new Dictionary<Vector3Int, LANDRecord>();
            foreach (var record in records)
            {
                if (record == null)
                    continue;
                // Add the record to the list for it's type.
                var recordType = record.GetType();
                if (recordsByType.TryGetValue(recordType, out List<IRecord> recordsOfSameType))
                    recordsOfSameType.Add(record);
                else
                {
                    recordsOfSameType = new List<IRecord> { record };
                    recordsByType.Add(recordType, recordsOfSameType);
                }
                // Add the record to the object dictionary if applicable.
                //if (record is GMSTRecord) objectsByIDString.Add(((GMSTRecord)record).NAME.value, record);
                //else if (record is GLOBRecord) objectsByIDString.Add(((GLOBRecord)record).NAME.value, record);
                //else if (record is SOUNRecord) objectsByIDString.Add(((SOUNRecord)record).NAME.value, record);
                //else if (record is REGNRecord) objectsByIDString.Add(((REGNRecord)record).NAME.value, record);
                //else if (record is LTEXRecord) objectsByIDString.Add(((LTEXRecord)record).NAME.value, record);
                if (record is STATRecord) objectsByIDString.Add(((STATRecord)record).Name, record);
                //else if (record is DOORRecord) objectsByIDString.Add(((DOORRecord)record).NAME.value, record);
                //else if (record is MISCRecord) objectsByIDString.Add(((MISCRecord)record).NAME.value, record);
                //else if (record is WEAPRecord) objectsByIDString.Add(((WEAPRecord)record).NAME.value, record);
                //else if (record is CONTRecord) objectsByIDString.Add(((CONTRecord)record).NAME.value, record);
                //else if (record is LIGHRecord) objectsByIDString.Add(((LIGHRecord)record).NAME.value, record);
                //else if (record is ARMORecord) objectsByIDString.Add(((ARMORecord)record).NAME.value, record);
                //else if (record is CLOTRecord) objectsByIDString.Add(((CLOTRecord)record).NAME.value, record);
                //else if (record is REPARecord) objectsByIDString.Add(((REPARecord)record).NAME.value, record);
                //else if (record is ACTIRecord) objectsByIDString.Add(((ACTIRecord)record).NAME.value, record);
                //else if (record is APPARecord) objectsByIDString.Add(((APPARecord)record).NAME.value, record);
                //else if (record is LOCKRecord) objectsByIDString.Add(((LOCKRecord)record).NAME.value, record);
                //else if (record is PROBRecord) objectsByIDString.Add(((PROBRecord)record).NAME.value, record);
                //else if (record is INGRRecord) objectsByIDString.Add(((INGRRecord)record).NAME.value, record);
                //else if (record is BOOKRecord) objectsByIDString.Add(((BOOKRecord)record).NAME.value, record);
                //else if (record is ALCHRecord) objectsByIDString.Add(((ALCHRecord)record).NAME.value, record);
                //else if (record is CREARecord) objectsByIDString.Add(((CREARecord)record).NAME.value, record);
                //else if (record is NPC_Record) objectsByIDString.Add(((NPC_Record)record).NAME.value, record);
                // Add the record to exteriorCELLRecordsByIndices if applicable.
                if (record is CELLRecord cell)
                {
                    if (!cell.IsInterior)
                        _CELLsById[cell.GridId] = cell;
                }
                // Add the record to LANDRecordsByIndices if applicable.
                if (record is LANDRecord land)
                    _LANDsById[land.GridId] = land;
            }
        }
    }
}