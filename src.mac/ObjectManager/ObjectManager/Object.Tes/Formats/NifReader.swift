//
//  NifReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public struct Ptr<T> {
    public var value: Int32
    public var isNull: Bool { return value < 0 }

    init(_ r: BinaryReader) {
        value = r.readLEInt32()
    }
}

// Refers to an object after the current one in the hierarchy.
public struct Ref<T> {
    public var value: Int32
    public var isNull: Bool { return value < 0 }

    init(_ r: BinaryReader) {
        value = r.readLEInt32()
    }
}

public class NiReaderUtils
{
//    public static func readPtr<T>(_ r: BinaryReader) -> Ptr<T> {
//        return Ptr<T>(r)
//    }
//
//    public static func readRef<T>(_ r: BinaryReader) -> Ref<T> {
//        return Ref<T>(r)
//    }

    public static func readLengthPrefixedRefs32<T>(_ r: BinaryReader) -> [Ref<T>] {
        var refs = [Ref<T>](); let capacity = Int(r.readLEUInt32()); refs.reserveCapacity(capacity)
        for _ in 0..<capacity { refs.append(Ref<T>(r)) }
        return refs
    }

    public static func readFlags(_ r: BinaryReader) -> UInt16 {
        return r.readLEUInt16()
    }

    public static func read<T>(_ r: BinaryReader) -> T {
        if T.self == Float.self { return r.readLESingle() as! T }
        else if T.self == UInt8.self { return r.readByte() as! T }
        else if T.self == String.self { return r.readLELength32PrefixedASCIIString() as! T }
        else if T.self == float3.self { return r.readLEFloat3() as! T }
        else if T.self == simd_quatf.self { return r.readLEQuaternionWFirst() as! T }
        else if T.self == Color4.self { return Color4(r) as! T }
        else { fatalError("Tried to read an unsupported type.") }
    }

    public static func readNiObject(_ r: BinaryReader) -> NiObject? {
        let nodeType = r.readLELength32PrefixedASCIIString()
        if nodeType == "NiNode" { return NiNode(r) }
        else if nodeType == "NiTriShape" { return NiTriShape(r) }
        else if nodeType == "NiTexturingProperty" { return NiTexturingProperty(r) }
        else if nodeType == "NiSourceTexture" { return NiSourceTexture(r) }
        else if nodeType == "NiMaterialProperty" { return NiMaterialProperty(r) }
        else if nodeType == "NiMaterialColorController" { return NiMaterialColorController(r) }
        else if nodeType == "NiTriShapeData" { return NiTriShapeData(r) }
        else if nodeType == "RootCollisionNode" { return RootCollisionNode(r) }
        else if nodeType == "NiStringExtraData" { return NiStringExtraData(r) }
        else if nodeType == "NiSkinInstance" { return NiSkinInstance(r) }
        else if nodeType == "NiSkinData" { return NiSkinData(r) }
        else if nodeType == "NiAlphaProperty" { return NiAlphaProperty(r) }
        else if nodeType == "NiZBufferProperty" { return NiZBufferProperty(r) }
        else if nodeType == "NiVertexColorProperty" { return NiVertexColorProperty(r) }
        else if nodeType == "NiBSAnimationNode" { return NiBSAnimationNode(r) }
        else if nodeType == "NiBSParticleNode" { return NiBSParticleNode(r) }
        else if nodeType == "NiParticles" { return NiParticles(r) }
        else if nodeType == "NiParticlesData" { return NiParticlesData(r) }
        else if nodeType == "NiRotatingParticles" { return NiRotatingParticles(r) }
        else if nodeType == "NiRotatingParticlesData" { return NiRotatingParticlesData(r) }
        else if nodeType == "NiAutoNormalParticles" { return NiAutoNormalParticles(r) }
        else if nodeType == "NiAutoNormalParticlesData" { return NiAutoNormalParticlesData(r) }
        else if nodeType == "NiUVController" { return NiUVController(r) }
        else if nodeType == "NiUVData" { return NiUVData(r) }
        else if nodeType == "NiTextureEffect" { return NiTextureEffect(r) }
        else if nodeType == "NiTextKeyExtraData" { return NiTextKeyExtraData(r) }
        else if nodeType == "NiVertWeightsExtraData" { return NiVertWeightsExtraData(r) }
        else if nodeType == "NiParticleSystemController" { return NiParticleSystemController(r) }
        else if nodeType == "NiBSPArrayController" { return NiBSPArrayController(r) }
        else if nodeType == "NiGravity" { return NiGravity(r) }
        else if nodeType == "NiParticleBomb" { return NiParticleBomb(r) }
        else if nodeType == "NiParticleColorModifier" { return NiParticleColorModifier(r) }
        else if nodeType == "NiParticleGrowFade" { return NiParticleGrowFade(r) }
        else if nodeType == "NiParticleMeshModifier" { return NiParticleMeshModifier(r) }
        else if nodeType == "NiParticleRotation" { return NiParticleRotation(r) }
        else if nodeType == "NiKeyframeController" { return NiKeyframeController(r) }
        else if nodeType == "NiKeyframeData" { return NiKeyframeData(r) }
        else if nodeType == "NiColorData" { return NiColorData(r) }
        else if nodeType == "NiGeomMorpherController" { return NiGeomMorpherController(r) }
        else if nodeType == "NiMorphData" { return NiMorphData(r) }
        else if nodeType == "AvoidNode" { return AvoidNode(r) }
        else if nodeType == "NiVisController" { return NiVisController(r) }
        else if nodeType == "NiVisData" { return NiVisData(r) }
        else if nodeType == "NiAlphaController" { return NiAlphaController(r) }
        else if nodeType == "NiFloatData" { return NiFloatData(r) }
        else if nodeType == "NiPosData" { return NiPosData(r) }
        else if nodeType == "NiBillboardNode" { return NiBillboardNode(r) }
        else if nodeType == "NiShadeProperty" { return NiShadeProperty(r) }
        else { debugPrint("Tried to read an unsupported NiObject type (\(nodeType))."); return nil }
    }

    public static func read3x3RotationMatrix(_ r: BinaryReader) -> float4x4 {
        return r.readLERowMajorMatrix3x3()
    }
}

public class NiFile {
    public let name: String
    public var header: NiHeader!
    public var blocks: [NiObject]!
    public var footer: NiFooter!

    init(name: String) {
        self.name = name
    }
    convenience init(_ r: BinaryReader, name: String) {
        self.init(name: name)
        header = NiHeader(r)
        blocks = [NiObject](); let capacity = Int(header.numBlocks); blocks.reserveCapacity(capacity)
        for _ in 0..<capacity { blocks.append(NiReaderUtils.readNiObject(r)!) }
        footer = NiFooter(r)
    }
}

// MARK: Enums

// texture enums
public enum ApplyMode: UInt32 {
    case APPLY_REPLACE = 0, APPLY_DECAL, APPLY_MODULATE, APPLY_HILIGHT, APPLY_HILIGHT2
}

public enum TexClampMode: UInt32 {
    case CLAMP_S_CLAMP_T = 0, CLAMP_S_WRAP_T, WRAP_S_CLAMP_T, WRAP_S_WRAP_T3
}

public enum TexFilterMode: UInt32 {
    case FILTER_NEAREST = 0, FILTER_BILERP, FILTER_TRILERP, FILTER_NEAREST_MIPNEAREST, FILTER_NEAREST_MIPLERP, FILTER_BILERP_MIPNEAREST
}

public enum PixelLayout: UInt32 {
    case PIX_LAY_PALETTISED = 0, PIX_LAY_HIGH_COLOR_16, PIX_LAY_TRUE_COLOR_32, PIX_LAY_COMPRESSED, PIX_LAY_BUMPMAP, PIX_LAY_PALETTISED_4, PIX_LAY_DEFAULT
}

public enum MipMapFormat: UInt32 {
    case MIP_FMT_NO = 0, MIP_FMT_YES, MIP_FMT_DEFAULT
}

public enum AlphaFormat: UInt32 {
    case ALPHA_NONE = 0, ALPHA_BINARY, ALPHA_SMOOTH, ALPHA_DEFAULT
}

// miscellaneous
public enum VertMode: UInt32 {
    case VERT_MODE_SRC_IGNORE = 0, VERT_MODE_SRC_EMISSIVE, VERT_MODE_SRC_AMB_DIF
}

public enum LightMode: UInt32 {
    case LIGHT_MODE_EMISSIVE = 0, LIGHT_MODE_EMI_AMB_DIF
}

public enum KeyType: UInt32 {
    case LINEAR_KEY = 1, QUADRATIC_KEY, TBC_KEY, XYZ_ROTATION_KEY, CONST_KEY
}

public enum EffectType: UInt32 {
    case EFFECT_PROJECTED_LIGHT = 0, EFFECT_PROJECTED_SHADOW, EFFECT_ENVIRONMENT_MAP, EFFECT_FOG_MAP
}

public enum CoordGenType: UInt32 {
    case CG_WORLD_PARALLEL = 0, CG_WORLD_PERSPECTIVE, CG_SPHERE_MAP, CG_SPECULAR_CUBE_MAP, CG_DIFFUSE_CUBE_MAP
}

public enum FieldType: UInt32 {
    case FIELD_WIND = 0, FIELD_POINT
}

public enum DecayType: UInt32 {
    case DECAY_NONE = 0, DECAY_LINEAR, DECAY_EXPONENTIAL
}


// MARK: Misc Classes

public struct BoundingBox {
    public let unknownInt: UInt32
    public let translation: float3
    public let rotation: float4x4
    public let radius: float3

    init(_ r: BinaryReader) {
        unknownInt = r.readLEUInt32()
        translation = r.readLEFloat3()
        rotation = NiReaderUtils.read3x3RotationMatrix(r)
        radius = r.readLEFloat3()
    }
}

public typealias Color3 = (
    r: Float,
    g: Float,
    b: Float
)

public typealias Color4 = (
    r: Float,
    g: Float,
    b: Float,
    a: Float
)

public struct TexDesc {
    public let source: Ref<NiSourceTexture>
    public let clampMode: TexClampMode
    public let filterMode: TexFilterMode
    public let uvSet: UInt32
    public let ps2L: Int16
    public let ps2K: Int16
    public let unknown1: UInt16

    init(_ r: BinaryReader) {
        source = Ref<NiSourceTexture>(r)
        clampMode = TexClampMode(rawValue: r.readLEUInt32())!
        filterMode = TexFilterMode(rawValue: r.readLEUInt32())!
        uvSet = r.readLEUInt32()
        ps2L = r.readLEInt16()
        ps2K = r.readLEInt16()
        unknown1 = r.readLEUInt16()
    }
}

public struct TexCoord {
    public let u: Float
    public let v: Float

    init() { u = 0; v = 0 }
    init(_ r: BinaryReader) {
        u = r.readLESingle()
        v = r.readLESingle()
    }
}

public struct Triangle {
    public let v1: UInt16
    public let v2: UInt16
    public let v3: UInt16

    init(_ r: BinaryReader) {
        v1 = r.readLEUInt16()
        v2 = r.readLEUInt16()
        v3 = r.readLEUInt16()
    }
}

public struct MatchGroup {
    public let numVertices: UInt16
    public var vertexIndices: [UInt16]

    init(_ r: BinaryReader) {
        numVertices = r.readLEUInt16()
        vertexIndices = [UInt16](); let capacity = Int(numVertices); vertexIndices.reserveCapacity(capacity)
        for _ in 0..<capacity { vertexIndices.append(r.readLEUInt16()) }
    }
}

public struct TBC {
    public let t: Float
    public let b: Float
    public let c: Float

    init(_ r: BinaryReader) {
        t = r.readLESingle()
        b = r.readLESingle()
        c = r.readLESingle()
    }
}

public class Key<T> {
    public let time: Float
    public let value: T
    public let forward: T!
    public let backward: T!
    public let tbc: TBC!

    init(_ r: BinaryReader, keyType: KeyType) {
        time = r.readLESingle()
        value = NiReaderUtils.read(r)
        if keyType == .QUADRATIC_KEY {
            forward = NiReaderUtils.read(r)
            backward = NiReaderUtils.read(r)
        }
        else if keyType == .TBC_KEY {
            tbc = TBC(r)
        }
    }
}

public class KeyGroup<T> {
    public let numKeys: UInt32
    public let interpolation: KeyType
    public var keys: [Key<T>]

    init(_ r: BinaryReader) {
        numKeys = r.readLEUInt32()
        if numKeys != 0 {
            interpolation = KeyType(rawValue: r.readLEUInt32())!
        }
        keys = [Key<T>](); let capacity = Int(numKeys); keys.reserveCapacity(capacity)
        for _ in 0..<capacity { keys.append(Key<T>(r, keyType: interpolation)) }
    }
}

public class QuatKey<T> {
    public let time: Float
    public let value: T
    public let tbc: TBC

    init(_ r: BinaryReader, keyType: KeyType) {
        time = r.readLESingle()
        if keyType != .XYZ_ROTATION_KEY {
            value = NiReaderUtils.read(r)
        }
        if keyType == .TBC_KEY {
            tbc = TBC(r)
        }
    }
}

public struct SkinData {
    public let skinTransform: SkinTransform
    public let boundingSphereOffset: float3
    public let boundingSphereRadius: Float
    public let numVertices: UInt16
    public var vertexWeights: [SkinWeight]

    init(_ r: BinaryReader) {
        skinTransform = SkinTransform(r)
        boundingSphereOffset = r.readLEFloat3()
        boundingSphereRadius = r.readLESingle()
        numVertices = r.readLEUInt16()
        vertexWeights = [SkinWeight](); let capacity = Int(numVertices); vertexWeights.reserveCapacity(capacity)
        for _ in 0..<capacity { vertexWeights.append(SkinWeight(r)) }
    }
}

public struct SkinWeight {
    public let index: UInt16
    public let weight: Float

    init(_ r: BinaryReader) {
        index = r.readLEUInt16()
        weight = r.readLESingle()
    }
}

public struct SkinTransform {
    public let rotation: float4x4
    public let translation: float3
    public let scale: Float

    init(_ r: BinaryReader) {
        rotation = NiReaderUtils.read3x3RotationMatrix(r)
        translation = r.readLEFloat3()
        scale = r.readLESingle()
    }
}

public struct Particle {
    public let velocity: float3
    public let unknownVector: float3
    public let lifetime: Float
    public let lifespan: Float
    public let timestamp: Float
    public let unknownShort: UInt16
    public let vertexId: UInt16

    init(_ r: BinaryReader) {
        velocity = r.readLEFloat3()
        unknownVector = r.readLEFloat3()
        lifetime = r.readLESingle()
        lifespan = r.readLESingle()
        timestamp = r.readLESingle()
        unknownShort = r.readLEUInt16()
        vertexId = r.readLEUInt16()
    }
}

public struct Morph {
    public let numKeys: UInt32
    public let interpolation: KeyType
    public var keys: [Key<Float>]
    public var vectors: [float3]

    init(_ r: BinaryReader, numVertices: UInt32) {
        numKeys = r.readLEUInt32()
        interpolation = KeyType(rawValue: r.readLEUInt32())!
        keys = [Key<Float>](); var capacity = Int(numKeys); keys.reserveCapacity(capacity)
        for _ in 0..<capacity { keys.append(Key<Float>(r, keyType: interpolation)) }
        vectors = [float3](); capacity = Int(numVertices); vectors.reserveCapacity(capacity)
        for _ in 0..<capacity { vectors.append(r.readLEFloat3()) }
    }
}

// MARK: A

public struct NiHeader {
    public let str: Data // 40 bytes (including \n)
    public let version: UInt32
    public let numBlocks: UInt32

    init(_ r: BinaryReader) {
        str = r.readBytes(40)
        version = r.readLEUInt32()
        numBlocks = r.readLEUInt32()
    }
}

public struct NiFooter {
    public let numRoots: UInt32
    public var roots: [Int32]

    init(_ r: BinaryReader) {
        numRoots = r.readLEUInt32()
        roots = [Int32](); let capacity = Int(numRoots); roots.reserveCapacity(capacity)
        for _ in 0..<capacity { roots.append(r.readLEInt32()) }
    }
}

public class NiObject {
    init(_ r: BinaryReader) {
    }
}

public class NiObjectNET: NiObject {
    public let name: String
    public let extraData: Ref<NiExtraData>
    public let controller: Ref<NiTimeController>

    override init(_ r: BinaryReader) {
        super.init(r)
        name = r.readLELength32PrefixedASCIIString()
        extraData = Ref<NiExtraData>(r)
        controller = Ref<NiTimeController>(r)
    }
}

public class NiAVObject: NiObjectNET {
    public struct NiFlags: OptionSet {
        public let rawValue: UInt16
        public static let hidden = NiFlags(rawValue: 0x1)
        
        public init(rawValue: UInt16) {
            self.rawValue = rawValue
        }
    }

    public let flags: NiFlags
    public let translation: float3
    public let rotation: float4x4
    public let scale: Float
    public let velocity: float3
    //public let numProperties: uint
    public let properties: [Ref<NiProperty>]
    public let hasBoundingBox: Bool
    public let boundingBox: BoundingBox

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiFlags(rawValue: NiReaderUtils.readFlags(r))
        translation = r.readLEFloat3()
        rotation = NiReaderUtils.read3x3RotationMatrix(r)
        scale = r.readLESingle()
        velocity = r.readLEFloat3()
        properties = NiReaderUtils.readLengthPrefixedRefs32(r)
        hasBoundingBox = r.readLEBool32()
        if hasBoundingBox {
            boundingBox = BoundingBox(r)
        }
    }
}

// Nodes
public class NiNode: NiAVObject {
    //public let numChildren: UInt32
    public let children: [Ref<NiAVObject>]
    //public let numEffects: UInt32
    public let effects: [Ref<NiDynamicEffect>]

    override init(_ r: BinaryReader) {
        super.init(r)
        children = NiReaderUtils.readLengthPrefixedRefs32(r)
        effects = NiReaderUtils.readLengthPrefixedRefs32(r)
    }
}

public class RootCollisionNode: NiNode { }

public class NiBSAnimationNode: NiNode { }

public class NiBSParticleNode: NiNode { }

public class NiBillboardNode: NiNode { }

public class AvoidNode: NiNode { }

// Geometry
public class NiGeometry: NiAVObject {
    public var data: Ref<NiGeometryData>
    public var skinInstance: Ref<NiSkinInstance>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiGeometryData>(r)
        skinInstance = Ref<NiSkinInstance>(r)
    }
}

public class NiGeometryData: NiObject {
    public let numVertices: UInt16!
    public let hasVertices: Bool!
    public var vertices: [float3]!
    public let hasNormals: Bool!
    public var normals: [float3]!
    public let center: float3!
    public let radius: Float!
    public let hasVertexColors: Bool!
    public var vertexColors: [Color4]!
    public let numUVSets: UInt16!
    public let hasUV: Bool!
    public var uvSets: [[TexCoord]]!

    override init(_ r: BinaryReader) {
        super.init(r)
        numVertices = r.readLEUInt16()
        hasVertices = r.readLEBool32()
        let capacity = Int(numVertices)
        if hasVertices {
            vertices = [float3](); vertices.reserveCapacity(capacity)
            for _ in 0..<capacity { vertices.append(r.readLEFloat3()) }
        }
        hasNormals = r.readLEBool32()
        if hasNormals {
            normals = [float3](); normals.reserveCapacity(capacity)
            for _ in 0..<capacity { normals.append(r.readLEFloat3()) }
        }
        center = r.readLEFloat3()
        radius = r.readLESingle()
        hasVertexColors = r.readLEBool32()
        if hasVertexColors {
            vertexColors = [Color4](); vertexColors.reserveCapacity(capacity)
            for _ in 0..<capacity { vertexColors.append(Color4(r)) }
        }
        numUVSets = r.readLEUInt16()
        hasUV = r.readLEBool32()
        if hasUV {
            let capacity2 = Int(numUVSets)
            uvSets = Array(repeating: Array(repeating: TexCoord(), count: capacity), count: capacity2)
            for i in 0..<capacity2 {
                for j in 0..<capacity {
                    uvSets[i][j] = TexCoord(r)
                }
            }
        }
    }
}

public class NiTriBasedGeom: NiGeometry {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiTriBasedGeomData: NiGeometryData {
    public let numTriangles: UInt16

    override init(_ r: BinaryReader) {
        super.init(r)
        numTriangles = r.readLEUInt16()
    }
}

public class NiTriShape: NiTriBasedGeom {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiTriShapeData: NiTriBasedGeomData {
    public let numTrianglePoints: UInt32
    public var triangles: [Triangle]
    public let numMatchGroups: UInt16
    public var matchGroups: [MatchGroup]

    override init(_ r: BinaryReader) {
        super.init(r)
        numTrianglePoints = r.readLEUInt32()
        triangles = [Triangle](); var capacity = Int(numTriangles); triangles.reserveCapacity(capacity)
        for _ in 0..<capacity { triangles.append(Triangle(r)) }
        numMatchGroups = r.readLEUInt16()
        matchGroups = [MatchGroup](); capacity = Int(numMatchGroups); matchGroups.reserveCapacity(capacity)
        for _ in 0..<capacity { matchGroups.append(MatchGroup(r)) }
    }
}

// Properties
public class NiProperty: NiObjectNET {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiTexturingProperty: NiProperty {
    public let flags: UInt16
    public let applyMode: ApplyMode
    public let textureCount: UInt32
    //public let hasBaseTexture: Bool
    public let baseTexture: TexDesc?
    //public let hasDarkTexture: Bool
    public let darkTexture: TexDesc?
    //public let hasDetailTexture: Bool
    public let detailTexture: TexDesc?
    //public let hasGlossTexture: Bool
    public let glossTexture: TexDesc?
    //public let hasGlowTexture: Bool
    public let glowTexture: TexDesc?
    //public let hasBumpMapTexture: Bool
    public let bumpMapTexture: TexDesc?
    //public let hasDecal0Texture: Bool
    public let decal0Texture: TexDesc?

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
        applyMode = ApplyMode(rawValue: r.readLEUInt32())!
        textureCount = r.readLEUInt32()
        let hasBaseTexture = r.readLEBool32()
        if hasBaseTexture {
            baseTexture = TexDesc(r)
        }
        let hasDarkTexture = r.readLEBool32()
        if hasDarkTexture {
            darkTexture = TexDesc(r)
        }
        let hasDetailTexture = r.readLEBool32()
        if hasDetailTexture {
            detailTexture = TexDesc(r)
        }
        let hasGlossTexture = r.readLEBool32()
        if hasGlossTexture {
            glossTexture = TexDesc(r)
        }
        let hasGlowTexture = r.readLEBool32()
        if hasGlowTexture {
            glowTexture = TexDesc(r)
        }
        let hasBumpMapTexture = r.readLEBool32()
        if hasBumpMapTexture {
            bumpMapTexture = TexDesc(r)
        }
        let hasDecal0Texture = r.readLEBool32()
        if hasDecal0Texture {
            decal0Texture = TexDesc(r)
        }
    }
}

public class NiAlphaProperty: NiProperty {
    public let flags: UInt16
    public let threshold: UInt8

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = r.readLEUInt16()
        threshold = r.readByte()
    }
}

public class NiZBufferProperty: NiProperty {
    public let flags: UInt16

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = r.readLEUInt16()
    }
}

public class NiVertexColorProperty: NiProperty {
    public let flags: UInt16
    public let vertexMode: VertMode
    public let lightingMode: LightMode

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
        vertexMode = VertMode(rawValue: r.readLEUInt32())!
        lightingMode = LightMode(rawValue: r.readLEUInt32())!
    }
}

public class NiShadeProperty: NiProperty {
    public let flags: UInt16

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
    }
}

// Data
public class NiUVData: NiObject {
    public var uvGroups: [KeyGroup<Float>]

    override init(_ r: BinaryReader) {
        super.init(r)
        uvGroups = [KeyGroup<Float>](); uvGroups.reserveCapacity(4)
        for _ in 0..<4 { uvGroups.append(KeyGroup<Float>(r)) }
    }
}

public class NiKeyframeData: NiObject {
    public let numRotationKeys: UInt32
    public let rotationType: KeyType
    public var quaternionKeys: [QuatKey<simd_quatf>]
    public let unknownFloat: Float
    public var xyzRotations: [KeyGroup<Float>]
    public let translations: KeyGroup<float3>
    public let scales: KeyGroup<Float>

    override init(_ r: BinaryReader) {
        super.init(r)
        numRotationKeys = r.readLEUInt32()
        if numRotationKeys != 0 {
            rotationType = KeyType(rawValue: r.readLEUInt32())!
            if rotationType != .XYZ_ROTATION_KEY {
                quaternionKeys = [QuatKey<simd_quatf>](); let capacity = Int(numRotationKeys); quaternionKeys.reserveCapacity(capacity)
                for _ in 0..<capacity { quaternionKeys.append(QuatKey<simd_quatf>(r, keyType: rotationType)) }
            }
            else {
                unknownFloat = r.readLESingle()
                xyzRotations = [KeyGroup<Float>](); xyzRotations.reserveCapacity(3)
                for _ in 0..<3 { xyzRotations.append(KeyGroup<Float>(r)) }
            }
        }
        translations = KeyGroup<float3>(r)
        scales = KeyGroup<Float>(r)
    }
}

public class NiColorData: NiObject {
    public let data: KeyGroup<Color4>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = KeyGroup<Color4>(r)
    }
}

public class NiMorphData: NiObject {
    public let numMorphs: UInt32
    public let numVertices: UInt32
    public let relativeTargets: UInt8
    public var morphs: [Morph]

    override init(_ r: BinaryReader) {
        super.init(r)
        numMorphs = r.readLEUInt32()
        numVertices = r.readLEUInt32()
        relativeTargets = r.readByte()
        morphs = [Morph](); let capacity = Int(numMorphs); morphs.reserveCapacity(capacity)
        for _ in 0..<capacity { morphs.append(Morph(r, numVertices: numVertices)) }
    }
}

public class NiVisData: NiObject {
    public let numKeys: UInt32
    public var keys: [Key<UInt8>]

    override init(_ r: BinaryReader) {
        super.init(r)
        numKeys = r.readLEUInt32()
        keys = [Key<UInt8>](); let capacity = Int(numKeys); keys.reserveCapacity(capacity)
        for _ in 0..<capacity { keys.append(Key<UInt8>(r, keyType: .LINEAR_KEY)) }
    }
}

public class NiFloatData: NiObject {
    public let data: KeyGroup<Float>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = KeyGroup<Float>(r)
    }
}

public class NiPosData: NiObject {
    public let data: KeyGroup<float3>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = KeyGroup<float3>(r)
    }
}

public class NiExtraData: NiObject {
    public let nextExtraData: Ref<NiExtraData>

    override init(_ r: BinaryReader) {
        super.init(r)
        nextExtraData = Ref<NiExtraData>(r)
    }
}

public class NiStringExtraData: NiExtraData {
    public let bytesRemaining: UInt32
    public let str: String

    override init(_ r: BinaryReader) {
        super.init(r)
        bytesRemaining = r.readLEUInt32()
        str = r.readLELength32PrefixedASCIIString()
    }
}

public class NiTextKeyExtraData: NiExtraData {
    public let unknownInt1: UInt32
    public let numTextKeys: UInt32
    public var textKeys: [Key<String>]

    override init(_ r: BinaryReader) {
        super.init(r)
        unknownInt1 = r.readLEUInt32()
        numTextKeys = r.readLEUInt32()
        textKeys = [Key<String>](); let capacity = Int(numTextKeys); textKeys.reserveCapacity(capacity)
        for _ in 0..<capacity { textKeys.append(Key<String>(r, keyType: .LINEAR_KEY)) }
    }
}

public class NiVertWeightsExtraData: NiExtraData {
    public let numBytes: UInt32
    public let numVertices: UInt16
    public var weights: [Float]

    override init(_ r: BinaryReader) {
        super.init(r)
        numBytes = r.readLEUInt32()
        numVertices = r.readLEUInt16()
        weights = [Float](); let capacity = Int(numVertices); weights.reserveCapacity(capacity)
        for _ in 0..<capacity { weights.append(r.readLESingle()) }
    }
}

// Particles
public class NiParticles: NiGeometry { }

public class NiParticlesData: NiGeometryData {
    public let numParticles: UInt16
    public let particleRadius: Float
    public let numActive: UInt16
    public let hasSizes: Bool
    public var sizes: [Float]

    override init(_ r: BinaryReader) {
        super.init(r)
        numParticles = r.readLEUInt16()
        particleRadius = r.readLESingle()
        numActive = r.readLEUInt16()
        hasSizes = r.readLEBool32()
        if hasSizes {
            sizes = [Float](); let capacity = Int(numVertices); sizes.reserveCapacity(capacity)
            for _ in 0..<capacity { sizes.append(r.readLESingle()) }
        }
    }
}

public class NiRotatingParticles: NiParticles { }

public class NiRotatingParticlesData: NiParticlesData {
    public let hasRotations: Bool
    public var rotations: [simd_quatf]

    override init(_ r: BinaryReader) {
        super.init(r)
        hasRotations = r.readLEBool32()
        if hasRotations {
            rotations = [simd_quatf](); let capacity = Int(numVertices); rotations.reserveCapacity(capacity)
            for _ in 0..<capacity { rotations.append(r.readLEQuaternionWFirst()) }
        }
    }
}

public class NiAutoNormalParticles: NiParticles { }

public class NiAutoNormalParticlesData: NiParticlesData { }

public class NiParticleSystemController: NiTimeController {
    public let speed: Float
    public let speedRandom: Float
    public let verticalDirection: Float
    public let verticalAngle: Float
    public let horizontalDirection: Float
    public let horizontalAngle: Float
    public let unknownNormal: float3
    public let unknownColor: Color4
    public let size: Float
    public let emitStartTime: Float
    public let emitStopTime: Float
    public let unknownByte: UInt8
    public let emitRate: Float
    public let lifetime: Float
    public let lifetimeRandom: Float
    public let emitFlags: UInt16
    public let startRandom: float3
    public let emitter: Ptr<NiObject>
    public let unknownShort2: UInt16
    public let unknownFloat13: Float
    public let unknownInt1: UInt32
    public let unknownInt2: UInt32
    public let unknownShort3: UInt16
    public let numParticles: UInt16
    public let numValid: UInt16
    public var particles: [Particle]
    public let unknownLink: Ref<NiObject>
    public let particleExtra: Ref<NiParticleModifier>
    public let unknownLink2: Ref<NiObject>
    public let trailer: UInt8

    override init(_ r: BinaryReader) {
        super.init(r)
        speed = r.readLESingle()
        speedRandom = r.readLESingle()
        verticalDirection = r.readLESingle()
        verticalAngle = r.readLESingle()
        horizontalDirection = r.readLESingle()
        horizontalAngle = r.readLESingle()
        unknownNormal = r.readLEFloat3()
        unknownColor = Color4(r)
        size = r.readLESingle()
        emitStartTime = r.readLESingle()
        emitStopTime = r.readLESingle()
        unknownByte = r.readByte()
        emitRate = r.readLESingle()
        lifetime = r.readLESingle()
        lifetimeRandom = r.readLESingle()
        emitFlags = r.readLEUInt16()
        startRandom = r.readLEFloat3()
        emitter = Ptr<NiObject>(r)
        unknownShort2 = r.readLEUInt16()
        unknownFloat13 = r.readLESingle()
        unknownInt1 = r.readLEUInt32()
        unknownInt2 = r.readLEUInt32()
        unknownShort3 = r.readLEUInt16()
        numParticles = r.readLEUInt16()
        numValid = r.readLEUInt16()
        particles = [Particle](); let capacity = Int(numParticles); particles.reserveCapacity(capacity)
        for _ in 0..<capacity { particles.append(Particle(r)) }
        unknownLink = Ref<NiObject>(r)
        particleExtra = Ref<NiParticleModifier>(r)
        unknownLink2 = Ref<NiObject>(r)
        trailer = r.readByte()
    }
}

public class NiBSPArrayController: NiParticleSystemController { }

// Particle Modifiers
public class NiParticleModifier: NiObject {
    public let nextModifier: Ref<NiParticleModifier>
    public let controller: Ptr<NiParticleSystemController>

    override init(_ r: BinaryReader) {
        super.init(r)
        nextModifier = Ref<NiParticleModifier>(r)
        controller = Ptr<NiParticleSystemController>(r)
    }
}

public class NiGravity: NiParticleModifier {
    public let unknownFloat1: Float
    public let force: Float
    public let type: FieldType
    public let position: float3
    public let direction: float3

    override init(_ r: BinaryReader) {
        super.init(r)
        unknownFloat1 = r.readLESingle()
        force = r.readLESingle()
        type = FieldType(rawValue: r.readLEUInt32())!
        position = r.readLEFloat3()
        direction = r.readLEFloat3()
    }
}

public class NiParticleBomb: NiParticleModifier {
    public let decay: Float
    public let duration: Float
    public let deltaV: Float
    public let start: Float
    public let decayType: DecayType
    public let position: float3
    public let direction: float3

    override init(_ r: BinaryReader) {
        super.init(r)
        decay = r.readLESingle()
        duration = r.readLESingle()
        deltaV = r.readLESingle()
        start = r.readLESingle()
        decayType = DecayType(rawValue: r.readLEUInt32())!
        position = r.readLEFloat3()
        direction = r.readLEFloat3()
    }
}

public class NiParticleColorModifier: NiParticleModifier {
    public let colorData: Ref<NiColorData>

    override init(_ r: BinaryReader) {
        super.init(r)
        colorData = Ref<NiColorData>(r)
    }
}

public class NiParticleGrowFade: NiParticleModifier {
    public let grow: Float
    public let fade: Float

    override init(_ r: BinaryReader) {
        super.init(r)
        grow = r.readLESingle()
        fade = r.readLESingle()
    }
}

public class NiParticleMeshModifier: NiParticleModifier {
    public let numParticleMeshes: UInt32
    public var particleMeshes: [Ref<NiAVObject>]

    override init(_ r: BinaryReader) {
        super.init(r)
        numParticleMeshes = r.readLEUInt32()
        particleMeshes = [Ref<NiAVObject>](); let capacity = Int(numParticleMeshes); particleMeshes.reserveCapacity(capacity)
        for _ in 0..<capacity { particleMeshes.append(Ref<NiAVObject>(r)) }
    }
}

public class NiParticleRotation: NiParticleModifier {
    public let randomInitialAxis: UInt8
    public let initialAxis: float3
    public let rotationSpeed: Float

    override init(_ r: BinaryReader) {
        super.init(r)
        randomInitialAxis = r.readByte()
        initialAxis = r.readLEFloat3()
        rotationSpeed = r.readLESingle()
    }
}

// Controllers
public class NiTimeController: NiObject {
    public let nextController: Ref<NiTimeController>
    public let flags: UInt16
    public let frequency: Float
    public let phase: Float
    public let startTime: Float
    public let stopTime: Float
    public let target: Ptr<NiObjectNET>

    override init(_ r: BinaryReader) {
        super.init(r)
        nextController = Ref<NiTimeController>(r)
        flags = r.readLEUInt16()
        frequency = r.readLESingle()
        phase = r.readLESingle()
        startTime = r.readLESingle()
        stopTime = r.readLESingle()
        target = Ptr<NiObjectNET>(r)
    }
}

public class NiUVController: NiTimeController {
    public let unknownShort: UInt16
    public let data: Ref<NiUVData>

    override init(_ r: BinaryReader) {
        super.init(r)
        unknownShort = r.readLEUInt16()
        data = Ref<NiUVData>(r)
    }
}

public class NiInterpController: NiTimeController { }

public class NiSingleInterpController: NiInterpController { }

public class NiKeyframeController: NiSingleInterpController {
    public let data: Ref<NiKeyframeData>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiKeyframeData>(r)
    }
}

public class NiGeomMorpherController: NiInterpController {
    public let data: Ref<NiMorphData>
    public let alwaysUpdate: UInt8

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiMorphData>(r)
        alwaysUpdate = r.readByte()
    }
}

public class NiBoolInterpController: NiSingleInterpController { }

public class NiVisController: NiBoolInterpController {
    public let data: Ref<NiVisData>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiVisData>(r)
    }
}

public class NiFloatInterpController: NiSingleInterpController { }

public class NiAlphaController: NiFloatInterpController {
    public let data: Ref<NiFloatData>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiFloatData>(r)
    }
}

// Skin Stuff
public class NiSkinInstance: NiObject {
    public let data: Ref<NiSkinData>
    public let skeletonRoot: Ptr<NiNode>
    public let numBones: UInt32
    public var bones: [Ptr<NiNode>]

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiSkinData>(r)
        skeletonRoot = Ptr<NiNode>(r)
        numBones = r.readLEUInt32()
        bones = [Ptr<NiNode>](); let capacity = Int(numBones); bones.reserveCapacity(capacity)
        for _ in 0..<capacity { bones.append(Ptr<NiNode>(r)) }
    }
}

public class NiSkinData: NiObject {
    public let skinTransform: SkinTransform
    public let numBones: UInt32
    public let skinPartition: Ref<NiSkinPartition>
    public var boneList: [SkinData]

    override init(_ r: BinaryReader) {
        super.init(r)
        skinTransform = SkinTransform(r)
        numBones = r.readLEUInt32()
        skinPartition = Ref<NiSkinPartition>(r)
        boneList = [SkinData](); let capacity = Int(numBones); boneList.reserveCapacity(capacity)
        for _ in 0..<capacity { boneList.append(SkinData(r)) }
    }
}

public class NiSkinPartition: NiObject { }

// Miscellaneous
public class NiTexture: NiObjectNET {
    override init(_ r: BinaryReader) {
        super.init(r)
    }
}

public class NiSourceTexture: NiTexture {
    public let useExternal: UInt8
    public let fileName: String
    public let pixelLayout: PixelLayout
    public let useMipMaps: MipMapFormat
    public let alphaFormat: AlphaFormat
    public let isStatic: UInt8

    override init(_ r: BinaryReader) {
        super.init(r)
        useExternal = r.readByte()
        fileName = r.readLELength32PrefixedASCIIString()
        pixelLayout = PixelLayout(rawValue: r.readLEUInt32())!
        useMipMaps = MipMapFormat(rawValue: r.readLEUInt32())!
        alphaFormat = AlphaFormat(rawValue: r.readLEUInt32())!
        isStatic = r.readByte()
    }
}

public class NiPoint3InterpController: NiSingleInterpController {
    public let data: Ref<NiPosData>

    override init(_ r: BinaryReader) {
        super.init(r)
        data = Ref<NiPosData>(r)
    }
}

public class NiMaterialProperty: NiProperty {
    public let flags: UInt16
    public let ambientColor: Color3
    public let diffuseColor: Color3
    public let specularColor: Color3
    public let emissiveColor: Color3
    public let glossiness: Float
    public let alpha: Float

    override init(_ r: BinaryReader) {
        super.init(r)
        flags = NiReaderUtils.readFlags(r)
        ambientColor = Color3(r)
        diffuseColor = Color3(r)
        specularColor = Color3(r)
        emissiveColor = Color3(r)
        glossiness = r.readLESingle()
        alpha = r.readLESingle()
    }
}

public class NiMaterialColorController: NiPoint3InterpController { }

public class NiDynamicEffect: NiAVObject {
    public let numAffectedNodeListPointers: UInt32
    public var affectedNodeListPointers: [UInt32]

    override init(_ r: BinaryReader) {
        super.init(r)
        numAffectedNodeListPointers = r.readLEUInt32()
        affectedNodeListPointers = [UInt32](); let capacity = Int(numAffectedNodeListPointers); affectedNodeListPointers.reserveCapacity(capacity)
        for _ in 0..<capacity { affectedNodeListPointers.append(r.readLEUInt32()) }
    }
}

public class NiTextureEffect: NiDynamicEffect {
    public let modelProjectionMatrix: float4x4
    public let modelProjectionTransform: float3
    public let textureFiltering: TexFilterMode
    public let textureClamping: TexClampMode
    public let textureType: EffectType
    public let coordinateGenerationType: CoordGenType
    public let sourceTexture: Ref<NiSourceTexture>
    public let clippingPlane: UInt8
    public let unknownVector: float3
    public let unknownFloat: Float
    public let ps2L: Int16
    public let ps2K: Int16
    public let unknownShort: UInt16

    override init(_ r: BinaryReader) {
        super.init(r)
        modelProjectionMatrix = NiReaderUtils.read3x3RotationMatrix(r)
        modelProjectionTransform = r.readLEFloat3()
        textureFiltering = TexFilterMode(rawValue: r.readLEUInt32())!
        textureClamping = TexClampMode(rawValue: r.readLEUInt32())!
        textureType = EffectType(rawValue: r.readLEUInt32())!
        coordinateGenerationType = CoordGenType(rawValue: r.readLEUInt32())!
        sourceTexture = Ref<NiSourceTexture>(r)
        clippingPlane = r.readByte()
        unknownVector = r.readLEFloat3()
        unknownFloat = r.readLESingle()
        ps2L = r.readLEInt16()
        ps2K = r.readLEInt16()
        unknownShort = r.readLEUInt16()
    }
}

