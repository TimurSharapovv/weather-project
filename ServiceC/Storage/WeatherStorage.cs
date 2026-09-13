using Microsoft.EntityFrameworkCore;
using ServiceC.DataBase;

namespace ServiceC.Storage;

public class WeatherStorage(AppDbContext context, ILogger<WeatherStorage> logger): IWeatherStorage
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<WeatherStorage> _logger = logger;
    
    public async Task SaveWeatherRecord(Request request, CancellationToken cancellationToken)
    {
        WeatherRecordDb record = new WeatherRecordDb()
        {
            Temperature = request.Temperature, 
            Humidity = request.Humidity, 
            Description = request.Description,
            Time = request.Time.ToDateTime()
        };
        _context.WeatherRecordsDb.Add(record);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("New weather record added {record}", record);
    }

    public async Task<IEnumerable<WeatherRecordDb>> GetLastRecordsAsync(int count, CancellationToken cancellationToken)
    {
        var ten_records = _context.WeatherRecordsDb.OrderByDescending(x => x.Time).Take(count);
        return await ten_records.ToListAsync(cancellationToken);
    }
    
    public async Task DeleteOlderThanAsync(CancellationToken cancellationToken = default)
    {
        var threshold = DateTime.UtcNow.AddHours(-10);
        
        var deletedCount =  await _context.WeatherRecordsDb
            .Where(x => x.Time < threshold)
            .ExecuteDeleteAsync(cancellationToken);
        
        _logger.LogInformation(
            "Deleted {Count} old weather records",
            deletedCount);
    }
}