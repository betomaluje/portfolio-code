using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BerserkPixel.StateMachine {
    public abstract class State<T> : ScriptableObject where T : MonoBehaviour {
        protected T _machine;

        public virtual void Enter(T parent) {
            _machine = parent;
        }

        public virtual void Tick(float deltaTime) { }
        public virtual void FixedTick(float fixedDeltaTime) { }
        public abstract void ChangeState();

        public virtual void AnimationTriggerEvent(AnimationTriggerType triggerType) { }

        public virtual void Exit() { }

        public override string ToString() {
            var type = GetType().ToString();
            var array = type.Split(".");
            // last index of the array
            return array[^1];
        }

        public override bool Equals(object other) {
            //Sequence of checks should be exactly the following.
            //If you don't check "other" on null, then "other.GetType()" further can 
            //throw NullReferenceException
            if (other == null) {
                return false;
            }

            //If references point to the same address, then objects identity is
            //guaranteed.
            if (ReferenceEquals(this, other)) {
                return true;
            }

            //If this type is on top of a class hierarchy, or just doesn't have any
            //inheritors, then you just can do the following:        
            //Vehicle tmp = other as Vehicle; if(tmp==null) return false;
            //After that you can immediately call this.Equals(tmp)
            if (GetType() != other.GetType()) {
                return false;
            }

            return Equals(other as T);
        }

        public override int GetHashCode() {
            unchecked // Overflow is fine, just wrap
            {
                var hash = (int)2166136261;
                // Suitable nullity checks etc, of course :)
                hash = hash * 16777619 + name.GetHashCode();
                hash = hash * 16777619 + GetType().GetHashCode();
                return hash;
            }
        }

        #region Passthrough Methods

        public virtual void OnDrawGizmos() { }

        public virtual void OnDrawGizmosSelected() { }

        /// <summary>
        ///     Removes a GameObject, component, or asset.
        /// </summary>
        /// <param name="obj">The type of Component to retrieve.</param>
        protected new static void Destroy(Object obj) {
            Object.Destroy(obj);
        }

        protected V AddComponent<V>() where V : Component {
            return _machine.gameObject.AddComponent<V>();
        }

        protected V GetOrAdd<V>() where V : Component {
            var component = _machine.GetComponent<V>() ?? AddComponent<V>();
            return component;
        }

        /// <summary>
        ///     Returns the component of type V if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        protected V GetComponent<V>() where V : Component {
            return _machine.GetComponent<V>();
        }

        /// <summary>
        ///     Returns the component of Type <paramref name="type" /> if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        /// <returns></returns>
        protected Component GetComponent(Type type) {
            return _machine.GetComponent(type);
        }

        /// <summary>
        ///     Returns the component with name <paramref name="type" /> if the game object has one attached, null if it doesn't.
        /// </summary>
        /// <param name="type">The type of Component to retrieve.</param>
        /// <returns></returns>
        protected Component GetComponent(string type) {
            return _machine.GetComponent(type);
        }

        /// <summary>
        ///     Returns the component of type V if the game object has one attached in a child object, null if it doesn't.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        protected V GetComponentInChildren<V>() where V : Component {
            return _machine.GetComponentInChildren<V>();
        }

        /// <summary>
        ///     Returns an array of the component of type V if the game object has one attached in a child object, null if it doesn't.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <returns></returns>
        public V[] GetComponentsInChildren<V>() where V : Component {
            return _machine.GetComponentsInChildren<V>();
        }

        #endregion
    }
}