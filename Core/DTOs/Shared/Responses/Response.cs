namespace DTOs.Shared.Responses
{
    public class Response<T>
    {
        public Response()
        {
        }
        public Response(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
            StatusCode = 200;
        }

        public Response(string message)
        {
            Succeeded = false;
            Message = message;
            StatusCode = 400;
        }

        public Response(string message, int statusCode)
        {
            Succeeded = false;
            Message = message;
            StatusCode = statusCode;
            StatusCode = 400;
        }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }
    }
}
