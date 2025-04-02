using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace Editor {
    public class CreateItem<T> where T : ScriptableObject {
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Hidden)]
        public T NewItem;

        private readonly string _name;
        private readonly string _folder;

        public CreateItem(string name, string folder) {
            _name = name;
            _folder = folder;

            NewItem = DOCreate();
        }

        private T DOCreate() {
            // create new instance of the SO
            var item = ScriptableObject.CreateInstance<T>();
            item.name = $"{_name}";

            return item;
        }

        public void Commit() {
            if (NewItem == null)
                return;

            System.IO.Directory.CreateDirectory($"{Shortcuts.ShopItemsPath}/{_folder}");

            AssetDatabase.CreateAsset(NewItem, $"{Shortcuts.ShopItemsPath}/{_folder}/{_name}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}