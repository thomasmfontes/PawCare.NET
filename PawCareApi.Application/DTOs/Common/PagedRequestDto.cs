namespace PawCareApi.Application.DTOs.Common
{
    public class PagedRequestDto
    {
        private const int MaxPageSize = 50;
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
        }

        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "asc"; // "asc" ou "desc"
    }
}

