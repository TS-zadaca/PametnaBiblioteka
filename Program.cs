namespace PametnaBiblioteka;

public static class Program
{
    public static void Main() => Pokreni(Console.In, Console.Out);

    // TextReader/TextWriter omogucavaju testiranje istog menija bez interaktivnog unosa.
    public static void Pokreni(TextReader ulaz, TextWriter izlaz)
    {
        var knjige = new KnjigaServis();
        PosudbaServis? posudbe = null;
        var korisnici = new KorisnikServis(id => posudbe!.DajAktivnePosudbe().Any(p => p.KorisnikId == id));
        posudbe = new PosudbaServis(knjige, korisnici);
        var preporuke = new PreporukaServis(knjige, korisnici, posudbe);

        string Tekst(string pitanje)
        {
            izlaz.Write(pitanje + ": ");
            return ulaz.ReadLine() ?? throw new EndOfStreamException();
        }
        int Broj(string pitanje)
        {
            if (!int.TryParse(Tekst(pitanje), out int broj))
                throw new FormatException("Unesite cijeli broj.");
            return broj;
        }
        void IspisiKnjigu(Knjiga k) => izlaz.WriteLine(
            $"[{k.Id}] {k.Naslov} - {k.Autor} ({k.Zanr}) | Dostupno: {k.DostupnoPrimjeraka}/{k.UkupnoPrimjeraka}");

        while (true)
        {
            izlaz.WriteLine("\n=== PAMETNA BIBLIOTEKA ===");
            izlaz.WriteLine("1 Registruj korisnika | 2 Korisnici | 3 Dodaj knjigu | 4 Pretraga");
            izlaz.WriteLine("5 Posudi | 6 Vrati | 7 Historija | 8 Preporuke knjiga");
            izlaz.WriteLine("9 Izmijeni korisnika | 10 Obrisi korisnika | 11 Dodijeli ulogu");
            izlaz.WriteLine("12 Izmijeni knjigu | 13 Obrisi knjigu | 14 Dostupnost | 0 Izlaz");
            izlaz.Write("Izbor: ");
            string? izbor = ulaz.ReadLine();
            if (izbor is null or "0")
                return;
            try
            {
                switch (izbor)
                {
                    case "1":
                        korisnici.RegistrujKorisnika(Broj("ID korisnika"), Tekst("Ime"), Tekst("Prezime"),
                            Tekst("Email"), (UlogaKorisnika)Broj("Uloga 1 Clan 2 Administrator"));
                        izlaz.WriteLine("Korisnik registrovan.");
                        break;
                    case "2":
                        foreach (var k in korisnici.DajSveKorisnike())
                            izlaz.WriteLine($"[{k.Id}] {k.Ime} {k.Prezime} ({k.Email}) - {k.Uloga}");
                        break;
                    case "3":
                        knjige.DodajKnjigu(Broj("ID knjige"), Tekst("Naslov"), Tekst("Autor"), Tekst("Zanr"), Broj("Kolicina"));
                        break;
                    case "4":
                        foreach (var k in knjige.PretraziKnjige(Tekst("Pojam")))
                            IspisiKnjigu(k);
                        break;
                    case "5":
                        posudbe.PosudiKnjigu(Broj("ID korisnika"), Broj("ID knjige"));
                        break;
                    case "6":
                        posudbe.VratiKnjigu(Broj("ID korisnika"), Broj("ID knjige"));
                        break;
                    case "7":
                        posudbe.PrikaziHistorijuKorisnika(Broj("ID korisnika"));
                        break;
                    case "8":
                        var rezultat = preporuke.PreporuciKnjige(Broj("ID korisnika"), Broj("Broj preporuka"));
                        if (rezultat.Count == 0)
                            izlaz.WriteLine("Nema dostupnih novih knjiga za preporuku.");
                        foreach (var p in rezultat)
                        {
                            IspisiKnjigu(p.Knjiga);
                            izlaz.WriteLine($"Bodovi: {p.Bodovi}. {p.Razlog}");
                        }
                        break;
                    case "9":
                        korisnici.AzurirajKorisnika(Broj("ID korisnika"), Tekst("Novo ime"), Tekst("Novo prezime"), Tekst("Novi email"));
                        izlaz.WriteLine("Korisnik azuriran.");
                        break;
                    case "10":
                        korisnici.ObrisiKorisnika(Broj("ID korisnika"));
                        izlaz.WriteLine("Korisnik obrisan.");
                        break;
                    case "11":
                        korisnici.DodijeliUlogu(Broj("ID korisnika"), (UlogaKorisnika)Broj("Uloga 1 Clan 2 Administrator"));
                        izlaz.WriteLine("Uloga promijenjena.");
                        break;
                    case "12":
                        knjige.AzurirajKnjigu(Broj("ID knjige"), Tekst("Novi naslov"), Tekst("Novi autor"), Tekst("Novi zanr"), Broj("Nova kolicina"));
                        break;
                    case "13":
                        knjige.ObrisiKnjigu(Broj("ID knjige"));
                        break;
                    case "14":
                        izlaz.WriteLine(knjige.DaLiJeDostupna(Broj("ID knjige")) ? "Dostupna." : "Nije dostupna.");
                        break;
                    default:
                        izlaz.WriteLine("Nepoznata opcija.");
                        break;
                }
            }
            catch (EndOfStreamException) { return; }
            catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException or InvalidOperationException or FormatException)
            {
                izlaz.WriteLine("Greska: " + ex.Message);
            }
        }
    }
}
