using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Model
{
   public interface IProduct
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        float Price { get; set; }
        byte[] Picture { get; set; }
        string Discriminator { get; set; }
    }
}
