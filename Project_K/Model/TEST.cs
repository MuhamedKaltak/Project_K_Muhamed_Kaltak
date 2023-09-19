using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_K.Model
{
    internal class TEST
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string UsernameTEST { get; set; }
    }
}
