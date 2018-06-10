//
//  FileManager.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

extension FileManager {
    static let _knownkeys = [
        GameId.oblivion : "Oblivion",
        GameId.skyrim : "Skyrim",
        GameId.fallout3 : "Fallout 3",
        GameId.falloutNV : "Fallout NV",
        GameId.morrowind : "Morrowind",
        GameId.fallout4 : "Fallout4",
        GameId.skyrimSE : "Skyrim SE",
        GameId.fallout4VR : "Fallout4 VR",
        GameId.skyrimVR : "Skyrim VR"
    ]

    static var _fileDirectories: [GameId : URL] = {
        // var game = TesSettings.Game;
        debugPrint("TES Installation(s):")
        // if (game.DataDirectory != null && Directory.Exists(game.DataDirectory))
        // {
        //     var gameId = (GameId)Enum.Parse(typeof(GameId), game.GameId);
        //     _fileDirectories.Add(gameId, game.DataDirectory); Utils.Log($"Settings: {game.DataDirectory}");
        //     _isDataPresent = true;
        // }
        // else
        var r = [GameId : URL]()
        let fileManager = FileManager.default
        let documentsURL = try! fileManager.url(for: .documentDirectory, in: .userDomainMask, appropriateFor: nil, create: false)
        for x in _knownkeys {
            let url = documentsURL.appendingPathComponent(x.value)
            var isDirectory: ObjCBool = false
            guard fileManager.fileExists(atPath: url.path, isDirectory: &isDirectory), isDirectory.boolValue else {
                continue
            }
            r[x.key] = url
            debugPrint("GameId: \(x.key)")
        }
        return r
    }()

    public func getFilePath(_ path: String, for game: GameId) -> URL? {
        guard let fileDirectory = FileManager._fileDirectories[game] else {
            return nil
        }
        let url = fileDirectory.appendingPathComponent(path)
        return fileExists(atPath: url.path) ? url : nil
    }

    public func getFilePaths(searchPattern: String, for game: GameId) -> [URL]?
    {
        guard let fileDirectory = FileManager._fileDirectories[game] else {
            return nil
        }
        let files: [URL]
        do {
            files = try FileManager.default.contentsOfDirectory(at: fileDirectory,
                includingPropertiesForKeys: [],
                options: [.skipsSubdirectoryDescendants, .skipsPackageDescendants, .skipsHiddenFiles])
        }
        catch {
            fatalError("define me later \(fileDirectory)")
        }
        return files
    }
}