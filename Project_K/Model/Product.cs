using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Model
{
    public abstract class Product : IProduct
    {
        public abstract int Id { get; set; }
        public abstract string Name { get; set; }
        public abstract string Description { get; set; }
        public abstract float Price { get; set; }
        public abstract byte[] Picture { get; set; }
        public abstract string Discriminator { get; set; }
    }
}
