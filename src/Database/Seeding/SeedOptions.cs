namespace Database.Seeding;

public sealed class SeedOptions
{
    public const string SectionName = "Seeding";

    public bool SeedOnStartup { get; set; }
    public string Profile { get; set; } = "Demo";
}
