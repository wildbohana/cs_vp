using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(Server)))
            {
                host.Open();

                Console.WriteLine("Server je pokrenut.");
                Console.WriteLine("Adresa servera: " + host.BaseAddresses.FirstOrDefault());

                // Spajanje sa XML bazom podataka
                Server.kanal = KonekcijaBaza();

                Console.WriteLine("Pritisnite bilo koji taster za zaustavljanje servera.");
                Console.ReadKey();
                host.Close();
            }
        }

        static IXmlDb KonekcijaBaza()
        {
            try
            {
                string adresa = "net.tcp://localhost:8002/XmlBaza";
                ChannelFactory<IXmlDb> cf = new ChannelFactory<IXmlDb>(new NetTcpBinding(), new EndpointAddress(adresa));
                IXmlDb kanal = cf.CreateChannel();

                Console.WriteLine("Uspešno spajanje Severa na XML bazu podataka.");
                return kanal;
            }
            catch
            {
                throw new AddressAccessDeniedException("Neuspešna konekcija sa bazom.");
            }
        }
    }
}
