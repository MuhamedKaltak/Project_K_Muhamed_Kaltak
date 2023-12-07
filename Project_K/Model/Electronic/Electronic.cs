using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Model.Electronic
{
    public abstract class Electronic : Product
    {
        public override int Id { get; set; }
        public override string Name { get; set; }
        public override string Description { get; set; }
        public override float Price { get; set; }
        public override byte[] Picture { get; set; }
        public override string Discriminator { get; set; }


        protected string OS { get; set; }
        protected string Processor { get; set; }
        protected int Ram {  get; set; }
        protected int Storage { get; set; }

    }
}
