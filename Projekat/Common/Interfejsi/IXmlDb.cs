using Common.Datoteke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IXmlDb
    {
        [OperationContract]
        IRadSaDatotekom OtvoriDatoteku(string putanjaDatoteke);
        [OperationContract]
        List<Load> ProcitajIzBazePodataka(DateTime trazeniDatum);
        [OperationContract]
        int NajveciAudit();
        [OperationContract]
        void UpisUBazuPodataka(List<Audit> auditi);
    }
}
