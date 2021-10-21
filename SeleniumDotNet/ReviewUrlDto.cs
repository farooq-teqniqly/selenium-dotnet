using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumDotNet
{
    public class ReviewUrlDto
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public bool? Processed { get; set; }
    }
}
