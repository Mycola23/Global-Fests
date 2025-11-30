namespace GlobalFests.DTOs
{
    public class CursorResult<T>
    {
        public List<T> Items { get; set; } = new();
        public DateTime? NextCursorDate { get; set; }
        public int? NextCursorId { get; set; }
        public bool HasNextPage { get; set; }
    }
}
