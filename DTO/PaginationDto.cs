namespace amazon_backend.DTO
{
    public class PaginationDto
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? orderBy { get; set; }
    }
}
