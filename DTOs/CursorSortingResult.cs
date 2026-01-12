namespace GlobalFests.DTOs
{
    public class CursorSortingResult<T>
    {
        public List<T> Items { get; set; } = new();
        public string? NextCursorValue { get; set; }
        public int? NextCursorId { get; set; }
        public bool HasNextPage { get; set; }
    }
}
