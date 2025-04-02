using UnityEngine;

namespace BerserkPixel.Health {
    public interface IHealth {
        void SetupHealth(int maxHealth);

        /// <summary>
        /// Performs a hitData into an Entity.
        /// </summary>
        /// <param name="hitData">All data related to an impact/hit.</param>
        void PerformDamage(HitData hitData);

        void GiveHealth(int health);

        bool CanGiveHealth();
    }

    [System.Serializable]
    public class HitData {
        public Weapons.Weapon weapon;
        public int damage;
        public Vector3 direction;
        public Transform attacker;
        public Transform victim;
        public float criticalHitChance = 0f;
        public float criticalHitMultiplier = 1f;
        [Range(0f, 1f)]
        public float reduceDamageMultiplier = 0f;
        public bool isCritical = false;
        public TimescaleData timescaleData;

        public HitData(Transform attacker, Transform victim) {
            this.attacker = attacker;
            this.victim = victim;
        }

        private HitData() { }
    }

    [System.Serializable]
    public class TimescaleData {
        public float TimeInSeconds = 0f;
        public float TimeScale = 1f;

        public TimescaleData(float timeInSeconds, float timeScale) {
            TimeInSeconds = timeInSeconds;
            TimeScale = timeScale;
        }

        public bool IsValid() => TimeScale != 1 && TimeInSeconds > 0;
    }

    public class HitDataBuilder {
        private Weapons.Weapon _weapon;
        private int _damage = -1;
        private Vector3 _direction;
        private float _criticalHitChance = 0f;
        private float _criticalHitMultiplier = 1f;
        private float _reduceDamageMultiplier = 0f; // ex.  0.25f Reduces damage by 25%
        private TimescaleData _timescaleData;

        public HitDataBuilder WithTimescaleData(TimescaleData timescaleData) {
            _timescaleData = timescaleData;
            return this;
        }

        public HitDataBuilder WithTimescaleData(float timeInSeconds, float timeScale) {
            _timescaleData = new TimescaleData(timeInSeconds, timeScale);
            return this;
        }

        public HitDataBuilder WithWeapon(Weapons.Weapon weapon) {
            _weapon = weapon;
            return this;
        }

        /// <summary>
        /// Adds damage to the hit. By default is the weapon damage
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public HitDataBuilder WithDamage(int damage) {
            _damage = damage;
            return this;
        }

        public HitDataBuilder WithCriticalHitChance(float chance) {
            _criticalHitChance = chance;
            return this;
        }

        public HitDataBuilder WithCriticalHitMultiplier(float multiplier) {
            _criticalHitMultiplier = multiplier;
            return this;
        }

        public HitDataBuilder WithDirection(Vector3 direction) {
            _direction = direction;
            return this;
        }

        public HitDataBuilder WithReduceDamageMultiplier(float multiplier) {
            _reduceDamageMultiplier = Mathf.Clamp01(multiplier);
            return this;
        }

        public HitData Build(Transform attacker, Transform victim) {
            (int finalDamage, bool criticalHit) = CalculateDamage();

            var data = new HitData(attacker, victim) {
                weapon = _weapon,
                damage = finalDamage,
                direction = _direction,
                criticalHitChance = _criticalHitChance,
                criticalHitMultiplier = _criticalHitMultiplier,
                isCritical = criticalHit,
                timescaleData = _timescaleData
            };

            return data;
        }

        /// <summary>
        /// Calculates the damage to be dealt and if it is a critical hit or not.
        /// </summary>
        /// <returns>A tuple with the final damage and a boolean indicating if it is a critical hit or not.</returns>
        private (int damage, bool isCritical) CalculateDamage() {
            int baseDamage = _weapon == null ? _damage : Mathf.Max(_damage, _weapon.GetDamage());

            // Apply defense reduction
            int damageAfterDefense = CalculateDamageAfterDefense(baseDamage, _reduceDamageMultiplier);

            var critical = damageAfterDefense > 0 && IsCriticalHit(_criticalHitChance);
            return (critical ? Mathf.RoundToInt(damageAfterDefense * _criticalHitMultiplier) : damageAfterDefense, critical);
        }

        /// <summary>
        /// Calculates the damage after applying defense mechanisms.
        /// </summary>
        /// <param name="baseDamage">The initial amount of damage to be applied.</param>
        /// <param name="defensePercentage">The amount of defense that reduces the damage. [0-1]</param>
        /// <returns>The actual damage after defense is considered.</returns>        
        private int CalculateDamageAfterDefense(int baseDamage, float defensePercentage) {
            int reducedDamage = Mathf.RoundToInt(baseDamage * (1 - defensePercentage));
            return Mathf.Max(0, reducedDamage);
        }

        private bool IsCriticalHit(float chanceValue) {
            float randomValue = Random.value; // value between 0 and 1
            return randomValue <= chanceValue;    // Critical hit if randomValue <= chance
        }

    }
}