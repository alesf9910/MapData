namespace Example.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public DateOnly Birthday { get; set; }
    public DateOnly CreatedDay { get; set; }
}
