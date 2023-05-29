using Common;
using Common.Datoteke;
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

namespace Client
{
    public class Klijent
    {
        public void RadKlijenta(IServer kanal)
        {
            // Unos i validacija datuma
            string format = "dd-MM-yyyy";
            DateTime datum;

            while (true)
            {
                do
                {
                    Console.WriteLine("Unos datuma (dd-MM-yyyy): ");
                    string input = Console.ReadLine();

                    if (DateTime.TryParseExact(input, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out datum))
                    {
                        Console.WriteLine("DATUM: " + datum.ToString("dd.MM.yyyy."));
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Pogrešan unos datuma!\n");
                    }
                }
                while (true);

                // Poziv serverske metode za pretragu
                Tuple<List<Load>, Audit> rezultat = kanal.UpitOdKlijenta(datum);

                // Ispis Audit fajla na konzolu
                Console.WriteLine("\n" + rezultat.Item2);

                // Load objekti - rezultat pretrage
                List<Load> loadovi = rezultat.Item1;
                if (loadovi.Count > 0)
                {
                    UpisUCSV(loadovi);
                }

                // Preglednosti radi
                Console.WriteLine();
            }
        }

        #region CSV
        public void UpisUCSV(List<Load> podaci)
        {
            // results_2023_01_20.csv
            string direktorijumZaCsvDatoteka = ConfigurationManager.AppSettings["putanja"];
            string imeDatoteke = "results_" + podaci[0].Timestamp.ToString("yyyy_MM_dd") + ".csv";
            string putanjaDatoteke = direktorijumZaCsvDatoteka + "\\" + imeDatoteke;

            // Pretvaramo listu podataka u MemoryStream
            string tekst = PretvoriListuUString(podaci);
            MemoryStream stream = GenerisanjeStrimaOdStringa(tekst);

            using (IRadSaDatotekom datoteka = new Klijent().UpisUDatoteku(putanjaDatoteke, stream))
            {
                // Dispose resursa za datoteku
                datoteka.Dispose();
                Console.WriteLine("Podaci su upisani u datoteku: " + putanjaDatoteke);
            }

            // Ne moramo pozivati Dispose ovde, ali olakšavamo Garbage Collector-u
            stream.Dispose();
        }

        public string PretvoriListuUString(List<Load> zaUpis)
        {
            string strim = "";
            strim  += "TIME_STAMP,FORECAST_VALUE,MEASURED_VALUE\n";

            foreach (Load l in zaUpis)
            {
                strim += l.Timestamp.ToString("yyyy-MM-dd") + ",";
                strim += l.Timestamp.ToString("HH:mm") + ",";
                strim += l.ForecastValue.ToString().Replace(",", ".") + ",";
                strim += l.MeasuredValue.ToString().Replace(",", ".") + "\n";
            }

            return strim;
        }

        public static MemoryStream GenerisanjeStrimaOdStringa(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        public IRadSaDatotekom UpisUDatoteku(string putanjaDatoteke, MemoryStream ms)
        {
            // Ako CSV datoteka postoji, obriši je
            if (File.Exists(putanjaDatoteke))
            {
                Console.WriteLine("Datoteka sa rezultatima pretrage za taj datum već postoji. Podaci će biti prepisani.");
                File.Delete(putanjaDatoteke);
            }

            using (FileStream csv = new FileStream(putanjaDatoteke, FileMode.CreateNew, FileAccess.Write))
            {
                csv.Write(ms.ToArray(), 0, ms.ToArray().Length);
                csv.Dispose();
            }

            return new RadSaDatotekom(ms, Path.GetFileName(putanjaDatoteke));
        }
        #endregion
    }
}
