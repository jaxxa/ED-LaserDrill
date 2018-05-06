using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace EnhancedDevelopment.LaserDrill
{
    class ModSettings_LaserDrill : ModSettings
    {

        //Fields
        public int RequiredDrillWork = 500;
        public int RequiredFillWork = 500;
        public bool AllowSimultaneousDrilling = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<int>(ref RequiredDrillWork, "RequiredDrillWork", 500);
            Scribe_Values.Look<int>(ref RequiredFillWork, "RequiredFillWork", 500);
            Scribe_Values.Look<bool>(ref AllowSimultaneousDrilling, "AllowSimultaneousDrilling", false);

        }


        public void DoSettingsWindowContents(Rect canvas)
        {
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = 250f;
            listing_Standard.Begin(canvas);

            listing_Standard.GapLine(12f);

            listing_Standard.Label("Drilling Work Required: " + RequiredDrillWork.ToString());
            listing_Standard.Gap();
            Listing_Standard _listing_Standard_RequiredDrillWork = new Listing_Standard();
            _listing_Standard_RequiredDrillWork.Begin(listing_Standard.GetRect(30f));
            _listing_Standard_RequiredDrillWork.ColumnWidth = 70;
            _listing_Standard_RequiredDrillWork.IntAdjuster(ref RequiredDrillWork, 10, 10);
            _listing_Standard_RequiredDrillWork.NewColumn();
            _listing_Standard_RequiredDrillWork.IntAdjuster(ref RequiredDrillWork, 100, 100);
            _listing_Standard_RequiredDrillWork.NewColumn();
            _listing_Standard_RequiredDrillWork.IntSetter(ref RequiredDrillWork, 500, "Default");
            _listing_Standard_RequiredDrillWork.End();


            listing_Standard.GapLine(12f);
            
            listing_Standard.Label("Filling Work Required: " + RequiredFillWork.ToString());
            listing_Standard.Gap();
            Listing_Standard _listing_Standard_RequiredFillWork = new Listing_Standard();
            _listing_Standard_RequiredFillWork.Begin(listing_Standard.GetRect(30f));
            _listing_Standard_RequiredFillWork.ColumnWidth = 70;
            _listing_Standard_RequiredFillWork.IntAdjuster(ref RequiredFillWork, 10, 10);
            _listing_Standard_RequiredFillWork.NewColumn();
            _listing_Standard_RequiredFillWork.IntAdjuster(ref RequiredFillWork, 100, 100);
            _listing_Standard_RequiredFillWork.NewColumn();
            _listing_Standard_RequiredFillWork.IntSetter(ref RequiredFillWork, 500, "Default");
            _listing_Standard_RequiredFillWork.End();
            
            listing_Standard.GapLine(12f);
            
            listing_Standard.Label("Allow Simultaneous Drilling:");
            listing_Standard.Gap(12f);
            listing_Standard.CheckboxLabeled("Allow Simultaneous Drilling", ref AllowSimultaneousDrilling, "True if you want to allow Multiple Drills at once.");
             listing_Standard.GapLine(12f);
            
            listing_Standard.End();


        }
    }
}

