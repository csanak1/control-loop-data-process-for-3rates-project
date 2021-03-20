using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLoopProcessFor3Rates
{
    class Handler
    {
        private readonly SQLDA _sqlDataAccess = new SQLDA();
        private readonly HistorianDA _historianDataAccess = new HistorianDA();

        public void ProcessCRData(DateTime queryStart, DateTime queryEnd)
        {
            var modeTagsData = _sqlDataAccess.GetModeTagsToProcess();
            var processedCLData = _historianDataAccess.QueryControlLoopsTagData(modeTagsData, queryStart, queryEnd);
            var processedCLDataCollection = new ControlLoopDataCollection(processedCLData);

            string dateId = queryStart.ToString("yyyy.MM.dd. HH:mm");

            _sqlDataAccess.InsertProcessedControlLoopData(processedCLDataCollection, dateId);
        }
    }
}
