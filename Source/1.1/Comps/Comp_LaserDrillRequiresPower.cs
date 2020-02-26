using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaxxa.EnhancedDevelopment.Core.Comp.Interface;
using Verse;

namespace Jaxxa.EnhancedDevelopment.LaserDrill.Comps
{
    class Comp_LaserDrillRequiresPower : ThingComp, Jaxxa.EnhancedDevelopment.Core.Comp.Interface.IRequiresShipResources
    {
        bool IRequiresShipResources.Satisfied
        {
            get
            {
                return true;
            }
        }

        string IRequiresShipResources.StatusString
        {
            get
            {
                return "Wooo, full power";
            }
        }
    }
}
