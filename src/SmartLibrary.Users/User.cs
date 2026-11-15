namespace SmartLibrary.Users;

public enum UserRole { Member = 1, Administrator = 2 }

// Immutable: drugi moduli ne mogu mijenjati korisnika mimo servisa.
public sealed record User(Guid Id, string FirstName, string LastName, string Email,
    UserRole Role, DateTimeOffset CreatedAtUtc);

public sealed class UserValidationException(string message) : Exception(message);
