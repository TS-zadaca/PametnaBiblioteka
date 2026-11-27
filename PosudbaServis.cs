using System;
using System.Collections.Generic;
using System.Linq;

namespace PametnaBiblioteka
{
    public class PosudbaServis
    {
        private readonly KnjigaServis knjigaServis;
        private readonly KorisnikServis korisnikServis;
        private readonly List<Posudba> posudbe;

        private int sljedeciId = 1;

        private readonly Func<DateTime> danas;
        private decimal cijenaPoDanu = 1m;
        private int brojDanaPosudbe = 14;
        public decimal CijenaPoDanu
        {
            get => cijenaPoDanu;
            set => cijenaPoDanu = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }
        public int BrojDanaPosudbe
        {
            get => brojDanaPosudbe;
            set => brojDanaPosudbe = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public PosudbaServis(KnjigaServis knjigaServis, KorisnikServis korisnikServis, Func<DateTime>? sat = null)
        {
            this.knjigaServis = knjigaServis ?? throw new ArgumentNullException(nameof(knjigaServis));
            this.korisnikServis = korisnikServis ?? throw new ArgumentNullException(nameof(korisnikServis));
            danas = sat ?? (() => DateTime.Today);
            posudbe = new List<Posudba>();
        }

        public void PosudiKnjigu(int korisnikId, int knjigaId)
        {
            if (korisnikId <= 0)
                throw new ArgumentException("Neispravan ID korisnika.");

            if (korisnikServis.PronadjiKorisnika(korisnikId) == null)
                throw new KeyNotFoundException("Korisnik nije pronadjen.");

            Knjiga? knjiga = knjigaServis.knjige
                .FirstOrDefault(k => k.Id == knjigaId);

            if (knjiga == null)
                throw new KeyNotFoundException("Knjiga nije pronadjena.");

            if (knjiga.DostupnoPrimjeraka <= 0)
                throw new InvalidOperationException("Knjiga nije dostupna.");

            bool vecPosudjena = posudbe.Any(p =>
                p.KorisnikId == korisnikId &&
                p.KnjigaId == knjigaId &&
                !p.Vracena);

            if (vecPosudjena)
                throw new InvalidOperationException(
                    "Korisnik vec ima posudjenu ovu knjigu.");

            DateTime danas = this.danas().Date;

            Posudba novaPosudba = new Posudba(
                sljedeciId++,
                korisnikId,
                knjigaId,
                danas,
                danas.AddDays(BrojDanaPosudbe)
            );

            posudbe.Add(novaPosudba);
            knjiga.DostupnoPrimjeraka--;

            Console.WriteLine("Knjiga je uspjesno posudjena.");
            Console.WriteLine("Rok vracanja: " +
                novaPosudba.RokVracanja.ToShortDateString());
        }

        public void VratiKnjigu(int korisnikId, int knjigaId)
        {
            Posudba? posudba = posudbe.FirstOrDefault(p =>
                p.KorisnikId == korisnikId &&
                p.KnjigaId == knjigaId &&
                !p.Vracena);

            if (posudba == null)
                throw new InvalidOperationException("Aktivna posudba nije pronadjena.");

            Knjiga? knjiga = knjigaServis.knjige
                .FirstOrDefault(k => k.Id == knjigaId);

            if (knjiga == null)
                throw new KeyNotFoundException("Knjiga nije pronadjena.");

            if (knjiga.DostupnoPrimjeraka >= knjiga.UkupnoPrimjeraka)
                throw new InvalidOperationException(
                    "Nekonzistentno stanje: dostupnih primjeraka ne moze biti vise od ukupnih.");

            DateTime danas = this.danas().Date;

            posudba.DatumVracanja = danas;
            posudba.Vracena = true;

            int kasnjenje = Math.Max(
                0, (danas - posudba.RokVracanja).Days
            );

            posudba.Zakasnina = kasnjenje * CijenaPoDanu;

            knjiga.DostupnoPrimjeraka++;

            Console.WriteLine("Knjiga je uspjesno vracena.");
            Console.WriteLine("Zakasnina: " +
                posudba.Zakasnina.ToString("0.00") + " KM");
        }

        public List<Posudba> DajHistorijuKorisnika(int korisnikId)
        {
            return posudbe
                .Where(p => p.KorisnikId == korisnikId)
                .OrderByDescending(p => p.DatumPosudbe)
                .ToList();
        }

        public List<Posudba> DajSvePosudbe()
        {
            return posudbe.ToList();
        }

        public List<Posudba> DajAktivnePosudbe()
        {
            return posudbe.Where(p => !p.Vracena).ToList();
        }

        public decimal ObracunajZakasninu(int posudbaId)
        {
            Posudba? posudba = posudbe.FirstOrDefault(
                p => p.Id == posudbaId
            );

            if (posudba == null)
                throw new KeyNotFoundException("Posudba nije pronadjena.");

            DateTime datum = posudba.DatumVracanja ?? danas().Date;

            int kasnjenje = Math.Max(
                0, (datum - posudba.RokVracanja).Days
            );

            return kasnjenje * CijenaPoDanu;
        }

        public void PrikaziHistorijuKorisnika(int korisnikId)
        {
            var historija = DajHistorijuKorisnika(korisnikId);

            if (historija.Count == 0)
            {
                Console.WriteLine("Korisnik nema historiju posudbi.");
                return;
            }

            foreach (var p in historija)
            {
                Console.WriteLine(
                    $"Posudba #{p.Id} | Knjiga: {p.KnjigaId} | " +
                    $"Datum: {p.DatumPosudbe.ToShortDateString()} | " +
                    $"Rok: {p.RokVracanja.ToShortDateString()} | " +
                    $"Vracena: {(p.Vracena ? "Da" : "Ne")} | " +
                    $"Zakasnina: {p.Zakasnina:0.00} KM"
                );
            }
        }
    }
}
