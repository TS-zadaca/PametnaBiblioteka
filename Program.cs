using System;

namespace PametnaBiblioteka
{
    class Program
    {
        static void Main(string[] args)
        {
            KnjigaServis knjigaServis = new KnjigaServis();
            KorisnikServis korisnikServis = new KorisnikServis();
            PosudbaServis posudbaServis = new PosudbaServis(knjigaServis, korisnikServis);
            bool radi = true;

            while (radi)
            {
                Console.WriteLine("\n=== PAMETNA BIBLIOTEKA ===");
                Console.WriteLine("1. Dodaj knjigu");
                Console.WriteLine("2. Pretrazi knjige");
                Console.WriteLine("3. Obrisi knjigu");
                Console.WriteLine("4. Posudi knjigu");
                Console.WriteLine("5. Vrati knjigu");
                Console.WriteLine("6. Historija posudbi korisnika");
                Console.WriteLine("0. Izlaz");
                Console.Write("Izbor: ");
                string izbor = Console.ReadLine() ?? "";

                switch (izbor)
                {
                    case "1":
                        Console.Write("ID knjige: ");
                        if (!int.TryParse(Console.ReadLine(), out int id))
                        {
                            Console.WriteLine("Neispravan unos za ID.");
                            break;
                        }
                        Console.Write("Naslov: ");
                        string naslov = Console.ReadLine() ?? "";
                        Console.Write("Autor: ");
                        string autor = Console.ReadLine() ?? "";
                        Console.Write("Zanr: ");
                        string zanr = Console.ReadLine() ?? "";
                        Console.Write("Kolicina: ");
                        if (!int.TryParse(Console.ReadLine(), out int kolicina))
                        {
                            Console.WriteLine("Neispravan unos za kolicinu.");
                            break;
                        }
                        knjigaServis.DodajKnjigu(id, naslov, autor, zanr, kolicina);
                        break;

                    case "2":
                        Console.Write("Unesi pojam za pretragu: ");
                        string pojam = Console.ReadLine() ?? "";
                        var rezultati = knjigaServis.PretraziKnjige(pojam);
                        foreach (var k in rezultati)
                        {
                            Console.WriteLine($"[{k.Id}] {k.Naslov} - {k.Autor} ({k.Zanr}) | Dostupno: {k.DostupnoPrimjeraka}/{k.UkupnoPrimjeraka}");
                        }
                        break;

                    case "3":
                        Console.Write("ID knjige za brisanje: ");
                        if (!int.TryParse(Console.ReadLine(), out int delId))
                        {
                            Console.WriteLine("Neispravan unos za ID.");
                            break;
                        }
                        knjigaServis.ObrisiKnjigu(delId);
                        break;

                    case "4":
                        Console.Write("ID korisnika: ");
                        if (!int.TryParse(Console.ReadLine(), out int posKorisnikId))
                        {
                            Console.WriteLine("Neispravan unos za ID korisnika.");
                            break;
                        }
                        Console.Write("ID knjige: ");
                        if (!int.TryParse(Console.ReadLine(), out int posKnjigaId))
                        {
                            Console.WriteLine("Neispravan unos za ID knjige.");
                            break;
                        }
                        try
                        {
                            posudbaServis.PosudiKnjigu(posKorisnikId, posKnjigaId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Greska: " + ex.Message);
                        }
                        break;

                    case "5":
                        Console.Write("ID korisnika: ");
                        if (!int.TryParse(Console.ReadLine(), out int vrKorisnikId))
                        {
                            Console.WriteLine("Neispravan unos za ID korisnika.");
                            break;
                        }
                        Console.Write("ID knjige: ");
                        if (!int.TryParse(Console.ReadLine(), out int vrKnjigaId))
                        {
                            Console.WriteLine("Neispravan unos za ID knjige.");
                            break;
                        }
                        try
                        {
                            posudbaServis.VratiKnjigu(vrKorisnikId, vrKnjigaId);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Greska: " + ex.Message);
                        }
                        break;

                    case "6":
                        Console.Write("ID korisnika: ");
                        if (!int.TryParse(Console.ReadLine(), out int histKorisnikId))
                        {
                            Console.WriteLine("Neispravan unos za ID korisnika.");
                            break;
                        }
                        posudbaServis.PrikaziHistorijuKorisnika(histKorisnikId);
                        break;

                    case "0":
                        radi = false;
                        break;

                    default:
                        Console.WriteLine("Nepoznata opcija.");
                        break;
                }
            }
        }
    }
}
