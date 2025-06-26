namespace AuthApi.Dtos;
public class RegisterRequest(string FullName, string Email, string Password)
{
    public string? Email { get; internal set; } = Email;
    public string? FullName { get; internal set; } = FullName;
    public string? Password { get; internal set; } = Password;
}
        

