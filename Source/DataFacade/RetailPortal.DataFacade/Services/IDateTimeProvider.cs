namespace RetailPortal.DataFacade.Services;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}