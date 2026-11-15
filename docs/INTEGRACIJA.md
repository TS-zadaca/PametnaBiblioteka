# Spajanje sa zajedničkom aplikacijom

Ovo je samostalan modul jer postojeći repozitorij i kod kolega nisu dostavljeni. Uskladite ciljnu .NET verziju prije spajanja. Nema ovisnosti o modulima knjiga i posudbi.

1. Dodajte `src/SmartLibrary.Users/SmartLibrary.Users.csproj` postojećem solutionu (**Add → Existing Project**).
2. U zajedničkom konzolnom projektu dodajte **Project Reference** na `SmartLibrary.Users`.
3. Kopirajte `UserMenu.cs` u zajednički konzolni projekt; nemojte kopirati drugi `Program.cs` preko postojećeg.
4. Kreirajte jednu instancu servisa i koristite je i u meniju i u modulu posudbe.

```csharp
using SmartLibrary.Users;

var path = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "SmartLibrary", "users.json");
var users = new UserService(new JsonUserRepository(path));
// U grani glavnog menija za korisnike:
new UserMenu(users).Run();
```

Modul posudbe provjerava člana preko `users.FindById(userId)`. Ako dobije null, posudba se odbija. Za veze koristite `Guid`, ne ime ili email.

Kad kolega implementira provjeru aktivnih posudbi, proslijedite je konstruktoru:

```csharp
// hasActiveLoans je stvarna funkcija modula posudbe: Func<Guid, bool>.
var users = new UserService(new JsonUserRepository(path), hasActiveLoans);
```

Naziv funkcije modula posudbe nije unaprijed izmišljen. Dok se provjera ne poveže, brisanje ne zna za posudbe. Za historiju posudbi dogovorite čuvanje snimka imena ili soft-delete politiku; ovaj modul implementira fizičko brisanje prema zahtjevu.

Ako vaš repozitorij već ima `.github` ili `Directory.Build.props`, ručno sjedinite sadržaj. Ne prepisujte postojeće postavke i rad kolega.
