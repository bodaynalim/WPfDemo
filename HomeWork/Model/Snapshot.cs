using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HomeWork.Annotations;

namespace HomeWork.Model
{
    public class Snapshot : BaseModel
    {
        public Snapshot()
        {
            Changes = new List<Changes>();
        }

        private string _name { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DateTime DateCreating { get; set; }

        public List<Changes> Changes { get; set; }
    }
}
