using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace XMLDB
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(XmlBaza)))
            {
                host.Open();

                Console.WriteLine("XML baza podataka je pokrenuta.");
                Console.WriteLine("Adresa XML baze podataka: " + host.BaseAddresses.FirstOrDefault());

                XmlBaza.Direktorijumi();

                Console.WriteLine("Pritisnite bilo koji taster za zaustavljanje servera.");
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
