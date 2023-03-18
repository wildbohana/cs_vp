using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Servis
{
    public class BibliotekaServis : IBiblioteka
    {
        public bool DodajKnjigu(Knjiga knjiga)
        {
            if (BazaPodataka.Biblioteka.ContainsKey(knjiga.IdKnjige))
            {
                Console.WriteLine("Ta knjiga se već nalazi u biblioteci.");
                return false;
            }
            else
            {
                BazaPodataka.Biblioteka.Add(knjiga.IdKnjige, knjiga);
                return true;
            }
        }

        public void ObrisiKnjigu(int idKnjige)
        {
            if (BazaPodataka.Biblioteka.ContainsKey(idKnjige))
                BazaPodataka.Biblioteka.Remove(idKnjige);
            else
                throw new FaultException<PrilagodjeniIzuzetak>(new PrilagodjeniIzuzetak("Ta knjiga ne postoji!"));
        }

        public Dictionary<int, Knjiga> SpisakKnjigaAutor(string ime, string prz)
        {
            Dictionary<int, Knjiga> pretraga = new Dictionary<int, Knjiga>();

            foreach (Knjiga k in BazaPodataka.Biblioteka.Values)
            {
                if (ime == k.ImeAutora && prz == k.PrezimeAutora)
                {
                    pretraga.Add(k.IdKnjige, k);
                }
            }
            return pretraga;
        }

        public Dictionary<int, Knjiga> SpisakKnjigaGodina(int god)
        {
            Dictionary<int, Knjiga> pretraga = new Dictionary<int, Knjiga>();

            foreach (Knjiga k in BazaPodataka.Biblioteka.Values)
            {
                if (k.DatumIzdavanja.Year == god)
                {
                    pretraga.Add(k.IdKnjige, k);
                }
            }
            return pretraga;
        }

        public Dictionary<int, Knjiga> SpisakKnjigaZanr(string znr)
        {
            Dictionary<int, Knjiga> pretraga = new Dictionary<int, Knjiga>();

            foreach (Knjiga k in BazaPodataka.Biblioteka.Values)
            {
                if (k.Zanr.ToString().Equals(znr))
                {
                    pretraga.Add(k.IdKnjige, k);
                }
            }
            return pretraga;
        }

        public Dictionary<int, Knjiga> SpisakSvihKnjiga()
        {
            return new Dictionary<int, Knjiga>(BazaPodataka.Biblioteka);
        }
    }
}
