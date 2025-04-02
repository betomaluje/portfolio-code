using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dungeon.Factory.Strategies {
    /// <summary>
    /// Evolves a population of room selections, using crossover and mutation to find an optimal set.
    /// </summary>
    [CreateAssetMenu(menuName = "Dungeon/Factory/Strategies/SelectGeneticStrategy")]
    public class SelectGeneticStrategy : SelectStrategy {
        [SerializeField]
        private int _minMainRooms = 6;

        [SerializeField]
        private int generations;

        private int _populationSize;
        private List<Room> _rooms;

        override public void Setup(List<Room> rooms) {
            _rooms = rooms;
            _populationSize = _rooms.Count;
        }

        public override List<Room> SelectMainRooms(int maxToTake) {
            if (maxToTake <= 0 || _rooms == null || _rooms.Count == 0)
                return new();

            // Step 1: Create an initial population of random room selections
            List<List<Room>> population = InitializePopulation(_populationSize);

            for (int generation = 0; generation < generations; generation++) {
                // Step 2: Evaluate fitness of each individual
                List<(List<Room> rooms, float fitness)> fitnessScores = population
                    .Select(individual => (individual, CalculateFitness(individual)))
                    .OrderByDescending(individual => individual.Item2) // Item2 is the CalculateFitness value
                    .ToList();

                // Step 3: Selection (Keep the best individuals)
                List<List<Room>> newPopulation = fitnessScores
                    .Take(_populationSize / 2)
                    .Select(individual => individual.rooms)
                    .ToList();

                // Step 4: Crossover and Mutation
                while (newPopulation.Count < _populationSize) {
                    // Crossover: Select two parents and create offspring
                    var parent1 = newPopulation[Random.Range(0, newPopulation.Count)];
                    var parent2 = newPopulation[Random.Range(0, newPopulation.Count)];
                    List<Room> offspring = Crossover(parent1, parent2);

                    // Mutation: Randomly swap rooms
                    if (Random.Range(0f, 1f) < 0.1f) {
                        offspring = MutateRooms(offspring);
                    }

                    newPopulation.Add(offspring);
                }

                population = newPopulation;
            }

            // Return the best individual from the final population
            return population.OrderByDescending(p => CalculateFitness(p)).First();
        }

        // Helper function: Initialize a population with random room selections
        private List<List<Room>> InitializePopulation(int populationSize) {
            List<List<Room>> population = new List<List<Room>>();

            for (int i = 0; i < populationSize; i++) {
                population.Add(GetRandomRooms(_minMainRooms));
            }

            return population;
        }

        // Helper function: Get a random set of rooms
        private List<Room> GetRandomRooms(int count) => _rooms.SimpleShuffle().Take(count);

        // Helper function: Calculate fitness (score) for a given room selection
        private float CalculateFitness(List<Room> rooms) {
            return CalculateScore(rooms); // Reuse the score calculation from Simulated Annealing
        }

        // Fitness function to calculate the "quality" of a room selection
        private float CalculateScore(List<Room> rooms) {
            // Example score: maximize the area and minimize the distance between rooms
            float totalArea = rooms.Sum(r => r.Width * r.Height);
            float totalDistance = 0f;

            for (int i = 0; i < rooms.Count; i++) {
                for (int j = i + 1; j < rooms.Count; j++) {
                    totalDistance += Vector2Int.Distance(rooms[i].Center, rooms[j].Center);
                }
            }

            return totalArea - totalDistance; // Higher score is better
        }

        // Helper function: Mutate the current room selection by swapping a random room
        private List<Room> MutateRooms(List<Room> rooms) {
            List<Room> mutatedRooms = new(rooms);

            // Randomly remove one room and add a different random room
            mutatedRooms.RemoveAt(Random.Range(0, mutatedRooms.Count));
            Room randomNewRoom = rooms[Random.Range(0, rooms.Count)];

            while (mutatedRooms.Contains(randomNewRoom)) {
                randomNewRoom = rooms[Random.Range(0, rooms.Count)];
            }

            mutatedRooms.Add(randomNewRoom);

            return mutatedRooms;
        }

        // Helper function: Crossover two parents to produce offspring
        private List<Room> Crossover(List<Room> parent1, List<Room> parent2) {
            int crossoverPoint = Random.Range(0, parent1.Count);
            List<Room> offspring = new List<Room>(parent1.Take(crossoverPoint));

            foreach (Room room in parent2) {
                if (!offspring.Contains(room) && offspring.Count < parent1.Count) {
                    offspring.Add(room);
                }
            }

            return offspring;
        }

    }
}