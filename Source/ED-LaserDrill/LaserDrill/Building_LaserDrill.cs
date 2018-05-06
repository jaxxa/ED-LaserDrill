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
        private int DrillIterationNumber = 1;
        private int DrillWork;
        private CompPowerTrader _PowerComp;
        private CompFlickable _FlickComp;

        public override void SpawnSetup(Map map, Boolean respawnAfterLoading)
        {
            base.SpawnSetup(map, respawnAfterLoading);
            this._PowerComp = this.GetComp<CompPowerTrader>();
            this._FlickComp = this.GetComp<CompFlickable>();
            
            if (!respawnAfterLoading)
            {
                this.CalculateWorkStart();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.DrillWork, "drillWork", 0);
            Scribe_Values.Look<int>(ref this.DrillIterationNumber, "DrillIterationNumber", 0);
        }

        public override void TickRare()
        {
            if (this.Map.GetComponent<MapComp_LaserDrill>().IsActive(this))
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

                GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), this.Position, this.Map);

                if (this.DrillIterationNumber >= Mod_LaserDrill.Settings.DrillCharges)
                {
                    //Destroy

                    this.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    //Respawn
                    IntVec3 _position = this.Position;
                    Map _Map = this.Map;

                    this.DrillIterationNumber += 1;
                    this.CalculateWorkStart();

                    MinifiedThing _MiniThing = this.MakeMinified();
                    GenPlace.TryPlaceThing(_MiniThing, _position, _Map, ThingPlaceMode.Near, null);
                }


            }
            base.Tick();
        }

        public override string GetInspectString()
        {
            StringBuilder _StringBuilder = new StringBuilder();

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
            else
            {
                _StringBuilder.AppendLine("Drill Status: Low Power");
            }

            _StringBuilder.Append("Drill Work Remaining: " + this.DrillWork);

            if (_PowerComp != null)
            {
                string _Text = _PowerComp.CompInspectStringExtra();
                if (!_Text.NullOrEmpty())
                {
                    _StringBuilder.AppendLine("");
                    _StringBuilder.Append(_Text);
                }
            }

            if (Mod_LaserDrill.Settings.DrillCharges > 1)
            {
                _StringBuilder.AppendLine("");
                _StringBuilder.Append("Charges: " + this.DrillIterationNumber + " / " + Mod_LaserDrill.Settings.DrillCharges);
            }

            return _StringBuilder.ToString();
        }

        private void CalculateWorkStart()
        {
            this.DrillWork = Mod_LaserDrill.Settings.RequiredDrillWork * (int)Math.Pow(2, this.DrillIterationNumber - 1);

        }

    } // End Class
}
