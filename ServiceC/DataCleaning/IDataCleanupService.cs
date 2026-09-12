namespace ServiceC.DataCleaning;

public interface IDataCleanupService
{
    public Task DeleteOldRecordsAsync(CancellationToken cancellationToken);
}