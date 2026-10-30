using System;
using System.Collections.Generic;

namespace Hackenbush
{
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public static class AlphaBetaSolver
    {
        private static readonly Random random = new Random();

        private const double WinScore = 10000.0;
        private const double HeuristicScore = 1000.0;

        // Отримати найкращий хід для комп'ютера залежно від рівня складності.
        public static Edge? GetBestMove(GameState currentState, Difficulty difficulty)
        {
            var moves = currentState.GetAvailableMoves();
            if (moves.Count == 0)
                return null;

            // Оскільки евристика точна, складність регулюється ймовірністю випадкового ходу
            double mistakeChance = difficulty switch
            {
                Difficulty.Easy => 0.5,
                Difficulty.Medium => 0.2,
                Difficulty.Hard => 0.0,
                _ => 0.2
            };

            if (random.NextDouble() < mistakeChance)
                return moves[random.Next(moves.Count)];

            int maxDepth = difficulty switch
            {
                Difficulty.Easy => 2,
                Difficulty.Medium => 3,
                Difficulty.Hard => 4,
                _ => 3
            };

            Edge? bestMove = null;
            double bestValue = double.NegativeInfinity;
            double alpha = double.NegativeInfinity;
            double beta = double.PositiveInfinity;

            foreach (var move in moves)
            {
                GameState nextState = currentState.MakeMove(move.Id);

                // Після ходу комп'ютера (Max) ходить гравець (Min)
                double moveValue = AlphaBeta(nextState, maxDepth - 1, alpha, beta, false);

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }

                alpha = Math.Max(alpha, bestValue);
            }

            return bestMove ?? moves[0];
        }

        private static double AlphaBeta(GameState state, int depth, double alpha, double beta, bool isMaximizingPlayer)
        {
            // Немає ходів - той, чий хід, програє. Швидша перемога краща, повільніша поразка краща.
            if (state.IsGameOver)
            {
                return isMaximizingPlayer ? -WinScore - depth : WinScore + depth;
            }

            if (depth <= 0)
            {
                return EvaluateState(state, isMaximizingPlayer);
            }

            var moves = state.GetAvailableMoves();

            if (isMaximizingPlayer)
            {
                double maxEval = double.NegativeInfinity;
                foreach (var move in moves)
                {
                    GameState nextState = state.MakeMove(move.Id);
                    double eval = AlphaBeta(nextState, depth - 1, alpha, beta, false);
                    maxEval = Math.Max(maxEval, eval);
                    alpha = Math.Max(alpha, eval);
                    if (beta <= alpha)
                        break;
                }
                return maxEval;
            }
            else
            {
                double minEval = double.PositiveInfinity;
                foreach (var move in moves)
                {
                    GameState nextState = state.MakeMove(move.Id);
                    double eval = AlphaBeta(nextState, depth - 1, alpha, beta, true);
                    minEval = Math.Min(minEval, eval);
                    beta = Math.Min(beta, eval);
                    if (beta <= alpha)
                        break;
                }
                return minEval;
            }
        }

        // Оцінка завжди з погляду Max-гравця (комп'ютера).
        // Нім-значення != 0 означає, що виграє той, чий хід.
        private static double EvaluateState(GameState state, bool isMaximizingPlayer)
        {
            bool moverWins = state.GetNimValue() != 0;
            bool maxWins = isMaximizingPlayer ? moverWins : !moverWins;
            return maxWins ? HeuristicScore : -HeuristicScore;
        }
    }
}