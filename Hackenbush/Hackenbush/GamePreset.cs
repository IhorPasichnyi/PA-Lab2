using System.Collections.Generic;

namespace Hackenbush
{
    public class GamePreset
    {
        public string Name { get; }
        public List<Edge> Edges { get; }

        public GamePreset(string name, List<Edge> edges)
        {
            Name = name;
            Edges = edges;
        }
    }
}
