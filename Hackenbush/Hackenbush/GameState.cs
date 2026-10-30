using System;
using System.Collections.Generic;
using System.Linq;

namespace Hackenbush
{
    public class GameState
    {
        public const int GroundVertexId = 0;

        public List<Edge> Edges { get; private set; }
        public bool IsPlayerTurn { get; set; }

        public GameState(List<Edge> edges, bool isPlayerTurn = true)
        {
            Edges = edges.Select(e => e.Clone()).ToList();
            IsPlayerTurn = isPlayerTurn;
        }

        public List<Edge> GetAvailableMoves() => Edges.ToList();

        public GameState MakeMove(int edgeId)
        {
            var remainingEdges = Edges.Where(e => e.Id != edgeId).ToList();

            HashSet<int> connectedToGround = FindVerticesConnectedToGround(remainingEdges);

            // Клонування відбувається в конструкторі GameState, тому тут воно зайве
            var validEdges = remainingEdges
                .Where(e => connectedToGround.Contains(e.VertexA) && connectedToGround.Contains(e.VertexB))
                .ToList();

            return new GameState(validEdges, !IsPlayerTurn);
        }

        private HashSet<int> FindVerticesConnectedToGround(List<Edge> edges)
        {
            var visitedVertices = new HashSet<int> { GroundVertexId };
            var queue = new Queue<int>();
            queue.Enqueue(GroundVertexId);

            var adj = new Dictionary<int, List<int>>();
            foreach (var edge in edges)
            {
                if (!adj.ContainsKey(edge.VertexA))
                    adj[edge.VertexA] = new List<int>();

                if (!adj.ContainsKey(edge.VertexB))
                    adj[edge.VertexB] = new List<int>();

                adj[edge.VertexA].Add(edge.VertexB);
                adj[edge.VertexB].Add(edge.VertexA);
            }

            while (queue.Count > 0)
            {
                int current = queue.Dequeue();

                if (adj.TryGetValue(current, out var neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        if (!visitedVertices.Contains(neighbor))
                        {
                            visitedVertices.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return visitedVertices;
        }

        public bool IsGameOver => Edges.Count == 0;

        /// <summary>
        /// Значення Шпрага-Гранді (нім-значення) позиції в безколірному Hackenbush.
        /// Гравець, чий хід, виграє тоді й тільки тоді, коли значення != 0.
        /// Принцип злиття: усі ребра на циклах (не-мости) стають петлями, кожна петля = *1,
        /// тому важить лише парність їх кількості.
        /// Колонний принцип: решта графа - дерево, значення вузла = XOR(значення дитини + 1).
        /// Передбачається, що всі ребра з'єднані із землею (це гарантує MakeMove).
        /// </summary>
        public int GetNimValue()
        {
            if (Edges.Count == 0)
                return 0;

            // Список суміжності з id ребер (підтримує кратні ребра та петлі)
            var adj = new Dictionary<int, List<(int To, int EdgeId)>>();
            void AddAdj(int from, int to, int id)
            {
                if (!adj.TryGetValue(from, out var list))
                    adj[from] = list = new List<(int To, int EdgeId)>();
                list.Add((to, id));
            }

            foreach (var e in Edges)
            {
                AddAdj(e.VertexA, e.VertexB, e.Id);
                if (e.VertexA != e.VertexB)
                    AddAdj(e.VertexB, e.VertexA, e.Id);
            }

            // 1. Пошук мостів (tin/low)
            var tin = new Dictionary<int, int>();
            var low = new Dictionary<int, int>();
            var bridges = new HashSet<int>();
            int timer = 0;

            void Dfs(int v, int parentEdge)
            {
                tin[v] = low[v] = timer++;
                foreach (var (to, id) in adj[v])
                {
                    if (id == parentEdge)
                        continue;

                    if (tin.ContainsKey(to))
                    {
                        low[v] = Math.Min(low[v], tin[to]);
                    }
                    else
                    {
                        Dfs(to, id);
                        low[v] = Math.Min(low[v], low[to]);
                        if (low[to] > tin[v])
                            bridges.Add(id);
                    }
                }
            }

            Dfs(GroundVertexId, -1);

            // 2. Злиття вершин, з'єднаних не-мостами (DSU)
            var dsu = new Dictionary<int, int>();
            int Find(int x)
            {
                if (!dsu.TryGetValue(x, out var p))
                {
                    dsu[x] = x;
                    return x;
                }
                if (p == x)
                    return x;

                return dsu[x] = Find(p);
            }

            int loopCount = 0;
            foreach (var e in Edges)
            {
                if (bridges.Contains(e.Id))
                    continue;

                loopCount++;
                int ra = Find(e.VertexA);
                int rb = Find(e.VertexB);
                dsu[ra] = rb;
            }

            // 3. Дерево з мостів після злиття
            var tree = new Dictionary<int, List<(int To, int EdgeId)>>();
            foreach (var e in Edges)
            {
                if (!bridges.Contains(e.Id))
                    continue;

                int ra = Find(e.VertexA);
                int rb = Find(e.VertexB);

                if (!tree.TryGetValue(ra, out var la))
                    tree[ra] = la = new List<(int To, int EdgeId)>();

                if (!tree.TryGetValue(rb, out var lb))
                    tree[rb] = lb = new List<(int To, int EdgeId)>();

                la.Add((rb, e.Id));
                lb.Add((ra, e.Id));
            }

            int TreeValue(int node, int parentEdge)
            {
                int x = 0;
                if (tree.TryGetValue(node, out var children))
                {
                    foreach (var (to, id) in children)
                    {
                        if (id == parentEdge) continue;
                        x ^= TreeValue(to, id) + 1;
                    }
                }
                return x;
            }

            int root = Find(GroundVertexId);
            return TreeValue(root, -1) ^ (loopCount & 1);
        }
    }
}