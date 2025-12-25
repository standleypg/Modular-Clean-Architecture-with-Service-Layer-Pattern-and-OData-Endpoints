using RetailPortal.Model.Db.Entities.Common.Base;

namespace RetailPortal.Model.Db.Entities.Common.ValueObjects;

public sealed class Password: ValueObject
{
    public byte[] PasswordHash { get; }
    public byte[] PasswordSalt { get; }

    private Password(byte[] passwordHash, byte[] passwordSalt)
    {
        this.PasswordHash = passwordHash;
        this.PasswordSalt = passwordSalt;
    }

    public static Password Create(byte[] passwordHash, byte[] passwordSalt)
    {
        return new Password(passwordHash, passwordSalt);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.PasswordHash;
        yield return this.PasswordSalt;
    }
}