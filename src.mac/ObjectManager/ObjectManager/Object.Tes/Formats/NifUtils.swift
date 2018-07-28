//
//  NifReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import simd

public class NifUtils {
    public static func nifVectorToUnityVector(_ vector: float3) -> float3 {
        var unity = vector
        let y = unity.y
        let z = unity.z
        unity.z = y
        unity.y = z
//        Utils.swap(&unity.y, &unity.z)
        return unity
    }

    public static func nifPointToUnityPoint(_ point: float3) -> float3 {
        return nifVectorToUnityVector(point) / ConvertUtils.meterInUnits
    }

    public static func nifRotationMatrixToUnityRotationMatrix(_ rotationMatrix: float4x4) -> float4x4 {
        return float4x4(
            float4(rotationMatrix[0].x, rotationMatrix[0].z, rotationMatrix[0].y, 0),
            float4(rotationMatrix[2].x, rotationMatrix[2].z, rotationMatrix[2].y, 0),
            float4(rotationMatrix[1].x, rotationMatrix[1].z, rotationMatrix[1].y, 0),
            float4(0, 0, 0, 1))
//        return float4x4(
//            m11: rotationMatrix.m11,
//            m12: rotationMatrix.m13,
//            m13: rotationMatrix.m12,
//            m14: 0,
//            m21: rotationMatrix.m31,
//            m22: rotationMatrix.m33,
//            m23: rotationMatrix.m32,
//            m24: 0,
//            m31: rotationMatrix.m21,
//            m32: rotationMatrix.m23,
//            m33: rotationMatrix.m22,
//            m34: 0,
//            m41: 0,
//            m42: 0,
//            m43: 0,
//            m44: 1
//        )
    }

    public static func nifRotationMatrixToUnityQuaternion(_ rotationMatrix: float4x4) -> simd_quatf {
        return ConvertUtils.rotationMatrixToQuaternion(nifRotationMatrixToUnityRotationMatrix(rotationMatrix))
    }

    public static func nifEulerAnglesToUnityQuaternion(_ nifEulerAngles: float3) -> simd_quatf {
        let eulerAngles2 = nifVectorToUnityVector(nifEulerAngles)
        let xRot = simd_quatf(angle: rad2Deg * eulerAngles2.x, axis: float3.right)
        let yRot = simd_quatf(angle: rad2Deg * eulerAngles2.y, axis: float3.up)
        let zRot = simd_quatf(angle: rad2Deg * eulerAngles2.z, axis: float3.forward)
        return xRot * zRot * yRot
    }
}
