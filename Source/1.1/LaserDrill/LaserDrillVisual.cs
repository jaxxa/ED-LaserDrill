using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EnhancedDevelopment.LaserDrill.LaserDrill
{
    class LaserDrillVisual : ThingWithComps
    {

        #region StaticSettings

        private static readonly SimpleCurve DistanceChanceFactor = new SimpleCurve
        {
            {
                new CurvePoint(0f, 1f),
                true
            },
            {
                new CurvePoint(10f, 0.0f),
                true
            }
        };


        private static readonly FloatRange AngleRange = new FloatRange(-12f, 12f);

        #endregion

        #region Variables

        private float Angle;

        public int Duration = 600;

        private int StartTick;

        #endregion

        #region Override Methods

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            //Log.Message("Laser");
            this.Angle = LaserDrillVisual.AngleRange.RandomInRange;

            this.StartTick = Find.TickManager.TicksGame;
            this.GetComp<CompAffectsSky>().StartFadeInHoldFadeOut(30, this.Duration - 30 - 15, 15, 1f);
            this.GetComp<CompOrbitalBeam>().StartAnimation(this.Duration, 10, this.Angle);

            MoteMaker.MakeBombardmentMote(this.Position, this.Map);
            MoteMaker.MakePowerBeamMote(this.Position, this.Map);
        }

        public override void Tick()
        {
            base.Tick();

            if (this.TicksPassed >= this.Duration)
            {
                this.Destroy(DestroyMode.Vanish);
            }

            if (!base.Destroyed)
            {
                if (Find.TickManager.TicksGame % 50 == 0)
                {
                    this.StartRandomFire();
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.Duration, "Duration", 0, false);
            Scribe_Values.Look<float>(ref this.Angle, "Angle", 0f, false);
            Scribe_Values.Look<int>(ref this.StartTick, "StartTick", 0, false);
        }

        public override void Draw()
        {
            base.Comps_PostDraw();
        }

        #endregion

        #region Methods

        private void StartRandomFire()
        {
            IntVec3 c = (from x in GenRadial.RadialCellsAround(base.Position, 25f, true)
                         where x.InBounds(base.Map)
                         select x).RandomElementByWeight((IntVec3 x) => LaserDrillVisual.DistanceChanceFactor.Evaluate(x.DistanceTo(base.Position)));
            FireUtility.TryStartFireIn(c, base.Map, Rand.Range(0.1f, 0.925f));
        }

        #endregion

        #region Properties

        protected int TicksLeft
        {
            get
            {
                return this.Duration - this.TicksPassed;
            }
        }

        protected int TicksPassed
        {
            get
            {
                return Find.TickManager.TicksGame - this.StartTick;
            }
        }

        #endregion

    }
}
