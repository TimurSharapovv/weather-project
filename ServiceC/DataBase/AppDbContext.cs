using Microsoft.EntityFrameworkCore;

namespace ServiceC.DataBase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<WeatherRecordDb> WeatherRecordsDb { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Явно указываем приватное свойство как первичный ключ
        modelBuilder.Entity<WeatherRecordDb>()
            .Property<int>("Id")
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<WeatherRecordDb>()
            .HasKey("Id");

        // Оставляем индекс по времени
        modelBuilder.Entity<WeatherRecordDb>()
            .HasIndex(wr => wr.Time);
    }
}