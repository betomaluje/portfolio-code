using BerserkPixel.Health;
using Weapons;

namespace Base {
    public interface ICharacterHolder {
        public CharacterAnimations Animations { get; }

        public IMove Movement { get; }

        public IWeaponManager WeaponManager { get; }

        public IHealth Health { get; }
    }
}