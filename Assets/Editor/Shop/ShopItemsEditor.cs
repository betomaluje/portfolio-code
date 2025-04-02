using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Shop;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;

namespace Editor {
    public partial class ShopItemsEditor : OdinMenuEditorWindow {
        private CreateItem<WeaponShopItem> createWeaponItem;
        private CreateItem<ConsumablesShopItem> createConsumableItem;
        private CreateItem<CompanionShopItem> createCompanionItem;

        private string _itemName = "Item";

        [MenuItem(Shortcuts.ToolsShopData, false, -100)]
        private static void OpenWindow() {
            var window = GetWindow<ShopItemsEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 500);
            window.titleContent = new GUIContent("Shop Items");
            window.Show();
        }

        protected override void OnDestroy() {
            base.OnDestroy();

            if (createWeaponItem != null) {
                DestroyImmediate(createWeaponItem.NewItem);
            }

            if (createConsumableItem != null) {
                DestroyImmediate(createConsumableItem.NewItem);
            }

            if (createCompanionItem != null) {
                DestroyImmediate(createCompanionItem.NewItem);
            }
        }

        protected override OdinMenuTree BuildMenuTree() {
            var tree = new OdinMenuTree();

            tree.AddAllAssetsAtPath("Weapons", $"{Shortcuts.ShopItemsPath}/Weapons", typeof(WeaponShopItem), true);
            tree.AddAllAssetsAtPath("Consumables", $"{Shortcuts.ShopItemsPath}/Consumables", typeof(ConsumablesShopItem), true);
            tree.AddAllAssetsAtPath("Companions", $"{Shortcuts.ShopItemsPath}/Companions", typeof(CompanionShopItem), true);

            return tree;
        }

        protected override void OnBeginDrawEditors() {
            OdinMenuTreeSelection selected = MenuTree.Selection;
            if (selected.SelectedValue == null) {
                foreach (var item in MenuTree.MenuItems) {
                    if (item.IsSelected) {
                        AddCreateButton(item.Name);
                        break;
                    }
                }
            }
        }

        private void AddCreateButton(string itemType) {
            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                _itemName = EditorGUILayout.TextField("Item Name", _itemName);
            }

            SirenixEditorGUI.EndIndentedHorizontal();

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton(new GUIContent($"Create New {itemType}"))) {
                    switch (itemType) {
                        case "Weapons":
                            createWeaponItem = new CreateItem<WeaponShopItem>(_itemName, "Weapons");
                            createWeaponItem.NewItem.Name = _itemName;
                            createWeaponItem.NewItem.ItemType = ShopItemCategory.Weapon;

                            Undo.RegisterCreatedObjectUndo(createWeaponItem.NewItem, $"Create {_itemName}");
                            break;

                        case "Consumables":
                            createConsumableItem = new CreateItem<ConsumablesShopItem>(_itemName, "Consumables");
                            createConsumableItem.NewItem.Name = _itemName;
                            createConsumableItem.NewItem.ItemType = ShopItemCategory.Consumable;

                            Undo.RegisterCreatedObjectUndo(createConsumableItem.NewItem, $"Create {_itemName}");
                            break;

                        case "Companions":
                            createCompanionItem = new CreateItem<CompanionShopItem>(_itemName, "Companions");
                            createCompanionItem.NewItem.Name = _itemName;
                            createCompanionItem.NewItem.ItemType = ShopItemCategory.Companion;

                            Undo.RegisterCreatedObjectUndo(createCompanionItem.NewItem, $"Create {_itemName}");
                            break;
                    }

                    _itemName = string.Empty;
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}