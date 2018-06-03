﻿//
//  EYESRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class EYESRecord: Record {
    public var description: String { return "EYES: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var FULL: STRVField
    public var ICON: FILEField
    public var DATA: BYTEField // Playable

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "DATA": DATA = BYTEField(r, dataSize)
        default: return false
        }
        return true
    }
}
