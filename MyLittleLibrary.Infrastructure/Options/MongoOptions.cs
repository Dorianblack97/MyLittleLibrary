using System.ComponentModel.DataAnnotations;

namespace MyLittleLibrary.Infrastructure.Options;

public class MongoOptions
{
    [Required]
    public string ConnectionString { get; set; }
    [Required]
    public string DatabaseName { get; set; }
}