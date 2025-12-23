using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Jaxxa.EnhancedDevelopment.LaserDrill.Settings
{
    class ModSettings_LaserDrill : ModSettings
    {

        //Fields
        public int RequiredScanningTimeDays = 10;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<int>(ref RequiredScanningTimeDays, "RequiredScanningTimeDays", 10, true);            
        }


        public void DoSettingsWindowContents(Rect canvas)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = 250f;
            listing_Standard.Begin(canvas);

            listing_Standard.GapLine(12f);

            listing_Standard.Label("Scanning Days Required: " + RequiredScanningTimeDays.ToString());
            listing_Standard.Gap();
            Listing_Standard _listing_Standard_RequiredDrillWork = new Listing_Standard();
            _listing_Standard_RequiredDrillWork.Begin(listing_Standard.GetRect(30f));
            _listing_Standard_RequiredDrillWork.ColumnWidth = 70;
            _listing_Standard_RequiredDrillWork.IntAdjuster(ref RequiredScanningTimeDays, 1, 1);
            _listing_Standard_RequiredDrillWork.NewColumn();
            _listing_Standard_RequiredDrillWork.IntSetter(ref RequiredScanningTimeDays, 10, "Default");
            _listing_Standard_RequiredDrillWork.End();
                               
            listing_Standard.GapLine(12f);
                        
            listing_Standard.End();


        }
    }
}

