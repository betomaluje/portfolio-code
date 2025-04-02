using Sirenix.OdinInspector;
using UnityEngine;

namespace Modifiers.Conditions {
    [InlineEditor]
    public abstract class BaseCondition : ScriptableObject, ICondition {
        public abstract void Setup(Transform owner);
        public abstract bool Check(float deltaTime);
        public abstract void Cleanup();

        public virtual void ResetCondition() { }
        public virtual void OnPowerupActivated() { }
    }

    public interface ICondition {
        void Setup(Transform owner);
        bool Check(float deltaTime);
        void ResetCondition();
    }

    public static class BaseConditionExt {
        public static string GetConditionText(BaseCondition[] conditions) {
            if (conditions == null || conditions.Length == 0) {
                return "No conditions";
            }

            string conditionText = "";
            for (int i = 0; i < conditions.Length; i++) {
                conditionText += conditions[i].ToString();
                if (conditionText.Contains('('))
                    conditionText = conditionText.Split('(')[0];

                if (i < conditions.Length - 1) {
                    conditionText += " and ";
                }
            }

            return conditionText;
        }
    }
}