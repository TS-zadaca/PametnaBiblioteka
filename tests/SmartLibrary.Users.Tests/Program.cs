using System.Text.Json;
using SmartLibrary.Users;

// Samostalni test-runner bez NuGet zavisnosti. Svaki test koristi novo stanje.
var cases = new List<(string Name, Action Body)>();
void Test(string name, Action body) => cases.Add((name, body));
UserService Fresh() => new(new MemoryRepository());
User Add(UserService s, string email = "faris@example.com") => s.Register("Faris", "Šišić", email);
void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
        throw new Exception($"Očekivano: {expected}; dobiveno: {actual}");
}
void Throws<T>(Action action) where T : Exception
{
    try { action(); }
    catch (T) { return; }
    throw new Exception($"Očekivan izuzetak {typeof(T).Name}.");
}
Test("Registracija i zadana uloga", () => {
    var s = Fresh(); var u = Add(s);
    Equal(1, s.GetAll().Count); Equal(UserRole.Member, u.Role);
    Equal(false, u.Id == Guid.Empty); Equal(true, u.CreatedAtUtc != default);
});
Test("Administrator pri registraciji", () => {
    Equal(UserRole.Administrator, Fresh().Register("Ana", "Anić", "ana@example.com", UserRole.Administrator).Role);
});
Test("Trim i dijakritika", () => {
    var u = Fresh().Register(" Faris ", " Šišić ", " faris@example.com ");
    Equal("Faris", u.FirstName); Equal("Šišić", u.LastName); Equal("faris@example.com", u.Email);
});
foreach (var name in new[] { "", " ", "A", "123", "Ana2", "<script>", new string('a', 51) })
{
    var input = name;
    Test($"Neispravno ime [{input}]", () => {
        var s = Fresh(); Throws<UserValidationException>(() => s.Register(input, "Šišić", "a@example.com"));
        Equal(0, s.GetAll().Count);
    });
}
Test("Validacija prezimena", () => Throws<UserValidationException>(() => Fresh().Register("Ana", "", "a@example.com")));
Test("Null ime odbijeno", () => Throws<UserValidationException>(() => Fresh().Register(null!, "Anić", "a@example.com")));
Test("Granice imena", () => {
    Equal(2, Fresh().Register("Al", "Li", "a@example.com").FirstName.Length);
    Equal(50, Fresh().Register(new string('a', 50), "Li", "a@example.com").FirstName.Length);
});
foreach (var email in new[] { "", "abc", "a@", "a@localhost", "a b@example.com", "Ana <a@example.com>", new string('a', 250) + "@example.com" })
{
    var input = email;
    Test($"Neispravan email [{input}]", () => Throws<UserValidationException>(() => Add(Fresh(), input)));
}
Test("Dupli email ne zavisi od veličine slova", () => {
    var s = Fresh(); Add(s); Throws<UserValidationException>(() => Add(s, "FARIS@EXAMPLE.COM")); Equal(1, s.GetAll().Count);
});
Test("Izmjena čuva ID, datum i ulogu", () => {
    var s = Fresh(); var u = Add(s); s.AssignRole(u.Id, UserRole.Administrator);
    var v = s.Update(u.Id, "Amir", "Hadžić", "amir@example.com");
    Equal(u.Id, v.Id); Equal(u.CreatedAtUtc, v.CreatedAtUtc); Equal(UserRole.Administrator, v.Role);
    Equal("Amir", s.FindById(u.Id)!.FirstName);
});
Test("Vlastiti email dozvoljen pri izmjeni", () => {
    var s = Fresh(); var u = Add(s); s.Update(u.Id, u.FirstName, u.LastName, u.Email); Equal(1, s.GetAll().Count);
});
Test("Tuđi email odbijen pri izmjeni bez promjene stanja", () => {
    var s = Fresh(); var u = Add(s); Add(s, "drugi@example.com");
    Throws<UserValidationException>(() => s.Update(u.Id, "Amir", "Hadžić", "drugi@example.com"));
    Equal(u, s.FindById(u.Id));
});
Test("Neispravna izmjena ne mijenja stanje", () => {
    var s = Fresh(); var u = Add(s);
    Throws<UserValidationException>(() => s.Update(u.Id, "", u.LastName, u.Email)); Equal(u, s.FindById(u.Id));
});
Test("Dodjela i promjena uloge", () => {
    var s = Fresh(); var u = Add(s); s.AssignRole(u.Id, UserRole.Administrator);
    Equal(UserRole.Administrator, s.FindById(u.Id)!.Role);
    s.AssignRole(u.Id, UserRole.Member); Equal(UserRole.Member, s.FindById(u.Id)!.Role);
});
Test("Nepostojeća enum uloga odbijena", () => {
    var s = Fresh(); var u = Add(s);
    Throws<UserValidationException>(() => s.AssignRole(u.Id, (UserRole)99)); Equal(u, s.FindById(u.Id));
    Throws<UserValidationException>(() => s.Register("Ana", "Anić", "ana@example.com", (UserRole)0));
});
Test("Brisanje", () => {
    var s = Fresh(); var u = Add(s); s.Delete(u.Id); Equal(0, s.GetAll().Count); Equal<User?>(null, s.FindById(u.Id));
});
Test("Nepostojeći ID za sve promjene", () => {
    var s = Fresh(); var id = Guid.NewGuid();
    Throws<KeyNotFoundException>(() => s.Delete(id));
    Throws<KeyNotFoundException>(() => s.AssignRole(id, UserRole.Member));
    Throws<KeyNotFoundException>(() => s.Update(id, "Ana", "Anić", "a@example.com"));
});
Test("Aktivna posudba blokira brisanje", () => {
    var s = new UserService(new MemoryRepository(), _ => true); var u = Add(s);
    Throws<UserValidationException>(() => s.Delete(u.Id)); Equal(1, s.GetAll().Count);
});
Test("Bez aktivnih posudbi brisanje dozvoljeno", () => {
    var s = new UserService(new MemoryRepository(), _ => false); var u = Add(s); s.Delete(u.Id); Equal(0, s.GetAll().Count);
});
Test("Vraćena lista ne mijenja servis", () => {
    var s = Fresh(); var u = Add(s); var snapshot = s.GetAll();
    ((User[])snapshot)[0] = u with { FirstName = "Izmjena" }; Equal(u, s.FindById(u.Id));
});
Test("Neuspjelo spremanje ne mijenja memoriju", () => {
    var repo = new MemoryRepository(); var s = new UserService(repo); var u = Add(s); repo.Fail = true;
    Throws<IOException>(() => Add(s, "drugi@example.com"));
    Throws<IOException>(() => s.Update(u.Id, "Ana", "Anić", u.Email));
    Throws<IOException>(() => s.AssignRole(u.Id, UserRole.Administrator));
    Throws<IOException>(() => s.Delete(u.Id)); Equal(1, s.GetAll().Count); Equal(u, s.FindById(u.Id));
});
void WithFile(Action<string> action)
{
    var dir = Path.Combine(Path.GetTempPath(), "SmartLibraryTests", Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(dir);
    try { action(Path.Combine(dir, "users.json")); }
    finally { Directory.Delete(dir, recursive: true); }
}
Test("JSON opstanak registracije, izmjene, uloge i brisanja", () => WithFile(path => {
    var s = new UserService(new JsonUserRepository(path)); Equal(0, s.GetAll().Count); var u = Add(s);
    s = new UserService(new JsonUserRepository(path)); Equal(u, s.FindById(u.Id));
    s.Update(u.Id, "Amir", "Hadžić", "amir@example.com"); s.AssignRole(u.Id, UserRole.Administrator);
    s = new UserService(new JsonUserRepository(path)); Equal("Amir", s.FindById(u.Id)!.FirstName);
    Equal(UserRole.Administrator, s.FindById(u.Id)!.Role); s.Delete(u.Id);
    Equal(0, new UserService(new JsonUserRepository(path)).GetAll().Count);
}));
Test("Oštećen JSON se ne prepisuje", () => WithFile(path => {
    File.WriteAllText(path, "{ broken");
    Throws<JsonException>(() => new UserService(new JsonUserRepository(path))); Equal("{ broken", File.ReadAllText(path));
}));
Test("JSON null se odbija", () => WithFile(path => {
    File.WriteAllText(path, "null"); Throws<InvalidDataException>(() => new UserService(new JsonUserRepository(path)));
}));
Test("Semantički neispravan JSON se odbija", () => WithFile(path => {
    var u = new User(Guid.NewGuid(), "Ana", "Anić", "a@example.com", (UserRole)99, DateTimeOffset.UtcNow);
    File.WriteAllText(path, JsonSerializer.Serialize(new[] { u }));
    Throws<InvalidDataException>(() => new UserService(new JsonUserRepository(path)));
}));
Test("Dupli ID i email u pohrani se odbijaju", () => WithFile(path => {
    var u = new User(Guid.NewGuid(), "Ana", "Anić", "a@example.com", UserRole.Member, DateTimeOffset.UtcNow);
    File.WriteAllText(path, JsonSerializer.Serialize(new[] { u, u }));
    Throws<InvalidDataException>(() => new UserService(new JsonUserRepository(path)));
    File.WriteAllText(path, JsonSerializer.Serialize(new[] { u, u with { Id = Guid.NewGuid(), Email = "A@example.com" } }));
    Throws<InvalidDataException>(() => new UserService(new JsonUserRepository(path)));
}));
int failed = 0;
foreach (var (name, body) in cases)
{
    try { body(); Console.WriteLine($"PASS: {name}"); }
    catch (Exception ex) { failed++; Console.Error.WriteLine($"FAIL: {name}: {ex.Message}"); }
}
Console.WriteLine($"Ukupno: {cases.Count}; uspješno: {cases.Count - failed}; neuspješno: {failed}");
return failed == 0 ? 0 : 1;

sealed class MemoryRepository : IUserRepository
{
    private List<User> _users = [];
    public bool Fail { get; set; }
    public IReadOnlyList<User> Load() => _users.ToArray();
    public void Save(IReadOnlyList<User> users)
    {
        if (Fail) throw new IOException("Simulirana greška zapisa.");
        _users = users.ToList();
    }
}
