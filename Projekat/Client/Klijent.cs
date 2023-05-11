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
            kanal.UpitOdKlijenta(datum);
            Console.WriteLine("prosledjen datum {0}", datum);

            // Čuvanje rezultata pretrage u CSV fajl
            // Ispis poruke o kreiranoj datoteci (poruka sadrži i podatke o putanji i imenu datoteke)
        }
       

        //ovo vrv nije dobro
        public Tuple<List<Load>, Audit> UpitOdKlijenta(DateTime datum)
        {
            Load load = new Load();
            load.Timestamp = datum;

            List<Load> list = new List<Load>();
            list.Add(load);
            
            Tuple<List<Load>, Audit> rez = new Tuple<List<Load>, Audit>(list, null);
            

            return rez;
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
