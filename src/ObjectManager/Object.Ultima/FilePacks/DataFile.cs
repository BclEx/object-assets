﻿using OA.Core;
using OA.Ultima.FilePacks.Records;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OA.Ultima.FilePacks
{
    public static class RecordUtils
    {
        public static string GetModelFileName(IRecord record)
        {
            return null;
        }
    }

    public partial class DataFile : IDisposable
    {
        const int recordHeaderSizeInBytes = 16;
        public Record[] records;
        public Dictionary<Type, List<IRecord>> recordsByType;
        public Dictionary<string, IRecord> objectsByIDString;
        public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
        public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

        public DataFile(string filePath)
        {
            ReadRecords(filePath);
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

        public void Close() { }

        public List<IRecord> GetRecordsOfType<T>() where T : Record
        {
            List<IRecord> records;
            if (recordsByType.TryGetValue(typeof(T), out records))
                return records;
            return null;
        }

        private void ReadRecords(string filePath)
        {
        }

        private void PostProcessRecords()
        {
            recordsByType = new Dictionary<Type, List<IRecord>>();
            objectsByIDString = new Dictionary<string, IRecord>();
            exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
            LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();
            foreach (var record in records)
            {
                //if (record == null)
                //    continue;
                //// Add the record to the list for it's type.
                //var recordType = record.GetType();
                //List<IRecord> recordsOfSameType;
                //if (recordsByType.TryGetValue(recordType, out recordsOfSameType))
                //    recordsOfSameType.Add(record);
                //else
                //{
                //    recordsOfSameType = new List<IRecord>();
                //    recordsOfSameType.Add(record);
                //    recordsByType.Add(recordType, recordsOfSameType);
                //}
                //// Add the record to the object dictionary if applicable.
                //if (record is GMSTRecord) objectsByIDString.Add(((GMSTRecord)record).NAME.value, record);
                //else if (record is GLOBRecord) objectsByIDString.Add(((GLOBRecord)record).NAME.value, record);
                //else if (record is SOUNRecord) objectsByIDString.Add(((SOUNRecord)record).NAME.value, record);
                //else if (record is REGNRecord) objectsByIDString.Add(((REGNRecord)record).NAME.value, record);
                //else if (record is LTEXRecord) objectsByIDString.Add(((LTEXRecord)record).NAME.value, record);
                //else if (record is STATRecord) objectsByIDString.Add(((STATRecord)record).NAME.value, record);
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
                //// Add the record to exteriorCELLRecordsByIndices if applicable.
                //if (record is CELLRecord)
                //{
                //    var cell = (CELLRecord)record;
                //    if (!cell.IsInterior)
                //        exteriorCELLRecordsByIndices[cell.GridCoords] = cell;
                //}
                //// Add the record to LANDRecordsByIndices if applicable.
                //if (record is LANDRecord)
                //{
                //    var land = (LANDRecord)record;
                //    LANDRecordsByIndices[land.gridCoords] = land;
                //}
            }
        }
    }
}