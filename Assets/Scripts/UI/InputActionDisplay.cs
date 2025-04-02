using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Input {
    /// <summary>
    /// Taken from url https://x.com/BinaryImpactG/status/1780176012526960865?t=wiuLti4RcQcGclcWJ0LupA&s=35
    /// <see href="https://x.com/BinaryImpactG/status/1780176012526960865?t=wiuLti4RcQcGclcWJ0LupA&s=35"
    /// </summary>
    public class InputActionDisplay : MonoBehaviour {
        [SerializeField]
        private PlayerInput _playerInput;

        [SerializeField]
        private InputActionReference _inputAction;

        public string DisplayString {
            get {
                InputBinding activeBinding = _inputAction.action.bindings
                    .FirstOrDefault(binding =>
                        binding.groups
                            .Split(";")
                            .Any(scheme => scheme == _playerInput.currentControlScheme)
                    );

                return activeBinding != default ? activeBinding.ToDisplayString(InputBinding.DisplayStringOptions.DontIncludeInteractions) : "No Active Binding";
            }
        }
    }
}