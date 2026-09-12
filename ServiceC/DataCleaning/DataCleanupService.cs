using ServiceC.Storage;

namespace ServiceC.DataCleaning;

public class DataCleanupService(IWeatherStorage storage): IDataCleanupService
{
    private readonly IWeatherStorage _storage = storage;

    public async Task DeleteOldRecordsAsync(CancellationToken cancellationToken = default)
    {
        await _storage.DeleteOlderThanAsync(cancellationToken);
    }
    
}