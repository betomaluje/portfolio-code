using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace EditorTool {
    public class ToolsUI : MonoBehaviour {
        [SerializeField]
        private ToolUI _toolUIPrefab;
        [SerializeField]
        private Transform _toolsParent;

        private List<ToolUI> _tools;
        private int _previousSelected;

        /// <summary>
        /// Populates the UI for all the given tools
        /// </summary>
        /// <param name="tools">The list of tools to display</param>
        public async void PupulateTools(List<Tool> tools) {
            await _toolsParent.DestroyChildrenAsync();
            _previousSelected = 0;
            _tools = new List<ToolUI>();
            foreach (var item in tools) {
                var toolUI = Instantiate(_toolUIPrefab, _toolsParent);
                toolUI.SetupTool(item);
                _tools.Add(toolUI);
            }
        }

        /// <summary>
        /// Called from Editor
        /// </summary>
        /// <param name="tool">The Tool that has changed</param>
        public void UpdateTool(Tool tool) {
            var selected = _tools.FirstOrDefault(t => t.ID == tool.ID);
            if (selected) {
                selected.UpdateTool(tool);
            }
        }

        public void NotEnough(Tool tool) {
            var selected = _tools.FirstOrDefault(t => t.ID == tool.ID);
            if (selected) {
                selected.Shake();
            }
        }

        /// <summary>
        /// Called from Editor
        /// </summary>
        /// <param name="index">The index that was selected</param>
        public void ChangeSelected(int index) {
            if (_tools.Count <= 0)
                return;

            _tools[_previousSelected].SetSelected(false);
            _previousSelected = index;
            _tools[_previousSelected].SetSelected(true);

        }
    }
}