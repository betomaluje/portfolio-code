using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

namespace EditorTool {
    [RequireComponent(typeof(ToolsUI))]
    public class ToolCategoriesUI : MonoBehaviour {
        [SerializeField]
        private ToolsUI _toolsUI;

        [SerializeField]
        private ToolCategoryUI _toolUIPrefab;

        [SerializeField]
        private Transform _toolsParent;

        private Dictionary<ToolType, List<Tool>> _toolsByCategory;
        private List<ToolCategoryUI> _tools;
        private int _previousSelected;

        private void Reset() {
            _toolsUI = GetComponent<ToolsUI>();
        }

        /// <summary>
        /// Called from Editor
        /// </summary>
        /// <param name="categories">Sets a list of tools by category</param>
        public async void PupulateCategories(Dictionary<ToolType, List<Tool>> categories) {
            if (categories == null || !categories.Any())
                return;

            await _toolsParent.DestroyChildrenAsync();
            _toolsByCategory = categories;
            _tools = new List<ToolCategoryUI>();
            _previousSelected = 0;
            foreach (var (category, _) in categories) {
                var toolCategoryUI = Instantiate(_toolUIPrefab, _toolsParent);
                toolCategoryUI.SetupTool(category);
                _tools.Add(toolCategoryUI);
            }

            var currentTools = _toolsByCategory[(ToolType)_previousSelected];
            _toolsUI.PupulateTools(currentTools);
        }

        /// <summary>
        /// Called from Editor
        /// </summary>
        /// <param name="index"></param>
        public void ChangeCategorySelected(int index) {
            _tools[_previousSelected].SetSelected(false);
            _previousSelected = index;
            _tools[_previousSelected].SetSelected(true);

            // and we update the list of tools for this new category
            var currentTools = _toolsByCategory[(ToolType)_previousSelected];
            _toolsUI.PupulateTools(currentTools);
        }
    }
}