using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// KLijent vrši upis Audit-a u bazu (poziva metodu) i vrši upis u CSV

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
                do
                {
                    Console.WriteLine("Unos datuma (dd-mm-yyyy): ");
                    string input = Console.ReadLine();

                    if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out datum))
                    {
                        Console.WriteLine("DATUM: " + datum);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Pogrešan unos datuma!");
                    }
                }
                while (true);

                // Poziv serverske metode
                Console.WriteLine("Prosledjen datum {0}", datum);
                Tuple<List<Load>, Audit> rezultat = kanal.UpitOdKlijenta(datum);

                // Ispis Audit fajla na konzolu
                Console.WriteLine(rezultat.Item2);

                // Čuvanje rezultata pretrage u CSV fajl
                // Ispis poruke o kreiranoj datoteci (poruka sadrži i podatke o putanji i imenu datoteke)

                // Load objekti - rezultat pretrage
                List<Load> loadovi = rezultat.Item1;
                if (loadovi.Count > 0)
                {
                    UpisUCSV(loadovi);
                }
            }
        }

        // Čisto ovako pitanje - ako več postoji CSV datoteka, ali je prepisujem ili šta radim?
        // Možda je najbolje da je samo prepišem

        #region CSV
        public bool UpisUCSV(List<Load> podaci)
        {
            // results_2023_01_22.csv
            bool uspesno = false;
            string direktorijumZaCsvDatoteka = ConfigurationManager.AppSettings["putanja"];
             string imeDatoteke = "results_" + podaci[0].Timestamp.ToString("d") + ".csv";
            
            //NapraviCsvDatoteku(direktorijumZaCsvDatoteke, imeDatoteke);

            // Promenljiva za memorijski tok
            MemoryStream stream = new MemoryStream();
            stream.Position = 0;
            uspesno = UpisCsvDatoteke(stream, podaci);

            return uspesno;
        }

        public bool UpisCsvDatoteke(MemoryStream csv, List<Load> zaUpis)
        {
            using (StreamWriter csv_stream = new StreamWriter(csv))
            {
                string prviRed = "TIME_STAMP, FORECAST_VALUE, MEASURED_VALUE";
                csv_stream.WriteLine(prviRed);

                foreach (Load l in zaUpis)
                {
                    string upis = "";
                    upis += l.Timestamp.ToString() + ",";
                    upis += l.ForecastValue.ToString() + ",";
                    upis += l.MeasuredValue.ToString() + ",";

                    csv_stream.WriteLine(upis);
                }
            }

            return true;
        }

        // TODO
        public void NapraviCsvDatoteku(string direktorijum, string imeDatoteke)
        {

        }
        #endregion
    }
}
