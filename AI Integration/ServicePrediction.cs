using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Option_Organizer
{
    public class ServicePrediction
    {
        public List<ServiceRanking> ServiceRankings { get; set; } = new List<ServiceRanking>();
        public string OverallInsight { get; set; } = "";
    }
}
