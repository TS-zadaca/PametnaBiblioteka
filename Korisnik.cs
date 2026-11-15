using System;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace PametnaBiblioteka
{
    public enum UlogaKorisnika
    {
        Clan = 1,
        Administrator = 2
    }

    // Podaci se ne mogu proizvoljno mijenjati nakon kreiranja.
    // Za izmjenu registrovanog korisnika koristi se KorisnikServis.
    public class Korisnik
    {
        public int Id { get; }
        public string Ime { get; }
        public string Prezime { get; }
        public string Email { get; }
        public UlogaKorisnika Uloga { get; }

        public Korisnik(int id, string ime, string prezime, string email,
            UlogaKorisnika uloga = UlogaKorisnika.Clan)
        {
            if (id <= 0)
                throw new ArgumentException("ID korisnika mora biti pozitivan broj.", nameof(id));

            if (!Enum.IsDefined(typeof(UlogaKorisnika), uloga))
                throw new ArgumentException("Uloga mora biti Clan ili Administrator.", nameof(uloga));

            Id = id;
            Ime = ValidirajIme(ime, "Ime");
            Prezime = ValidirajIme(prezime, "Prezime");
            Email = ValidirajEmail(email);
            Uloga = uloga;
        }

        private static string ValidirajIme(string vrijednost, string nazivPolja)
        {
            string tekst = (vrijednost ?? string.Empty).Trim().Normalize(NormalizationForm.FormC);

            if (tekst.Length < 2 || tekst.Length > 50 || !tekst.Any(char.IsLetter) ||
                tekst.Any(c => !char.IsLetter(c) && c != ' ' && c != '-' && c != '\''))
            {
                throw new ArgumentException(
                    nazivPolja + " mora imati 2 do 50 znakova: slova, razmak, crticu ili apostrof.");
            }

            return tekst;
        }

        private static string ValidirajEmail(string vrijednost)
        {
            string email = (vrijednost ?? string.Empty).Trim();

            if (email.Length < 3 || email.Length > 254 || email.Any(char.IsWhiteSpace))
                throw new ArgumentException("Email adresa nije ispravna.", nameof(vrijednost));

            if (!MailAddress.TryCreate(email, out var adresa) ||
                adresa.Address != email || !adresa.Host.Contains('.'))
            {
                throw new ArgumentException("Email adresa nije ispravna.", nameof(vrijednost));
            }

            return email;
        }
    }
}
