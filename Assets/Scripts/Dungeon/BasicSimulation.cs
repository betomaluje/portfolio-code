using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Dungeon {
    public class BasicSimulation {
        public static async UniTask Simulate(int maxIterations = 50) {
            Physics2D.simulationMode = SimulationMode2D.Script;

            for (var i = 0; i < maxIterations; i++) {
                // do nothing if simulation is not set to be executed via script.
                if (Physics2D.simulationMode != SimulationMode2D.Script)
                    break;

                Physics2D.Simulate(Time.fixedDeltaTime);
                await UniTask.Yield();
            }

            await UniTask.Delay(500);

            Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        }
    }

}