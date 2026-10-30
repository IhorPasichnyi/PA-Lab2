using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EightQueens
{
    public class BfsSolver
    {
        /// <summary>
        /// Запуск BFS для пошуку розв'язку.
        /// </summary>
        public SearchResult Solve(BoardState initialState, double maxTimeMinutes = 5.0)
        {
            var result = new SearchResult();
            var stopwatch = Stopwatch.StartNew();
            TimeSpan timeLimit = TimeSpan.FromMinutes(maxTimeMinutes);

            Queue<BoardState> queue = new Queue<BoardState>();

            // Перевіряємо початковий стан
            if (initialState.IsGoal && Heuristics.CalculateF1(initialState) == 0)
            {
                stopwatch.Stop();
                result.Solution = initialState;
                result.ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
                return result;
            }

            queue.Enqueue(initialState);
            result.TotalNodesGenerated++;
            result.MaxMemoryNodes = queue.Count;

            while (queue.Count > 0)
            {
                // 1. Перевірка обмеження по часу
                if (stopwatch.Elapsed > timeLimit)
                {
                    Console.WriteLine("Перевищено ліміт часу (5 хвилин)!");
                    break;
                }

                BoardState current = queue.Dequeue();
                result.TotalIterations++;

                // 2. Генерація наступників
                List<BoardState> successors = current.GetSuccessors();

                if (successors.Count == 0)
                {
                    result.DeadEndsCount++;
                    continue;
                }

                foreach (var successor in successors)
                {
                    result.TotalNodesGenerated++;

                    // Перевіряємо, чи є новий стан розв'язком
                    if (successor.IsGoal)
                    {
                        if (Heuristics.CalculateF1(successor) == 0)
                        {
                            stopwatch.Stop();
                            result.Solution = successor;
                            result.ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;

                            // Оновлюємо пік пам'яті перед виходом
                            if (queue.Count > result.MaxMemoryNodes)
                                result.MaxMemoryNodes = queue.Count;

                            return result; // Знайшли перший (найкоротший) розв'язок
                        }
                        else
                        {
                            // Дійшли до 8-го рівня, але є конфлікти — глухий кут
                            result.DeadEndsCount++;
                        }
                    }
                    else
                    {
                        queue.Enqueue(successor);
                    }
                }

                // 3. Фіксування максимуму вузлів у пам'яті
                if (queue.Count > result.MaxMemoryNodes)
                {
                    result.MaxMemoryNodes = queue.Count;
                }
            }

            stopwatch.Stop();
            result.ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds;
            return result;
        }
    }
}