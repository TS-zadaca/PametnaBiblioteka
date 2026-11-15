using System;
using System.Collections.Generic;
using System.Linq;

namespace PametnaBiblioteka
{
    // Jednu instancu servisa koristi zajednicki Program.cs tokom rada aplikacije.
    // Podaci se cuvaju u memoriji, kao u modulu inventara knjiga.
    public class KorisnikServis
    {
        private readonly List<Korisnik> korisnici = new List<Korisnik>();
        private readonly Func<int, bool>? imaAktivnePosudbe;

        // Modul posudbe moze proslijediti svoju provjeru aktivnih posudbi.
        // Ako provjera nije proslijedjena, ovaj servis nema informacije o posudbama.
        public KorisnikServis(Func<int, bool>? provjeraAktivnihPosudbi = null)
        {
            imaAktivnePosudbe = provjeraAktivnihPosudbi;
        }

        public Korisnik RegistrujKorisnika(int id, string ime, string prezime, string email,
            UlogaKorisnika uloga = UlogaKorisnika.Clan)
        {
            // Konstruktor provjerava sva polja prije promjene liste.
            Korisnik noviKorisnik = new Korisnik(id, ime, prezime, email, uloga);

            if (korisnici.Any(k => k.Id == id))
                throw new ArgumentException("Korisnik s tim ID-em vec postoji.", nameof(id));

            ProvjeriJedinstvenEmail(noviKorisnik.Email, id);
            korisnici.Add(noviKorisnik);
            return noviKorisnik;
        }

        public Korisnik AzurirajKorisnika(int id, string novoIme, string novoPrezime, string noviEmail)
        {
            int indeks = PronadjiIndeks(id);
            Korisnik postojeci = korisnici[indeks];
            Korisnik izmijenjeni = new Korisnik(id, novoIme, novoPrezime, noviEmail, postojeci.Uloga);

            ProvjeriJedinstvenEmail(izmijenjeni.Email, id);
            // Zamjena nastupa tek kada sve provjere prodju. ID i uloga ostaju isti.
            korisnici[indeks] = izmijenjeni;
            return izmijenjeni;
        }

        public void ObrisiKorisnika(int id)
        {
            int indeks = PronadjiIndeks(id);

            if (imaAktivnePosudbe != null && imaAktivnePosudbe(id))
                throw new InvalidOperationException("Korisnik ima aktivne posudbe i ne moze se obrisati.");

            korisnici.RemoveAt(indeks);
        }

        public Korisnik DodijeliUlogu(int id, UlogaKorisnika novaUloga)
        {
            int indeks = PronadjiIndeks(id);
            Korisnik postojeci = korisnici[indeks];
            Korisnik izmijenjeni = new Korisnik(postojeci.Id, postojeci.Ime,
                postojeci.Prezime, postojeci.Email, novaUloga);

            korisnici[indeks] = izmijenjeni;
            return izmijenjeni;
        }

        public Korisnik? PronadjiKorisnika(int id)
        {
            return korisnici.FirstOrDefault(k => k.Id == id);
        }

        public IReadOnlyList<Korisnik> PrikaziSveKorisnike()
        {
            // Kopija liste i nepromjenjivi objekti sprecavaju zaobilazenje provjera.
            return korisnici.OrderBy(k => k.Prezime).ThenBy(k => k.Ime).ToList().AsReadOnly();
        }

        private int PronadjiIndeks(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID korisnika mora biti pozitivan broj.", nameof(id));

            int indeks = korisnici.FindIndex(k => k.Id == id);
            if (indeks < 0)
                throw new KeyNotFoundException("Korisnik s tim ID-em nije pronadjen.");

            return indeks;
        }

        private void ProvjeriJedinstvenEmail(string email, int idKorisnika)
        {
            if (korisnici.Any(k => k.Id != idKorisnika &&
                string.Equals(k.Email, email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Korisnik s tom email adresom vec postoji.", nameof(email));
            }
        }
    }
}
