//
//  ROADRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ROADRecord: Record {
    public override var description: String { return "ROAD:" }
    public var PGRPs: [PGRDRecord.PGRPField]!
    public var PGRR: UNKNField!
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "PGRP":
            PGRPs = [PGRDRecord.PGRPField](); let capacity = dataSize >> 4; PGRPs.reserveCapacity(capacity)
            for _ in 0..<capacity { PGRPs.append(PGRDRecord.PGRPField(r, dataSize)) }
        case "PGRR": PGRR = r.readBYTV(dataSize)
        default: return false
        }
        return true
    }
}
