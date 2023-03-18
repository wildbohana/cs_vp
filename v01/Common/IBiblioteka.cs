using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IBiblioteka
    {
        [OperationContract]
        bool DodajKnjigu(Knjiga knjiga);

        [OperationContract]
        [FaultContract(typeof(PrilagodjeniIzuzetak))]
        void ObrisiKnjigu(int idKnjige);

        [OperationContract]
        Dictionary<int, Knjiga> SpisakSvihKnjiga();

        [OperationContract]
        Dictionary<int, Knjiga> SpisakKnjigaAutor(string ime, string prz);

        [OperationContract]
        Dictionary<int, Knjiga> SpisakKnjigaZanr(string znr);

        [OperationContract]
        Dictionary<int, Knjiga> SpisakKnjigaGodina(int god);
    }
}
