using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

using System.Reflection;

using UnityEngine;
using KSP;
using KSP.UI.Screens;
using KSPPluginFramework;
using ClickThroughFix;

namespace KerbalAlarmClock
{
	public partial class KerbalAlarmClock
	{
		//On OnGUI - draw alarms if needed
		internal void TriggeredAlarms()
		{
			foreach (KACAlarm tmpAlarm in alarms)
			{
				if (tmpAlarm.Enabled)
				{
					//also test triggered and actioned
					//if (KACWorkerGameState.CurrentTime.UT >= tmpAlarm.AlarmTime.UT)
					if ((tmpAlarm.Remaining.UT<=0))
					{
						if (tmpAlarm.Actioned && !tmpAlarm.AlarmWindowClosed)
						{
							if (tmpAlarm.AlarmWindowID == 0)
							{
								tmpAlarm.AlarmWindowID = rnd.Next(1, 2000000);
								tmpAlarm.AlarmWindow = new Rect((Screen.width / 2) - 160, (Screen.height / 2) - 100, 320, tmpAlarm.AlarmWindowHeight);
								if (settings.AlarmPosition == 0)
									tmpAlarm.AlarmWindow.x = 5;
								else if (settings.AlarmPosition == 2)
									tmpAlarm.AlarmWindow.x = Screen.width - tmpAlarm.AlarmWindow.width - 5;

								//tmpAlarm.DeleteOnClose = settings.AlarmDeleteOnClose;
							}
							else
							{
								tmpAlarm.AlarmWindow.height = tmpAlarm.AlarmWindowHeight;
							}
							String strAlarmText = tmpAlarm.Name;
							
							switch (tmpAlarm.TypeOfAlarm)
							{
								case KACAlarm.AlarmTypeEnum.Raw:
									strAlarmText+= Localizer.Format("#LOC_KAC_279");break;
								case KACAlarm.AlarmTypeEnum.Maneuver:
								case KACAlarm.AlarmTypeEnum.ManeuverAuto:
									strAlarmText += Localizer.Format("#LOC_KAC_280"); break;
								case KACAlarm.AlarmTypeEnum.SOIChange:
								case KACAlarm.AlarmTypeEnum.SOIChangeAuto:
									strAlarmText += Localizer.Format("#LOC_KAC_281"); break;
								case KACAlarm.AlarmTypeEnum.Transfer:
								case KACAlarm.AlarmTypeEnum.TransferModelled:
									strAlarmText += Localizer.Format("#LOC_KAC_282"); break;
								case KACAlarm.AlarmTypeEnum.Apoapsis:
									strAlarmText += Localizer.Format("#LOC_KAC_283"); break;
								case KACAlarm.AlarmTypeEnum.Periapsis:
									strAlarmText += Localizer.Format("#LOC_KAC_284"); break;
								case KACAlarm.AlarmTypeEnum.AscendingNode:
									strAlarmText += Localizer.Format("#LOC_KAC_285"); break;
								case KACAlarm.AlarmTypeEnum.DescendingNode:
									strAlarmText += Localizer.Format("#LOC_KAC_286"); break;
								case KACAlarm.AlarmTypeEnum.LaunchRendevous:
									strAlarmText += Localizer.Format("#LOC_KAC_287"); break;
								case KACAlarm.AlarmTypeEnum.Closest:
									strAlarmText += Localizer.Format("#LOC_KAC_288"); break;
								case KACAlarm.AlarmTypeEnum.EarthTime:
									strAlarmText += Localizer.Format("#LOC_KAC_289"); break;
								case KACAlarm.AlarmTypeEnum.Crew:
									strAlarmText += Localizer.Format("#LOC_KAC_290"); break;
								case KACAlarm.AlarmTypeEnum.Contract:
								case KACAlarm.AlarmTypeEnum.ContractAuto:
									strAlarmText += Localizer.Format("#LOC_KAC_291"); break;
                                case KACAlarm.AlarmTypeEnum.ScienceLab:
                                    strAlarmText += Localizer.Format("#LOC_KAC_292"); break;
								default:
									strAlarmText+= Localizer.Format("#LOC_KAC_279");break;
							}
							tmpAlarm.AlarmWindow = ClickThruBlocker.GUILayoutWindow(tmpAlarm.AlarmWindowID, tmpAlarm.AlarmWindow, FillAlarmWindow, strAlarmText, KACResources.styleWindow, GUILayout.MinWidth(320));
						}
					}
				}
			}

		}

		internal void FillAlarmWindow(int windowID)
		{
			KACAlarm tmpAlarm = alarms.GetByWindowID(windowID);

			GUILayout.BeginVertical();

			GUILayout.BeginVertical(GUI.skin.textArea);

			GUILayout.BeginHorizontal();
			GUILayout.Label(Localizer.Format("#LOC_KAC_293"), KACResources.styleAlarmMessageTime);
			if (tmpAlarm.TypeOfAlarm!= KACAlarm.AlarmTypeEnum.EarthTime)
				GUILayout.Label(tmpAlarm.AlarmTime.ToStringStandard(settings.DateTimeFormat), KACResources.styleAlarmMessageTime);
			else
				GUILayout.Label(EarthTimeDecode(tmpAlarm.AlarmTime.UT).ToLongTimeString(), KACResources.styleAlarmMessageTime);
			if (tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && tmpAlarm.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
				GUILayout.Label("(m: " + new KSPTimeSpan(tmpAlarm.AlarmMarginSecs).ToStringStandard(settings.TimeSpanFormat, 3) + ")", KACResources.styleAlarmMessageTime); // NO_LOCALIZATION
            GUILayout.EndHorizontal();

			GUILayout.Label(tmpAlarm.Notes, KACResources.styleAlarmMessage);

			GUILayout.BeginHorizontal();
			DrawCheckbox(ref tmpAlarm.Actions.DeleteWhenDone, "Delete On Close",0 );
			if (tmpAlarm.PauseGame)
			{
				if (FlightDriver.Pause)
					GUILayout.Label(Localizer.Format("#LOC_KAC_294"), KACResources.styleAlarmMessageActionPause);
				else
					GUILayout.Label(Localizer.Format("#LOC_KAC_295"), KACResources.styleAlarmMessageActionPause);
			}
			else if (tmpAlarm.HaltWarp)
			{
				GUILayout.Label(Localizer.Format("#LOC_KAC_296"), KACResources.styleAlarmMessageAction);
			}
			GUILayout.EndHorizontal();
			if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
				DrawStoredCrewMissing(tmpAlarm.VesselID);
			else
				DrawStoredVesselIDMissing(tmpAlarm.VesselID);
			GUILayout.EndVertical();

			int intNoOfActionButtons = 0;
			int intNoOfActionButtonsDoubleLine = 0;
			//if the alarm has a vessel ID/Kerbal associated
			if (CheckVesselOrCrewForJump(tmpAlarm.VesselID,tmpAlarm.TypeOfAlarm))
				//option to allow jumping from SC and TS
				if (settings.AllowJumpFromViewOnly)
					intNoOfActionButtons = DrawAlarmActionButtons(tmpAlarm, out intNoOfActionButtonsDoubleLine);

            intNoOfActionButtons += DrawTransferAngleButtons(tmpAlarm);

            //Work out the text
            String strText = Localizer.Format("#LOC_KAC_297");
			if (tmpAlarm.PauseGame)
			{
				if (FlightDriver.Pause) strText = Localizer.Format("#LOC_KAC_298");
			}
			//Now draw the button
			if (GUILayout.Button(strText, KACResources.styleButton))
			{
				tmpAlarm.AlarmWindowClosed = true;

                //Stop playing the sound if it is playing
                if (audioController.isPlaying)
                    audioController.Stop();

				//tmpAlarm.ActionedAt = KACWorkerGameState.CurrentTime.UT;
				if (tmpAlarm.PauseGame)
					FlightDriver.SetPause(false);

				try { 
					APIInstance_AlarmStateChanged(tmpAlarm, AlarmStateEventsEnum.Closed);
				} catch (Exception ex) {
					MonoBehaviourExtended.LogFormatted("Error Raising API Event-Closed Alarm: {0}" +
						"\r\n"  + // NO_LOCALIZATION
						"{1}", ex.Message, ex.StackTrace);
				} 

				if (tmpAlarm.Actions.DeleteWhenDone)
					alarms.Remove(tmpAlarm);
				//settings.SaveAlarms();
			}
		  
			GUILayout.EndVertical();

			int intLines = tmpAlarm.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length;  // NO_LOCALIZATION
            if (intLines == 0) intLines = 1;
			tmpAlarm.AlarmWindowHeight = 148 +
				 intLines * 16 +
				intNoOfActionButtons * 32 +
				intNoOfActionButtonsDoubleLine * 14;

			SetTooltipText();
			GUI.DragWindow();

		}


		//VesselOrCrewStuff
		private static Boolean CheckVesselOrCrewForJump(String ID, KACAlarm.AlarmTypeEnum aType)
		{
            if (aType == KACAlarm.AlarmTypeEnum.Crew && StoredCrewExists(ID))
            {
                return true;
            }
            else
            {
                Vessel v = StoredVessel(ID);

                if (v != null)
                {
                    if (v.vesselType != VesselType.SpaceObject && v.DiscoveryInfo.Level != DiscoveryLevels.Owned)
                        return false;
                    else if (settings.AllowJumpToAsteroid)
                        return true;
                    else if (StoredVessel(ID).vesselType != VesselType.SpaceObject)
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
		}


		//Stuff to do with stored VesselIDs
		private static void DrawStoredVesselIDMissing(String VesselID)
		{
			if (!(VesselID == null || VesselID == "") && !StoredVesselExists(VesselID))
			{
				GUILayout.Label(Localizer.Format("#LOC_KAC_299"),KACResources.styleLabelWarning);
			}
		}
		internal static Boolean StoredVesselExists(String VesselID)
		{
            return StoredVessel(VesselID) != null;
			//return (VesselID != null) && (VesselID != "") && (FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString() == VesselID) != null);
		}

        internal static Vessel StoredVessel(String VesselID)
        {
            if (VesselID == null || VesselID == "")
            {
                return null;
            }

            try
            {
                Guid g = new Guid(VesselID);
                return FlightGlobals.FindVessel(g);
            }
            catch { }

            return null;
			//return FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString() == VesselID);
		}

		//Stuff to do with Stored Kerbal Crew
		internal static List<ProtoCrewMember> AllAssignedCrew()
		{
			List<ProtoCrewMember> lstReturn = new List<ProtoCrewMember>();
			foreach (Vessel v in FlightGlobals.Vessels)
			{
				List<ProtoCrewMember> pCM = v.GetVesselCrew();
				foreach (ProtoCrewMember CM in pCM)
				{
					lstReturn.Add(CM);
				}
			}
			return lstReturn;
		}
		private static void DrawStoredCrewMissing(String KerbalName)
		{
			if (KerbalName != null && KerbalName != "" && !StoredCrewExists(KerbalName))
			{
				GUILayout.Label(Localizer.Format("#LOC_KAC_300"), KACResources.styleLabelWarning);
			}
		}
		internal static Boolean StoredCrewExists(String KerbalName)
		{
			return (KerbalName != null) && (KerbalName != "") && (AllAssignedCrew().FirstOrDefault(cm=>cm.name==KerbalName) != null);
		}

		internal static ProtoCrewMember StoredCrew(String KerbalName)
		{
			return AllAssignedCrew().FirstOrDefault(cm => cm.name == KerbalName);
		}
		internal static Vessel StoredCrewVessel(String KerbalName)
		{
			foreach (Vessel v in FlightGlobals.Vessels)
			{
				List<ProtoCrewMember> pCM = v.GetVesselCrew();
				foreach (ProtoCrewMember CM in pCM)
				{
					if (CM.name == KerbalName)
					{
						return v;
					}
				}
			}
			return null;
		}

		//Stuff to do with Celestial Bodies
		internal static Boolean CelestialBodyExists(String BodyName)
		{
			return (BodyName != "") && (FlightGlobals.Bodies.FirstOrDefault(b => b.bodyName == BodyName) != null);
		}
		internal static CelestialBody CelestialBody(String BodyName)
		{
			return FlightGlobals.Bodies.FirstOrDefault(a => a.bodyName == BodyName);
		}

		private KACAlarm alarmEdit;
		//track the height as we add/remove stuff
		private Int32 intAlarmEditHeight;
		public void FillEditWindow(int WindowID)
		{
			if (alarmEdit.Remaining.UT > 0)
			{
				//Edit the Alarm if its not yet passed
				Double MarginStarting = alarmEdit.AlarmMarginSecs;
				int intHeight_EditWindowCommon = 103 +
					alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16; // NO_LOCALIZATION
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
					intHeight_EditWindowCommon += 28;

                AlarmActions atemp = alarmEdit.Actions;
				WindowLayout_CommonFields(ref alarmEdit.Name, ref alarmEdit.Notes, ref atemp, ref alarmEdit.AlarmMarginSecs, alarmEdit.TypeOfAlarm, intHeight_EditWindowCommon);
                alarmEdit.Actions = atemp;
				//Adjust the UT of the alarm if the margin changed
				if (alarmEdit.AlarmMarginSecs != MarginStarting)
				{
					alarmEdit.AlarmTime.UT += MarginStarting - alarmEdit.AlarmMarginSecs;
				}
				//Draw warning if the vessel no longer exists
				if (alarmEdit.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
					DrawStoredCrewMissing(alarmEdit.VesselID);
				else
					DrawStoredVesselIDMissing(alarmEdit.VesselID);


                //Draw the old and new times
                GUILayout.BeginHorizontal();
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
                {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_301"), KACResources.styleContent);
                    GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
                }
                GUILayout.Label(Localizer.Format("#LOC_KAC_302"), KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).Add(new KSPTimeSpan(alarmEdit.AlarmMarginSecs)).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
                else
                    GUILayout.Label(alarmEdit.Remaining.ToStringStandard(TimeSpanStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_KAC_303"), KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    GUILayout.Label(alarmEdit.AlarmTime.AddSeconds(alarmEdit.AlarmMarginSecs).ToStringStandard(DateStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
                else
                    GUILayout.Label(DateTime.Now.AddSeconds(alarmEdit.Remaining.UT).ToLongTimeString(), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

				int intNoOfActionButtons = 0;
				int intNoOfActionButtonsDoubleLine = 0;
				//if the alarm has a vessel ID/Kerbal associated
				if (CheckVesselOrCrewForJump(alarmEdit.VesselID, alarmEdit.TypeOfAlarm))
					//option to allow jumping from SC and TS
					if (settings.AllowJumpFromViewOnly)
						intNoOfActionButtons = DrawAlarmActionButtons(alarmEdit, out intNoOfActionButtonsDoubleLine);

                intNoOfActionButtons += DrawTransferAngleButtons(alarmEdit);

				if (GUILayout.Button(Localizer.Format("#LOC_KAC_304"), KACResources.styleButton))
				{
					settings.Save();
					_ShowEditPane = false;
				}

				//TODO: Edit the height of this for when we have big text in restore button
				 intAlarmEditHeight = 197 + 16 + 20 + alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16 + intNoOfActionButtons * 32 + intNoOfActionButtonsDoubleLine*14; // NO_LOCALIZATION
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab)
					intAlarmEditHeight += 28;
                if (alarmEdit.TypeOfAlarm==KACAlarm.AlarmTypeEnum.EarthTime)
                    intAlarmEditHeight -= 28;
            }
			else
			{

				//otherwise just show the details
				GUILayout.BeginVertical(GUI.skin.textArea);

				GUILayout.BeginHorizontal();
				GUILayout.Label(Localizer.Format("#LOC_KAC_82"), KACResources.styleAlarmMessageTime);
				GUILayout.Label(alarmEdit.Name, KACResources.styleAlarmMessageTime);
				GUILayout.EndHorizontal();
				GUILayout.Label(alarmEdit.Notes, KACResources.styleAlarmMessage);

				//Draw warning if the vessel no longer exists
				if (alarmEdit.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
					DrawStoredCrewMissing(alarmEdit.VesselID);
				else
					DrawStoredVesselIDMissing(alarmEdit.VesselID);
				GUILayout.EndVertical();

				//Draw the old and new times
				GUILayout.BeginHorizontal();
				if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Raw && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.Crew && alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.ScienceLab) {
					GUILayout.Label(Localizer.Format("#LOC_KAC_301"), KACResources.styleContent);
					GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
				}
				GUILayout.Label(Localizer.Format("#LOC_KAC_302"), KACResources.styleContent);
				if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
					GUILayout.Label((alarmEdit.AlarmTime - KACWorkerGameState.CurrentTime).Add(new KSPTimeSpan(alarmEdit.AlarmMarginSecs)).ToStringStandard(settings.TimeSpanFormat), KACResources.styleAddHeading);
				else
                    GUILayout.Label(alarmEdit.Remaining.ToStringStandard(TimeSpanStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
				GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(Localizer.Format("#LOC_KAC_303"), KACResources.styleContent);
                if (alarmEdit.TypeOfAlarm != KACAlarm.AlarmTypeEnum.EarthTime)
                    GUILayout.Label(alarmEdit.AlarmTime.AddSeconds(alarmEdit.AlarmMarginSecs).ToStringStandard(DateStringFormatsEnum.DateTimeFormat), KACResources.styleAddHeading);
                else
                    GUILayout.Label(DateTime.Now.AddSeconds(alarmEdit.Remaining.UT).ToLongTimeString(), KACResources.styleAddHeading);
                GUILayout.EndHorizontal();

				int intNoOfActionButtons = 0;
				int intNoOfActionButtonsDoubleLine = 0;
				//if the alarm has a vessel ID/Kerbal associated
				if (CheckVesselOrCrewForJump(alarmEdit.VesselID, alarmEdit.TypeOfAlarm))
					//option to allow jumping from SC and TS
					if (settings.AllowJumpFromViewOnly)
						intNoOfActionButtons = DrawAlarmActionButtons(alarmEdit, out intNoOfActionButtonsDoubleLine);

                intNoOfActionButtons += DrawTransferAngleButtons(alarmEdit);

                if (GUILayout.Button(Localizer.Format("#LOC_KAC_304"), KACResources.styleButton))
					_ShowEditPane = false;

				intAlarmEditHeight = 152 + 20 +
					alarmEdit.Notes.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length * 16 + // NO_LOCALIZATION
                    intNoOfActionButtons * 32 + intNoOfActionButtonsDoubleLine * 14;
			}
			SetTooltipText();
		}
		
        private int DrawTransferAngleButtons(KACAlarm tmpAlarm)
        {
            if((tmpAlarm.TypeOfAlarm== KACAlarm.AlarmTypeEnum.Transfer|| tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.TransferModelled) &&
                (HighLogic.LoadedScene == GameScenes.TRACKSTATION || HighLogic.LoadedScene == GameScenes.FLIGHT))
            {
                //right type of alarm, now is the text there
                Match matchPhase = Regex.Match(tmpAlarm.Notes, Localizer.Format("#LOC_KAC_305"));
                Match matchEjectPro = Regex.Match(tmpAlarm.Notes, Localizer.Format("#LOC_KAC_306"));
                Match matchEjectRetro = Regex.Match(tmpAlarm.Notes, Localizer.Format("#LOC_KAC_307"));
                if (matchPhase.Success && (matchEjectPro.Success || matchEjectRetro.Success))
                {

                    try
                    {
                        //LogFormatted_DebugOnly("{0}", matchPhase.Value);
                        Double dblPhase = Convert.ToDouble(matchPhase.Value);
                        Double dblEject;
                        if (matchEjectPro.Success)
                            dblEject = Convert.ToDouble(matchEjectPro.Value);
                        else
                            dblEject = Convert.ToDouble(matchEjectRetro.Value);

                        GUILayout.BeginHorizontal();

                        CelestialBody cbOrigin = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferOriginBodyName);
                        CelestialBody cbTarget = FlightGlobals.Bodies.Single(b => b.bodyName == tmpAlarm.XferTargetBodyName);

                        GUIStyle styleAngleButton = new GUIStyle(KACResources.styleSmallButton) { fixedWidth = 180 };

                        if (DrawToggle(ref blnShowPhaseAngle,Localizer.Format("#LOC_KAC_308"), styleAngleButton)){
                            if (blnShowPhaseAngle)
                            {
                                EjectAngle.HideAngle();
                                blnShowEjectAngle = false;
                                PhaseAngle.DrawAngle(cbOrigin, cbTarget, dblPhase);
                            }
                            else
                                PhaseAngle.HideAngle();
                        }
                        if (DrawToggle(ref blnShowEjectAngle, Localizer.Format("#LOC_KAC_309"), styleAngleButton))
                        {
                            if (blnShowEjectAngle)
                            {
                                PhaseAngle.HideAngle();
                                blnShowPhaseAngle = false;
                                EjectAngle.DrawAngle(cbOrigin, dblEject, matchEjectRetro.Success);
                            }
                            else
                                EjectAngle.HideAngle();
                        }
                        GUILayout.EndHorizontal();

                        //if (GUILayout.Toggle()) {

                        //}
                        //GUILayout.Label(String.Format("P:{0} - E:{1}",dblPhase,dblEject));

                        return 1;

                    }
                    catch (Exception)
                    {
                        GUILayout.Label(Localizer.Format("#LOC_KAC_310"));
                        return 1;
                    }
                } else {
                    GUILayout.Label(Localizer.Format("#LOC_KAC_311"));
                    return 1;
                }
            } else { return 0; }
        }

        private int DrawAlarmActionButtons(KACAlarm tmpAlarm, out int NoOfDoubleLineButtons)
		{
			int intReturnNoOfButtons = 0;
			NoOfDoubleLineButtons = 0;
			
			////is it the current vessel?
			//if ((!ViewAlarmsOnly) && (KACWorkerGameState.CurrentVessel != null) && (FindVesselForAlarm(tmpAlarm).id.ToString() == KACWorkerGameState.CurrentVessel.id.ToString()))
			if ((KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT) && (KACWorkerGameState.CurrentVessel != null) && (FindVesselForAlarm(tmpAlarm).id.ToString() == KACWorkerGameState.CurrentVessel.id.ToString()))
			{
				//There is a stored Target, that hasnt passed
				//if ((tmpAlarm.TargetObject != null) && ((tmpAlarm.Remaining.UT + tmpAlarm.AlarmMarginSecs) > 0))
				if ((tmpAlarm.TargetObject != null))
					{
					String strRestoretext = Localizer.Format("#LOC_KAC_312");
					if (KACWorkerGameState.CurrentVesselTarget != null)
					{
						strRestoretext = Localizer.Format("#LOC_KAC_313");
						if (KACWorkerGameState.CurrentVesselTarget != tmpAlarm.TargetObject)
							strRestoretext += Localizer.Format("#LOC_KAC_314");
						else
							strRestoretext += Localizer.Format("#LOC_KAC_315");
						NoOfDoubleLineButtons++;
					}
					intReturnNoOfButtons++;
					if (GUILayout.Button(strRestoretext, KACResources.styleButton))
					{
						if (tmpAlarm.TargetObject is Vessel)
							FlightGlobals.fetch.SetVesselTarget(tmpAlarm.TargetObject as Vessel);
						else if (tmpAlarm.TargetObject is CelestialBody)
							FlightGlobals.fetch.SetVesselTarget(tmpAlarm.TargetObject as CelestialBody);
					}
				}
			}
			else
			{
				intReturnNoOfButtons++;
				//Or just jump to ship - regardless of alarm time
				String strButton = Localizer.Format("#LOC_KAC_316");
				if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew) strButton = strButton.Replace(Localizer.Format("#LOC_KAC_317"), Localizer.Format("#LOC_KAC_318"));
				if (GUILayout.Button(strButton, KACResources.styleButton))
				{

					Vessel tmpVessel = FindVesselForAlarm(tmpAlarm);
					// tmpVessel.MakeActive();

					JumpToVessel(tmpVessel);
				}

                //////////////////////////////////////////////////////////////////////////////////
                // Focus Vessel Code - reflecting to get SetVessel Focus in TS
                //////////////////////////////////////////////////////////////////////////////////
                if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
                {
                    Vessel vTarget = FlightGlobals.Vessels.FirstOrDefault(v => v.id.ToString().ToLower() == tmpAlarm.VesselID);
                    if (vTarget != null)
                    {
                        intReturnNoOfButtons++;
                        if (GUILayout.Button(Localizer.Format("#LOC_KAC_319"), KACResources.styleButton))
                        {

                            SetVesselActiveInTS(vTarget);

                            //FlightGlobals.Vessels.ForEach(v =>
                            //    {
                            //        v.DetachPatchedConicsSolver();
                            //        v.orbitRenderer.isFocused = false;
                            //    });

                            //vTarget.orbitRenderer.isFocused = true;
                            //vTarget.AttachPatchedConicsSolver();
                            //FlightGlobals.SetActiveVessel(vTarget);

                            //SpaceTracking.GoToAndFocusVessel(vTarget);
                            //st.mainCamera.SetTarget(getVesselIdx(vTarget));
                        }
                    }
                    //}
                }
			}
			return intReturnNoOfButtons;
		}

        private static void SetVesselActiveInTS(Vessel vTarget)
        {
            if (KACWorkerGameState.CurrentGUIScene == GameScenes.TRACKSTATION)
            {
                try
                {
                    SpaceTracking st = (SpaceTracking)KACSpaceCenter.FindObjectOfType(typeof(SpaceTracking));
                    st.SetVessel(vTarget, true);
                }
                catch (Exception ex)
                {
                    LogFormatted("Unable to set vessel as active in Tracking station:" +
						"\r\n{0}", ex.Message); // NO_LOCALIZATION
                }
            }
        }

        private static Vessel vesselToJumpTo = null;
		private Boolean JumpToVessel(Vessel vTarget)
		{
			Boolean blnJumped = true;
			if (KACWorkerGameState.CurrentGUIScene == GameScenes.FLIGHT)
			{
                LogFormatted_DebugOnly("Switching in Scene");
                if(KACUtils.BackupSaves() || !KerbalAlarmClock.settings.CancelFlightModeJumpOnBackupFailure)
                    vesselToJumpTo = vTarget;

                    //if(FlightGlobals.SetActiveVessel(vTarget))
                    //{
                    //    FlightInputHandler.SetNeutralControls();
                    //}
				else 
				{
					LogFormatted("Not Switching - unable to backup saves");
					ShowBackupFailedWindow(Localizer.Format("#LOC_KAC_320"));
					blnJumped = false;
				}
			}
			else
			{
                LogFormatted_DebugOnly("Switching in by Save");

                int intVesselidx = getVesselIdx(vTarget);
				if (intVesselidx < 0)
				{
					LogFormatted("Couldn't find the index for the vessel {0}({1})", vTarget.vesselName, vTarget.id.ToString());
					ShowBackupFailedWindow(Localizer.Format("#LOC_KAC_321"));
					blnJumped = false;
				}
				else
				{
					try
					{
						if (KACUtils.BackupSaves())
						{
							String strret = GamePersistence.SaveGame("KACJumpToShip", HighLogic.SaveFolder, SaveMode.OVERWRITE);
							Game tmpGame = GamePersistence.LoadGame(strret, HighLogic.SaveFolder, false, false);
                            FlightDriver.StartAndFocusVessel(tmpGame, intVesselidx);
                            //if (tmpAlarm.PauseGame)
                            //FlightDriver.SetPause(false);
                            //tmpGame.Start();
						}
						else
						{
							LogFormatted("Not Switching - unable to backup saves");
							ShowBackupFailedWindow(Localizer.Format("#LOC_KAC_320"));
							blnJumped = false;
						}
					}
					catch (Exception ex)
					{
						LogFormatted("Unable to save/load for jump to ship: {0}", ex.Message);
						ShowBackupFailedWindow(Localizer.Format("#LOC_KAC_322"));
						blnJumped = false;
					}
				}
			}
			return blnJumped;
		}

		private static Vessel FindVesselForAlarm(KACAlarm tmpAlarm)
		{
            Vessel tmpVessel;
			String strVesselID = "";
			if (tmpAlarm.TypeOfAlarm == KACAlarm.AlarmTypeEnum.Crew)
			{
				strVesselID = StoredCrewVessel(tmpAlarm.VesselID).id.ToString();
			}
			else
			{
				strVesselID = tmpAlarm.VesselID;
			}

			tmpVessel = FlightGlobals.Vessels.Find(delegate(Vessel v)
				{
					return (strVesselID == v.id.ToString());
				}
			);
			return tmpVessel;
		}

		private static int getVesselIdx(Vessel vtarget)
		{
			for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
			{
				if (FlightGlobals.Vessels[i].id == vtarget.id)
				{
					LogFormatted("Found Target idx={0} ({1})", i, vtarget.id.ToString());
					return i;
				}
			}
			return -1;
		}

		#region Localizer.Format("#LOC_KAC_323")
		internal void ShowBackupFailedWindow(String Message)
		{
			BackupFailedMessage = Message;
			GUIContent contFailMessage = new GUIContent(BackupFailedMessage);
			float minwidth = 0; float maxwidth = 0;
			KACResources.styleAddHeading.CalcMinMaxWidth(contFailMessage, out minwidth, out maxwidth);

			switch (KACWorkerGameState.CurrentGUIScene)
			{
				case GameScenes.SPACECENTER: 
					_WindowBackupFailedRect = new Rect((Screen.width - maxwidth - 20) , Screen.height - 90 - 37, maxwidth + 20, 90);
					break;
				case GameScenes.TRACKSTATION: 
					_WindowBackupFailedRect = new Rect((Screen.width - maxwidth - 20) , Screen.height - 90, maxwidth + 20, 90);
					break;
				default: 
					_WindowBackupFailedRect = new Rect((Screen.width - maxwidth - 20) , Screen.height - 90 - 122, maxwidth + 20, 90);
					break;
			}

			_ShowBackupFailedMessageAt=DateTime.Now;
			_ShowBackupFailedMessage = true;
		}


		#region "Stuff for backupFailed dialog per scene"
		internal Rect ShowBackupFailedWindowPosByActiveScene
		{
			get
			{
				switch (KACWorkerGameState.CurrentGUIScene)
				{
					case GameScenes.SPACECENTER: return settings.WindowPos_SpaceCenter;
                    case GameScenes.TRACKSTATION: return settings.WindowPos_TrackingStation;
                    case GameScenes.EDITOR:
                        if (isEditorVAB) 
                            return settings.WindowPos_EditorVAB;
                        else
                            return settings.WindowPos_EditorSPH;
                    default: return settings.WindowPos;
				}
			}
		}

		#endregion

		internal void ResetBackupFailedWindow()
		{
			_ShowBackupFailedMessage = false;
			BackupFailedMessage = "";
		}

		private static String BackupFailedMessage = "";
		internal void FillBackupFailedWindow(int windowID)
		{
			GUILayout.BeginVertical();

			GUILayout.Label(new GUIContent(BackupFailedMessage), KACResources.styleAddHeading);

			int SecsToClose = _ShowBackupFailedMessageForSecs - DateTime.Now.Subtract(_ShowBackupFailedMessageAt).Seconds;
			if (GUILayout.Button(string.Format( "Close (" + "{0} " +Localizer.Format("#LOC_KAC_324"), SecsToClose)))
				ResetBackupFailedWindow();

			GUILayout.EndVertical();

		}
		#endregion
	}
}
