using Microsoft.EntityFrameworkCore;
using ServiceC.DataBase;

namespace ServiceC.Storage;

public class WeatherStorageService(AppDbContext context): IWeatherStorage
{
    private readonly AppDbContext _context = context;

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
    }

    public async Task<IEnumerable<WeatherRecordDb>> GetLastRecordsAsync(int count, CancellationToken cancellationToken)
    {
        var ten_records = _context.WeatherRecordsDb.OrderByDescending(x => x.Time).Take(count);
        return await ten_records.ToListAsync(cancellationToken);
    }
}