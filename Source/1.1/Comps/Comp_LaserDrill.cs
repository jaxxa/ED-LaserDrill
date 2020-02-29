using Jaxxa.EnhancedDevelopment.Core.Comp.Interface;
using Jaxxa.EnhancedDevelopment.LaserDrill.Things;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Jaxxa.EnhancedDevelopment.LaserDrill.Comps
{
    [StaticConstructorOnStartup]
    class Comp_LaserDrill : ThingComp
    {

        #region Variables

        //Saved
        private int DrillScanningRemainingTicks;

        //Unsaved
        private CompProperties_LaserDrill Properties;

        private static Texture2D UI_LASER_ACTIVATE;
        private static Texture2D UI_LASER_ACTIVATEFILL;

        private CompPowerTrader m_PowerComp;
        private CompFlickable m_FlickComp;

        private IRequiresShipResources m_RequiresShipResourcesComp;

        #endregion Variables

        #region Initilisation

        static Comp_LaserDrill()
        {

            UI_LASER_ACTIVATE = ContentFinder<Texture2D>.Get("UI/Power/SteamGeyser", true);
            UI_LASER_ACTIVATEFILL = ContentFinder<Texture2D>.Get("UI/Power/RemoveSteamGeyser", true);
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.m_FlickComp = this.parent.GetComp<CompFlickable>();
            this.Properties = this.props as CompProperties_LaserDrill;
            this.m_PowerComp = parent.TryGetComp<CompPowerTrader>();

            //Add IRequiresShipResources Comp
            var _Comp = this.parent.GetComps<ThingComp>().FirstOrDefault(x => x is IRequiresShipResources);
            var _ResourcesCompInterface = _Comp as IRequiresShipResources;
            if (_ResourcesCompInterface == null)
            {
                Log.Error(nameof(Comp_LaserDrill) + " Failed to get Comp With " + nameof(IRequiresShipResources));
            }
            else
            {
                this.m_RequiresShipResourcesComp = _ResourcesCompInterface;
            }


            if (!respawningAfterLoad)
            {
                this.SetRequiredDrillScanningToDefault();
            }

            parent.Map.GetComponent<LaserDrillMapComp>().Register(this);
        }

        #endregion Initilisation

        #region IRequiresShipResources
        
        private bool HasSufficientShipResources()
        {
            return this.m_RequiresShipResourcesComp.Satisfied;
        }

        #endregion

        private bool IsScanComplete()
        {
            return (this.DrillScanningRemainingTicks <= 0);
        }

        private bool HasPowerToScan()
        {
            if (this.m_PowerComp != null)
            {
                return this.m_PowerComp.PowerOn;
            }
            return true;
        }
        
        public bool IsScanning()
        {
            return (!this.IsScanComplete() & this.HasPowerToScan());
        }

        public void StopScanning()
        {
            if (!this.m_FlickComp.WantsFlick() & this.m_FlickComp.SwitchIsOn)
            {
                var _Gizmos = this.m_FlickComp.CompGetGizmosExtra().ToList();

                Command_Toggle _Temp = (Command_Toggle)_Gizmos.First();
                _Temp.toggleAction.Invoke();

                this.m_FlickComp.SwitchIsOn = false;
            }
        }

        #region Overrides

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<int>(ref this.DrillScanningRemainingTicks, "DrillScanningRemainingTicks", 0);
        } //PostExposeData()

        public override void CompTickRare()
        {
            if (this.IsScanning())
            {
                //250 Ticks per Rare Tick
                this.DrillScanningRemainingTicks -= 250;

            }

            base.CompTickRare();

        } //CompTickRare()

        public override string CompInspectStringExtra()
        {
            // return base.CompInspectStringExtra();

            StringBuilder _StringBuilder = new StringBuilder();

            //if (this.parent.Map != null && this.parent.Map.GetComponent<MapComp_LaserDrill>() != null)

            {
                if(this.IsScanComplete())
                {

                    _StringBuilder.AppendLine("Scan complete");
                }
                else
                {
                    if (this.HasPowerToScan())
                    {
                        _StringBuilder.AppendLine("Scanning in Progress - Remaining: " + this.DrillScanningRemainingTicks.ToStringTicksToPeriod());
                    }
                    else 
                    {
                        _StringBuilder.AppendLine("Scanning Paused, Power Offline.");
                    }
                }

                _StringBuilder.Append(this.m_RequiresShipResourcesComp.StatusString);

            }

            return _StringBuilder.ToString();
        } //CompInspectStringExtra()

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            //return base.CompGetGizmosExtra();

            //Add the stock Gizmoes
            foreach (var g in base.CompGetGizmosExtra())
            {
                yield return g;
            }

            if (this.IsScanComplete() & this.HasSufficientShipResources())
            {


                if (true)
                {
                    Command_Action act = new Command_Action();
                    act.action = () => this.TriggerLaser();
                    act.icon = UI_LASER_ACTIVATE;
                    act.defaultLabel = "Activate Laser";
                    act.defaultDesc = "Activate Laser";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }

                if (true)
                {
                    Command_Action act = new Command_Action();
                    act.action = () => this.TriggerLaserToFill();
                    act.icon = UI_LASER_ACTIVATEFILL;
                    act.defaultLabel = "Activate Laser Fill";
                    act.defaultDesc = "Activate Laser Fill";
                    act.activateSound = SoundDef.Named("Click");
                    //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                    //act.groupKey = 689736;
                    yield return act;
                }
            }

        } //CompGetGizmosExtra()

        public override void PostDeSpawn(Map map)
        {
            this.SetRequiredDrillScanningToDefault();


            parent.Map.GetComponent<LaserDrillMapComp>().Deregister(this);
            base.PostDeSpawn(map);
        }

        #endregion Overrides

        #region Methods

        private void DisableOtherDrills()
        {
            List<Building> _Buildings = this.parent.Map.listerBuildings.allBuildingsColonist;

            foreach (Building _Building in _Buildings)
            {
                Comp_LaserDrill _LaserComp = _Building.GetComp<Comp_LaserDrill>();


            }
            
        }

        private void SetRequiredDrillScanningToDefault()
        {
            this.DrillScanningRemainingTicks = Settings.Mod_LaserDrill.Settings.RequiredScanningTimeDays * 60000;
        }

        public Thing FindClosestGeyser()
        {
            List<Thing> steamGeyser = this.parent.Map.listerThings.ThingsOfDef(ThingDefOf.SteamGeyser);
            Thing currentLowestGeyser = null;

            double lowestDistance = double.MaxValue;

            foreach (Thing currentGuyser in steamGeyser)
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
                            currentLowestGeyser = currentGuyser;
                        }
                    }
                }
            }
            return currentLowestGeyser;
        }

        public void TriggerLaserToFill()
        {
            if (this.FindClosestGeyser() != null)
            {
                Messages.Message("SteamGeyser Removed.", MessageTypeDefOf.TaskCompletion);
                this.FindClosestGeyser().DeSpawn();
                //TODO JW: Remove Power from Comp
                this.ShowLaserVisually();

                this.parent.Destroy(DestroyMode.Vanish);
            }
            else
            {
                Messages.Message("SteamGeyser not found to Remove.", MessageTypeDefOf.NegativeEvent);
            }
        }

        public void TriggerLaser()
        {
            Messages.Message("SteamGeyser Created.", MessageTypeDefOf.TaskCompletion);
            //TODO JW: Remove Power from ship
            this.ShowLaserVisually();
            GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), this.parent.Position, this.parent.Map);

            //Destroy

            this.parent.Destroy(DestroyMode.Vanish);
        }

        private void ShowLaserVisually()
        {
            IntVec3 _Position = IntVec3.FromVector3(new UnityEngine.Vector3(parent.Position.x, parent.Position.y, parent.Position.z - 2));
            LaserDrillVisual _LaserDrillVisual = (LaserDrillVisual)GenSpawn.Spawn(ThingDef.Named("LaserDrillVisual"), _Position, parent.Map, WipeMode.Vanish);
        }

        #endregion


    } //Comp_LaserDrill

}

