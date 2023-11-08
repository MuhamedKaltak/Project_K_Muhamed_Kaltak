using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.ViewModel
{
    public partial class ProductCreationViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public byte[] Picture { get; set; }
        public string Discriminator { get; set; }
    }
}
