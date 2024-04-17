namespace StudentPortal.Models.Dtos.Responses;

public class LoggedInUserResponse
{
    public JWTToken JwtToken { get; set; }
    public string  Username { get; set; }
    public string  Email { get; set; }
}