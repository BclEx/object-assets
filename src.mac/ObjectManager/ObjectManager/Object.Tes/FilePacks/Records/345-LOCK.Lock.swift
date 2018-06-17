//
//  LOCKRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LOCKRecord: Record, IHaveEDID, IHaveMODL {
    public struct LKDTField {
        public let weight: Float
        public let value: Int32
        public let quality: Float
        public let uses: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            weight = r.readLESingle()
            value = r.readLEInt32()
            quality = r.readLESingle()
            uses = r.readLEInt32()
        }
    }

    public override var description: String { return "LOCK: \(EDID!)" }
    public var EDID: STRVField! // Editor ID
    public var MODL: MODLGroup! // Model Name
    public var FNAM: STRVField! // Item Name
    public var LKDT: LKDTField! // Lock Data
    public var ICON: FILEField! // Inventory Icon
    public var SCRI: FMIDField<SCPTRecord>! // Script Name

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == GameFormatId.TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = STRVField(r, dataSize)
        case "LKDT": LKDT = LKDTField(r, dataSize)
        case "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
