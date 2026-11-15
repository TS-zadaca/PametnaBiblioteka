# Plan stvarne inspekcije

Podjela za tim od tri člana, ako svi rade jedan modul:

| Autor | Modul | Pregledaju |
|---|---|---|
| Faris | Upravljanje korisnicima | Kolega 2 i Kolega 3 |
| Kolega 2 | Inventar knjiga | Faris i Kolega 3 |
| Kolega 3 | Posudbe | Faris i Kolega 2 |

Zamijenite nazive kolega stvarnim imenima. Svaki član pregleda druga dva modula. Obim pregleda uskladite prema veličini modula tako da zaduženja budu ravnomjerna.

## Postupak

1. Autor otvara PR i bilježi commit koji se pregledava.
2. Oba reviewera samostalno čitaju kod i provjeravaju scenarije.
3. Nalaz se upisuje kao komentar na liniji ili issue, s očekivanim i dobivenim ponašanjem.
4. Autor ispravlja, referencira nalaz i šalje novi commit.
5. Reviewer ponavlja provjeru i označava nalaz zatvorenim tek kada je riješen.
6. Tim popunjava izvještaj i dijagrame iz istog skupa nalaza te sprema screenshotove.

## Kontrolna lista

- [ ] Sve tražene operacije postoje i rade i nad praznom listom.
- [ ] Prazna, preduga i neispravna imena se odbijaju; dijakritika se prihvata.
- [ ] Neispravan i dupli email se odbijaju, uključujući različitu veličinu slova.
- [ ] Izmjena dopušta postojeći vlastiti email, ali ne tuđi.
- [ ] Neispravan unos ne mijenja postojeće podatke.
- [ ] ID i datum ostaju isti nakon izmjene.
- [ ] Moguće su samo dvije podržane uloge.
- [ ] Nepostojeći ID ne uzrokuje pogrešnu promjenu drugog korisnika.
- [ ] Brisanje u meniju zahtijeva potvrdu.
- [ ] Kod povezanog modula posudbe aktivna posudba blokira brisanje.
- [ ] Podaci se pravilno učitavaju nakon ponovnog pokretanja.
- [ ] Oštećen zapis se ne prepisuje praznom listom.
- [ ] Greška spremanja ne mijenja memorijsko stanje.
- [ ] Meni obrađuje pogrešan izbor i prekid ulaza.
- [ ] Model, servis, pohrana i meni imaju jasna zaduženja.
- [ ] Testovi provjeravaju ponašanje, a ne samo da metoda ne baca izuzetak.
- [ ] Nema stvarnih ličnih podataka ili izlaznih binarnih datoteka u repozitoriju.

Ovo je alternativna kontrolna lista koju zadatak dopušta. Nalaze ne izmišljati i ne ubacivati namjerne greške radi broja nalaza. Sugestije poboljšanja odvojiti od potvrđenih grešaka.
