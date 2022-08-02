using UnityEngine;
using Object = UnityEngine.Object;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Avrahamy.EditorGadgets {
    public static class EditorUtils {
        public static T[] FindAllAssetsOfType<T>() where T : Object {
            var filter = "t:" + typeof(T).Name;
            var guids = AssetDatabase.FindAssets(filter);
            var assets = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++) {
                string guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets[i] = asset;
            }
            return assets;
        }

        /// <summary>
        /// Used to get assets of a certain type and file extension from entire project.
        /// </summary>
        /// <param name="fileExtension">The file extension the type uses eg ".prefab".</param>
        /// <returns>An Object array of assets.</returns>
        public static T[] FindAllAssetsOfType<T>(string fileExtension) where T : Object {
            var tempObjects = new List<T>();
            var directory = new DirectoryInfo(Application.dataPath);
            var goFileInfo = directory.GetFiles("*" + fileExtension, SearchOption.AllDirectories);

            var goFileInfoLength = goFileInfo.Length;
            for (int i = 0; i < goFileInfoLength; i++) {
                var tempGoFileInfo = goFileInfo[i];
                if (tempGoFileInfo == null) continue;

                var tempFilePath = tempGoFileInfo.FullName;
                tempFilePath = tempFilePath.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                var obj = AssetDatabase.LoadMainAssetAtPath(tempFilePath);
                var tempGO = obj as GameObject;
                if (tempGO == null) continue;
                var components = tempGO.GetComponentsInChildren<T>();

                tempObjects.AddRange(components);
            }

            return tempObjects.ToArray();
        }

        public static Dictionary<T, string> FindAllAssetsOfTypeWithGuids<T>() where T : Object {
            var filter = "t:" + typeof(T);
            var guids = AssetDatabase.FindAssets(filter);
            var assets = new Dictionary<T, string>(guids.Length);
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                assets[asset] = guid;
            }
            return assets;
        }

        public static Dictionary<Object, string> FindAssetsOfTypeWithGuids(Type type) {
            var filter = "t:" + type;
            var guids = AssetDatabase.FindAssets(filter);
            var assets = new Dictionary<Object, string>(guids.Length);
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                assets[asset] = guid;
            }
            return assets;
        }

        public static Dictionary<T, string> FindAllPrefabsOfTypeWithGuids<T>() where T : Object {
            const string FILTER = "t:Prefab";
            var guids = AssetDatabase.FindAssets(FILTER);
            var assets = new Dictionary<T, string>(guids.Length);
            foreach (var guid in guids) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset == null) continue;
                assets[asset] = guid;
            }
            return assets;
        }

        public static void CreateScriptableObject<T>(string path, string fileName) where T : ScriptableObject {
            var asset = ScriptableObject.CreateInstance<T>();

            var assetCreationPath = GetAssetCreationPath(path, fileName);
            AssetDatabase.CreateAsset(asset, assetCreationPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created {assetCreationPath}");
            EditorGUIUtility.PingObject(asset);
        }

        public static string GetAssetCreationPath(string folderAbsolutePath, string fileName) {
            var assetAbsoluteFullPath = Path.Combine(folderAbsolutePath, $"{fileName}.asset");
            var relativeAssetPath = GetPathWithinProject(assetAbsoluteFullPath);
            return relativeAssetPath;
        }

        public static string GetPathWithinProject(string absolutePath) {
            var projectParentPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../"));
            var relativePath = absolutePath.Substring(projectParentPath.Length);
            return relativePath;
        }

        public static string GetCurrentProjectFolder() {
            string path;
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets)) {
                path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path)) continue;

                if (Directory.Exists(path)) return path;
                if (File.Exists(path)) return Path.GetDirectoryName(path);
            }

            // No asset selected. Try to use reflection.
            var projectWindowUtilType = typeof(ProjectWindowUtil);
            var getActiveFolderPath = projectWindowUtilType.GetMethod(
                "GetActiveFolderPath",
                BindingFlags.Static | BindingFlags.NonPublic);
            path = getActiveFolderPath?.Invoke(null, Array.Empty<object>()) as string;

            return path ?? "Assets";
        }
    }
}
