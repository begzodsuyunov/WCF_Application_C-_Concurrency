using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace _00010358_Service
{
    [ServiceContract]
    public interface IAirportService
    {
        [OperationContract]
        bool InsertFlight(Flight flight);

    }
}
