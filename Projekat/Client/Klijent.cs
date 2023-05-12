using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Klijent
    {
        // Zoran
        public void RadKlijenta(IServer kanal)
        {
            // Unos i validacija datuma
            string format = "dd-mm-yyyy"; //naveden random format, ispravicu kad budem video kako je u bazi napisano
            DateTime datum;

            while (true)
            {
                Console.WriteLine("Unos datuma(dd-mm-yyyy): ");
                string input = Console.ReadLine();
                if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out datum))
                {
                    Console.WriteLine("DATE: " + datum);
                    break;
                }
                else
                {
                    Console.WriteLine("Pogresan unos datuma!");
                }
            }
            // Poziv serverske metode
            // List<Load> = kanal.UpitOdKlijenta(datum);
            Tuple<List<Load>, Audit> rezultat = kanal.UpitOdKlijenta(datum);
            Console.WriteLine("prosledjen datum {0}", datum);

            //DODATI ISPIS AUDIT FAJLA

            Console.WriteLine(rezultat.Item2);




            // Čuvanje rezultata pretrage u CSV fajl
            // Ispis poruke o kreiranoj datoteci (poruka sadrži i podatke o putanji i imenu datoteke)

            List<Load> loadovi = rezultat.Item1;
        }       

       

        // Nikola
        public void UpisUCSV(List<Load> podaci)
        {
            string putanja = GetSetting("putanja");

        }

        // Za učitavanje putanje iz App.config
        private static string GetSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
