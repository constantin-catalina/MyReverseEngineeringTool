using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_3
{
    public class DiagramOptions
    {
        public List<string> IgnoredClasses { get; set; } = new List<string>();
        public bool UseFullyQualifiedNames { get; set; } = false;
        public bool ShowMethods { get; set; } = true;
        public bool ShowAttributes { get; set; } = true;
    }
}
