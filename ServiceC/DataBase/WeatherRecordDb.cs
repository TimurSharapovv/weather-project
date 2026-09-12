using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceC.DataBase;

[Table("weather_records")]
public class WeatherRecordDb
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    private int Id { get; set; }

    [Required]
    [Column("temperature")]
    public float Temperature { get; set; }

    [Required]
    [Column("humidity")]
    public int Humidity { get; set; }

    [Required]
    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Column("time")]
    public DateTime Time { get; set; }
}