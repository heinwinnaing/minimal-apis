using System.Text.Json.Serialization;

namespace MinimalApis.Model;

public class ResultModel
{
    [JsonIgnore]
    public bool IsSuccess { get; init; } = false;
    public int Code { get; set; }
    public string? Message { get; set; }
    public static ResultModel Success() => new ResultModel { Code = 0, Message = "Ok", IsSuccess = true };
    public static ResultModel Error(int errorCode, string errorMessage) => new ResultModel { Code = errorCode, Message = errorMessage, IsSuccess = false };
    public static ResultModel Error(int errorCode, string[] errorMessages) 
        => new ResultModel 
        { 
            Code = errorCode, 
            Message = string.Join(',', errorMessages), 
            IsSuccess = false 
        };
}

public class ResultModel<T>
    : ResultModel
{
    public T? Data { get; private set; }
    public static ResultModel<T> Success(T data)
    {
        return new ResultModel<T>
        {
            IsSuccess = true,
            Code = 0,
            Message = "Ok",
            Data = data
        };
    }
    public static new ResultModel<T> Error(int errorCode, string errorMessage)
    {
        return new ResultModel<T>
        {
            IsSuccess = false,
            Code = errorCode,
            Message = errorMessage
        };
    }
    public static new ResultModel<T> Error(int errorCode, string[] errorMessages)
    {
        return new ResultModel<T>
        {
            IsSuccess = false,
            Code = errorCode,
            Message = string.Join (",", errorMessages),
        };
    }
}
