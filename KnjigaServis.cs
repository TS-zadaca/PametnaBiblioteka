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

            k.Naslov = noviNaslov;
            k.Autor = noviAutor;
            k.Zanr = noviZanr;
            k.UkupnoPrimjeraka = novaKolicina;
            Console.WriteLine("Knjiga azurirana.");
        }

        public void ObrisiKnjigu(int id)
        {
            Knjiga k = knjige.FirstOrDefault(x => x.Id == id);
            if (k != null)
            {
                knjige.Remove(k);
                Console.WriteLine("Knjiga obrisana.");
            }
        }

        public List<Knjiga> PretraziKnjige(string pojam)
        {
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