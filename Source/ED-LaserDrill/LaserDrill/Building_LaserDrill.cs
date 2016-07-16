using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace EnhancedDevelopment.LaserDrill
{
    [StaticConstructorOnStartup]
    public class Building_LaserDrill : Building
    {
        private int drillWork = 500;
        private CompPowerTrader _PowerComp;
        private CompFlickable _FlickComp;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this._PowerComp = this.GetComp<CompPowerTrader>();
            this._FlickComp = this.GetComp<CompFlickable>();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref this.drillWork, "drillWork", 0, false);
        }

        public override void TickRare()
        {
            if (this._PowerComp.PowerOn)
            {
                //Log.Message("Reducing count");
                this.disableOthers();
                this.drillWork = this.drillWork - 1;
            }
            else
            {
                //Log.Message("No Power for drill.");
            }

            if (this.drillWork <= 0)
            {
                Messages.Message("SteamGeyser Created.", MessageSound.Benefit);

                this.Destroy(DestroyMode.Vanish);
                GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), this.Position);
            }
            base.Tick();
        }

        public void disableOthers()
        {
            //<Pawn>(t => t.Position.WithinHorizontalDistanceOf(this.Position, this.MAX_DISTANCE));               
            IEnumerable<Building> LaserBuildings = Find.ListerBuildings.allBuildingsColonist.Where<Building>(t => t.def.defName == "LaserDrill");

            if (LaserBuildings != null)
            {
                //List<Thing> fireTo
                foreach (Building_LaserDrill currentBuilding in LaserBuildings.ToList())
                {
                    //Log.Message("Checking");
                    if (currentBuilding != this)
                    {
                        //if (currentBuilding._PowerComp.DesirePowerOn)
                        if (currentBuilding._FlickComp.SwitchIsOn)
                        {
                            Messages.Message("Only One Laser Drill Can be active at a Time.", MessageSound.Negative);
                            //currentBuilding.powerComp.DesirePowerOn = false;

                            // currentBuilding._PowerComp.PowerOn = false;

                            currentBuilding._FlickComp.DoFlick();

                           // currentBuilding.powerComp. = false;
                        }

                    }
                }
            }
            else
            {
               // Log.Error("List Null");

            }
                             
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string text;

            if (_PowerComp != null)
            {
                if (this._PowerComp.PowerOn)
                {
                    text = "Drill Status: Online";

                }
                else
                {
                    text = "Drill Status: Low Power";
                }
            }
            else
            {
                text = "Drill Status: Low Power";
            }

            stringBuilder.AppendLine(text);

            text = "Drill Work Remaining: " + this.drillWork;
            stringBuilder.AppendLine(text);

            if (_PowerComp != null)
            {
                text = _PowerComp.CompInspectStringExtra();
                if (!text.NullOrEmpty())
                {
                    stringBuilder.AppendLine(text);
                }
            }


            return stringBuilder.ToString();
        }
    }
}
