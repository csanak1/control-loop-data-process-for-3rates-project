using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLoopProcessFor3Rates.Models
{
    class ControlLoopDataModel
    {
        public string HistTagName { get; set; }
        public int NumOfGoodData { get; set; }
        public decimal NumOfGoodQltyPercentage { get; set; }
        public int NumOfGoodMANData { get; set; }
        public decimal NumOfGoodQltyMANPercentage { get; set; }
        public int NumOfGoodAUTData { get; set; }
        public decimal NumOfGoodQltyAUTPercentage { get; set; }
    }
}
