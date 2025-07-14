using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Option_Organizer
{
    public class OllamaResponse
    {
        public string model { get; set; }
        public string created_at { get; set; }
        public string response { get; set; }
        public bool done { get; set; }
        public string done_reason { get; set; }
    }
}
