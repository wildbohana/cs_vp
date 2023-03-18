using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Servis
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(BibliotekaServis));
            host.Open();

            Console.WriteLine("Servis je otvoren, pritisni bilo koji taster da ga ugasiš.");
            Console.ReadKey();

            host.Close();
            Console.WriteLine("Service je ugašen.");
        }
    }
}
