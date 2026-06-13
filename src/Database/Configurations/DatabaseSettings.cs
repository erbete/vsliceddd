using System.ComponentModel.DataAnnotations;

namespace Database.Configurations;

public sealed class DatabaseSettings
{
	public const string SectionName = "Database";

	[Required(ErrorMessage = "Database connection string is required")]
	public required string DefaultConnection { get; init; }
}