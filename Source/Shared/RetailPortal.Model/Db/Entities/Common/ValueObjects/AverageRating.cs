using RetailPortal.Model.Db.Entities.Common.Base;

namespace RetailPortal.Model.Db.Entities.Common.ValueObjects;

public sealed class AverageRating : ValueObject
{
    private int TotalRating { get; }
    private int NumberOfRatings { get; }
    public decimal Value => this.NumberOfRatings == 0 ? 0 : (decimal)this.TotalRating / this.NumberOfRatings;

    private AverageRating(int totalRating, int numberOfRatings)
    {
        this.TotalRating = totalRating;
        this.NumberOfRatings = numberOfRatings;
    }

    public static AverageRating Create(int totalRating = 0, int numberOfRatings = 0)
    {
        return new AverageRating(totalRating, numberOfRatings);
    }

    public AverageRating AddRating(int rating)
    {
        return new AverageRating(this.TotalRating + rating, this.NumberOfRatings + 1);
    }

    public override string ToString() => this.Value.ToString("0.0");

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.TotalRating;
        yield return this.NumberOfRatings;
    }
}