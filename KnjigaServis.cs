using System;
using System.Collections.Generic;
using System.Linq;

namespace PametnaBiblioteka
{
    public class KnjigaServis
    {
        public List<Knjiga> knjige = new List<Knjiga>();

        public void DodajKnjigu(int id, string naslov, string autor, string zanr, int kolicina)
        {
            if (knjige.Any(x => x.Id == id))
            {
                Console.WriteLine("Greska: Knjiga sa tim ID-em vec postoji.");
                return;
            }

            if (string.IsNullOrWhiteSpace(naslov) || string.IsNullOrWhiteSpace(autor) || string.IsNullOrWhiteSpace(zanr))
            {
                Console.WriteLine("Greska: Naslov, autor i zanr ne smiju biti prazni.");
                return;
            }

            if (kolicina <= 0)
            {
                Console.WriteLine("Greska: Kolicina mora biti veca od 0.");
                return;
            }

            Knjiga novaKnjiga = new Knjiga(id, naslov, autor, zanr, kolicina);
            knjige.Add(novaKnjiga);
            Console.WriteLine("Knjiga uspjesno dodana.");
        }

        public void AzurirajKnjigu(int id, string noviNaslov, string noviAutor, string noviZanr, int novaKolicina)
        {
            Knjiga k = knjige.FirstOrDefault(x => x.Id == id);
            if (k == null)
            {
                Console.WriteLine("Knjiga nije pronadjena.");
                return;
            }

            int posudjeno = k.UkupnoPrimjeraka - k.DostupnoPrimjeraka;
            if (novaKolicina < posudjeno)
            {
                Console.WriteLine($"Greska: Trenutno je posudjeno {posudjeno} primjeraka, nova kolicina ne moze biti manja od toga.");
                return;
            }

            k.Naslov = noviNaslov;
            k.Autor = noviAutor;
            k.Zanr = noviZanr;
            k.UkupnoPrimjeraka = novaKolicina;
            k.DostupnoPrimjeraka = novaKolicina - posudjeno;
            Console.WriteLine("Knjiga uspjesno azurirana.");
        }

        public void ObrisiKnjigu(int id)
        {
            Knjiga k = knjige.FirstOrDefault(x => x.Id == id);
            if (k == null)
            {
                Console.WriteLine("Knjiga nije pronadjena.");
                return;
            }

            if (k.DostupnoPrimjeraka < k.UkupnoPrimjeraka)
            {
                Console.WriteLine("Greska: Nije moguce obrisati knjigu jer su neki primjerci trenutno posudjeni.");
                return;
            }

            knjige.Remove(k);
            Console.WriteLine("Knjiga uspjesno obrisana.");
        }

        public List<Knjiga> PretraziKnjige(string pojam)
        {
            if (string.IsNullOrWhiteSpace(pojam))
                return new List<Knjiga>();

            return knjige.Where(k =>
                k.Naslov.ToLower().Contains(pojam.ToLower()) ||
                k.Autor.ToLower().Contains(pojam.ToLower()) ||
                k.Zanr.ToLower().Contains(pojam.ToLower())
            ).ToList();
        }

        public bool DaLiJeDostupna(int id)
        {
            Knjiga k = knjige.FirstOrDefault(x => x.Id == id);
            return k != null && k.DostupnoPrimjeraka > 0;
        }
    }
}