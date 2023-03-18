using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servis
{
    public class BazaPodataka
    {
        // Polje
        readonly static Dictionary<int, Knjiga> biblioteka;

        // Geter
        public static Dictionary<int, Knjiga> Biblioteka 
        { 
            get { return biblioteka;} 
        }

        // Konstruktor
        // ne može public na static (iako i dalje ne razumem znam zašto static al aj)
        static BazaPodataka()
        {
            biblioteka = new Dictionary<int, Knjiga>();
        }
    }
}
