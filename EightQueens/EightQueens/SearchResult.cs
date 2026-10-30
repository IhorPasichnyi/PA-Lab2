namespace EightQueens
{
    public class SearchResult
    {
        public BoardState Solution { get; set; }
        public bool IsSuccess => Solution != null;
        public long TotalNodesGenerated { get; set; }
        public long TotalIterations { get; set; }
        public long MaxMemoryNodes { get; set; }
        public double ElapsedMilliseconds { get; set; }
        public int DeadEndsCount { get; set; } // Для BFS глухий кут = відсутність нащадків у непроектному стані
    }
}