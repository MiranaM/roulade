using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace roulade.Models
{

    class USinger
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Readme { get; set; }
        public Stream[] Paths { get; set; }
        public UOto[] UOtos { get; set; }

        public USinger()
        {

        }
    }
}
