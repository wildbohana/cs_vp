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
        bool UpisUXmlBazuLoad(List<Load> tabela);
        [OperationContract]
        List<Load> CitanjeIzXmlBazeLoad(DateTime datum);

        [OperationContract]
        bool UpisUXmlBazuAudit(Audit podatak);
        [OperationContract]
        Audit CitanjeIzXmlBazeAudit(DateTime datum);
    }
}
