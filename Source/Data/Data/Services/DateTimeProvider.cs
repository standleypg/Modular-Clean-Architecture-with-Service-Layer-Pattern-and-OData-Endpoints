
using RetailPortal.DataFacade.Services;

namespace RetailPortal.Data.Services;

public sealed class DateTimeProvider: IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}