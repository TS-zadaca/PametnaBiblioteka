using System.Net.Mail;
using System.Text;

namespace SmartLibrary.Users;

/// <summary>Poslovna pravila korisničkog modula. Sve promjene prolaze kroz ovaj servis.</summary>
public sealed class UserService
{
    private readonly IUserRepository _repository;
    private List<User> _users;
    // Modul posudbe može postaviti provjeru; bez nje ovaj modul ne zna za posudbe.
    private readonly Func<Guid, bool>? _hasActiveLoans;

    public UserService(IUserRepository repository, Func<Guid, bool>? hasActiveLoans = null)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _hasActiveLoans = hasActiveLoans;
        _users = repository.Load().ToList();
        ValidateStoredUsers();
    }

    public IReadOnlyList<User> GetAll() => _users.OrderBy(u => u.LastName)
        .ThenBy(u => u.FirstName).ToArray();

    public User? FindById(Guid id) => _users.Find(u => u.Id == id);

    public User Register(string firstName, string lastName, string email,
        UserRole role = UserRole.Member)
    {
        var first = ValidateName(firstName, "Ime");
        var last = ValidateName(lastName, "Prezime");
        var address = ValidateEmail(email);
        ValidateRole(role);
        EnsureUniqueEmail(address, null);
        var user = new User(Guid.NewGuid(), first, last, address, role, DateTimeOffset.UtcNow);
        Commit([.. _users, user]);
        return user;
    }

    public User Update(Guid id, string firstName, string lastName, string email)
    {
        var current = RequireUser(id);
        var first = ValidateName(firstName, "Ime");
        var last = ValidateName(lastName, "Prezime");
        var address = ValidateEmail(email);
        EnsureUniqueEmail(address, id);
        var updated = current with { FirstName = first, LastName = last, Email = address };
        Commit(_users.Select(u => u.Id == id ? updated : u).ToList());
        return updated;
    }

    public User AssignRole(Guid id, UserRole role)
    {
        var current = RequireUser(id);
        ValidateRole(role);
        var updated = current with { Role = role };
        Commit(_users.Select(u => u.Id == id ? updated : u).ToList());
        return updated;
    }

    public void Delete(Guid id)
    {
        RequireUser(id);
        if (_hasActiveLoans?.Invoke(id) == true)
            throw new UserValidationException("Korisnik ima aktivne posudbe i ne može se obrisati.");
        Commit(_users.Where(u => u.Id != id).ToList());
    }

    private User RequireUser(Guid id) => FindById(id)
        ?? throw new KeyNotFoundException("Korisnik s tim ID-em ne postoji.");

    private void Commit(List<User> next)
    {
        // Memorijsko stanje se mijenja tek nakon uspješnog zapisa.
        _repository.Save(next);
        _users = next;
    }

    private void EnsureUniqueEmail(string email, Guid? exceptId)
    {
        if (_users.Any(u => u.Id != exceptId &&
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
            throw new UserValidationException("Korisnik s tom email adresom već postoji.");
    }

    private static string ValidateName(string? value, string field)
    {
        var name = (value ?? "").Trim().Normalize(NormalizationForm.FormC);
        if (name.Length is < 2 or > 50 || !name.Any(char.IsLetter) ||
            name.Any(c => !char.IsLetter(c) && c != ' ' && c != '-' && c != '\''))
            throw new UserValidationException($"{field} mora imati 2–50 znakova: slova, razmak, apostrof ili crticu.");
        return name;
    }

    private static string ValidateEmail(string? value)
    {
        var email = (value ?? "").Trim();
        if (email.Length is < 3 or > 254 || email.Any(char.IsWhiteSpace) ||
            !MailAddress.TryCreate(email, out var parsed) || parsed.Address != email ||
            !parsed.Host.Contains('.'))
            throw new UserValidationException("Unesite ispravnu email adresu, npr. ime@example.com.");
        return email;
    }

    private static void ValidateRole(UserRole role)
    {
        if (!Enum.IsDefined(role))
            throw new UserValidationException("Dozvoljene uloge su Član (1) i Administrator (2).");
    }

    private void ValidateStoredUsers()
    {
        var ids = new HashSet<Guid>();
        var emails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var user in _users)
        {
            if (user is null || user.Id == Guid.Empty || !ids.Add(user.Id))
                throw new InvalidDataException("Neispravni ili dupli ID-evi u datoteci korisnika.");
            try
            {
                var first = ValidateName(user.FirstName, "Ime");
                var last = ValidateName(user.LastName, "Prezime");
                var email = ValidateEmail(user.Email);
                ValidateRole(user.Role);
                if (first != user.FirstName || last != user.LastName || email != user.Email ||
                    !emails.Add(email) || user.CreatedAtUtc == default)
                    throw new InvalidDataException("Neispravni ili dupli podaci u datoteci korisnika.");
            }
            catch (UserValidationException ex)
            {
                throw new InvalidDataException("Datoteka korisnika sadrži neispravne podatke.", ex);
            }
        }
    }
}
