using System.Collections.Generic;
using System.Data;

namespace EasySQL
{
    public interface ISQL
    {
        bool CreateConnection();
        bool Delete(string from, string where);
        void Dispose();
        List<object[]> Get(string rows, string from, string where);
        ConnectionState getConnectionState();
        bool Insert(string into, List<string> keys, List<object> values);
        bool Update(string into, List<string> keys, List<object> values, string where);
    }
}