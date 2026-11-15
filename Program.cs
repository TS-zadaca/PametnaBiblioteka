using System;

namespace PametnaBiblioteka
{
    class Program
    {
        static void Main(string[] args)
        {
            KnjigaServis knjigaServis = new KnjigaServis();
            PosudbaServis posudbaServis = new PosudbaServis(knjigaServis);
            KorisnikServis korisnikServis = new KorisnikServis(
                id => posudbaServis.DajAktivnePosudbe().Exists(p => p.KorisnikId == id)
            );

            bool radi = true;

            while (radi)
            {
                Console.WriteLine("\n=== PAMETNA BIBLIOTEKA ===");
                Console.WriteLine("1. Registruj korisnika");
                Console.WriteLine("2. Prikazi sve korisnike");
                Console.WriteLine("3. Dodaj knjigu");
                Console.WriteLine("4. Pretrazi knjige");
                Console.WriteLine("5. Posudi knjigu");
                Console.WriteLine("6. Vrati knjigu");
                Console.WriteLine("7. Historija posudbi korisnika");
                Console.WriteLine("0. Izlaz");
                Console.Write("Izbor: ");
                string izbor = Console.ReadLine() ?? "";

                try
                {
                    switch (izbor)
                    {
                        case "1":
                            Console.Write("ID korisnika: ");
                            int kId = int.Parse(Console.ReadLine() ?? "0");
                            Console.Write("Ime: ");
                            string ime = Console.ReadLine() ?? "";
                            Console.Write("Prezime: ");
                            string prezime = Console.ReadLine() ?? "";
                            Console.Write("Email: ");
                            string email = Console.ReadLine() ?? "";
                            Console.Write("Uloga (1 - Clan, 2 - Administrator): ");
                            int ulogaBroj = int.Parse(Console.ReadLine() ?? "1");
                            UlogaKorisnika uloga = ulogaBroj == 2 ? UlogaKorisnika.Administrator : UlogaKorisnika.Clan;
                            korisnikServis.RegistrujKorisnika(kId, ime, prezime, email, uloga);
                            Console.WriteLine("Korisnik uspjesno registrovan.");
                            break;

                        case "2":
                            foreach (var k in korisnikServis.PrikaziSveKorisnike())
                                Console.WriteLine($"[{k.Id}] {k.Ime} {k.Prezime} ({k.Email}) - {k.Uloga}");
                            break;

                        case "3":
                            Console.Write("ID knjige: ");
                            int knjigaId = int.Parse(Console.ReadLine() ?? "0");
                            Console.Write("Naslov: ");
                            string naslov = Console.ReadLine() ?? "";
                            Console.Write("Autor: ");
                            string autor = Console.ReadLine() ?? "";
                            Console.Write("Zanr: ");
                            string zanr = Console.ReadLine() ?? "";
                            Console.Write("Kolicina: ");
                            int kolicina = int.Parse(Console.ReadLine() ?? "0");
                            knjigaServis.DodajKnjigu(knjigaId, naslov, autor, zanr, kolicina);
                            break;

                        case "4":
                            Console.Write("Pojam za pretragu: ");
                            string pojam = Console.ReadLine() ?? "";
                            foreach (var k in knjigaServis.PretraziKnjige(pojam))
                                Console.WriteLine($"[{k.Id}] {k.Naslov} - {k.Autor} ({k.Zanr}) | Dostupno: {k.DostupnoPrimjeraka}/{k.UkupnoPrimjeraka}");
                            break;

                        case "5":
                            Console.Write("ID korisnika: ");
                            int posKorisnikId = int.Parse(Console.ReadLine() ?? "0");
                            if (korisnikServis.PronadjiKorisnika(posKorisnikId) == null)
                            {
                                Console.WriteLine("Greska: Korisnik ne postoji.");
                                break;
                            }
                            Console.Write("ID knjige: ");
                            int posKnjigaId = int.Parse(Console.ReadLine() ?? "0");
                            posudbaServis.PosudiKnjigu(posKorisnikId, posKnjigaId);
                            break;

                        case "6":
                            Console.Write("ID korisnika: ");
                            int vrKorisnikId = int.Parse(Console.ReadLine() ?? "0");
                            Console.Write("ID knjige: ");
                            int vrKnjigaId = int.Parse(Console.ReadLine() ?? "0");
                            posudbaServis.VratiKnjigu(vrKorisnikId, vrKnjigaId);
                            break;

                        case "7":
                            Console.Write("ID korisnika: ");
                            int histKorisnikId = int.Parse(Console.ReadLine() ?? "0");
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
                catch (Exception ex)
                {
                    Console.WriteLine("Greska: " + ex.Message);
                }
            }
        }
    }
}
