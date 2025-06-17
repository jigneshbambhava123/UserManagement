namespace UserManagement.ViewModels;

public class PaginatedList<T>
{
        public List<T> Items { get; }
        public int TotalItems { get; }
        public int PageSize { get; }
        public int CurrentPage { get; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        public PaginatedList(List<T> items, int totalItems, int currentPage, int pageSize)
        {
            Items = items;
            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;    
        }
}
