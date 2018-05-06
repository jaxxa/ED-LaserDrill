using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.LaserDrill
{
    class MapComp_LaserDrill : MapComponent
    {
        //String ActiveBuildingThingID;

        Thing ActiveLaserDrill;

        public MapComp_LaserDrill(Map map) : base(map)
        {

        }

        public bool IsActive(Thing building)
        {
            //Clear Building if empty
            if (this.ActiveLaserDrill != null)
            {
                if (!this.ActiveLaserDrill.Spawned)
                {
                    this.ActiveLaserDrill = null;
                }
            }

            if (this.ActiveLaserDrill == null)
            {
                //If not set set it
                this.ActiveLaserDrill = building;
                return true;
            }
            else
            {
                //Check if it is marked
                return string.Equals(building.thingIDNumber, this.ActiveLaserDrill.thingIDNumber);
            }
        }

        //public override void MapComponentTick()
        //{
        //    base.MapComponentTick();
        //    Log.Message("MapCompTick");
        //}

        public override void ExposeData()
        {
            base.ExposeData();
            
            //Scribe_Values.Look<int>(ref this.DrillWork, "drillWork", 0);
            Scribe_References.Look<Thing>(ref ActiveLaserDrill, "ActiveLaserDrill");
        }

    }
}
