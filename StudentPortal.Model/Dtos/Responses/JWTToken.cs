namespace StudentPortal.Models.Dtos.Responses;

public class JWTToken
{
    public string Token { get; set; }
    public DateTime Issued { get; set; }
    public DateTime? Expires { get; set; }
}