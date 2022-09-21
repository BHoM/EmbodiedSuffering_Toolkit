using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace BH.oM.EmbodiedSuffering.Elements
{
    [Description("Enum describing if the ratios for datasets used should be by mass or by cost.")]
    public enum ImportSourceType
    {
        Undefined,
        ByCost,
        ByMass
    }
}
