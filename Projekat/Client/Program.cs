using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Otvaranje kanala
            ChannelFactory<IServer> cf = new ChannelFactory<IServer>("Konekcija");
            IServer kanal;

            try
            {
                kanal = cf.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n");
                Console.WriteLine("Klijent se gasi. Pokušajte ponovo da ga pokrenete malo kasnije.");
                Console.ReadKey();
                return;
            }

            // Početak rada klijenta
            Klijent k = new Klijent();
            k.RadKlijenta(kanal);

            // Gašenje klijenta
            Console.WriteLine("Klijent je završio sa radom. Pritisni bilo koji taster za izlaz.");
            Console.ReadKey();
        }
    }
}
