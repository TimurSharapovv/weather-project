using ServiceC.DataBase;

namespace ServiceC.Storage;

public interface IWeatherStorage
{
    public Task SaveWeatherRecord(Request request, CancellationToken cancellationToken);
    
    public Task<IEnumerable<WeatherRecordDb>> GetLastRecordsAsync(int count, CancellationToken cancellationToken);
    
    public Task DeleteOlderThanAsync(CancellationToken cancellationToken = default); 
}   