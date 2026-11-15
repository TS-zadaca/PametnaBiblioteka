# Ručni i automatski testovi

Automatsko pokretanje: `dotnet run --project tests/SmartLibrary.Users.Tests --configuration Release`. Runner vraća exit code 1 ako bilo koji test padne i ispisuje PASS/FAIL za svaki slučaj. Uključuje pozitivne i negativne slučajeve, očuvanje stanja pri grešci, provjeru aktivne posudbe i ponovno učitavanje JSON podataka.

## Ručni scenariji

| Korak | Unos/radnja | Očekivani rezultat |
|---|---|---|
| 1 | Pokreni sa svježom pohranom, opcija 2 | Nema korisnika |
| 2 | Opcija 1: Faris, Šišić, faris@example.com, 1 | Registracija i novi ID |
| 3 | Opcija 1 s istim emailom velikim slovima | Duplikat odbijen |
| 4 | Registracija s praznim imenom ili lošim emailom | Jasna greška; nema dodatog korisnika |
| 5 | Opcija 3, kopiraj ID, promijeni ime | Izmjena; ID, datum i uloga ostaju |
| 6 | Opcija 3, ostavi polja prazna | Postojeće vrijednosti ostaju |
| 7 | Opcija 5, isti ID, uloga 2 | Administrator |
| 8 | Opcija 5, uloga 9 | Greška bez promjene |
| 9 | Pogrešan GUID ili postojeći GUID koji nije korisnički | Greška, bez rušenja |
| 10 | Opcija 4, potvrda NE | Korisnik ostaje |
| 11 | Izlaz 0 i ponovno pokretanje | Podaci ostaju |
| 12 | Opcija 4, potvrda DA | Korisnik izbrisan, i nakon restarta |
| 13 | Izbor 99 ili slovo | Poruka za izbor 0–5 |

Za test oštećenog JSON-a prvo sačuvaj rezervnu kopiju testnih podataka. U testnoj datoteci napravi neispravan JSON. Aplikacija treba prijaviti grešku i završiti bez prepisivanja. Vrati rezervnu kopiju nakon provjere.

Aktivna posudba je pokrivena automatskim testom pomoću funkcije koja vraća true. U spojenom projektu ponoviti s pravom aktivnom posudbom. Modul samostalno nema podatke o knjigama i posudbama.
