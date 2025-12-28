using RetailPortal.Model.Db.Entities.Common.Base;

namespace RetailPortal.Model.Db.Entities;

public sealed class Address : EntityBase
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }

    public string FullAddress => $"{this.Street}, {this.City}, {this.State}, {this.PostalCode}, {this.Country}";

    public long UserId { get; set; }
    public User User { get; private set; } = default!;

    private Address(string street, string city, string state, string postalCode, string country)
    {
        this.Street = street;
        this.City = city;
        this.State = state;
        this.PostalCode = postalCode;
        this.Country = country;
    }

    public static Address Create(string street, string city, string state, string postalCode, string country)
    {
        // if (string.IsNullOrWhiteSpace(street))
        //     return Result<Address>.Failure("Street must not be empty");
        //
        // if (string.IsNullOrWhiteSpace(city))
        //     return Result<Address>.Failure("City must not be empty");
        //
        // if (string.IsNullOrWhiteSpace(state))
        //     return Result<Address>.Failure("State must not be empty");
        //
        // if (string.IsNullOrWhiteSpace(postalCode))
        //     return Result<Address>.Failure("Postal code must not be empty");
        //
        // if (string.IsNullOrWhiteSpace(country))
        //     return Result<Address>.Failure("Country must not be empty");

        return new Address(street, city, state, postalCode, country);
    }
}