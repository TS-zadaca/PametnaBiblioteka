# Postavljanje i inspekcija na GitHubu

## Ako prijatelj već ima timski repozitorij

Prihvati njegov poziv. Zatim kopiraj URL repozitorija preko dugmeta **Code**. U terminalu zamijeni tekst u uglastim zagradama stvarnim URL-om/nazivom; ne unosi zagrade.

```bash
git clone <URL_TIMSKOG_REPOZITORIJA>
cd <NAZIV_REPOZITORIJA>
git switch -c feature/upravljanje-korisnicima
```

Sada kopiraj svoj modul u klonirani folder prema `INTEGRACIJA.md`. Ako repozitorij još nema kod, možeš kopirati sadržaj ovog paketa. Ne kopiraj sadržaj nekog drugog `.git` foldera. Nemoj prepisivati postojeće datoteke kolega.

```bash
dotnet build SmartLibrary.sln --configuration Release
dotnet run --project tests/SmartLibrary.Users.Tests --configuration Release
git status
git add .
git commit -m "Implement user management module with validation and checks"
git push -u origin feature/upravljanje-korisnicima
```

Naredbe build/test pretpostavljaju da je zadržana struktura ovog rješenja. Ako spajate projekte u drugo rješenje, prilagodite putanje i workflow.

Na GitHubu otvori Pull Request iz ove grane prema glavnoj grani projekta. Dodaj oba kolege kao reviewere gdje postavke repozitorija to dozvoljavaju; mogu i ručno otvoriti PR i pregledati kod. Nemoj odmah spajati PR: prvo trebaju stvarni pregled, nalazi i ispravke.

Predloženi naslov: `Upravljanje korisnicima: registracija, izmjene, brisanje i uloge`.
Opis: `Implementiran korisnički modul pametne biblioteke s validacijom i JSON pohranom. Molim pregled oba člana prema kontrolnoj listi. Build i samostalne testove provjerava priloženi workflow.`

## Ako pravite novi repozitorij

Zadatak traži organizaciju i projekat. Tim neka prvo napravi ili potvrdi zajedničku GitHub organizaciju te u njoj repozitorij za biblioteku. Dodajte sva tri člana. Za prazan repozitorij pokreni iz raspakovanog foldera:

```bash
git init -b main
git add .
git commit -m "Add user management module and review materials"
git remote add origin <URL_PRAZNOG_REPOZITORIJA>
git push -u origin main
```

Ako želite inspekciju prvog koda kao PR, prvo na GitHubu napravite repozitorij s README datotekom, klonirajte ga i radite u feature grani prema prvom postupku. Ne gurajte preko postojećeg repozitorija i ne koristite force push.

## Dokazi procesa

Sačuvajte screenshot PR-a s reviewerima, komentara na konkretnim linijama, odgovora/ispravki i ponovnog pregleda. Sačuvajte i linkove nalaza te hash commita koji je pregledan. Nije dovoljno samo poslati kod: pregled i dokazi nose glavni dio bodova.
