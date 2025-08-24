namespace MonthlyRevenueApi.Models.Base
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Msg { get; set; } = string.Empty;

        public ApiResponse() { }

        public ApiResponse(bool isSuccess, T? data, string msg)
        {
            IsSuccess = isSuccess;
            Data = data;
            Msg = msg;
        }

        public static ApiResponse<T> Success(T? data, string msg = "")
            => new ApiResponse<T>(true, data, msg);

        public static ApiResponse<T> Fail(string msg, T? data = default)
            => new ApiResponse<T>(false, data, msg);
    }
}
