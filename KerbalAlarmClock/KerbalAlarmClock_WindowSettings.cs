using KSP.Localization;
using KSP.UI.Screens;
using KSPPluginFramework;
using System;
using System.Linq;
using UnityEngine;

namespace KerbalAlarmClock
{
    public partial class KerbalAlarmClock
    {
        private Int32 intSettingsTab = 0;
        private Int32 intSettingsHeight = 334;

        private Int32 intAlarmDefaultsBoxheight = 105;
        private Int32 intUpdateBoxheight = 116;
        private Int32 intSOIBoxheight = 178; //166;

        internal KACTimeStringArray timeDefaultMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeAutoManNodeThreshold = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        private KACTimeStringArray timeQuickManNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeQuickSOIMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeQuickNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        private KACTimeStringArray timeContractExpireMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        private KACTimeStringArray timeContractDeadlineMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        //private KACTimeStringArray timeQuickApNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickPeNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickANNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);
        //private KACTimeStringArray timeQuickDNNodeMargin = new KACTimeStringArray(KACTimeStringArray.TimeEntryPrecisionEnum.Hours);

        private void NewSettingsWindow()
        {
            if (settings.VersionAttentionFlag)
            {
                intSettingsTab = 2;
            }
            else
            {
                intSettingsTab = 0;
            }

            //reset the flag
            settings.VersionAttentionFlag = false;

            //work out the correct kerbaltime values
            timeDefaultMargin.BuildFromUT(settings.AlarmDefaultMargin);
            timeAutoSOIMargin.BuildFromUT(settings.AlarmAutoSOIMargin);
            timeAutoManNodeMargin.BuildFromUT(settings.AlarmAddManAutoMargin);
            timeAutoManNodeThreshold.BuildFromUT(settings.AlarmAddManAutoThreshold);

            timeQuickManNodeMargin.BuildFromUT(settings.AlarmAddManQuickMargin);
            timeQuickSOIMargin.BuildFromUT(settings.AlarmAddSOIQuickMargin);
            timeQuickNodeMargin.BuildFromUT(settings.AlarmAddNodeQuickMargin);

            timeContractExpireMargin.BuildFromUT(settings.AlarmOnContractExpireMargin);
            timeContractDeadlineMargin.BuildFromUT(settings.AlarmOnContractDeadlineMargin);

            //timeQuickApNodeMargin.BuildFromUT(settings.AlarmAddApQuickMargin);
            //timeQuickPeNodeMargin.BuildFromUT(settings.AlarmAddPeQuickMargin);
            //timeQuickANNodeMargin.BuildFromUT(settings.AlarmAddANQuickMargin);
            //timeQuickDNNodeMargin.BuildFromUT(settings.AlarmAddDNQuickMargin);

        }

        internal void FillSettingsWindow(int WindowID)
        {
            strAlarmDescSOI = String.Format(strAlarmDescSOI, settings.AlarmAddSOIAutoThreshold.ToString());
            strAlarmDescXfer = String.Format(strAlarmDescXfer, settings.AlarmXferRecalcThreshold.ToString());
            strAlarmDescNode = String.Format(strAlarmDescNode, settings.AlarmNodeRecalcThreshold.ToString());
            strAlarmDescMan = String.Format(strAlarmDescMan, settings.AlarmAddManAutoThreshold.ToString());

            GUILayout.BeginVertical();

            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "Sounds", "About" };
            //String[] strSettingsTabs = new String[] { "All Alarms", "Specific Types", "About" };
            GUIContent[] contSettingsTabs = new GUIContent[]
            {
                new GUIContent(Localizer.Format("#LOC_KAC_347"),Localizer.Format("#LOC_KAC_348")), 
                //new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                //new GUIContent("Specifics-2","Man Node Specific Settings"), 
                //new GUIContent("Alarm Settings","Specific Settings for Alarm Types"), 
                new GUIContent(Localizer.Format("#LOC_KAC_349"),Localizer.Format("#LOC_KAC_350")),
                new GUIContent(Localizer.Format("#LOC_KAC_351"),Localizer.Format("#LOC_KAC_352")),
                new GUIContent(Localizer.Format("#LOC_KAC_353"), Localizer.Format("#LOC_KAC_354")),
                new GUIContent(Localizer.Format("#LOC_KAC_355"), Localizer.Format("#LOC_KAC_356")),
                new GUIContent(Localizer.Format("#LOC_KAC_357"))
            };
            GUIContent[] contSettingsTabsNewVersion = new GUIContent[]
            {
                new GUIContent(Localizer.Format("#LOC_KAC_358"),Localizer.Format("#LOC_KAC_348")), 
                //new GUIContent("Specifics-1","SOI, Ap, Pe, AN, DN Specific Settings" ), 
                //new GUIContent("Specifics-2","Man Node Specific Settings"), 
                //new GUIContent("Alarm Specifics","Specific Settings for Alarm Types"), 
                new GUIContent(Localizer.Format("#LOC_KAC_349"),Localizer.Format("#LOC_KAC_350")),
                new GUIContent(Localizer.Format("#LOC_KAC_351"),Localizer.Format("#LOC_KAC_352")),
                new GUIContent(Localizer.Format("#LOC_KAC_353"), Localizer.Format("#LOC_KAC_354")),
                new GUIContent(Localizer.Format("#LOC_KAC_355"), Localizer.Format("#LOC_KAC_356")),
                new GUIContent(Localizer.Format("#LOC_KAC_359"), KACResources.btnSettingsAttention)
            };

            GUIContent[] conTabstoShow = contSettingsTabs;
            if (settings.VersionAvailable) conTabstoShow = contSettingsTabsNewVersion;
            intSettingsTab = GUILayout.Toolbar(intSettingsTab, conTabstoShow, KACResources.styleButton);

            switch (intSettingsTab)
            {
                case 0:
                    WindowLayout_SettingsGlobal();
                    intSettingsHeight = 620;// 591; // 567;// 514; //462; //463; //434;// 572;//542;
                    break;
                //case 1:
                //    WindowLayout_SettingsSpecifics1();
                //    intSettingsHeight = 422;//600; //513;// 374;
                //    break;
                //case 2:
                //    WindowLayout_SettingsSpecifics2();
                //    intSettingsHeight = 354 ;//600; //513;// 374;
                //    break;
                case 1:
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(Localizer.Format("#LOC_KAC_360"), KACResources.styleAddHeading, GUILayout.Width(120));
                    ddlSettingsAlarmSpecs.DrawButton();
                    GUILayout.EndHorizontal();
                    switch (SettingsAlarmSpecSelected)
                    {
                        case SettingsAlarmSpecsEnum.Default:
                            WindowLayout_SettingsSpecifics_Default();
                            intSettingsHeight = 221; // 234;
                            break;
                        case SettingsAlarmSpecsEnum.WarpTo:
                            WindowLayout_SettingsSpecifics_WarpTo();
                            intSettingsHeight = 477; // 453; // 419;//  395;//221; //318;
                            break;
                        case SettingsAlarmSpecsEnum.ManNode:
                            WindowLayout_SettingsSpecifics_ManNode();
                            intSettingsHeight = 437;// 387; //318;
                            break;
                        case SettingsAlarmSpecsEnum.SOI:
                            WindowLayout_SettingsSpecifics_SOI();
                            intSettingsHeight = 362;// 367; // 358; //288;
                            break;
                        case SettingsAlarmSpecsEnum.Contract:
                            WindowLayout_SettingsSpecifics_Contract();
                            intSettingsHeight = 400;
                            break;
                        case SettingsAlarmSpecsEnum.Other:
                            WindowLayout_SettingsSpecifics_Other();
                            intSettingsHeight = 342; //270;
                            break;
                        default:
                            WindowLayout_SettingsSpecifics_Default();
                            intSettingsHeight = 221; //234;
                            break;
                    }
                    break;
                case 2:
                    WindowLayout_SettingsAudio();
                    intSettingsHeight = 543;
                    break;
                case 3:
                    WindowLayout_SettingsIcons();
                    intSettingsHeight = 518; //  509; //518;//466 //406;
                    break;
                case 4:
                    WindowLayout_SettingsCalendar();
                    intSettingsHeight = 226;
                    break;
                case 5:
                    WindowLayout_SettingsAbout();
                    intSettingsHeight = 300; // 350; // 294; //306;
                    break;

                default:
                    break;
            }
            //if (settings.SelectedSkin!= Settings.DisplaySkin.Default)
            //    intSettingsHeight -= intTestheight;
            GUILayout.EndVertical();

            SetTooltipText();
        }

        private void WindowLayout_SettingsGlobal()
        {
            //Styles
            GUILayout.Label(Localizer.Format("#LOC_KAC_361"), KACResources.styleAddSectionHeading);
            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas))
            {

                //two columns
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_362"), KACResources.styleAddHeading, GUILayout.Width(90));
                    ddlSettingsSkin.DrawButton();
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_363"), KACResources.styleAddHeading, GUILayout.Width(90));
#if true
                    ddlSettingsButtonStyle.DrawButton();
#endif
                }

                if (DrawCheckbox(ref settings.WindowChildPosBelow, Localizer.Format("#LOC_KAC_364")))
                    settings.Save();
            }
            //if (settings.SelectedSkin == Settings.DisplaySkin.Default) GUILayout.Space(38);
            //Preferences
            GUILayout.Label(Localizer.Format("#LOC_KAC_365"), KACResources.styleAddSectionHeading);

            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas))
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (DrawTextBox(ref settings.AlarmListMaxAlarms, KACResources.styleAddField, GUILayout.Width(45)))
                        settings.Save();
                    GUILayout.Label(Localizer.Format("#LOC_KAC_366"), KACResources.styleAddHeading);
                }
                using (new GUILayout.HorizontalScope())
                {
                    if (DrawTextBox(ref settings.MaxToolTipTime, KACResources.styleAddField, GUILayout.Width(45)))
                        settings.Save();
                    GUILayout.Label(Localizer.Format("#LOC_KAC_367"), KACResources.styleAddHeading);
                }
                if (DrawCheckbox(ref settings.HideOnPause, Localizer.Format("#LOC_KAC_368")))
                    settings.Save();

                if (DrawCheckbox(ref settings.ShowTooltips, Localizer.Format("#LOC_KAC_369")))
                    settings.Save();

                if (DrawCheckbox(ref settings.KillWarpOnThrottleCutOffKeystroke, Localizer.Format("#LOC_KAC_370")))
                    settings.Save();

                int intTimeFormat = (int)settings.DateTimeFormat;
                if (intTimeFormat > 1) intTimeFormat--;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_371"), KACResources.styleAddHeading, GUILayout.Width(90));
                    if (DrawRadioList(ref intTimeFormat, new String[] { Localizer.Format("#LOC_KAC_372"), Localizer.Format("#LOC_KAC_373"), Localizer.Format("#LOC_KAC_374") }))
                    {
                        if (intTimeFormat > 0) intTimeFormat++;
                        settings.DateTimeFormat = (DateStringFormatsEnum)intTimeFormat;
                        settings.Save();
                    }
                }
            }

            GUILayout.Label(Localizer.Format("#LOC_KAC_375"), KACResources.styleAddSectionHeading);


            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas))
            {
                if (DrawCheckbox(ref settings.ConfirmAlarmDeletes, Localizer.Format("#LOC_KAC_376")))
                    settings.Save();

                if (DrawCheckbox(ref settings.AllowJumpFromViewOnly, Localizer.Format("#LOC_KAC_377")))
                    settings.Save();

                if (DrawCheckbox(ref settings.AllowJumpToAsteroid, Localizer.Format("#LOC_KAC_378")))
                    settings.Save();

                //if (DrawCheckbox(ref Settings.TimeAsUT, "Display Times as UT (instead of Date/Time)"))
                //    Settings.Save();

            }
            GUIContent Saveheader = new GUIContent(Localizer.Format("#LOC_KAC_379"), Localizer.Format("#LOC_KAC_380"));
            GUILayout.Label(Saveheader, KACResources.styleAddSectionHeading);

            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas, GUILayout.Height(64)))
            {
                if (DrawCheckbox(ref settings.BackupSaves, Localizer.Format("#LOC_KAC_381")))
                    settings.Save();

                if (DrawCheckbox(ref settings.CancelFlightModeJumpOnBackupFailure, Localizer.Format("#LOC_KAC_382")))
                    settings.Save();

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_383"), KACResources.styleAddHeading, GUILayout.Width(110));
                    GUILayout.Label(settings.BackupSavesToKeep.ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
                    settings.BackupSavesToKeep = (int)Math.Floor(GUILayout.HorizontalSlider((float)settings.BackupSavesToKeep, 3, 50));
                }
            }

            GUILayout.Label(Localizer.Format("#LOC_KAC_384"), KACResources.styleAddSectionHeading);

            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_385"), KACResources.styleAddHeading, GUILayout.Width(100));
                    ddlChecksPerSec.DrawButton();
                }

                if (DrawCheckbox(ref settings.WarpTransitions_Instant, new GUIContent(Localizer.Format("#LOC_KAC_386"), Localizer.Format("#LOC_KAC_387"))))
                    settings.Save();

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(new GUIContent(Localizer.Format("#LOC_KAC_388"), Localizer.Format("#LOC_KAC_389")),
                    KACResources.styleAddHeading, GUILayout.Width(115)); //110
                    GUILayout.Label(settings.WarpTransitions_UTToRateTimesOneTenths.ToString(), KACResources.styleAddXferName, GUILayout.Width(25));
                    Int32 intReturn = (Int32)Math.Floor(GUILayout.HorizontalSlider((float)settings.WarpTransitions_UTToRateTimesOneTenths, 10, 50));
                    if (intReturn != settings.WarpTransitions_UTToRateTimesOneTenths)
                    {
                        settings.WarpTransitions_UTToRateTimesOneTenths = intReturn;
                        settings.Save();
                    }
                    if (GUILayout.Button(Localizer.Format("#LOC_KAC_390"), GUILayout.Height(16), GUILayout.Width(40)))
                    {
                        settings.WarpTransitions_UTToRateTimesOneTenths = 15;
                        settings.Save();
                    }
                }
            }

        }

        private void WindowLayout_SettingsSpecifics_Default()
        {
            GUILayout.Label(Localizer.Format("#LOC_KAC_391"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intAlarmDefaultsBoxheight));

            //Alarm position
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_KAC_392"), KACResources.styleAddHeading, GUILayout.Width(90));
            if (DrawRadioList(ref settings.AlarmPosition, Localizer.Format("#LOC_KAC_393"), Localizer.Format("#LOC_KAC_394"), Localizer.Format("#LOC_KAC_395")))
            {
                settings.Save();
            }
            GUILayout.EndHorizontal();

            //Default Alarm Action
            if (DrawAlarmActionChoice4(ref settings.AlarmDefaultAction, Localizer.Format("#LOC_KAC_396"), 108))
                settings.Save();

            if (DrawTimeEntry(ref timeDefaultMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_397"), 100))
            {
                //convert it and save it in the settings
                settings.AlarmDefaultMargin = timeDefaultMargin.UT;
                settings.Save();
            }
            //if (DrawCheckbox(ref settings.AlarmDeleteOnClose, "Delete Alarm On Close"))
            //    settings.Save();

            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsSpecifics_WarpTo()
        {
            GUILayout.Label(Localizer.Format("#LOC_KAC_398"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.WarpToEnabled, new GUIContent(Localizer.Format("#LOC_KAC_399"), Localizer.Format("#LOC_KAC_400"))))
            {
                settings.Save();
            }
            if (DrawCheckbox(ref settings.WarpToRequiresConfirm, new GUIContent(Localizer.Format("#LOC_KAC_401"), Localizer.Format("#LOC_KAC_402"))))
            {
                settings.Save();
            }
            if (DrawCheckbox(ref settings.WarpToTipsHidden, new GUIContent(Localizer.Format("#LOC_KAC_403"), Localizer.Format("#LOC_KAC_404"))))
            {
                settings.Save();
            }
            if (DrawCheckbox(ref settings.WarpToHideWhenManGizmoShown, new GUIContent(Localizer.Format("#LOC_KAC_405"), Localizer.Format("#LOC_KAC_406"))))
            {
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            String strTemp = settings.WarpToDupeProximitySecs.ToString("0");
            if (DrawTextBox(ref strTemp, KACResources.styleAddField, GUILayout.Width(45)))
            {
                try
                {
                    settings.WarpToDupeProximitySecs = Convert.ToInt32(strTemp);
                    settings.Save();
                }
                catch (Exception)
                {

                }
            }
            GUILayout.Label(Localizer.Format("#LOC_KAC_407"), KACResources.styleAddHeading);
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (DrawToggle(ref settings.WarpToLimitMaxWarp, Localizer.Format("#LOC_KAC_408"), KACResources.styleCheckbox))
                settings.Save();

            if (settings.WarpToLimitMaxWarp)
            {
                GUILayout.Space(200);
                strTemp = settings.WarpToMaxWarp.ToString("0");
                if (DrawTextField(ref strTemp, "\\d+", false, "Limit:", 80, 0)) // NO_LOCALIZATION
                {
                    settings.WarpToMaxWarp = Convert.ToInt32(strTemp);
                    settings.Save();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Label(Localizer.Format("#LOC_KAC_409"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            DrawWarpToMarginCheck(ref settings.WarpToAddMarginAp, Localizer.Format("#LOC_KAC_4"), Localizer.Format("#LOC_KAC_4"), KACResources.iconAp);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginPe, Localizer.Format("#LOC_KAC_5"), Localizer.Format("#LOC_KAC_5"), KACResources.iconPe);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginAN, Localizer.Format("#LOC_KAC_8"), Localizer.Format("#LOC_KAC_8"), KACResources.iconAN);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginDN, Localizer.Format("#LOC_KAC_9"), Localizer.Format("#LOC_KAC_9"), KACResources.iconDN);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginSOI, Localizer.Format("#LOC_KAC_6"), Localizer.Format("#LOC_KAC_6"), KACResources.iconSOI);
            DrawWarpToMarginCheck(ref settings.WarpToAddMarginManNode, Localizer.Format("#LOC_KAC_410"), Localizer.Format("#LOC_KAC_140"), KACResources.iconMNode);

            GUILayout.EndVertical();
        }

        private void DrawWarpToMarginCheck(ref Boolean settingsBool, String ShortName, String LongName, Texture2D icon)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(icon, GUILayout.Width(20));
            if (DrawCheckbox(ref settingsBool, new GUIContent(Localizer.Format("#LOC_KAC_411") + ShortName + Localizer.Format("#LOC_KAC_412"), Localizer.Format("#LOC_KAC_413") + LongName + Localizer.Format("#LOC_KAC_414"))))
                settings.Save();
            GUILayout.EndHorizontal();
        }

        private void WindowLayout_SettingsSpecifics_ManNode()
        {
            GUILayout.Label(Localizer.Format("#LOC_KAC_415"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(207)); //155
            if (DrawCheckbox(ref settings.AlarmAddManAuto, new GUIContent(Localizer.Format("#LOC_KAC_416"), strAlarmDescMan)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                //if (Settings.AlarmAddManAuto)
                //{
                //    //RecalcManNodeAlarms(true);
                //}
            }
            if (settings.AlarmAddManAuto)
            {
                if (DrawCheckbox(ref settings.AlarmAddManAuto_andRemove, new GUIContent(Localizer.Format("#LOC_KAC_417"))))
                {
                    settings.Save();
                }
                GUILayout.Label(Localizer.Format("#LOC_KAC_418"), KACResources.styleAddHeading);
                if (DrawTimeEntry(ref timeAutoManNodeThreshold, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_419"), 100))
                {
                    //convert it and save it in the settings
                    settings.AlarmAddManAutoThreshold = timeAutoManNodeThreshold.UT;
                    settings.Save();
                }

                GUILayout.Label(Localizer.Format("#LOC_KAC_420"), KACResources.styleAddSectionHeading);
                if (DrawAlarmActionChoice4(ref settings.AlarmAddManAuto_Action, Localizer.Format("#LOC_KAC_421"), 108))
                {
                    settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoManNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_77"), 100))
                {
                    //convert it and save it in the settings
                    settings.AlarmAddManAutoMargin = timeAutoManNodeMargin.UT;
                    settings.Save();
                }

            }
            GUILayout.EndVertical();

            GUILayout.Label(Localizer.Format("#LOC_KAC_422"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawAlarmActionChoice4(ref settings.AlarmAddManQuickAction, Localizer.Format("#LOC_KAC_423"), 108))
                settings.Save();

            if (DrawTimeEntry(ref timeQuickManNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_424"), 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddManQuickMargin = timeQuickManNodeMargin.UT;
                settings.Save();
            }
            GUILayout.EndVertical();
            GUILayout.Label(Localizer.Format("#LOC_KAC_425"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            GUILayout.BeginHorizontal();

            GUILayout.Label(Localizer.Format("#LOC_KAC_426"), KACResources.styleAddHeading);
            ddlSettingsKERNodeMargin.DrawButton();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
        private void WindowLayout_SettingsSpecifics_SOI()
        {
            //Sphere of Influence Stuff
            GUILayout.Label(Localizer.Format("#LOC_KAC_427"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intSOIBoxheight));

            if (DrawCheckbox(ref settings.AlarmSOIRecalc, new GUIContent(Localizer.Format("#LOC_KAC_428"))))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (settings.AlarmSOIRecalc)
                {
                    RecalcSOIAlarmTimes(true);
                }
            }

            if (DrawCheckbox(ref settings.AlarmAddSOIAuto, new GUIContent(Localizer.Format("#LOC_KAC_429"), strAlarmDescSOI)))
                settings.Save();
            //if (!settings.AlarmAddSOIAuto)
            //    settings.AlarmCatchSOIChange = false;
            if (settings.AlarmAddSOIAuto)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox(ref settings.AlarmAddSOIAuto_ExcludeEVA, new GUIContent(Localizer.Format("#LOC_KAC_430"), Localizer.Format("#LOC_KAC_431"))))
                    settings.Save();
                GUILayout.EndHorizontal();
                GUILayout.Space(-5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                if (DrawCheckbox(ref settings.AlarmAddSOIAuto_ExcludeDebris, new GUIContent(Localizer.Format("#LOC_KAC_432"), Localizer.Format("#LOC_KAC_433"))))
                    settings.Save();
                GUILayout.EndHorizontal();
                //GUILayout.BeginHorizontal();
                //GUILayout.Space(20);
                //if (DrawCheckbox(ref settings.AlarmCatchSOIChange, new GUIContent("Throw alarm on background SOI Change", "This will throw an alarm whenever the name of the body a ship is orbiting changes.\r\n\r\nIt wont slow time as this approaches, just a big hammer in case we never looked at the flight path before it happened")))
                //    settings.Save();
                //GUILayout.EndHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_KAC_434"), KACResources.styleAddSectionHeading);
                if (DrawAlarmActionChoice4(ref settings.AlarmOnSOIChange_Action, Localizer.Format("#LOC_KAC_421"), 108))
                {
                    settings.Save();
                }
                if (DrawTimeEntry(ref timeAutoSOIMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_77"), 100))
                {
                    //convert it and save it in the settings
                    settings.AlarmAutoSOIMargin = timeAutoSOIMargin.UT;
                    settings.Save();
                }

            }
            GUILayout.EndVertical();

            GUILayout.Label(Localizer.Format("#LOC_KAC_435"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawAlarmActionChoice4(ref settings.AlarmAddSOIQuickAction, Localizer.Format("#LOC_KAC_423"), 108))
                settings.Save();

            if (DrawTimeEntry(ref timeQuickSOIMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_424"), 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddSOIQuickMargin = timeQuickSOIMargin.UT;
                settings.Save();
            }
            GUILayout.EndVertical();

        }

        private void WindowLayout_SettingsSpecifics_Contract()
        {
            GUILayout.Label(Localizer.Format("#LOC_KAC_436"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawAlarmActionChoice4(ref settings.AlarmOnContractDeadline_Action, Localizer.Format("#LOC_KAC_421"), 108))
            {
                settings.Save();
            }
            if (DrawTimeEntry(ref timeContractDeadlineMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Days, Localizer.Format("#LOC_KAC_77"), 100))
            {
                //convert it and save it in the settings
                settings.AlarmOnContractDeadlineMargin = timeContractDeadlineMargin.UT;
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_KAC_437"));
            ddlSettingsContractAutoActive.DrawButton();
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.ContractDeadlineDontCreateInsideMargin, Localizer.Format("#LOC_KAC_438")))
                settings.Save();
            if (DrawCheckbox(ref settings.ContractDeadlineDelete, Localizer.Format("#LOC_KAC_439")))
                settings.Save();

            GUILayout.EndVertical();


            GUILayout.Label(Localizer.Format("#LOC_KAC_440"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawAlarmActionChoice4(ref settings.AlarmOnContractExpire_Action, Localizer.Format("#LOC_KAC_421"), 108))
            {
                settings.Save();
            }
            if (DrawTimeEntry(ref timeContractExpireMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Days, Localizer.Format("#LOC_KAC_77"), 100))
            {
                //convert it and save it in the settings
                settings.AlarmOnContractExpireMargin = timeContractExpireMargin.UT;
                settings.Save();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_KAC_441"));
            ddlSettingsContractAutoOffered.DrawButton();
            GUILayout.EndHorizontal();

            if (DrawCheckbox(ref settings.ContractExpireDontCreateInsideMargin, Localizer.Format("#LOC_KAC_438")))
                settings.Save();
            if (DrawCheckbox(ref settings.ContractExpireDelete, Localizer.Format("#LOC_KAC_442")))
                settings.Save();

            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsSpecifics_Other()
        {
            //Crew Alarm Stuff
            GUILayout.Label(Localizer.Format("#LOC_KAC_443"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            if (DrawCheckbox(ref settings.AlarmCrewDefaultStoreNode, new GUIContent(Localizer.Format("#LOC_KAC_167"))))
            {
                settings.Save();
            }

            GUILayout.EndVertical();

            //Node Alarm Stuff
            GUILayout.Label(Localizer.Format("#LOC_KAC_444"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.AlarmNodeRecalc, new GUIContent(Localizer.Format("#LOC_KAC_445"), strAlarmDescNode)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (settings.AlarmNodeRecalc)
                {
                    RecalcNodeAlarmTimes(true);
                }
            }

            GUILayout.Label(Localizer.Format("#LOC_KAC_446"), KACResources.styleAddSectionHeading);

            if (DrawAlarmActionChoice4(ref settings.AlarmAddNodeQuickAction, "Quick Action:", 108))
                settings.Save();

            if (DrawTimeEntry(ref timeQuickNodeMargin, KACTimeStringArray.TimeEntryPrecisionEnum.Hours, Localizer.Format("#LOC_KAC_424"), 100))
            {
                //convert it and save it in the settings
                settings.AlarmAddNodeQuickMargin = timeQuickNodeMargin.UT;
                settings.Save();
            }

            GUILayout.EndVertical();

            //Transfer Alarm Stuff
            GUILayout.Label(Localizer.Format("#LOC_KAC_447"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            if (DrawCheckbox(ref settings.AlarmXferRecalc, new GUIContent(Localizer.Format("#LOC_KAC_448"), strAlarmDescXfer)))
            {
                settings.Save();
                //if it was turned on then force a recalc regardless of the gap
                if (settings.AlarmXferRecalc)
                {
                    RecalcTransferAlarmTimes(true);
                }
            }
            GUILayout.EndVertical();
        }

        private void WindowLayout_SettingsAudio()
        {
            GUILayout.Label(Localizer.Format("#LOC_KAC_348"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //Columns
            GUILayout.BeginHorizontal();

            //Column1
            GUILayout.BeginVertical(GUILayout.Width(70));
            GUILayout.Space(0);
            GUILayout.Label(Localizer.Format("#LOC_KAC_449"), KACResources.styleAddSectionHeading);
            GUILayout.Space(4);
            GUILayout.Label(Localizer.Format("#LOC_KAC_450"), KACResources.styleAddHeading);
            GUILayout.EndVertical();

            //Column2
            GUILayout.BeginVertical();
            GUILayout.Space(-5);
            if (DrawToggle(ref settings.AlarmsVolumeFromUI, Localizer.Format("#LOC_KAC_451"), KACResources.styleCheckbox))
            {
                settings.Save();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (settings.AlarmsVolumeFromUI)
                GUILayout.HorizontalSlider((Int32)(GameSettings.UI_VOLUME * 100), 0, 100, GUILayout.Width(160));
            else
                settings.AlarmsVolume = GUILayout.HorizontalSlider(settings.AlarmsVolume * 100, 0, 100, GUILayout.Width(160)) / 100;
            GUIStyle stylePct = new GUIStyle(KACResources.styleAddHeading);
            stylePct.padding.top = -2;
            GUILayout.Label(KerbalAlarmClock.audioController.VolumePct.ToString() + "%", stylePct);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //End Columns
            GUILayout.EndHorizontal();

            //Draw Raw Sound
            AlarmSound raw = settings.AlarmSounds.First(s => s.Name == Localizer.Format("#LOC_KAC_452"));
            DrawSoundLine(ref raw, true);
            GUILayout.EndVertical();

            GUILayout.Label(Localizer.Format("#LOC_KAC_453"), KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.Label(Localizer.Format("#LOC_KAC_454"), KACResources.styleAddHeading);

            for (int i = 0; i < settings.AlarmSounds.Count - 1; i++)
            {
                AlarmSound sound = settings.AlarmSounds.Where(s => s.Name != Localizer.Format("#LOC_KAC_452")).ElementAt(i);
                DrawSoundLine(ref sound);
            }

            GUILayout.EndVertical();

        }

        private void DrawSoundLine(ref AlarmSound sound, Boolean HideCheck = false)
        {
            GUILayout.BeginHorizontal();

            if (HideCheck)
            {
                GUILayout.Label("     " + sound.Name, KACResources.styleCheckboxLabel, GUILayout.Width(100));
            }
            else
            {
                if (DrawToggle(ref sound.Enabled, sound.Name, KACResources.styleCheckbox, GUILayout.Width(100)))
                {
                    settings.Save();
                }
            }
            sound.ddl.DrawButton();

            if (KACResources.clipAlarms.ContainsKey(sound.SoundName))
                DrawTestSoundButton(KACResources.clipAlarms[sound.SoundName], sound.RepeatCount);
            else
                DrawTestSoundButton(null, sound.RepeatCount);

            GUILayout.Label(new GUIContent(Localizer.Format("#LOC_KAC_455"), Localizer.Format("#LOC_KAC_456")), KACResources.styleAddHeading, GUILayout.Width(14));
            //sound.RepeatCount = (Int32)GUILayout.HorizontalSlider(sound.RepeatCount, 1, 6, GUILayout.Width(intTestheight3));
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(8);
            if (DrawHorizontalSlider(ref sound.RepeatCount, 1, 6, GUILayout.Width(60)))
            {
                settings.Save();
            }
            GUILayout.EndVertical();
            GUILayout.Space(3);
            GUILayout.Label(sound.RepeatCount < 6 ? sound.RepeatCount.ToString() : Localizer.Format("#LOC_KAC_457"), KACResources.styleAddHeading, GUILayout.Width(14));

            GUILayout.EndHorizontal();
        }

        private void WindowLayout_SettingsIcons()
        {
            Boolean blnTemp = false;

            //GUILayout.Label("Common Toolbar Integration (By Blizzy78)", KACResources.styleAddSectionHeading);
            //GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

            //if (BlizzyToolbarIsAvailable)
            //{
            //    if (DrawCheckbox(ref settings.UseBlizzyToolbarIfAvailable, "Use Toolbar Button instead of KAC Button"))
            //    {
            //        DestroyToolbarButton(btnToolbarKAC);
            //        if (settings.UseBlizzyToolbarIfAvailable) InitToolbarButton();
            //        settings.Save();
            //    }
            //}
            //else
            //{
            //    GUILayout.BeginHorizontal();
            //    GUILayout.Label("Get the Common Toolbar:", KACResources.styleAddHeading);
            //    GUILayout.FlexibleSpace();
            //    if (GUILayout.Button("Click here", KACResources.styleContent))
            //        Application.OpenURL("https://forum.kerbalspaceprogram.com/topic/161857-15-toolbar-continued-common-api-for-draggableresizable-buttons-toolbar/");
            //    GUILayout.EndHorizontal();
            //}
            //GUILayout.EndVertical();

            GUILayout.Label(Localizer.Format("#LOC_KAC_458"), KACResources.styleAddSectionHeading);
            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas))
            {
                using (new GUILayout.HorizontalScope())
                {
                    bool b = settings.WindowRememberLastOpenStatus;
                    if (DrawCheckbox(ref settings.WindowRememberLastOpenStatus, new GUIContent(Localizer.Format("#LOC_KAC_459"), Localizer.Format("#LOC_KAC_460"))))
                    {
                        if (b != settings.WindowRememberLastOpenStatus)
                        {
                            settings.Save();
                        }
                    }
                }
            }

            int MinimalDisplayChoice = (int)settings.WindowMinimizedType;
            GUILayout.Label(Localizer.Format("#LOC_KAC_461"), KACResources.styleAddSectionHeading);
            using (new GUILayout.VerticalScope(KACResources.styleAddFieldAreas))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_462"), KACResources.styleAddHeading, GUILayout.Width(120));
                    if (DrawRadioList(ref MinimalDisplayChoice, Localizer.Format("#LOC_KAC_463"), Localizer.Format("#LOC_KAC_464")))
                    {
                        settings.WindowMinimizedType = (MiminalDisplayType)MinimalDisplayChoice;
                        settings.Save();
                    }
                }
            }

            DrawIconPos(Localizer.Format("#LOC_KAC_465"), ApplicationLauncher.AppScenes.FLIGHT, false, ref blnTemp, ref settings.IconPos, ref settings.WindowVisible, ref settings.ClickThroughProtect_Flight);

            DrawIconPos(Localizer.Format("#LOC_KAC_466"), ApplicationLauncher.AppScenes.SPACECENTER, true, ref settings.IconShow_SpaceCenter, ref settings.IconPos_SpaceCenter, ref settings.WindowVisible_SpaceCenter, ref settings.ClickThroughProtect_KSC);

            DrawIconPos(Localizer.Format("#LOC_KAC_467"), ApplicationLauncher.AppScenes.TRACKSTATION, true, ref settings.IconShow_TrackingStation, ref settings.IconPos_TrackingStation, ref settings.WindowVisible_TrackingStation, ref settings.ClickThroughProtect_Tracking);

            DrawIconPos(Localizer.Format("#LOC_KAC_468"), ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH, true, ref settings.IconShow_EditorVAB, ref settings.IconPos_EditorVAB, ref settings.WindowVisible_EditorVAB, ref settings.ClickThroughProtect_Editor);

        }

        private void DrawIconPos(String Title, ApplicationLauncher.AppScenes scene, Boolean Toggleable, ref Boolean IconShow, ref Rect IconPos, ref Boolean WindowVisible, ref Boolean ClickThroughProtect)
        {
            GUILayout.Label(Title, KACResources.styleAddSectionHeading);
            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            //Checkbox to show/hide
            if (Toggleable)
            {
                if (DrawCheckbox(ref IconShow, new GUIContent(Localizer.Format("#LOC_KAC_469"), Localizer.Format("#LOC_KAC_470"))))
                {
                    WindowVisible = IconShow;
                    //DestroyToolbarButton(btnToolbarKAC);
                    //if (settings.UseBlizzyToolbarIfAvailable) InitToolbarButton();
                    settings.Save();

                    KerbalAlarmClock.ChangeSceneVisibility(scene, IconShow);
                    DestroyToolbarControllerButton(btnToolbarControl);
                    btnToolbarControl = InitToolbarControlButton();
                }
            }

            GUILayout.Label(Localizer.Format("#LOC_KAC_471"), KACResources.styleAddSectionHeading);
            //Now two columns
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_KAC_472"), KACResources.styleAddHeading);
            GUILayout.Label(string.Format("{0}", Math.Floor((IconPos.xMin)).ToString()), KACResources.styleAddXferName, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            IconPos.xMin = Convert.ToInt32(Math.Floor(GUILayout.HorizontalSlider(IconPos.xMin, 0, Screen.width - 32)));
            IconPos.xMax = IconPos.xMin + 32;
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localizer.Format("#LOC_KAC_473"), KACResources.styleAddHeading);
            GUILayout.Label(string.Format("{0}", Math.Floor((IconPos.yMin)).ToString()), KACResources.styleAddXferName, GUILayout.Width(50));
            GUILayout.EndHorizontal();
            IconPos.yMin = Convert.ToInt32(Math.Floor(GUILayout.HorizontalSlider(IconPos.yMin, 0, Screen.height - 32)));
            IconPos.yMax = IconPos.yMin + 32;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }

        private void WindowLayout_SettingsCalendar()
        {
            //Update Check Area
            GUILayout.Label(Localizer.Format("#LOC_KAC_474"), KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(60));
            GUILayout.Space(2); //to even up the text
            GUILayout.Label(Localizer.Format("#LOC_KAC_475"), KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            ddlSettingsCalendar.DrawButton();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (DrawToggle(ref settings.ShowCalendarToggle, Localizer.Format("#LOC_KAC_476"), KACResources.styleCheckbox))
                settings.Save();
            GUILayout.EndVertical();


            if (settings.SelectedCalendar == CalendarTypeEnum.Earth)
            {
                GUILayout.Label(Localizer.Format("#LOC_KAC_477"), KACResources.styleAddSectionHeading);
                GUILayout.BeginVertical(KACResources.styleAddFieldAreas);

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_KAC_478"));

                String strYear, strMonth, strDay;
                strYear = KSPDateStructure.CustomEpochEarth.Year.ToString();
                strMonth = KSPDateStructure.CustomEpochEarth.Month.ToString();
                strDay = KSPDateStructure.CustomEpochEarth.Day.ToString();
                if (DrawYearMonthDay(ref strYear, ref strMonth, ref strDay))
                {
                    try
                    {
                        KSPDateStructure.SetEarthCalendar(strYear.ToInt32(), strMonth.ToInt32(), strDay.ToInt32());
                        settings.EarthEpoch = KSPDateStructure.CustomEpochEarth.ToString(Localizer.Format("#LOC_KAC_479"));
                        settings.Save();
                    }
                    catch (Exception)
                    {
                        LogFormatted("Unable to set the Epoch date using the values provided-{0}-{1}-{2}", strYear, strMonth, strDay);
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(Localizer.Format("#LOC_KAC_480")))
                {
                    KSPDateStructure.SetEarthCalendar();
                    settings.EarthEpoch = KSPDateStructure.CustomEpochEarth.ToString(Localizer.Format("#LOC_KAC_481"));
                    settings.Save();
                }
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
            }

            //if RSS not installed and RSS chosen...

            ///section for custom stuff
        }

        private void WindowLayout_SettingsAbout()
        {
            //Update Check Area
            GUILayout.Label(Localizer.Format("#LOC_KAC_482"), KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas, GUILayout.Height(intUpdateBoxheight));
            GUILayout.BeginHorizontal();
            if (DrawCheckbox(ref settings.DailyVersionCheck, Localizer.Format("#LOC_KAC_483")))
                settings.Save();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Localizer.Format("#LOC_KAC_484"), KACResources.styleButton))
            {
                settings.VersionCheck(this, true);
                //Hide the flag as we already have the window open;
                settings.VersionAttentionFlag = false;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label(Localizer.Format("#LOC_KAC_485"), KACResources.styleAddHeading);
            GUILayout.Label(Localizer.Format("#LOC_KAC_486"), KACResources.styleAddHeading);
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(settings.VersionCheckDate_AttemptString, KACResources.styleContent);

            if (settings.VersionCheckRunning)
            {
                Int32 intDots = Convert.ToInt32(Math.Truncate(DateTime.Now.Millisecond / 250d)) + 1;
                GUILayout.Label(String.Format("{0} " +Localizer.Format("#LOC_KAC_487"), new String('.', intDots)), KACResources.styleVersionHighlight);
            }
            else
            {
                if (settings.VersionAvailable)
                    GUILayout.Label(String.Format("{0} " + "@" + " {1}", settings.VersionWeb, settings.VersionCheckDate_SuccessString), KACResources.styleVersionHighlight);
                else
                    GUILayout.Label(String.Format("{0} " + "@" + " {1}", settings.VersionWeb, settings.VersionCheckDate_SuccessString), KACResources.styleContent);
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            if (settings.VersionAvailable)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(80);
                if (GUILayout.Button(Localizer.Format("#LOC_KAC_488"), KACResources.styleVersionHighlight))
                    Application.OpenURL(Localizer.Format("#LOC_KAC_489"));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            //About Area
            GUILayout.Label(Localizer.Format("#LOC_KAC_357"), KACResources.styleAddSectionHeading);

            GUILayout.BeginVertical(KACResources.styleAddFieldAreas);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //GUILayout.Label("Written by:", KACResources.styleAddHeading);
            GUILayout.Label(Localizer.Format("#LOC_KAC_490"), KACResources.styleAddHeading);
            GUILayout.Label(Localizer.Format("#LOC_KAC_491"), KACResources.styleAddHeading);
            GUILayout.Label(Localizer.Format("#LOC_KAC_492"), KACResources.styleAddHeading);
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            //GUILayout.Label("Trigger Au",KACResources.styleContent);
            if (GUILayout.Button(Localizer.Format("#LOC_KAC_493"), KACResources.styleContent))
                Application.OpenURL("https://linuxgurugamer.github.io/KerbalAlarmClock/"); // NO_LOCALIZATION
            if (GUILayout.Button(Localizer.Format("#LOC_KAC_493"), KACResources.styleContent))
                Application.OpenURL("https://github.com/linuxgurugamer/KerbalAlarmClock/"); // NO_LOCALIZATION
            if (GUILayout.Button(Localizer.Format("#LOC_KAC_493"), KACResources.styleContent))
                Application.OpenURL("https://forum.kerbalspaceprogram.com/topic/22809-kerbal-alarm-clock/"); // NO_LOCALIZATION

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

        }
    }
}
