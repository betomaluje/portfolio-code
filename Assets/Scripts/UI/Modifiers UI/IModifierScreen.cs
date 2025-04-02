using Modifiers;

namespace UI {
    public interface IModifierScreen {
        void ShowModifierDetails(IModifier modifier);

        void HideModifierDetails();

        void SelectModifier(IModifier modifier);
    }
}