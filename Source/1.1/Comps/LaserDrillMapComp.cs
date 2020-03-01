using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Jaxxa.EnhancedDevelopment.LaserDrill.Comps
{
    class LaserDrillMapComp : MapComponent
    {
        Map m_Map;
        public const int CHECK_INTERVAL = 1000;

        private List<Comp_LaserDrill> comps = new List<Comp_LaserDrill>();

        public LaserDrillMapComp(Map map) : base(map)
        {
            this.m_Map = map;
        }

        public override void MapComponentTick()
        {
            if (!Settings.Mod_LaserDrill.Settings.AllowSimultaneousDrilling && 
                Find.TickManager.TicksGame % LaserDrillMapComp.CHECK_INTERVAL == 0)
            {
                //Only allow the first Drill to Scan
                bool _Scanning = false;
                for (int i = 0; i < comps.Count; i++)
                {
                    if (_Scanning)
                    {
                        comps[i].StopScanning();
                    }
                    if (comps[i].IsScanning())
                    {
                        _Scanning = true;
                    }
                }
            }
        }


        public void Register(Comp_LaserDrill c)
        {
            comps.Add(c);
        }

        public void Deregister(Comp_LaserDrill c)
        {
            comps.Remove(c);
        }

    }
}
