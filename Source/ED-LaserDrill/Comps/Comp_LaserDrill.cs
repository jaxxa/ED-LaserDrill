﻿using RimWorld;
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
        private int DrillIterationNumber = 1;
        private int DrillWork;

        //Unsaved
        private CompPowerTrader _PowerComp;
        private CompFlickable _FlickComp;
        
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this._PowerComp = this.parent.GetComp<CompPowerTrader>();
            this._FlickComp = this.parent.GetComp<CompFlickable>();

            if (!respawningAfterLoad)
            {
                this.CalculateWorkStart();
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.DrillWork, "drillWork", 0);
            Scribe_Values.Look<int>(ref this.DrillIterationNumber, "DrillIterationNumber", 0);
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

                GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), this.parent.Position, this.parent.Map);

                if (this.DrillIterationNumber >= Mod_LaserDrill.Settings.DrillCharges)
                {
                    //Destroy

                    this.parent.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    //Respawn
                    IntVec3 _position = this.parent.Position;
                    Map _Map = this.parent.Map;

                    this.DrillIterationNumber += 1;
                    this.CalculateWorkStart();

                    MinifiedThing _MiniThing = this.parent.MakeMinified();
                    GenPlace.TryPlaceThing(_MiniThing, _position, _Map, ThingPlaceMode.Near, null);
                }


            }
            base.CompTickRare();
            
        }

        public override string CompInspectStringExtra()
        {
            // return base.CompInspectStringExtra();

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

    }
}
