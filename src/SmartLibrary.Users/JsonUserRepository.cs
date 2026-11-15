using System.Text.Json;

namespace SmartLibrary.Users;

/// <summary>Lokalna pohrana za jednu instancu aplikacije. Nije namijenjena paralelnim procesima.</summary>
public sealed class JsonUserRepository(string filePath) : IUserRepository
{
    private readonly string _path = Path.GetFullPath(filePath);
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public IReadOnlyList<User> Load()
    {
        if (!File.Exists(_path)) return Array.Empty<User>();
        // Ne pretvarati oštećene podatke u praznu listu: time bi se izgubili pri sljedećem spremanju.
        return JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_path), Options)
            ?? throw new InvalidDataException("Datoteka korisnika ne smije sadržavati null.");
    }

    public void Save(IReadOnlyList<User> users)
    {
        var directory = Path.GetDirectoryName(_path)!;
        Directory.CreateDirectory(directory);
        var temporary = Path.Combine(directory, $".{Guid.NewGuid():N}.tmp");
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(users, Options));
            File.Move(temporary, _path, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }
}
