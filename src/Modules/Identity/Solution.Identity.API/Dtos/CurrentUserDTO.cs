namespace Solution.Identity.API.Dtos;

public class CurrentUserDto
{
    public long Id { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public string Role { get; set; }
}
