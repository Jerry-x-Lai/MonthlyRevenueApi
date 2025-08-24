namespace MonthlyRevenueApi.Models.Base
{
    // 非泛型版本，只包含 IsSuccess 與 Msg
    public class ApiResponse
    {
    public bool IsSuccess { get; set; }
    public string Msg { get; set; } = string.Empty;

    public ApiResponse() { }

    public ApiResponse(bool isSuccess, string msg)
    {
        IsSuccess = isSuccess;
        Msg = msg;
    }

    public static ApiResponse Success(string msg = "")
        => new ApiResponse(true, msg);

    public static ApiResponse Fail(string msg)
        => new ApiResponse(false, msg);
    }

    // 泛型版本，保留原本功能
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
