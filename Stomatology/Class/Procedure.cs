using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stomatology.Class
{
    public class Procedure
    {
        public Procedure(string name, int id)
        {

            Name = name;
            ID = id;
        }
        public string Name { get; private set; }
        public int ID { get; private set; }
    }
}
