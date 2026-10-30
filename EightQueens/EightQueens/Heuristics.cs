namespace EightQueens
{
    public static class Heuristics
    {
        private static readonly int[] DRow = { 0, -1, 1 };
        private static readonly int[] DCol = { 1, 1, 1 };

        /// <summary>
        /// F1: Кількість пар ферзів, які б'ють один одного з урахуванням видимості.
        /// </summary>
        public static double CalculateF1(BoardState state)
        {
            int placed = state.PlacedCount;

            if (placed <= 1)
                return 0; // Для 0 або 1 ферзя конфліктів бути не може

            int conflicts = 0;

            // Матриця для швидкої перевірки наявності ферзя
            bool[,] board = new bool[BoardState.BoardSize, BoardState.BoardSize];
            for (int c = 0; c < placed; c++)
            {
                int r = state.Queens[c];
                if (r != -1)
                {
                    board[r, c] = true;
                }
            }

            for (int c = 0; c < placed; c++)
            {
                int r = state.Queens[c];
                if (r == -1) continue;

                for (int dir = 0; dir < 3; dir++)
                {
                    int currR = r + DRow[dir];
                    int currC = c + DCol[dir];

                    // Пускаємо промінь лише в межах дошки та вже розміщених стовпчиків
                    while (currR >= 0 && currR < BoardState.BoardSize &&
                           currC >= 0 && currC < placed)
                    {
                        if (board[currR, currC])
                        {
                            conflicts++;
                            break; // Перший ферзь закриває видимість для наступних
                        }

                        currR += DRow[dir];
                        currC += DCol[dir];
                    }
                }
            }

            return conflicts;
        }

        public static double CalculateCustom(BoardState state)
        {
            double f1 = CalculateF1(state);
            int placed = state.PlacedCount;

            if (placed == 0 || placed == BoardState.BoardSize)
                return f1;

            // Масиви зайнятих ліній розміщеними ферзями
            bool[] usedRows = new bool[BoardState.BoardSize];
            bool[] usedDiagMain = new bool[2 * BoardState.BoardSize]; // r - c + 7
            bool[] usedDiagAnti = new bool[2 * BoardState.BoardSize]; // r + c

            for (int c = 0; c < placed; c++)
            {
                int r = state.Queens[c];
                if (r != -1)
                {
                    usedRows[r] = true;
                    usedDiagMain[r - c + 7] = true;
                    usedDiagAnti[r + c] = true;
                }
            }

            int fullyBlockedColumns = 0;
            int totalAttackedFutureCells = 0;

            for (int targetCol = placed; targetCol < BoardState.BoardSize; targetCol++)
            {
                int safeRowsInColumn = 0;

                for (int row = 0; row < BoardState.BoardSize; row++)
                {
                    // O(1) перевірка загрози замість внутрішнього циклу
                    if (usedRows[row] ||
                        usedDiagMain[row - targetCol + 7] ||
                        usedDiagAnti[row + targetCol])
                    {
                        totalAttackedFutureCells++;
                    }
                    else
                    {
                        safeRowsInColumn++;
                    }
                }

                if (safeRowsInColumn == 0)
                {
                    fullyBlockedColumns++;
                }
            }

            double deadEndPenalty = fullyBlockedColumns * 10.0;
            double densityPenalty = totalAttackedFutureCells * 0.1;

            return f1 + deadEndPenalty + densityPenalty;
        }
    }
}