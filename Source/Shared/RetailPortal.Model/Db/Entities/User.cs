using RetailPortal.Model.Db.Entities.Common;
using RetailPortal.Model.Db.Entities.Common.Base;
using RetailPortal.Model.Db.Entities.Common.ValueObjects;
using System.Text.Json.Serialization;

namespace RetailPortal.Model.Db.Entities;

public sealed class User : EntityBase
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public Password? Password { get; private set; }
    public TokenProvider TokenProvider { get; private set; }
    public ICollection<Address> Addresses { get; private set; }
    public ICollection<Role> Roles { get; private set; }
    public ICollection<Product>? Products { get; private set;}

    // Whenever there is ValueObject, parameterless private constructor is required
    private User() { }

    private User(string firstName, string lastName, string email, TokenProvider tokenProvider = TokenProvider.RetailPortalApp, Password? password = null)
    {
        this.FirstName = firstName;
        this.LastName = lastName;
        this.Email = email;
        this.TokenProvider = tokenProvider;
        this.Password = password;
        this.Addresses = new List<Address>();
        this.Roles = new List<Role>();
    }

    public static User Create(string firstName, string lastName, string email, Password password)
    {
        return new User(firstName, lastName, email, password: password);
    }

    public static User Create(string firstName, string lastName, string email, TokenProvider tokenProvider)
    {
        return new User(firstName, lastName, email, tokenProvider);
    }

    public void AddRole(Role role)
    {
        this.Roles.Add(role);
    }

    public void AddAddress(Address address)
    {
        this.Addresses.Add(address);
    }
    public void Update(string? firstName = null, string? lastName = null, string? email = null, Password? password = null)
    {
        if (!string.IsNullOrWhiteSpace(firstName))
        {
            this.FirstName = firstName;
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            this.LastName = lastName;
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            this.Email = email;
        }

        if (password != null)
        {
            this.Password = password;
        }
    }
}