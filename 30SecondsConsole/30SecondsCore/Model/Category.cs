using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WZWVAPI;

namespace _30SecondsCore.Model
{
    public class Category : DataObject
    {
        public string Name { get; set; }

        public Category(int ID, string Name)
            : base(ID)
        {
            this.Name = Name;
        }
    }
}
