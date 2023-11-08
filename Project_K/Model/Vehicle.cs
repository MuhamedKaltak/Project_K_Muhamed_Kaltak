using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Model
{
    public abstract class Vehicle : Product
    {
        public override int Id { get; set ; }
        public override string Name { get ; set; }
        public override string Description { get; set; }
        public override float Price { get; set; }
        public override byte[] Picture { get; set; }
        public override string Discriminator { get; set; }

        public int Year { get; set; }
        public string Manufacturer {  get; set; }
        public int MaxSpeed { get; set; }
        public int NumSeats { get; set; }


    }
}
