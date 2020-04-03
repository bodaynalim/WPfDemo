using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HomeWork.Annotations;
using MVVMbasics.Attributes;

namespace HomeWork.Model
{
    public enum ElemetnType
    {
        File = 0,
        Directory = 1,
    }
    public class Changes : BaseModel
    {
        public string OldName { get; set; }

        public string OldFullPath { get; set; }

        public string Name { get; set; }

        public  ElemetnType Type { get; set; }

        public string ParentName { get; set; }

        public DateTime DateTimeAction { get; set; }

        public WatcherChangeTypes ChangeType { get; set; }

        public string FullPath { get; set; }

    }
}
