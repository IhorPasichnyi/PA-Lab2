using System;
using System.Collections.Generic;
using System.Linq;

namespace EightQueens
{
    public class ExperimentRunner
    {
        public void RunAllExperiments()
        {
            var initialStates = Generate20InitialStates();

            // 1. Експерименти для BFS
            RunSuite("1. BFS (Пошук у ширирину)", initialStates, state => new BfsSolver().Solve(state));

            // 2. Експерименти для RBFS + F1
            RunSuite("2. RBFS + F1", initialStates, state => new RbfsSolver().Solve(state, Heuristics.CalculateF1));

            // 3. Експерименти для RBFS + Custom
            RunSuite("3. RBFS + власна евристика", initialStates, state => new RbfsSolver().Solve(state, Heuristics.CalculateCustom));
        }

        /// <summary>
        /// Генерація 20 унікальних початкових станів (наприклад, із 1 випадково встановленим ферзем).
        /// </summary>
        private List<BoardState> Generate20InitialStates()
        {
            var states = new List<BoardState>();
            var usedSignatures = new HashSet<string>();
            var rand = new Random(42); // Фіксоване зерно (seed) для відтворюваності результатів

            while (states.Count < 20)
            {
                // Випадково обираємо кількість розміщених ферзів на початку (наприклад, 1 або 2)
                int initialCount = rand.Next(1, 3);
                int[] queens = new int[BoardState.BoardSize];
                Array.Fill(queens, -1);

                for (int c = 0; c < initialCount; c++)
                {
                    queens[c] = rand.Next(0, BoardState.BoardSize);
                }

                // Створюємо унікальний підпис для перевірки дублікатів
                string signature = string.Join(",", queens);

                if (!usedSignatures.Contains(signature))
                {
                    usedSignatures.Add(signature);
                    states.Add(new BoardState(queens, initialCount));
                }
            }

            return states;
        }

        private void RunSuite(string title, List<BoardState> states, Func<BoardState, SearchResult> solveFunc)
        {
            Console.WriteLine();
            Console.WriteLine(title);
            Console.WriteLine(new string('-', 83));
            Console.WriteLine($"{"#",-3} | {"Ітерації",-10} | {"Глухі кути",-11} | {"Всього вузлів",-13} | {"Всього вузлів у пам'яті",-23} | {"Час (мс)",-10}");
            Console.WriteLine(new string('-', 83));

            var results = new List<SearchResult>();

            for (int i = 0; i < states.Count; i++)
            {
                var res = solveFunc(states[i]);
                results.Add(res);

                Console.WriteLine($"{i + 1,-3} | {res.TotalIterations,-10} | {res.DeadEndsCount,-11} | {res.TotalNodesGenerated,-13} | {res.MaxMemoryNodes,-23} | {res.ElapsedMilliseconds,-10:F2}");
            }

            // Розрахунок середніх значень для таблиць у звіті
            double avgIterations = results.Average(r => r.TotalIterations);
            double avgDeadEnds = results.Average(r => r.DeadEndsCount);
            double avgGenerated = results.Average(r => r.TotalNodesGenerated);
            double avgMemory = results.Average(r => r.MaxMemoryNodes);
            double avgTime = results.Average(r => r.ElapsedMilliseconds);

            Console.WriteLine(new string('-', 83));
            Console.WriteLine($"СЕРЕДНЄ: Ітерації: {avgIterations:F1}; Глухі кути: {avgDeadEnds:F1}; Всього вузлів: {avgGenerated:F1};");
            Console.WriteLine($"         Всього вузлів: {avgGenerated:F1}; Всього вузлів у пам'яті: {avgMemory:F1}; Час: {avgTime:F2} мс");
        }
    }
}