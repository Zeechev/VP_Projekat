using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaxyPPG.Models;

namespace GalaxyPPG.Events
{
    public class SampleReceivedEventArgs : EventArgs
    {
        public PpgSample Sample { get; set; }
    }
}
