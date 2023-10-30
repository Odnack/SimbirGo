namespace Simbir.GO.Entities.Options;

public class JwtOptions
{
    public const string SectionOption = "JWT"; 
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}
