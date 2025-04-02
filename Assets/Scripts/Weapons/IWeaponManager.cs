using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Weapons {
    public interface IWeaponManager {
        Weapon Weapon { get; }

        CharacterAnimations Animations { get; }

        bool CanAttack();

        void Attack();

        void Attack(Vector2 direction, float chargePower = 1f);

        void ChangeWeapon(int weaponIndex);

        void ChangeNextWeapon();

        void ChangePreviousWeapon();

        void OverrideWeapons(List<Weapon> weapons, int lastSelectedWeapon);

        void Equip(Weapon weapon);

        void EquipAll(IList<Weapon> weapons);

        void Clear();

        void SetStrengthInfluence(float strength);

        void ResetStrengthInfluence();

        int TotalWeapons { get; }
    }
}