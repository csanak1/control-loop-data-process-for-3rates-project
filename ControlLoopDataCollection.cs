using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.Server;

using ControlLoopProcessFor3Rates.Models;

namespace ControlLoopProcessFor3Rates
{
    class ControlLoopDataCollection : List<ControlLoopDataModel>, IEnumerable<SqlDataRecord>
    {
        public ControlLoopDataCollection(IEnumerable<ControlLoopDataModel> collection) : base(collection) { }

        IEnumerator<SqlDataRecord> IEnumerable<SqlDataRecord>.GetEnumerator()
        {
            var sqlRow = new SqlDataRecord(
                new SqlMetaData("TagName", SqlDbType.VarChar),
                new SqlMetaData("NumOfGoodData", SqlDbType.Int),
                new SqlMetaData("NumOfGoodQltyPercentage", SqlDbType.Decimal),
                new SqlMetaData("NumOfGoodMANData", SqlDbType.Int),
                new SqlMetaData("NumOfGoodQltyMANPercentage", SqlDbType.Decimal),
                new SqlMetaData("NumOfGoodAUTData", SqlDbType.Int),
                new SqlMetaData("NumOfGoodQltyAUTPercentage", SqlDbType.Int),
                new SqlMetaData("DateID", SqlDbType.VarChar)
                );

            foreach (var d in this)
            {
                sqlRow.SetSqlString(0, d.HistTagName);
                sqlRow.SetSqlInt32(1, d.NumOfGoodData);
                sqlRow.SetSqlDecimal(2, d.NumOfGoodQltyPercentage);
                sqlRow.SetSqlInt32(3, d.NumOfGoodMANData);
                sqlRow.SetSqlDecimal(4, d.NumOfGoodQltyMANPercentage);
                sqlRow.SetSqlInt32(5, d.NumOfGoodAUTData);
                sqlRow.SetSqlDecimal(6, d.NumOfGoodQltyAUTPercentage);

                yield return sqlRow;
            }
        }
    }
}
