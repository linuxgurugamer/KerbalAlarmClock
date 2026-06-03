#if false

using KSP.Localization;
using KerbalAlarmClock;
using UnityEngine;

namespace YourModNamespace
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KACControlWindow : MonoBehaviour
    {
        private Rect windowRect = new Rect(200, 200, 250, 110);
        private const int WindowId = 8675310;

        private bool overrideStockToolbar = false;

        private void OnGUI()
        {
            windowRect = GUILayout.Window(
                WindowId,
                windowRect,
                DrawWindow,
                Localizer.Format("#LOC_KAC_517")
            );
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.BeginVertical();

            bool newValue = GUILayout.Toggle(
                overrideStockToolbar,
                Localizer.Format("#LOC_KAC_518")
            );

            if (newValue != overrideStockToolbar)
            {
                overrideStockToolbar = newValue;
                KACToolbarAPI.OverrideStockToolbar(overrideStockToolbar);
            }

            if (GUILayout.Button(Localizer.Format("#LOC_KAC_519"), GUILayout.Height(30)))
            {
                KerbalAlarmClock.KerbalAlarmClock.ToggleAppLauncherButton();
            }

            GUILayout.EndVertical();

            GUI.DragWindow();
        }
    

    private void Start()
        {
            //overrideStockToolbar = KACToolbarAPI.GetOverrideStockToolbar();
        }
    }
}

#endif
