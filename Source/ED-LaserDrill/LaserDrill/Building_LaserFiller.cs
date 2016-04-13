using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Enhanced_Development.Power.LaserDrill
{

    public class Building_LaserFiller : Building
    {
        private static Texture2D UI_ACTIVATE_GATE;
        private int drillWork = 500;
        private CompPowerTrader powerComp;
        Thing targetSteamGeyser = null;
        bool active = false;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.powerComp = this.GetComp<CompPowerTrader>();

            UI_ACTIVATE_GATE = ContentFinder<Texture2D>.Get("UI/nuke", true);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref this.drillWork, "drillWork", 0, false);
            Scribe_Values.LookValue<bool>(ref this.active, "active", false, false);
            //Scribe_Values.LookValue<Thing>(ref this.targetSteamGeyser, "targetSteamGeyser", null, false);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            //Add the stock Gizmoes
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }

            if (true)
            {
                Command_Action act = new Command_Action();
                //act.action = () => Designator_Deconstruct.DesignateDeconstruct(this);
                act.action = () => this.ActivateDrill();
                act.icon = UI_ACTIVATE_GATE;
                act.defaultLabel = "Activate Drill";
                act.defaultDesc = "Activate Drill";
                act.activateSound = SoundDef.Named("Click");
                //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                //act.groupKey = 689736;
                yield return act;
            }
        }

        public Thing FindClosestGuyser()
        {
            List<Thing> steamGeysers = Find.ListerThings.ThingsOfDef(ThingDefOf.SteamGeyser);
            Thing currentLowestGuyser = null;

            double lowestDistance = double.MaxValue;

            foreach (Thing currentGuyser in steamGeysers)
            {
                //if (currentGuyser.SpawnedInWorld)
                if (currentGuyser.Spawned)
                {
                    if (this.Position.InHorDistOf(currentGuyser.Position, 5))
                    {
                        double distance = Math.Sqrt(Math.Pow((this.Position.x - currentGuyser.Position.x), 2) + Math.Pow((this.Position.y - currentGuyser.Position.y), 2));

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

        public void ActivateDrill()
        {
            this.targetSteamGeyser = this.FindClosestGuyser();
            this.active = true;
        }

        //public void RemoveGuyser()
        //{
        //    List<Thing> steamGeysers = Find.ListerThings.ThingsOfDef(ThingDefOf.SteamGeyser);

        //    foreach (Thing currentGuyser in steamGeysers)
        //    {
        //        currentGuyser.DeSpawn();
        //    }
        //}

        public override void TickRare()
        {
            if (this.active)
            {
                if (this.powerComp.PowerOn)
                {
                    Log.Message("Reducing count");
                    this.drillWork = this.drillWork - 1;
                }
                else
                {
                    Log.Message("No Power for drill.");
                }

                if (this.drillWork <= 0)
                {
                    //Thing SteamGeyser = Find.ThingGrid.ThingAt(this.Position, ThingDef.Named("SteamGeyser"));
                    //SteamGeyser.Destroy(DestroyMode.Vanish);
                    //SteamGeyser.DeSpawn();
                    if (this.targetSteamGeyser != null)
                    {
                        this.targetSteamGeyser.DeSpawn();
                        this.Destroy(DestroyMode.Vanish);
                        //GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), this.Position);
                    }
                    else
                    {
                        this.targetSteamGeyser = this.FindClosestGuyser();

                        if (this.targetSteamGeyser != null)
                        {
                            this.targetSteamGeyser.DeSpawn();
                        }
                        this.Destroy(DestroyMode.Vanish);
                    }
                }
            }
            base.Tick();
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            string text;
            if (this.active)
            {
                if (powerComp != null)
                {
                    if (this.powerComp.PowerOn)
                    {
                        text = "Fill Status: Online";

                    }
                    else
                    {
                        text = "Fill Status: Low Power";
                    }
                }
                else
                {
                    text = "Fill Status: Low Power";
                }
            }
            else
            {
                text = "No Steam Geyser found";
            }


            stringBuilder.AppendLine(text);

            text = "Fill Work Remaining: " + this.drillWork;
            stringBuilder.AppendLine(text);

            if (powerComp != null)
            {
                text = powerComp.CompInspectStringExtra();
                if (!text.NullOrEmpty())
                {
                    stringBuilder.AppendLine(text);
                }
            }


            return stringBuilder.ToString();
        }
    }
}
