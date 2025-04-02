using System;
using System.Collections.Generic;
using EditorTool.Models;

namespace EditorTool.Storage {
    interface IGridStorage {
        /// <summary>
        /// Saves the grid to a file.
        /// </summary>
        public void SaveGrid(string fileName = null);

        /// <summary>
        /// Loads the grid from a file.
        /// </summary>
        public void LoadGrid(Action<List<GridSaveData>> callback = null, string fileName = null);

        /// <summary>
        /// Loads a players grid
        /// </summary>
        public void LoadPlayerGrid(Action<List<GridSaveData>> callback = null, string contentId = null);

        /// <summary>
        /// Deletes the grid from the file.
        /// </summary>
        public void DeleteGrid(string fileName = null);
    }
}