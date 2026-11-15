# Pametna biblioteka — Upravljanje korisnicima

Dio timske zadaće iz Testiranja softvera. Implementiran je samo modul **Upravljanje korisnicima**. Inventar i posudbe rade ostali članovi tima.

## Pokretanje u Visual Studiu

1. Raspakuj ZIP u novi folder.
2. U Visual Studio Installeru instaliraj podršku za C#/.NET (workload **.NET desktop development**) i .NET 8 SDK ako ih nemaš.
3. Otvori `SmartLibrary.sln`.
4. Desni klik na `SmartLibrary.Console` → **Set as Startup Project**.
5. Pokreni preko **Ctrl+F5**.
6. Za automatske testove otvori terminal u folderu rješenja i upotrijebi naredbe ispod.

## Pokretanje iz terminala

Potreban je .NET 8 SDK ili noviji SDK s podrškom za net8.0.

```bash
dotnet build SmartLibrary.sln --configuration Release
dotnet run --project src/SmartLibrary.Console
dotnet run --project tests/SmartLibrary.Users.Tests --configuration Release
```

Testovi su samostalna konzolna aplikacija bez vanjskih biblioteka. Pokreću se preko `dotnet run`, ne preko `dotnet test` ili Visual Studio Test Explorera. Framework za testove nije propisan priloženim zadatkom.

## Funkcionalnosti

| Zahtjev | Implementacija |
|---|---|
| Registracija | Ime, prezime, jedinstven email, uloga, automatski GUID i datum |
| Ažuriranje | Izmjena imena, prezimena i emaila uz očuvanje ID-a, datuma i uloge |
| Brisanje | Potvrda `DA` u meniju; provjera postojanja korisnika |
| Dodjela uloga | Član ili Administrator; odbijanje nepoznatih vrijednosti |
| Validacija | Ime/prezime 2–50 znakova, slova i odabrani razdjelnici; validacija emaila i duplikata |
| Pregled | Lista korisnika i pronalazak po ID-u |
| Pohrana | JSON; podaci ostaju nakon ponovnog pokretanja |

Uloge su podaci o korisniku. Prijava, lozinke i zabrana pristupa prema ulozi nisu traženi niti implementirani. Meni je namijenjen operateru biblioteke. Dozvoljeno je imati nula administratora jer ovo nije sistem autentifikacije.

Pohrana je u `LocalApplicationData/SmartLibrary/users.json`. Na Windowsu je to uobičajeno `%LOCALAPPDATA%\SmartLibrary\users.json`. Podaci nisu u repozitoriju. Oštećen zapis se prijavljuje, ne briše se automatski. Za demonstraciju koristi izmišljene podatke.

## Struktura i objašnjenje

- `src/SmartLibrary.Users/User.cs`: nepromjenjiv model korisnika i uloge.
- `UserService.cs`: sva poslovna pravila; prvo validira, zatim sprema, pa mijenja memorijsko stanje.
- `IUserRepository.cs`: ugovor pohrane; kasnije se može zamijeniti bazom bez promjene servisa.
- `JsonUserRepository.cs`: učitavanje i zapis preko privremene datoteke.
- `src/SmartLibrary.Console/UserMenu.cs`: korisnički meni, unos, poruke i potvrda brisanja.
- `tests/SmartLibrary.Users.Tests`: izolovani automatski testovi i testovi JSON pohrane.
- `docs/`: integracija, testiranje i upute za stvarnu inspekciju.
- `.github/`: automatska izgradnja i testovi, predložak PR-a i prijave nalaza.

Projekt koristi samo standardnu .NET biblioteku. Pohrana podržava jednu instancu aplikacije; paralelno uređivanje iz više procesa nije podržano. Provjera emaila je sintaksna, ne šalje verifikacijski email. Unicode imena se normalizuju u Form C; prihvataju se slova, obični razmak, ASCII apostrof i crtica.

## Šta ostaje za predaju

Implementacija i prateći materijali su pripremljeni. Stvarni proces pregleda treba provesti s kolegama na GitHubu: nalazi, ispravke, njihove potvrde i screenshotovi moraju odgovarati onome što se zaista dogodilo. Obrazac izvještaja s c2 i kontrolna lista s vježbi nisu dostavljeni; ovdje je ponuđen zamjenski predložak. Ako nastavnik traži svoj obrazac, prepiši stvarne nalaze u njega.

Pogledaj `docs/GITHUB.md`, zatim `docs/INSPEKCIJA.md` i `docs/IZVJESTAJ.md`. Dijagrami se prave tek iz stvarnih nalaza prema `docs/DIJAGRAMI.md`.

Status lokalne provjere je u `docs/PROVJERA.md`. Automatski GitHub posao provjerava build i testove nakon objave.
