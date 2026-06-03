using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KSP;
using UnityEngine;
using KSPPluginFramework;

using KACAPITester_KACWrapper;

namespace KerbalAlarmClock_APITester
{
    [KSPAddon(KSPAddon.Startup.Flight, false),
    WindowInitials(Visible = true, Caption = "#LOC_KAC_524", DragEnabled = true)]
    public class KACAPITester : MonoBehaviourWindow
    {
        internal override void Start()
        {
            LogFormatted("Start");
            KACWrapper.InitKACWrapper();

            //Register the event handler
            if (KACWrapper.APIReady)
                KACWrapper.KAC.onAlarmStateChanged += KAC_onAlarmStateChanged;
        }

        void KAC_onAlarmStateChanged(KACWrapper.KACAPI.AlarmStateChangedEventArgs e)
        {
            //output whats happened
            LogFormatted("{0}->{1}", e.alarm.Name, e.eventType);
        }


        internal override void Awake()
        {
            WindowRect = new Rect(600, 100, 300, 200);
        }

        internal override void OnDestroy()
        {
            //destroy the event hook
            KACWrapper.KAC.onAlarmStateChanged -= KAC_onAlarmStateChanged;
        }

        internal override void DrawWindow(int id)
        {
            GUILayout.Label(Localizer.Format("#LOC_KAC_525") + KACWrapper.AssemblyExists.ToString());
            GUILayout.Label(Localizer.Format("#LOC_KAC_526") + KACWrapper.InstanceExists.ToString());
            GUILayout.Label(Localizer.Format("#LOC_KAC_527") + KACWrapper.APIReady.ToString());

            //ifthe API hooked
            if (KACWrapper.APIReady)
            {
                //Draw the alarms
                foreach (KACWrapper.KACAPI.KACAlarm a in KACWrapper.KAC.Alarms)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Format("{0}" + "-" + "{1}" + "-" + "{2} " + "(" + "{3}" + ") -" + " {4}" + ":" + "{5}",a.Name, a.AlarmType,a.Notes,a.ID, a.RepeatAlarm,a.RepeatAlarmPeriod  ));
                    
                    //Option to delete each alarm
                    if (GUILayout.Button(Localizer.Format("#LOC_KAC_528"),GUILayout.Width(50))) {
                            KACWrapper.KAC.DeleteAlarm(a.ID);
                        }
                    GUILayout.EndHorizontal();
                }

                //option to create a new alarm
                if (GUILayout.Button(Localizer.Format("#LOC_KAC_529")))
                {
                    String aID = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.TransferModelled, Localizer.Format("#LOC_KAC_530"), Planetarium.GetUniversalTime() + 900);

                    KACWrapper.KAC.Alarms.First(z => z.ID == aID).Notes = Localizer.Format("#LOC_KAC_531");

                }
                if (GUILayout.Button(Localizer.Format("#LOC_KAC_532")))
                {
                    String aID = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.TransferModelled, Localizer.Format("#LOC_KAC_530"), Planetarium.GetUniversalTime() + 900);

                    KACWrapper.KAC.Alarms.First(z => z.ID == aID).Notes = Localizer.Format("#LOC_KAC_531");
                    KACWrapper.KAC.Alarms.First(z => z.ID == aID).AlarmMargin = 300;
                }

                GUILayout.BeginHorizontal();
                UT = GUILayout.TextField(UT);
                if (GUILayout.Button(Localizer.Format("#LOC_KAC_533")))
                {
                    KACWrapper.KAC.Alarms.First().AlarmTime = Convert.ToDouble(UT);
                }
                GUILayout.EndHorizontal();

                if (GUILayout.Button(Localizer.Format("#LOC_KAC_534")))
                {
                    String aID = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.TransferModelled, Localizer.Format("#LOC_KAC_530"), Planetarium.GetUniversalTime() + 900);

                    KACWrapper.KAC.Alarms.First(z => z.ID == aID).Notes = Localizer.Format("#LOC_KAC_531");
                    KACWrapper.KAC.Alarms.First(z => z.ID == aID).AlarmMargin = 600;
                    KACWrapper.KAC.Alarms.First(z => z.ID == aID).AlarmAction = KACWrapper.KACAPI.AlarmActionEnum.DoNothing;
                }
            }
        }
        String UT="";
    }
}
