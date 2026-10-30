using System.Diagnostics;

namespace EightQueens
{
    public class RbfsSolver
    {
        private long _nodesGenerated;
        private long _iterations;
        private long _maxMemoryNodes;
        private int _deadEndsCount;
        private Stopwatch _stopwatch;
        private TimeSpan _timeLimit;
        private Func<BoardState, double> _heuristicFunc;

        public SearchResult Solve(BoardState initialState, Func<BoardState, double> heuristic = null, double maxTimeMinutes = 5.0)
        {
            _nodesGenerated = 0;
            _iterations = 0;
            _maxMemoryNodes = 0;
            _deadEndsCount = 0;
            _heuristicFunc = heuristic ?? Heuristics.CalculateF1;

            _stopwatch = Stopwatch.StartNew();
            _timeLimit = TimeSpan.FromMinutes(maxTimeMinutes);

            var result = new SearchResult();

            // Початкове обчислення f(n) = g(n) + h(n)
            initialState.H = _heuristicFunc(initialState);
            initialState.F = initialState.PlacedCount + initialState.H;
            _nodesGenerated++;

            var (solution, _) = Rbfs(initialState, double.PositiveInfinity, 1);

            _stopwatch.Stop();
            result.Solution = solution;
            result.TotalNodesGenerated = _nodesGenerated;
            result.TotalIterations = _iterations;
            result.MaxMemoryNodes = _maxMemoryNodes;
            result.DeadEndsCount = _deadEndsCount;
            result.ElapsedMilliseconds = _stopwatch.Elapsed.TotalMilliseconds;

            return result;
        }

        private (BoardState solution, double newF) Rbfs(BoardState node, double fLimit, int currentDepth)
        {
            _iterations++;

            if (currentDepth > _maxMemoryNodes)
            {
                _maxMemoryNodes = currentDepth;
            }

            if (_stopwatch.Elapsed > _timeLimit)
            {
                return (null, double.PositiveInfinity);
            }

            // Перевірка цільового стану
            if (node.IsGoal)
            {
                if (_heuristicFunc(node) == 0)
                {
                    return (node, node.F);
                }

                // Якщо 8 ферзів стоять, але є конфлікт — це тупик
                _deadEndsCount++;
                return (null, double.PositiveInfinity);
            }

            // Генерація нащадків
            List<BoardState> successors = node.GetSuccessors();

            if (successors.Count == 0)
            {
                _deadEndsCount++;
                return (null, double.PositiveInfinity);
            }

            // Обчислення f-значень для всіх згенерованих нащадків
            foreach (var child in successors)
            {
                _nodesGenerated++;
                child.H = _heuristicFunc(child);

                double gChild = child.PlacedCount;
                double calculatedF = gChild + child.H;
                child.F = Math.Max(node.F, calculatedF);
            }

            while (true)
            {
                // Сортуємо нащадків за зростанням f-значення
                successors.Sort((a, b) => a.F.CompareTo(b.F));

                BoardState best = successors[0];

                // Якщо найкраще f-значення перевищує межу fLimit або є нескінченністю
                if (best.F > fLimit || double.IsPositiveInfinity(best.F))
                {
                    return (null, best.F);
                }

                // Альтернативне значення
                double alternativeF = successors.Count > 1 ? successors[1].F : double.PositiveInfinity;

                // Рекурсивний виклик
                var (solution, newF) = Rbfs(best, Math.Min(fLimit, alternativeF), currentDepth + 1);

                if (solution != null)
                {
                    return (solution, newF);
                }

                // Оновлюємо f-значення
                best.F = newF;
            }
        }
    }
}