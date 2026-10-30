using System.Collections.Generic;

namespace Hackenbush
{
    public static class PresetLevels
    {
        public static List<GamePreset> GetPresets()
        {
            return new List<GamePreset>
            {
                new GamePreset("Будинок", CreateHouse()),
                new GamePreset("Фігурка", CreateFigure()),
                new GamePreset("Вежа з діагоналями", CreateDiagonalTower()),
                new GamePreset("Паралельні мости", CreateBridgeNetwork())
            };
        }

        // 1. Будиночок: 2 похилі ноги від землі (0->1 та 0->2), між ними підлога (1->2) у повітрі
        private static List<Edge> CreateHouse()
        {
            return new List<Edge>
            {
                new Edge(1, 0, 1), // Ліва нога від землі вгору-ліворуч
                new Edge(2, 0, 2), // Права нога від землі вгору-праворуч
                new Edge(3, 1, 2), // Підлога у повітрі між 1 і 2
                new Edge(4, 1, 3), // Ліва стіна
                new Edge(5, 2, 4), // Права стіна
                new Edge(6, 3, 4), // Стеля
                new Edge(7, 3, 5), // Лівий дах
                new Edge(8, 4, 5)  // Правий дах
            };
        }

        // 2. Дівчинка: 2 нозі від землі (0->1 та 0->2), спідниця та тулуб у повітрі
        private static List<Edge> CreateFigure()
        {
            return new List<Edge>
            {
                new Edge(1, 0, 1), // Ліва нога від землі
                new Edge(2, 0, 2), // Права нога від землі
                new Edge(3, 1, 2), // Нижній край спідниці (у повітрі)
                new Edge(4, 1, 3), // Ліва сторона спідниці
                new Edge(5, 2, 3), // Права сторона спідниці
                new Edge(6, 3, 4), // Тулуб
                new Edge(7, 4, 5), // Ліва рука
                new Edge(8, 4, 6), // Права рука
                new Edge(9, 4, 7), // Ліва частина голови
                new Edge(10, 4, 8),// Права частина голови
                new Edge(11, 7, 8) // Макушка
            };
        }

        // 3. Вежа з діагоналями
        private static List<Edge> CreateDiagonalTower()
        {
            return new List<Edge>
            {
                new Edge(1, 0, 1), // Ліва опора від землі
                new Edge(2, 0, 2), // Права опора від землі
                new Edge(3, 1, 2), // Нижнє з'єднання у повітрі
                new Edge(4, 1, 3), // Ліва стіна
                new Edge(5, 2, 4), // Права стіна
                new Edge(6, 1, 4), // Похила діагональ 1
                new Edge(7, 2, 3), // Похила діагональ 2
                new Edge(8, 3, 4), // Верхнє з'єднання
                new Edge(9, 3, 5), // Шпиль лівий
                new Edge(10, 4, 5) // Шпиль правий
            };
        }

        // 4. Мережа мостів
        private static List<Edge> CreateBridgeNetwork()
        {
            return new List<Edge>
            {
                new Edge(1, 0, 1), // Ліва опора від землі
                new Edge(2, 0, 2), // Права опора від землі
                new Edge(3, 1, 2), // Перемичка над землею
                new Edge(4, 1, 3), // Лівий рівень 1
                new Edge(5, 2, 4), // Правий рівень 1
                new Edge(6, 3, 4), // Нижній міст
                new Edge(7, 3, 5), // Лівий рівень 2
                new Edge(8, 4, 6), // Правий рівень 2
                new Edge(9, 5, 6)  // Верхній міст
            };
        }
    }
}