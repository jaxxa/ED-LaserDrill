﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaxxa.EnhancedDevelopment.Core.Comp.Interface;
using RimWorld;
using Verse;

namespace Jaxxa.EnhancedDevelopment.LaserDrill.Comps
{
    class Comp_LaserDrillRequiresPower : ThingComp, Jaxxa.EnhancedDevelopment.Core.Comp.Interface.IRequiresShipResources
    {

        private CompPowerTrader m_PowerComp;

        private int m_RequiredEnergy = 6000;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.m_PowerComp = parent.TryGetComp<CompPowerTrader>();

        }
               
        private bool HasEnoughEnergy()
        {
            return this.m_PowerComp?.PowerNet?.CurrentStoredEnergy() >= this.m_RequiredEnergy;
        }

        public bool UseResources()
        {
            if (!this.HasEnoughEnergy())
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

        bool IRequiresShipResources.Satisfied
        {
            get
            {
                return this.HasEnoughEnergy();
            }
        }

        string IRequiresShipResources.StatusString
        {
            get
            {
                if (this.HasEnoughEnergy())
                {
                    return "Sufficient Power for Drill Activation, ready to use 6,000 Wd.";
                }
                else
                {
                    return "Insufficient Power stored for Drill Activation, needs 6,000 Wd. Currently has " + Math.Floor(this.m_PowerComp.PowerNet.CurrentStoredEnergy()).ToString() + " Wd.";

                }
            }
        }

    }
}
