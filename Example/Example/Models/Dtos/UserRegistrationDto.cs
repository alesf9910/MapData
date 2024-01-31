namespace Example.Models.Dto;

public record class UserRegistrationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
    public string Birthday { get; set; }
}
