﻿//
//  EFSHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class EFSHRecord: Record {
    public struct DATAField {
        public let flags: UInt8
        public let membraneShader_sourceBlendMode: UInt32
        public let membraneShader_blendOperation: UInt32
        public let membraneShader_ztestFunction: UInt32
        public let fillTextureEffect_color: ColorRef
        public let fillTextureEffect_alphaFadeInTime: Float
        public let fillTextureEffect_fullAlphaTime: Float
        public let fillTextureEffect_alphaFadeOutTime: Float
        public let fillTextureEffect_presistentAlphaRatio: Float
        public let fillTextureEffect_alphaPulseAmplitude: Float
        public let fillTextureEffect_alphaPulseFrequency: Float
        public let fillTextureEffect_textureAnimationSpeed_U: Float
        public let fillTextureEffect_textureAnimationSpeed_V: Float
        public let edgeEffect_fallOff: Float
        public let edgeEffect_color: ColorRef
        public let edgeEffect_alphaFadeInTime: Float
        public let edgeEffect_fullAlphaTime: Float
        public let edgeEffect_alphaFadeOutTime: Float
        public let edgeEffect_presistentAlphaRatio: Float
        public let edgeEffect_alphaPulseAmplitude: Float
        public let edgeEffect_alphaPulseFrequency: Float
        public let fillTextureEffect_fullAlphaRatio: Float
        public let edgeEffect_fullAlphaRatio: Float
        public let membraneShader_destBlendMode: UInt32
        public let particleShader_sourceBlendMode: UInt32
        public let particleShader_blendOperation: UInt32
        public let particleShader_ztestFunction: UInt32
        public let particleShader_destBlendMode: UInt32
        public let particleShader_particleBirthRampUpTime: Float
        public let particleShader_fullParticleBirthTime: Float
        public let particleShader_particleBirthRampDownTime: Float
        public let particleShader_fullParticleBirthRatio: Float
        public let particleShader_persistantParticleBirthRatio: Float
        public let particleShader_particleLifetime: Float
        public let particleShader_particleLifetime_Delta: Float
        public let particleShader_initialSpeedAlongNormal: Float
        public let particleShader_accelerationAlongNormal: Float
        public let particleShader_initialVelocity1: Float
        public let particleShader_initialVelocity2: Float
        public let particleShader_initialVelocity3: Float
        public let particleShader_acceleration1: Float
        public let particleShader_acceleration2: Float
        public let particleShader_acceleration3: Float
        public let particleShader_scaleKey1: Float
        public let particleShader_scaleKey2: Float
        public let particleShader_scaleKey1Time: Float
        public let particleShader_scaleKey2Time: Float
        public let colorKey1_color: ColorRef
        public let colorKey2_color: ColorRef
        public let colorKey3_color: ColorRef
        public let colorKey1_colorAlpha: Float
        public let colorKey2_colorAlpha: Float
        public let colorKey3_colorAlpha: Float
        public let colorKey1_colorKeyTime: Float
        public let colorKey2_colorKeyTime: Float
        public let colorKey3_colorKeyTime: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            flags = r.readByte()
            r.skipBytes(3) // Unused
            membraneShader_sourceBlendMode = r.readLEUInt32()
            membraneShader_blendOperation = r.readLEUInt32()
            membraneShader_ztestFunction = r.readLEUInt32()
            fillTextureEffect_color = ColorRef(r)
            fillTextureEffect_alphaFadeInTime = r.readLESingle()
            fillTextureEffect_fullAlphaTime = r.readLESingle()
            fillTextureEffect_alphaFadeOutTime = r.readLESingle()
            fillTextureEffect_presistentAlphaRatio = r.readLESingle()
            fillTextureEffect_alphaPulseAmplitude = r.readLESingle()
            fillTextureEffect_alphaPulseFrequency = r.readLESingle()
            fillTextureEffect_textureAnimationSpeed_u = r.readLESingle()
            fillTextureEffect_textureAnimationSpeed_v = r.readLESingle()
            edgeEffect_fallOff = r.readLESingle()
            edgeEffect_color = ColorRef(r)
            edgeEffect_alphaFadeInTime = r.readLESingle()
            edgeEffect_fullAlphaTime = r.readLESingle()
            edgeEffect_alphaFadeOutTime = r.readLESingle()
            edgeEffect_presistentAlphaRatio = r.readLESingle()
            edgeEffect_alphaPulseAmplitude = r.readLESingle()
            edgeEffect_alphaPulseFrequency = r.readLESingle()
            fillTextureEffect_fullAlphaRatio = r.readLESingle()
            edgeEffect_fullAlphaRatio = r.readLESingle()
            membraneShader_destBlendMode = r.readLEUInt32()
            guard dataSize != 96 else {
                return
            }
            particleShader_SourceBlendMode = r.readLEUInt32()
            particleShader_BlendOperation = r.readLEUInt32()
            particleShader_ZTestFunction = r.readLEUInt32()
            particleShader_DestBlendMode = r.readLEUInt32()
            particleShader_ParticleBirthRampUpTime = r.readLESingle()
            particleShader_FullParticleBirthTime = r.readLESingle()
            particleShader_ParticleBirthRampDownTime = r.readLESingle()
            particleShader_FullParticleBirthRatio = r.readLESingle()
            particleShader_PersistantParticleBirthRatio = r.readLESingle()
            particleShader_ParticleLifetime = r.readLESingle()
            particleShader_ParticleLifetime_Delta = r.readLESingle()
            particleShader_InitialSpeedAlongNormal = r.readLESingle()
            particleShader_AccelerationAlongNormal = r.readLESingle()
            particleShader_InitialVelocity1 = r.readLESingle()
            particleShader_InitialVelocity2 = r.readLESingle()
            particleShader_InitialVelocity3 = r.readLESingle()
            particleShader_Acceleration1 = r.readLESingle()
            particleShader_Acceleration2 = r.readLESingle()
            particleShader_Acceleration3 = r.readLESingle()
            particleShader_ScaleKey1 = r.readLESingle()
            particleShader_ScaleKey2 = r.readLESingle()
            particleShader_ScaleKey1Time = r.readLESingle()
            particleShader_ScaleKey2Time = r.readLESingle()
            colorKey1_color = ColorRef(r)
            colorKey2_color = ColorRef(r)
            colorKey3_color = ColorRef(r)
            colorKey1_colorAlpha = r.readLESingle()
            colorKey2_colorAlpha = r.readLESingle()
            colorKey3_colorAlpha = r.readLESingle()
            colorKey1_colorKeyTime = r.readLESingle()
            colorKey2_colorKeyTime = r.readLESingle()
            colorKey3_colorKeyTime = r.readLESingle()
        }
    }

    public var description: String { return "EFSH: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var ICON: FILEField // Fill Texture
    public var ICO2: FILEField // Particle Shader Texture
    public var DATA: DATAField // Data

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize
        case "ICO2": ICO2 = FILEField(r, dataSize
        case "DATA": DATA = DATAField(r, dataSize
        default: return false
        }
        return true
    }
}
