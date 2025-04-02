using System.Collections;
using Modifiers.Powerups;
using Modifiers.Skills;
using Sounds;
using UnityEngine;

namespace Player {
    public class PlayerSounds : MonoBehaviour {
        [SerializeField]
        private Vector2 _pitchRange;

        [SerializeField]
        private float _timeBetweenSounds = .5f;

        private bool _canPlaySounds = true;
        private Coroutine _coroutine;

        private CharacterSkills _characterSkills;
        private CharacterPowerup _characterPowerup;

        private void Awake() {
            _characterSkills = GetComponent<CharacterSkills>();
            _characterPowerup = GetComponent<CharacterPowerup>();
        }

        private void OnEnable() {
            PlayerMoneyManager.OnMoneyChange += PlayCoinSound;

            _characterSkills.OnSkillActivated += PlaySkillActivated;
            _characterPowerup.OnPowerupActivated += PlayPowerupSound;
        }

        private void OnDisable() {
            PlayerMoneyManager.OnMoneyChange -= PlayCoinSound;

            _characterSkills.OnSkillActivated -= PlaySkillActivated;
            _characterPowerup.OnPowerupActivated -= PlayPowerupSound;

            if (_coroutine != null) {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private void OnDestroy() {
            PlayerMoneyManager.OnMoneyChange -= PlayCoinSound;

            if (_coroutine != null) {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }
        }

        private void PlayPowerupSound(PowerupConfig config) {
            SoundManager.instance.Play("powerup_activated");
        }

        private void PlayCoinSound(int obj) {
            if (_canPlaySounds) {
                SoundManager.instance.Play("coin");
                _coroutine = StartCoroutine(CancelSounds());
            }
        }

        private IEnumerator CancelSounds() {
            _canPlaySounds = false;
            yield return new WaitForSeconds(_timeBetweenSounds);
            _canPlaySounds = true;
        }

        public void PlaySkillActivated(SkillConfig config) {
            SoundManager.instance.Play("skill_activated");
        }

        public void PlayFootstep() {
            var pitchLevel = Random.Range(_pitchRange.x, _pitchRange.y);
            SoundManager.instance.PlayWithPitch("footstep", pitchLevel);
        }

        public void PlayJump() {
            var pitchLevel = Random.Range(_pitchRange.x, _pitchRange.y);
            SoundManager.instance.PlayWithPitch("jump", pitchLevel);
        }

        public void PlayRoll() {
            var pitchLevel = Random.Range(_pitchRange.x, _pitchRange.y);
            SoundManager.instance.PlayWithPitch("roll", pitchLevel);
        }
    }
}