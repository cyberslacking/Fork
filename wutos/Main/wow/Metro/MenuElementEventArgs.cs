using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metro
{
    public class MenuElementEventArgs:EventArgs
    {
        public MetroMenuElement Element { get; set; }
    }
}
