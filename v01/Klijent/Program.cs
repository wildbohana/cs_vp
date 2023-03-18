using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Klijent
{
    public class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IBiblioteka> factory = new ChannelFactory<IBiblioteka>("BibliotekaServis");

            IBiblioteka proxy = factory.CreateChannel();

            Knjiga knjiga1 = new Knjiga("Scott", "Fitzgerald", "The Great Gatsby", Enumeracija.Zanrovi.Krimi, new DateTime(1925, 10, 1));
            Knjiga knjiga2 = new Knjiga("Vladimir", "Nabokov", "Lolita", Enumeracija.Zanrovi.Drama, new DateTime(1955, 2, 5));
            Knjiga knjiga3 = new Knjiga("Hermann", "Hesse", "Siddhartha", Enumeracija.Zanrovi.Ljubav, new DateTime(1925, 5, 6));
            Knjiga knjiga4 = new Knjiga("Paulo", " Coelho", "The Alchemist", Enumeracija.Zanrovi.Krimi, new DateTime(1988, 8, 10));
            Knjiga knjiga5 = new Knjiga("Joanne", "Rowling", "Harry Potter : Philosopher's Stone", Enumeracija.Zanrovi.Krimi, new DateTime(1997, 7, 26));
            Knjiga knjiga6 = new Knjiga("Joanne", "Rowling", "Harry Potter : Chamber of Secrets", Enumeracija.Zanrovi.Krimi, new DateTime(1998, 7, 26));
            Knjiga knjiga7 = new Knjiga("Joanne", "Rowling", "Harry Potter : Prisoner of Azkaban", Enumeracija.Zanrovi.Krimi, new DateTime(1999, 7, 26));
            Knjiga knjiga8 = new Knjiga("Joanne", "Rowling", "Harry Potter : Goblet of Fire", Enumeracija.Zanrovi.Krimi, new DateTime(2000, 7, 26));
            Knjiga knjiga9 = new Knjiga("Gabriel", "García Márquez", "One Hundred Years of Solitude", Enumeracija.Zanrovi.Ljubav, new DateTime(1967, 12, 1));
            Knjiga knjiga10 = new Knjiga("Gabriel", "García Márquez", "The Year of the Plague", Enumeracija.Zanrovi.Scifi, new DateTime(1979, 10, 20));

            // Duplikat (namerno)
            proxy.DodajKnjigu(knjiga1);
            proxy.DodajKnjigu(knjiga1);
            proxy.DodajKnjigu(knjiga2);
            proxy.DodajKnjigu(knjiga3);     
            proxy.DodajKnjigu(knjiga4);
            proxy.DodajKnjigu(knjiga5);
            proxy.DodajKnjigu(knjiga6);
            proxy.DodajKnjigu(knjiga7);
            proxy.DodajKnjigu(knjiga8);
            proxy.DodajKnjigu(knjiga9);
            proxy.DodajKnjigu(knjiga10);

            try
            {
                proxy.ObrisiKnjigu(4);

            }
            catch (FaultException<PrilagodjeniIzuzetak> e)
            {
                Console.WriteLine($"GREŠKA : {e.Detail.Poruka}");
            }

            try
            {
                proxy.ObrisiKnjigu(25);

            }
            catch (FaultException<PrilagodjeniIzuzetak> e)
            {
                Console.WriteLine($"GREŠKA : {e.Detail.Poruka}");
            }

            Console.WriteLine("SVE KNJIGE:");
            IspisSvihKnjiga(proxy.SpisakSvihKnjiga());
            Console.WriteLine("ŽANR KRIMI:");
            IspisSvihKnjiga(proxy.SpisakKnjigaZanr("Krimi"));
            Console.WriteLine("GODINA 1925");
            IspisSvihKnjiga(proxy.SpisakKnjigaGodina(1925));
            Console.WriteLine("KNJIGE OD AUTORKE JOANNE ROWLING");
            IspisSvihKnjiga(proxy.SpisakKnjigaAutor("Joanne", "Rowling"));
            Console.Read();
        }

        static void IspisSvihKnjiga(Dictionary<int, Knjiga> knjige)
        {
            Console.WriteLine("--------------------SPISAK KNJIGA-------------------------");
            foreach (Knjiga k in knjige.Values)
            {
                Console.WriteLine(k.ToString());
            }
            Console.WriteLine("----------------------------------------------------------\n");
        }
    }
}
