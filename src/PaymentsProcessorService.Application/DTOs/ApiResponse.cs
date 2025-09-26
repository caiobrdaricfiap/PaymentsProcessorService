namespace PaymentsProcessorService.Application.DTOs
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public object? Data { get; set; }
        public string? Message { get; set; }

        public ApiResponse(){ }
        public ApiResponse(bool success) {

            Success = success;
        }

        public ApiResponse(bool success, object? data, string? message = null)
        {
            Success = success;
            Data = data;
            Message = message;
        }

        public static ApiResponse Ok()
        {
            return new ApiResponse(true);
        }

        public static ApiResponse Ok(object data, string? message = null)
        {
            return new ApiResponse(true, data, message);
        }

        public static ApiResponse Fail(string message)
        {
            return new ApiResponse(false, null, message);
        }
    }
}
