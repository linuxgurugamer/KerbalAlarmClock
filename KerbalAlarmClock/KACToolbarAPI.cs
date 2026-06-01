using KSPPluginFramework;

namespace KerbalAlarmClock
{
    public static class KACToolbarAPI
    {
        internal static bool buttonVisibility = true;
        public static void OverrideStockToolbar(bool overrideStock)
        {

            if (KerbalAlarmClock.Instance == null)
            {
                MonoBehaviourExtended.LogFormatted("KerbalAlarmClock, instance null");
                return;
            }

            if (overrideStock)
            {
                if (buttonVisibility)
                {
                    buttonVisibility = false;
                    KerbalAlarmClock.Instance.DestroyToolbarControllerButton(KerbalAlarmClock.btnToolbarControl);
                }
            }
            else
            {
                if (!buttonVisibility)
                {
                    buttonVisibility = true;
                    KerbalAlarmClock.btnToolbarControl = KerbalAlarmClock.Instance.InitToolbarControlButton();
                }
            }
        }

        public static void onAppLaunchToggleOn()
        {
            if (KerbalAlarmClock.Instance == null)
            {
                MonoBehaviourExtended.LogFormatted("KerbalAlarmClock, onAppLaunchToggleOn, instance null");
                return;
            }
            KerbalAlarmClock.Instance.onAppLaunchToggleOn();
        }
        public static void onAppLaunchToggleOff()
        {
            if (KerbalAlarmClock.Instance == null)
            {
                MonoBehaviourExtended.LogFormatted("KerbalAlarmClock, onAppLaunchToggleOff, instance null");
                return;
            }
            KerbalAlarmClock.Instance.onAppLaunchToggleOff();
        }
        public static void onAppLaunchHoverOn()
        {
            if (KerbalAlarmClock.Instance == null)
            {
                MonoBehaviourExtended.LogFormatted("KerbalAlarmClock, onAppLaunchHoverOn, instance null");
                return;
            }
            KerbalAlarmClock.Instance.onAppLaunchHoverOn();
        }
        public static void onAppLaunchHoverOff()
        {
            if (KerbalAlarmClock.Instance == null)
            {
                MonoBehaviourExtended.LogFormatted("KerbalAlarmClock, onAppLaunchHoverOff, instance null");
                return;
            }
            KerbalAlarmClock.Instance.onAppLaunchHoverOff();
        }


    }
}
