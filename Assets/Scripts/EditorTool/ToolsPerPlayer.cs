using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EditorTool {
    [CreateAssetMenu(fileName = "ToolsPerPlayer", menuName = "Aurora/ToolsPerPlayer")]
    public class ToolsPerPlayer : SerializedScriptableObject {
        /// <summary>
        /// The amount of each tool per player.
        /// </summary>
        public Dictionary<Tool, int> Tools;
    }
}