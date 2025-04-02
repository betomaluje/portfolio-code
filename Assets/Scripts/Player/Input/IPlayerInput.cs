using System;
using UnityEngine;

namespace Player.Input {
    public interface IPlayerInput {
        Vector2 GetMoveDirection();

        Vector2 GetAimDirection();

        public event Action<bool> SouthButtonEvent;
        public event Action EastButtonEvent;
        public event Action NorthButtonEvent;
        public event Action WestButtonEvent;
    }
}