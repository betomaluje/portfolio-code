using System.Collections.Generic;
using UnityEngine;

namespace Dungeon {
    public interface IFillChunk {
        List<Vector3Int> GenerateChunks();

        void Dispose();
    }
}