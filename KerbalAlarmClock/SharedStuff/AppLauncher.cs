using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSP.UI;
using KSP.UI.Screens;
using KSPPluginFramework;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {

        internal Boolean AppLauncherToBeSetTrue = false;
        internal DateTime AppLauncherToBeSetTrueAttemptDate;
        internal void SetAppButtonToTrue()
        {
            if (!ApplicationLauncher.Ready)
            {
                LogFormatted_DebugOnly("not ready yet");
                AppLauncherToBeSetTrueAttemptDate = DateTime.Now;
                return;
            }
            //ApplicationLauncherButton ButtonToToggle = btnAppLauncher;

            if (btnToolbarControl == null)
            {
                LogFormatted_DebugOnly("Button Is Null");
                AppLauncherToBeSetTrueAttemptDate = DateTime.Now;
                return;
            }

            btnToolbarControl.SetTrue(true);
            {
                AppLauncherToBeSetTrue = false;
            }
        }


        void onAppLaunchToggleOn() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOn");

            WindowVisibleByActiveScene = true;
            settings.Save();
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}",WindowVisibleByActiveScene);
        }
        void onAppLaunchToggleOff() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOff");

            WindowVisibleByActiveScene = false;
            settings.Save();
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}", WindowVisibleByActiveScene);
        }
        void onAppLaunchHoverOn() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOn");
            //MouseOverAppLauncherBtn = true;
        }
        void onAppLaunchHoverOff() {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOff");
            //MouseOverAppLauncherBtn = false; 
        }

        //Boolean MouseOverAppLauncherBtn = false;
    }
}
