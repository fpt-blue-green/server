using BusinessObjects.Enum;

namespace BusinessObjects.ModelsDTO
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
