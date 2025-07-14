using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Option_Organizer
{
    public class Vehicle
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int ZipCode { get; set; }
        public bool VehicleService { get; set; }
        public bool Gap { get; set; }
        public bool Maintenance { get; set; }
        public bool DentAndDing { get; set; }
        public bool Appearance { get; set; }
        public bool Windshield { get; set; }
        public bool KeyReplacement { get; set; }
        public bool Theft { get; set; }

        public override string ToString()
        {
            return $"{Year} {Make} {Model} (Zip: {ZipCode}) - Services: " +
                   $"VS:{(VehicleService ? "Y" : "N")} " +
                   $"Gap:{(Gap ? "Y" : "N")} " +
                   $"Maint:{(Maintenance ? "Y" : "N")} " +
                   $"D&D:{(DentAndDing ? "Y" : "N")} " +
                   $"App:{(Appearance ? "Y" : "N")} " +
                   $"Wind:{(Windshield ? "Y" : "N")} " +
                   $"Key:{(KeyReplacement ? "Y" : "N")} " +
                   $"Theft:{(Theft ? "Y" : "N")}";
        }
    }
}
