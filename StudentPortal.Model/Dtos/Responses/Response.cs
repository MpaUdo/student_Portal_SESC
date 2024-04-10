using Newtonsoft.Json;

namespace StudentPortal.Models.Dtos.Responses;

public class SuccessResponse
{
    public bool Success { get; set; }
    public object Data { get; set; }
}

public class ErrorResponse
{
    public int Status { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
public enum ResponseStatus
{
    OK = 1,
    APP_ERROR = 2,
    FATAL_ERROR = 3,
    NOT_FOUND = 4
}