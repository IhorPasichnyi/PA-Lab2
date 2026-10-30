using System;
using System.Collections.Generic;

public class BoardState : IEquatable<BoardState>
{
    public const int BoardSize = 8;

    // Стан: Queens[col] = row. Для ще не розміщених ферзів значення -1.
    public int[] Queens { get; }

    // Скільки стовпців (і відповідно ферзів) уже заповнено [0..8]
    public int PlacedCount { get; }

    // Значення евристики h(n) та загальної оцінки f(n) для RBFS
    public double H { get; set; }
    public double F { get; set; }

    // Конструктор початкового (порожнього) стану
    public BoardState()
    {
        Queens = new int[BoardSize];
        for (int i = 0; i < BoardSize; i++)
        {
            Queens[i] = -1;
        }
        PlacedCount = 0;
        H = 0;
        F = 0;
    }

    // Конструктор для створення нового стану на основі попереднього
    public BoardState(int[] queens, int placedCount)
    {
        Queens = (int[])queens.Clone();
        PlacedCount = placedCount;
        H = 0;
        F = 0;
    }

    // Чи є стан повністю сформованим рішенням (усі 8 ферзів розміщені)
    public bool IsGoal => PlacedCount == BoardSize;

    // Генерація всіх можливих наступних станів (додавання ферзя в наступний вільний стовпець)
    public List<BoardState> GetSuccessors()
    {
        var successors = new List<BoardState>();

        // Якщо всі 8 ферзів уже стоять, нащадків немає
        if (PlacedCount >= BoardSize)
            return successors;

        int nextCol = PlacedCount;

        // Пробуємо поставити ферзя в кожен рядок нового стовпця
        for (int row = 0; row < BoardSize; row++)
        {
            int[] newQueens = (int[])Queens.Clone();
            newQueens[nextCol] = row;
            successors.Add(new BoardState(newQueens, PlacedCount + 1));
        }

        return successors;
    }

    // Перевірка рівності станів для порівняння
    public bool Equals(BoardState other)
    {
        if (other is null) return false;
        if (PlacedCount != other.PlacedCount) return false;

        for (int i = 0; i < PlacedCount; i++)
        {
            if (Queens[i] != other.Queens[i]) return false;
        }
        return true;
    }

    public override bool Equals(object obj) => Equals(obj as BoardState);

    public override int GetHashCode()
    {
        int hash = PlacedCount;
        for (int i = 0; i < PlacedCount; i++)
        {
            hash = HashCode.Combine(hash, Queens[i]);
        }
        return hash;
    }

    // Зручний вивід стану в консоль
    public void PrintBoard()
    {
        Console.WriteLine($"--- Стан (Розміщено ферзів: {PlacedCount}) ---");
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (c < PlacedCount && Queens[c] == r)
                    Console.Write(" Q ");
                else
                    Console.Write(" . ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}