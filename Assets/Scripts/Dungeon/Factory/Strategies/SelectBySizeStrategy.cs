using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectBySizeStrategy")]
    public class SelectBySizeStrategy : SelectStrategy {
        [SerializeField]
        private float _sizeFactor = 1.25f;

        private int _widthMean;
        private int _heightMean;

        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;

            _widthMean = (int)rooms.Average(room => room.Width);
            _heightMean = (int)rooms.Average(room => room.Height);
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();
                
            // we can also only care about rooms with only width or height greater than the mean
            return _rooms
                .Where(room => room.Width >= _widthMean * _sizeFactor || room.Height >= _heightMean * _sizeFactor)
                .Take(maxToTake)
                .ToList();
        }
    }
}