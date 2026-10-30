namespace Hackenbush
{
    public class Edge
    {
        public int Id { get; }
        public int VertexA { get; }
        public int VertexB { get; }

        public Edge(int id, int vertexA, int vertexB)
        {
            Id = id;
            VertexA = vertexA;
            VertexB = vertexB;
        }

        public Edge Clone() => new Edge(Id, VertexA, VertexB);
    }
}
