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

        private int m_RequiredEnergy = 6000;

        #endregion Variables

        #region Initilisation

        static Comp_LaserDrill()
        {

            UI_LASER_ACTIVATE = ContentFinder<Texture2D>.Get("UI/Power/SteamGeyser", true);
            UI_LASER_ACTIVATEFILL = ContentFinder<Texture2D>.Get("UI/Power/RemoveSteamGeyser", true);
        }

        #endregion Initilisation

        #region Overrides

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.m_FlickComp = this.parent.GetComp<CompFlickable>();
            this.Properties = this.props as CompProperties_LaserDrill;
            this.m_PowerComp = parent.TryGetComp<CompPowerTrader>();


            if (!respawningAfterLoad)
            {
                this.SetRequiredDrillScanningToDefault();
            }

            parent.Map.GetComponent<LaserDrillMapComp>().Register(this);
        }

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


            if (this.IsScanComplete())
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

            if (this.HasEnoughEnergyToActivate())
            {
                _StringBuilder.Append("Sufficient Power for Drill Activation, ready to use 6,000 Wd.");
            }
            else
            {
                if (this.m_PowerComp.PowerNet != null)
                {

                    _StringBuilder.Append("Insufficient Power stored for Drill Activation, needs 6,000 Wd. Currently has " + Math.Floor(this.m_PowerComp.PowerNet.CurrentStoredEnergy()).ToString() + " Wd.");
                }
                else
                {
                    _StringBuilder.Append("No Power Net");
                }
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

            if (DebugSettings.godMode)
            {
                Command_Action act = new Command_Action();
                act.action = () =>
                {
                    //60,000 is 1 day
                    this.DrillScanningRemainingTicks -= 30000;
                };
                //act.icon = UI_LASER_ACTIVATEFILL;
                act.defaultLabel = "Debug: Progress Scann";
                act.defaultDesc = "Debug: Progress Scann";
                act.activateSound = SoundDef.Named("Click");
                //act.hotKey = KeyBindingDefOf.DesignatorDeconstruct;
                //act.groupKey = 689736;
                yield return act;
            }

        } //CompGetGizmosExtra()

        public override void PostDeSpawn(Map map)
        {
            map.GetComponent<LaserDrillMapComp>().Deregister(this);
            this.SetRequiredDrillScanningToDefault();

            base.PostDeSpawn(map);
        }

        #endregion Overrides

        #region Methods

        private Boolean IsValidForActivation()
        {
            if (this.IsScanComplete() & this.HasEnoughEnergyToActivate())
            {
                return true;
            }

            StringBuilder _StringBuilder = new StringBuilder();
            _StringBuilder.AppendLine("Laser Activation Failure:");

            if (!this.IsScanComplete())
            {
                if (this.IsScanning())
                {
                    _StringBuilder.AppendLine(" * Scanning incomplete - Time Remaining: " + this.DrillScanningRemainingTicks.ToStringTicksToPeriod());
                }
                else
                {
                    _StringBuilder.AppendLine(" * Scanning paused - Time Remaining after resuming: " + this.DrillScanningRemainingTicks.ToStringTicksToPeriod());
                }
            }

            if (this.HasEnoughEnergyToActivate())
            {
                _StringBuilder.AppendLine("Sufficient Power for Drill Activation, ready to use 6,000 Wd.");
            }
            else
            {
                _StringBuilder.AppendLine("Insufficient Power stored for Drill Activation, needs 6,000 Wd. Currently has " + Math.Floor(this.m_PowerComp.PowerNet.CurrentStoredEnergy()).ToString() + " Wd.");
            }

            Find.LetterStack.ReceiveLetter("Scann in progress", _StringBuilder.ToString(), LetterDefOf.NeutralEvent, new LookTargets(this.parent));

            Messages.Message(_StringBuilder.ToString(), MessageTypeDefOf.NegativeEvent);

            return false;
        }

        private void SetRequiredDrillScanningToDefault()
        {
            this.DrillScanningRemainingTicks = Settings.Mod_Laser_Drill.Settings.RequiredScanningTimeDays * 60000;
        }

        public Thing FindClosestGeyserToPoint(IntVec3 location)
        {
            List<Thing> steamGeyser = this.parent.Map.listerThings.ThingsOfDef(ThingDefOf.SteamGeyser);
            Thing currentLowestGeyser = null;

            double lowestDistance = double.MaxValue;

            foreach (Thing currentGuyser in steamGeyser)
            {
                //if (currentGuyser.SpawnedInWorld)
                if (currentGuyser.Spawned)
                {
                    if (location.InHorDistOf(currentGuyser.Position, 5))
                    {
                        double distance = Math.Sqrt(Math.Pow((location.x - currentGuyser.Position.x), 2) + Math.Pow((location.y - currentGuyser.Position.y), 2));

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

            TargetingParameters targetingParams = new TargetingParameters() { canTargetLocations = true };
            Find.Targeter.BeginTargeting(targetingParams, delegate (LocalTargetInfo target)
            {
                IntVec3 _LocationCell = new IntVec3(target.Cell.x, target.Cell.y, target.Cell.z);

                if (!IsValidForActivation()) { return; }

                var _ClosestGyser = this.FindClosestGeyserToPoint(_LocationCell);

                if (_ClosestGyser != null)
                {
                    this.ShowLaserVisually(_ClosestGyser.Position);
                    _ClosestGyser.DeSpawn();

                    Messages.Message("SteamGeyser Removed.", MessageTypeDefOf.TaskCompletion);
                    this.parent.Destroy(DestroyMode.Vanish);
                }
                else
                {
                    Messages.Message("SteamGeyser not found to Remove.", MessageTypeDefOf.NegativeEvent);
                }
            }, delegate (LocalTargetInfo target)
            {
                //Highlght action

                GenDraw.DrawRadiusRing(target.Cell, 5.0f);

            }, null, null, null);

        }

        public void TriggerLaser()
        {

            TargetingParameters targetingParams = new TargetingParameters() { canTargetLocations = true };

            Find.Targeter.BeginTargeting(targetingParams, delegate (LocalTargetInfo target)
            {
                IntVec3 _LocationCell = new IntVec3(target.Cell.x, target.Cell.y, target.Cell.z);

                if (!IsValidForActivation()) { return; }
                this.ShowLaserVisually(_LocationCell);

                GenSpawn.Spawn(ThingDef.Named("SteamGeyser"), _LocationCell, this.parent.Map);

                Messages.Message("SteamGeyser Created.", MessageTypeDefOf.TaskCompletion);
                this.UseStoredPower();
                this.parent.Destroy(DestroyMode.Vanish);

            }, delegate (LocalTargetInfo target)
            {
                //Highlght action

                GenDraw.DrawRadiusRing(target.Cell, 0.1f);
                GenDraw.DrawRadiusRing(new IntVec3(target.Cell.x + 1, target.Cell.y, target.Cell.z), 0.1f);
                GenDraw.DrawRadiusRing(new IntVec3(target.Cell.x, target.Cell.y, target.Cell.z + 1), 0.1f);
                GenDraw.DrawRadiusRing(new IntVec3(target.Cell.x + 1, target.Cell.y, target.Cell.z + 1), 0.1f);

            }, null, null, null);

        }

        private void ShowLaserVisually(IntVec3 position)
        {
            LaserDrillVisual _LaserDrillVisual = (LaserDrillVisual)GenSpawn.Spawn(ThingDef.Named("LaserDrillVisual"), position, parent.Map, WipeMode.Vanish);
        }

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

                Messages.Message("Drill Shutdown, Multiple Drills Scanning at once will cause interference.", this.parent, MessageTypeDefOf.RejectInput);

            }
        }

        private bool HasEnoughEnergyToActivate()
        {
            return this.m_PowerComp?.PowerNet?.CurrentStoredEnergy() >= this.m_RequiredEnergy;
        }

        public bool UseStoredPower()
        {
            if (!this.HasEnoughEnergyToActivate())
            {
                return false;
            }

            float _EnergyLeftToDrain = this.m_RequiredEnergy;

            for (int i = 0; i < this.m_PowerComp.PowerNet.batteryComps.Count; i++)
            {
                CompPowerBattery compPowerBattery = this.m_PowerComp.PowerNet.batteryComps[i];
                float _DrainThisTime = Math.Min(_EnergyLeftToDrain, compPowerBattery.StoredEnergy);

                _EnergyLeftToDrain -= _DrainThisTime;
                compPowerBattery.DrawPower(_DrainThisTime);
            }

            return true;

        }

        #endregion

    } //Comp_LaserDrill

}

