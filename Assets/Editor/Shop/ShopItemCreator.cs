using Companions;
using Consumables;
using Editor;
using Shop;
using UnityEditor;

public static class ShopItemCreator {

    [MenuItem("Assets/Create/Shop Items", false, 1)]
    static void CreateMultipleShopItems() {
        var selected = Selection.objects;

        foreach (var obj in selected) {
            DOCreate(obj);
        }
    }

    private static void DOCreate(UnityEngine.Object selected) {
        if (selected == null || (selected is not Weapons.Weapon && selected is not ConsumableSO && selected is not CompanionStats)) {
            UnityEngine.Debug.LogWarning($"{selected.GetType()} is not a valid shop item");
            return;
        }

        if (selected is CompanionStats companion) {
            var createCompanionItem = new CreateItem<CompanionShopItem>(companion.Name, "Companions");
            createCompanionItem.NewItem.Name = companion.Name;
            createCompanionItem.NewItem.Item = companion;
            createCompanionItem.NewItem.ItemType = ShopItemCategory.Companion;
            createCompanionItem.Commit();

            Undo.RegisterCreatedObjectUndo(createCompanionItem.NewItem, $"Create {companion.Name}");

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = createCompanionItem.NewItem;
            return;
        }

        if (selected is Weapons.Weapon weapon) {
            var createWeaponItem = new CreateItem<WeaponShopItem>(weapon.Name, "Weapons");
            createWeaponItem.NewItem.Name = weapon.Name;
            createWeaponItem.NewItem.Item = weapon;
            createWeaponItem.NewItem.ItemType = ShopItemCategory.Weapon;
            createWeaponItem.Commit();

            Undo.RegisterCreatedObjectUndo(createWeaponItem.NewItem, $"Create {weapon.Name}");

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = createWeaponItem.NewItem;
            return;
        }

        if (selected is ConsumableSO consumable) {
            var createConsumableItem = new CreateItem<ConsumablesShopItem>(consumable.name, "Consumables");
            createConsumableItem.NewItem.Name = consumable.name;
            createConsumableItem.NewItem.Item = consumable;
            createConsumableItem.NewItem.ItemType = ShopItemCategory.Consumable;
            createConsumableItem.Commit();

            Undo.RegisterCreatedObjectUndo(createConsumableItem.NewItem, $"Create {consumable.name}");

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = createConsumableItem.NewItem;
            return;
        }
    }
}