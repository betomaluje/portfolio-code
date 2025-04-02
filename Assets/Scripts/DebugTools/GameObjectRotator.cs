using UnityEngine;

namespace DebugTools {    
    public class GameObjectRotator : MonoBehaviour {
        [SerializeField]
        private GameObject[] _allObjects;

        [SerializeField]
        private KeyCode _nextObjectKey;

        private int _currentIndex;

        private void Awake() {
            _currentIndex = 0;
            _allObjects[_currentIndex].SetActive(true);
        }

        private void NextObject() {
            var position = _allObjects[_currentIndex].transform.position;
            _allObjects[_currentIndex].SetActive(false);
            _currentIndex = (_currentIndex + 1) % _allObjects.Length;
            _allObjects[_currentIndex].SetActive(true);
            _allObjects[_currentIndex].transform.position = position;
        }

        private void Update() {
            if (Input.GetKeyDown(_nextObjectKey)) {
                NextObject();
            }
        }
    }
}