using UnityEngine;

namespace Weapons.DesignEx {
    /// <summary>
    /// The physical portal on the wall.
    /// Mastery: Teleports any IBullet component coming through its threshold.
    /// </summary>
    public class PortalNode : MonoBehaviour {
        
        [SerializeField] private Color _blueColor = Color.cyan;
        [SerializeField] private Color _orangeColor = Color.red;
        [SerializeField] private SpriteRenderer _display;

        private PortalNode _linkedNode;
        private bool _isOrange;

        public void SetColor(bool isOrange) {
            _isOrange = isOrange;
            if (_display != null) {
                _display.color = _isOrange ? _orangeColor : _blueColor;
            }
        }

        public void SetLink(PortalNode destination) {
            _linkedNode = destination;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (_linkedNode == null) return;

            // Teleport anything that is a bullet OR has a Rigidbody2D 
            // and isn't the player's core transform (unless you want player teleport)
            if (other.TryGetComponent<IBullet>(out var bullet)) {
                TeleportObject(other.transform, other.GetComponent<Rigidbody2D>());
            }
        }

        private void TeleportObject(Transform obj, Rigidbody2D rb) {
            // Preservation of relative velocity and angle
            // 1. Position shift: Move precisely to the partner node
            obj.position = _linkedNode.transform.position + (_linkedNode.transform.right * 0.5f);
            
            // 2. Velocity shift: Match the new orientation of the partner portal
            if (rb != null) {
                float incomingSpeed = rb.linearVelocity.magnitude;
                rb.linearVelocity = _linkedNode.transform.right * incomingSpeed;
            }

            // 3. Visual rotation
            obj.right = _linkedNode.transform.right;

            // Optional FX
            PlayTeleportSound(obj.position);
        }

        private void PlayTeleportSound(Vector2 pos) {
            // Sound implementation
        }
    }
}
