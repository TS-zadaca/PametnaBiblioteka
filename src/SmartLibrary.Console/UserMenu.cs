using System.Text.Json;
using SmartLibrary.Users;

// Pri spajanju tima premjestiti u zajednički konzolni projekat i pozvati Run().
public sealed class UserMenu(UserService service)
{
    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== PAMETNA BIBLIOTEKA: UPRAVLJANJE KORISNICIMA ===");
            Console.WriteLine("1. Registracija\n2. Pregled korisnika\n3. Izmjena korisnika\n4. Brisanje korisnika\n5. Dodjela uloge\n0. Povratak / izlaz");
            Console.Write("Izbor: ");
            var choice = Console.ReadLine();
            if (choice is null or "0") return;
            try
            {
                switch (choice)
                {
                    case "1":
                        var added = service.Register(Read("Ime"), Read("Prezime"), Read("Email"), ReadRole());
                        Console.WriteLine($"Korisnik registrovan. ID: {added.Id}");
                        break;
                    case "2":
                        var users = service.GetAll();
                        if (users.Count == 0) Console.WriteLine("Nema registrovanih korisnika.");
                        foreach (var u in users)
                            Console.WriteLine($"{u.Id} | {u.FirstName} {u.LastName} | {u.Email} | {RoleLabel(u.Role)}");
                        break;
                    case "3":
                        var id = ReadId();
                        var current = service.FindById(id) ?? throw new KeyNotFoundException("Korisnik ne postoji.");
                        Console.WriteLine("Pritisnite Enter da zadržite postojeću vrijednost.");
                        service.Update(id, ReadOrKeep("Ime", current.FirstName),
                            ReadOrKeep("Prezime", current.LastName), ReadOrKeep("Email", current.Email));
                        Console.WriteLine("Podaci ažurirani.");
                        break;
                    case "4":
                        var deleteId = ReadId();
                        var user = service.FindById(deleteId) ?? throw new KeyNotFoundException("Korisnik ne postoji.");
                        if (Read($"Obrisati {user.FirstName} {user.LastName}? Unesite DA") == "DA")
                        {
                            service.Delete(deleteId);
                            Console.WriteLine("Korisnik obrisan.");
                        }
                        else Console.WriteLine("Brisanje otkazano.");
                        break;
                    case "5":
                        service.AssignRole(ReadId(), ReadRole());
                        Console.WriteLine("Uloga dodijeljena.");
                        break;
                    default: Console.WriteLine("Odaberite opciju od 0 do 5."); break;
                }
            }
            catch (EndOfStreamException) { return; }
            catch (Exception ex) when (ex is UserValidationException or KeyNotFoundException or
                FormatException or IOException or UnauthorizedAccessException or JsonException)
            {
                Console.WriteLine($"Greška: {ex.Message}");
            }
        }
    }

    private static string Read(string label)
    {
        Console.Write($"{label}: ");
        return Console.ReadLine() ?? throw new EndOfStreamException();
    }
    private static string ReadOrKeep(string label, string current)
    {
        var value = Read($"{label} [{current}]");
        return string.IsNullOrWhiteSpace(value) ? current : value;
    }
    private static Guid ReadId() => Guid.TryParse(Read("ID korisnika"), out var id) && id != Guid.Empty
        ? id : throw new FormatException("ID mora biti ispravan GUID. Kopirajte ga iz pregleda korisnika.");
    private static UserRole ReadRole() => Read("Uloga: 1 = Član, 2 = Administrator") switch
    {
        "1" => UserRole.Member,
        "2" => UserRole.Administrator,
        _ => throw new UserValidationException("Unesite 1 ili 2 za ulogu.")
    };
    private static string RoleLabel(UserRole role) => role == UserRole.Administrator ? "Administrator" : "Član";
}
