//
//  TextureManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TextureManager {
    let _asset: IAssetPack
    var _textureFilePreloadTasks = [String : Texture2DInfo]()
    var _cachedTextures = [String : Texture2D]()

    init(asset: IAssetPack) {
        _asset = asset
    }

    public func loadTexture(_ texturePath: String, method: Int = 0) -> Texture2D {
        if let texture = _cachedTextures[texturePath] {
            return texture
        }
        // Load & cache the texture.
        let textureInfo = loadTextureInfo(texturePath)
        let texture = textureInfo != nil ? textureInfo!.toTexture2D() : Texture2D()
        _cachedTextures[texturePath] = texture
        return texture
    }

    public func preloadTextureFileAsync(_ texturePath: String) {
        // If the texture has already been created we don't have to load the file again.
        guard _cachedTextures[texturePath] == nil else { return }
        // Start loading the texture file asynchronously if we haven't already started.
        var textureFileLoadingTask = _textureFilePreloadTasks[texturePath]
        if textureFileLoadingTask == nil {
            textureFileLoadingTask = _asset.loadTextureInfoAsync(texturePath)
            _textureFilePreloadTasks[texturePath] = textureFileLoadingTask
        }
    }

    func loadTextureInfo(_ texturePath: String) -> Texture2DInfo? {
        assert(_cachedTextures[texturePath] != nil, "Invalid parameter")
        preloadTextureFileAsync(texturePath)
        let textureInfo = _textureFilePreloadTasks[texturePath]
        _textureFilePreloadTasks.removeValue(forKey: texturePath)
        return textureInfo
    }
}
