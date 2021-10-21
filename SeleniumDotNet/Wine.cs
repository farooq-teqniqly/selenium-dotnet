using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumDotNet
{
    public class Wine
    {
        public Wine()
        {
            this.TastingNotes = new HashSet<string>();
        }

        public string Name { get; set; }
        public string Winery { get; set; }
        public string Vintage { get; set; }
        public string Price { get; set; }
        public decimal Rating { get; set; }
        public int RatingCount { get; set; }
        public HashSet<string> TastingNotes { get; set; }
    } 
}
