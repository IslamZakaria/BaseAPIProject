namespace DTOs.Shared.Responses
{
    public class PagedResponse<T> : Response<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long pg_total { get; set; }

        public PagedResponse(T data, int pageNumber, int pageSize, long pg_total = 0)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Data = data;
            Message = null;
            Succeeded = true;
            Errors = null;
            StatusCode = 200;
            this.pg_total = pg_total;
        }
        public PagedResponse(string message) : base(message)
        {

        }
    }
}
