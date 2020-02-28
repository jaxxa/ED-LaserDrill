using System;
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

        private int m_RequiredEnergy = 10000;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            //this._FlickComp = this.parent.GetComp<CompFlickable>();
           // this.Properties = this.props as CompProperties_LaserDrill;
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

            float _EnergtLeftToDrain = this.m_RequiredEnergy;

            for (int i = 0; i < this.m_PowerComp.PowerNet.batteryComps.Count; i++)
            {
                CompPowerBattery compPowerBattery = this.m_PowerComp.PowerNet.batteryComps[i];
                float _DrainThisTime = Math.Min(_EnergtLeftToDrain, compPowerBattery.StoredEnergy);

                _DrainThisTime -= _DrainThisTime;
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
                    return "Sufficient Power for Drill Activation, ready to use 10,000 power.";
                }
                else
                {
                    return "Insufficient Power stored for Drill Activation, needs 10,000";

                }
            }
        }
    }
}
