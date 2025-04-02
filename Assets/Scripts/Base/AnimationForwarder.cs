using BerserkPixel.StateMachine;
using UnityEngine;

namespace Base {
    /// <summary>
    /// Used to forward animation trigger events from children to the root element.
    /// Used mostly for child animations in the Editor
    /// </summary>
    public class AnimationForwarder : MonoBehaviour {
        private IStateAnimationTrigger _stateAnimationTrigger;

        private void Awake() {
            _stateAnimationTrigger = transform.root.GetComponentInChildren<IStateAnimationTrigger>();
        }

        /// <summary>
        ///     Can be called from the Animation Timeline. This will propagate the AnimationTriggerType
        ///     to the current active state.
        /// </summary>
        /// <param name="triggerType"></param>
        public void SetAnimationTriggerEvent(AnimationTriggerType triggerType) {
            if (_stateAnimationTrigger == null) {
                return;
            }
            _stateAnimationTrigger.SetAnimationTriggerEvent(triggerType);
        }
    }
}