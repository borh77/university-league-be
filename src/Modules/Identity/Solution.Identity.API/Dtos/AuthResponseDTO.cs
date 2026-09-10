namespace Solution.Identity.API.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
    public DateTime ExpiresAt { get; set; }
}
