using KSP.UI.Screens;
using KSPPluginFramework;
using System;

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

        static public void ToggleAppLauncherButton()
        {
            if (Instance == null)
            {
                LogFormatted_DebugOnly("Instance Is Null");
                return;
            }

            if (Instance.WindowVisibleByActiveScene)
                Instance.onAppLaunchToggleOff();
            else
                Instance.onAppLaunchToggleOn();
        }



        internal void onAppLaunchToggleOn()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOn");

            WindowVisibleByActiveScene = true;
            settings.Save();
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}", WindowVisibleByActiveScene);
        }
        internal void onAppLaunchToggleOff()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("TOff");

            WindowVisibleByActiveScene = false;
            settings.Save();
            MonoBehaviourExtended.LogFormatted_DebugOnly("{0}", WindowVisibleByActiveScene);
        }
        internal void onAppLaunchHoverOn()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOn");
            //MouseOverAppLauncherBtn = true;
        }
        internal void onAppLaunchHoverOff()
        {
            MonoBehaviourExtended.LogFormatted_DebugOnly("HovOff");
            //MouseOverAppLauncherBtn = false; 
        }

        //Boolean MouseOverAppLauncherBtn = false;
    }
}
