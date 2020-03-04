using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Jaxxa.EnhancedDevelopment.LaserDrill.Settings
{
        class Mod_Laser_Drill : Verse.Mod
        {

            public static ModSettings_LaserDrill Settings;

            public Mod_Laser_Drill(ModContentPack content) : base(content)
            {
            Mod_Laser_Drill.Settings = GetSettings<ModSettings_LaserDrill>();
            }

            public override string SettingsCategory()
            {
                return "ED-Laser Drill";
                //return base.SettingsCategory();
            }


            public override void DoSettingsWindowContents(Rect inRect)
            {
                Settings.DoSettingsWindowContents(inRect);
                //base.DoSettingsWindowContents(inRect);
            }

        }
    }

