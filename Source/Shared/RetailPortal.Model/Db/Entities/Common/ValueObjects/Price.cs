using RetailPortal.Model.Db.Entities.Common.Base;

namespace RetailPortal.Model.Db.Entities.Common.ValueObjects;

public sealed class Price: ValueObject
{
    public decimal Value { get; }
    public string Currency { get; }

    private Price(decimal value, string currency)
    {
        this.Value = value;
        this.Currency = currency;
    }

    public static Price Create(decimal value, string currency)
    {
        return new Price(value, currency);
    }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Value;
        yield return this.Currency;
    }
}