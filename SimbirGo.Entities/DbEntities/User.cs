namespace Simbir.GO.Entities.DbEntities;

public class User : Entity<Guid>
{
    public string Username { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public double Money { get; set; }
}
