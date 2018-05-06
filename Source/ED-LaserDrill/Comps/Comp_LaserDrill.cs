using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.LaserDrill.Comps
{
    class Comp_LaserDrill : ThingComp
    {
        //Saved
        private int DrillWork;

        //Unsaved
        private CompPowerTrader _PowerComp;
        private CompFlickable _FlickComp;
        private CompProperties_LaserDrill Properties;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this._PowerComp = this.parent.GetComp<CompPowerTrader>();
            this._FlickComp = this.parent.GetComp<CompFlickable>();
            this.Properties = this.props as CompProperties_LaserDrill;

            if (!respawningAfterLoad)
            {
                this.CalculateWorkStart();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.DrillWork, "DrillWork", 0);
        }

        public override void CompTickRare()
        {
            if (this.parent.Map.GetComponent<MapComp_LaserDrill>().IsActive(this.parent))
            {
                Log.Message("Active");
            }
            else
            {
                Log.Message("Inactive");
                return;
            }

            if (this._PowerComp.PowerOn)
            {
                this.DrillWork = this.DrillWork - 1;
            }

            if (this.DrillWork <= 0)
            {
                Messages.Message("SteamGeyser Created.", MessageTypeDefOf.TaskCompletion);

                if (this.Properties.FillMode)
                {
                    if (this.FindClosestGuyser() != null)
                    {
                        this.FindClosestGuyser().DeSpawn();
                        this.parent.Destroy(DestroyMode.Vanish);
                    }
                }
                else
                {
                    GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), this.parent.Position, this.parent.Map);

                    //Destroy

                    this.parent.Destroy(DestroyMode.Vanish);

                }
            }
            base.CompTickRare();

        }

        public override string CompInspectStringExtra()
        {
            // return base.CompInspectStringExtra();

            StringBuilder _StringBuilder = new StringBuilder();

            if (this.parent.Map != null && this.parent.Map.GetComponent<MapComp_LaserDrill>() != null)
            {
                if (!this.parent.Map.GetComponent<MapComp_LaserDrill>().IsActive(this.parent))
                {
                    _StringBuilder.AppendLine("Drill Status: Offline, Waiting for another drill to finish.");
                }
                else
                {
                    if (_PowerComp != null)
                    {
                        if (this._PowerComp.PowerOn)
                        {
                            _StringBuilder.AppendLine("Drill Status: Online");
                        }
                        else
                        {
                            _StringBuilder.AppendLine("Drill Status: Low Power");
                        }
                    }
                    _StringBuilder.Append("Drill Work Remaining: " + this.DrillWork);
                }
            }

            return _StringBuilder.ToString();
        }

        private void CalculateWorkStart()
        {
            if (this.Properties.FillMode)
            {
                this.DrillWork = Mod_LaserDrill.Settings.RequiredFillWork;
            }
            else
            {
                this.DrillWork = Mod_LaserDrill.Settings.RequiredDrillWork;
            }
        }

        public Thing FindClosestGuyser()
        {
            List<Thing> steamGeysers = this.parent.Map.listerThings.ThingsOfDef(ThingDefOf.SteamGeyser);
            Thing currentLowestGuyser = null;

            double lowestDistance = double.MaxValue;

            foreach (Thing currentGuyser in steamGeysers)
            {
                //if (currentGuyser.SpawnedInWorld)
                if (currentGuyser.Spawned)
                {
                    if (this.parent.Position.InHorDistOf(currentGuyser.Position, 5))
                    {
                        double distance = Math.Sqrt(Math.Pow((this.parent.Position.x - currentGuyser.Position.x), 2) + Math.Pow((this.parent.Position.y - currentGuyser.Position.y), 2));

                        if (distance < lowestDistance)
                        {

                            lowestDistance = distance;
                            currentLowestGuyser = currentGuyser;
                        }
                    }
                }
            }
            return currentLowestGuyser;
        }
    }
}
