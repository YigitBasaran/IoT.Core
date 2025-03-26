using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Core.CommonInfrastructure.Extensions.DbSettings
{
    public record MongoDbSettings(string ConnectionString, string DatabaseName, string CollectionName)

    {

    }
}
