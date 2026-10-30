using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hackenbush
{
    public partial class MainWindow : Window
    {
        private GameState _currentState;
        private Difficulty _currentDifficulty = Difficulty.Medium;
        private Dictionary<int, (double X, double Y)> _vertexPositions;
        private readonly Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void OnNewGameClicked(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void StartNewGame()
        {
            // 1. Зчитуємо складність
            if (DifficultyComboBox.SelectedItem is ComboBoxItem item && Enum.TryParse<Difficulty>(item.Tag?.ToString(), out var diff))
            {
                _currentDifficulty = diff;
            }

            // 2. Обираємо РАНДОМНИЙ малюнок із заготовлених пресетів
            var presets = PresetLevels.GetPresets();
            var selectedPreset = presets[_random.Next(presets.Count)];

            _currentState = new GameState(selectedPreset.Edges, isPlayerTurn: true);

            // 3. Розраховуємо координати для обраного пресета
            SetupVertexPositions(selectedPreset.Name);

            UpdateStatusUI();
            RenderBoard();
        }

        private void SetupVertexPositions(string presetName)
        {
            _vertexPositions = new Dictionary<int, (double X, double Y)>();

            // 0 — точка торкання на лінії землі (Y = 440)
            _vertexPositions[0] = (340, 440);

            if (presetName.Contains("Будинок"))
            {
                // Вершини 1 і 2 знаходяться на Y = 360 (над землею!)
                _vertexPositions[1] = (260, 360); // Лівий нижній кут
                _vertexPositions[2] = (420, 360); // Правий нижній кут
                _vertexPositions[3] = (260, 240); // Ліва стеля
                _vertexPositions[4] = (420, 240); // Права стеля
                _vertexPositions[5] = (340, 130); // Верхівка даху
            }
            else if (presetName.Contains("Фігурка"))
            {
                // Стопи 1 і 2 підняті над землею, до землі йдуть ноги 0->1 і 0->2
                _vertexPositions[1] = (290, 370); // Ліва стопа (над землею)
                _vertexPositions[2] = (390, 370); // Права стопа (над землею)
                _vertexPositions[3] = (340, 300); // Талія / Спідниця
                _vertexPositions[4] = (340, 200); // Плечі
                _vertexPositions[5] = (250, 200); // Ліва долоня
                _vertexPositions[6] = (430, 200); // Права долоня
                _vertexPositions[7] = (300, 110); // Ліва частина голови
                _vertexPositions[8] = (380, 110); // Права частина голови
            }
            else if (presetName.Contains("Вежа"))
            {
                _vertexPositions[1] = (270, 360);
                _vertexPositions[2] = (410, 360);
                _vertexPositions[3] = (270, 240);
                _vertexPositions[4] = (410, 240);
                _vertexPositions[5] = (340, 130);
            }
            else // Паралельні мости
            {
                _vertexPositions[1] = (260, 370);
                _vertexPositions[2] = (420, 370);
                _vertexPositions[3] = (260, 260);
                _vertexPositions[4] = (420, 260);
                _vertexPositions[5] = (260, 150);
                _vertexPositions[6] = (420, 150);
            }
        }

        private void UpdateStatusUI()
        {
            if (_currentState.IsGameOver)
            {
                if (_currentState.IsPlayerTurn)
                {
                    StatusTextBlock.Text = "ГРУ ЗАВЕРШЕНО: ПЕРЕМІГ КОМП'ЮТЕР!";
                    StatusTextBlock.Foreground = Brushes.LightCoral;
                }
                else
                {
                    StatusTextBlock.Text = "ВІТАЄМО! ВИ ПЕРЕМОГЛИ!";
                    StatusTextBlock.Foreground = Brushes.LightGreen;
                }
                return;
            }

            if (_currentState.IsPlayerTurn)
            {
                StatusTextBlock.Text = "Хід: Гравець (Ваш хід)";
                StatusTextBlock.Foreground = Brushes.White;
            }
            else
            {
                StatusTextBlock.Text = "Хід: Комп'ютер (Обдумання...)";
                StatusTextBlock.Foreground = Brushes.Gray;
            }
        }

        private void RenderBoard()
        {
            GameCanvas.Children.Clear();

            // 1. Пунктирна лінія ЗЕМЛІ
            var groundLine = new Line
            {
                StartPoint = new Avalonia.Point(40, 440),
                EndPoint = new Avalonia.Point(640, 440),
                Stroke = Brushes.Black,
                StrokeThickness = 2.5,
                StrokeDashArray = new Avalonia.Collections.AvaloniaList<double> { 5, 5 } // Пунктир
            };
            GameCanvas.Children.Add(groundLine);

            // 2. Чорні суцільні ребра
            foreach (var edge in _currentState.Edges)
            {
                var posA = _vertexPositions[edge.VertexA];
                var posB = _vertexPositions[edge.VertexB];

                var line = new Line
                {
                    StartPoint = new Avalonia.Point(posA.X, posA.Y),
                    EndPoint = new Avalonia.Point(posB.X, posB.Y),
                    Stroke = Brushes.Black,
                    StrokeThickness = 3,
                    Tag = edge.Id,
                    Cursor = new Cursor(StandardCursorType.Hand)
                };

                GameCanvas.Children.Add(line);
            }

            // 3. Вершини — пусті всередині колам (біле наповнення + чорна рамка)
            foreach (var (vertexId, pos) in _vertexPositions)
            {
                if (vertexId != 0 && !_currentState.Edges.Any(e => e.VertexA == vertexId || e.VertexB == vertexId))
                    continue;

                var ellipse = new Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Fill = Brushes.White,           // Пусті всередині
                    Stroke = Brushes.Black,         // Чорна обводка
                    StrokeThickness = 2,
                    Margin = new Avalonia.Thickness(pos.X - 7, pos.Y - 7, 0, 0)
                };
                GameCanvas.Children.Add(ellipse);
            }
        }

        private void OnCanvasPointerPressed(object sender, PointerPressedEventArgs e)
        {
            if (!_currentState.IsPlayerTurn || _currentState.IsGameOver) return;

            var point = e.GetCurrentPoint(GameCanvas);

            foreach (var child in GameCanvas.Children)
            {
                if (child is Line line && line.Tag is int edgeId)
                {
                    if (IsPointNearLine(point.Position, line.StartPoint, line.EndPoint, maxDistance: 10.0))
                    {
                        _currentState = _currentState.MakeMove(edgeId);
                        UpdateStatusUI();
                        RenderBoard();

                        if (!_currentState.IsGameOver && !_currentState.IsPlayerTurn)
                        {
                            MakeComputerMoveWithDelay();
                        }
                        break;
                    }
                }
            }
        }

        private void MakeComputerMoveWithDelay()
        {
            DispatcherTimer.RunOnce(() =>
            {
                var bestMove = AlphaBetaSolver.GetBestMove(_currentState, _currentDifficulty);
                if (bestMove != null)
                {
                    _currentState = _currentState.MakeMove(bestMove.Id);
                }

                UpdateStatusUI();
                RenderBoard();

            }, TimeSpan.FromMilliseconds(1000));
        }

        private bool IsPointNearLine(Avalonia.Point pt, Avalonia.Point p1, Avalonia.Point p2, double maxDistance)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            if (dx == 0 && dy == 0) return false;

            double t = ((pt.X - p1.X) * dx + (pt.Y - p1.Y) * dy) / (dx * dx + dy * dy);
            t = Math.Clamp(t, 0, 1);

            double nearestX = p1.X + t * dx;
            double nearestY = p1.Y + t * dy;

            double dist = Math.Sqrt((pt.X - nearestX) * (pt.X - nearestX) + (pt.Y - nearestY) * (pt.Y - nearestY));
            return dist <= maxDistance;
        }
    }
}