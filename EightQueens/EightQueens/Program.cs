using System;
using System.Text;

namespace EightQueens
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("Дослідження алгоритмів BFS та RBFS для задачі 8-ферзів");

            try
            {
                var runner = new ExperimentRunner();
                runner.RunAllExperiments();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine();
                Console.WriteLine($"Сталася помилка під час виконання експериментів: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine();
            Console.WriteLine("Експерименти завершено.");
            Console.Write("Натисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}