using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Base.Swarm {
    public abstract class Swarm<C> : MonoBehaviour {
        [Header("Swarm Config")]
        [SerializeField]
        protected C _swarmConfig;

        [SerializeField]
        protected float _spawnDelay = 1f;
        
        [FoldoutGroup("Events")]       
        [SerializeField]
        protected UnityEvent OnSpawnStart;

        [FoldoutGroup("Events")]
        [SerializeField]
        protected UnityEvent OnSpawnEnd;

        [Header("Debug")]
        [SerializeField]
        private Color _debugColor = Color.red;

        protected Vector3Int[] _spawnPoints;

        public virtual void OnDrawGizmosSelected() {
            if (_spawnPoints is not {Length: > 0}) {
                return;
            }

            Gizmos.color = _debugColor;

            foreach (var point in _spawnPoints) {
                Gizmos.DrawWireSphere(point, .2f);
            }
        }

        protected abstract void Spawn();

        protected abstract void Cleanup();

        protected abstract void PopulatePoints();
    }
}