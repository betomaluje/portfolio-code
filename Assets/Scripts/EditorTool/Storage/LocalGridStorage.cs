using System;
using System.Collections.Generic;
using System.IO;
using EditorTool.Models;
using UnityEngine;

namespace EditorTool.Storage {
    public class LocalGridStorage : IGridStorage {
        private static readonly string Saved_Tools_Folder = Application.dataPath + "/Saves/Tools/";
        private const string File_Name = "SavedObjects.json";

        private BattleGrid _grid;

        public LocalGridStorage(BattleGrid grid) {
            _grid = grid;

            if (!Directory.Exists(Saved_Tools_Folder)) {
                Directory.CreateDirectory(Saved_Tools_Folder);
            }
        }

        public void SaveGrid(string fileName = null) {
            var savedObjects = ConvertToJson(_grid.PlacedTools);

            if (string.IsNullOrEmpty(fileName)) {
                fileName = File_Name;
            }

            File.WriteAllText(Saved_Tools_Folder + fileName, savedObjects, System.Text.Encoding.UTF8);
        }

        public void LoadGrid(Action<List<GridSaveData>> callback = null, string fileName = null) {
            if (string.IsNullOrEmpty(fileName)) {
                fileName = File_Name;
            }

            var loadedObjects = JsonUtility.FromJson<SerializableList<GridSaveData>>(
                File.ReadAllText(Saved_Tools_Folder + fileName, System.Text.Encoding.UTF8));

            callback?.Invoke(loadedObjects.list);
        }

        // Mock the player file this way.
        public void LoadPlayerGrid(Action<List<GridSaveData>> callback = null, string contentId = null) {
            LoadGrid(callback, contentId);
        }

        public void DeleteGrid(string fileName = null) {
            if (string.IsNullOrEmpty(fileName)) {
                fileName = File_Name;
            }
            if (File.Exists(Saved_Tools_Folder + fileName)) {
                File.Delete(Saved_Tools_Folder + fileName);
            }
        }

        protected string ConvertToJson(Dictionary<Vector3Int, Tool> placedObjects) {
            var listToSave = new List<GridSaveData>(placedObjects.Count);
            foreach (var (position, tool) in placedObjects) {
                listToSave.Add(new GridSaveData(tool, position));
            }

            var savedObjects = JsonUtility.ToJson(listToSave.ToSerializable());
            return savedObjects;
        }
    }
}