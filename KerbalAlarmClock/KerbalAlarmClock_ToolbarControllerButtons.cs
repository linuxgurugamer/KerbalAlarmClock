using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using KACToolbarWrapper;
using ToolbarControl_NS;
using UnityEngine;
using KSP.UI.Screens;

namespace KerbalAlarmClock
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(KerbalAlarmClock.MODID, KerbalAlarmClock.MODNAME);
        }
    }

    public partial class KerbalAlarmClock
    {
        internal ToolbarControl btnToolbarControl = null;

        internal const string MODID = "btnToolbarIcon";
        internal const string MODNAME = "Kerbal Alarm Clock";

        /// <summary>
        /// initialises a Toolbar Button for this mod
        /// </summary>
        /// <returns>The ToolbarButtonWrapper that was created</returns>
        internal ToolbarControl InitToolbarControlButton()
        {
            ToolbarControl btnReturn = null;
            try
            {
                LogFormatted("Initialising the ToolbarController Icon");
                if (btnToolbarControl == null)
                {
                    //         public void AddToAllToolbars(TC_ClickHandler onTrue, TC_ClickHandler onFalse, TC_ClickHandler onHover, TC_ClickHandler onHoverOut, TC_ClickHandler onEnable, TC_ClickHandler onDisable, ApplicationLauncher.AppScenes visibleInScenes, string nameSpace, string toolbarId, string largeToolbarIconActive, string largeToolbarIconInactive, string smallToolbarIconActive, string smallToolbarIconInactive, string toolTip = null);

                    btnReturn = gameObject.AddComponent<ToolbarControl>();
                    btnReturn.AddToAllToolbars(
                        onAppLaunchToggleOn, onAppLaunchToggleOff,
                        onAppLaunchHoverOn, onAppLaunchHoverOff,
                        null, null,
                        ApplicationLauncher.AppScenes.SPACECENTER,
                        MODID,
                        "kacBtn",
                        KACUtils.PathToolbarIcons + "KACIconBig-Norm.png",
                        KACUtils.PathToolbarTexturePath + "/KACIcon-Norm",
                        MODNAME
                    );
                }

            }
            catch (Exception ex)
            {
                DestroyToolbarControllerButton(btnReturn);
                LogFormatted("Error Initialising ToolbarController Button: {0}", ex.Message);
            }
            return btnReturn;
        }

        /// <summary>
        /// Destroys theToolbarButtonWrapper object
        /// </summary>
        /// <param name="btnToDestroy">Object to Destroy</param>
        internal void DestroyToolbarControllerButton(ToolbarControl btnToDestroy)
        {
            if (btnToDestroy != null)
            {
                LogFormatted("Destroying ToolbarController Button");
                btnToDestroy.OnDestroy();
                Destroy(btnToDestroy);
                btnToDestroy = null;

            }
            btnToDestroy = null;
        }

    }
}
