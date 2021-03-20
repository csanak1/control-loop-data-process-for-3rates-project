using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlLoopProcessFor3Rates.Models
{
    class ModeTagsDataModel
    {
        public string TagName { get; set; }
        public string PlantLoadFilterTagName { get; set; }
        public bool ConditionFilter { get; set; }
        public string ConditionFilterTagName { get; set; }
        public int ConditionFilterType { get; set; }
        public double ConditionFilterValue { get; set; }
    }
}
