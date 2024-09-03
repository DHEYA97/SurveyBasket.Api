namespace SurveyBasket.Api.Abstractions
{
    public class PageList<T>(IList<T> items,int pageNumber,int count ,int pageSize)
    {
        public IList<T> Items { get; private set; } = items;
        public int PageNumber { get; private set; } = pageNumber;
        public int TotalPages { get; private set; } = (int)Math.Ceiling(count / (double)pageSize);
        public int TotalItems { get; private set; } = count;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;


        public static async Task<PageList<T>> CreateAsync(IQueryable<T> source , int pageNumber,int pageSize,CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PageList<T>(items,pageNumber,count,pageSize);
        }
    }
}
