using System;
using System.Collections.Generic;
using System.Linq;
using DiscordRPC;
using Rage;
using LSPD_First_Response.Mod.API;

namespace BetterPresence
{
    public class Presence
    {
        private static bool _stopLoop = false;
        private static bool _loopStarted = false;
        private static bool _clientReady = false;

        private static RichPresence CurrentRichPresence;

        private static DiscordRpcClient rpcClient;

        public static DateTime startTime;

        public static void StartLoop()
        {
            if (!_loopStarted)
            {
                _loopStarted = true;
                _stopLoop = false;
                _clientReady = false;

                startTime = DateTime.Now;
                startTime = startTime.AddHours(Settings.HourOffset * -1);

                rpcClient = new DiscordRpcClient("999781142876135434");
                rpcClient.Logger = new DiscordRPC.Logging.NullLogger();

                rpcClient.OnReady += (sender, e) =>
                {
                    _clientReady = true;
                };

                rpcClient.OnPresenceUpdate += (sender, e) =>
                {
                    Game.Console.Print("[BetterPresence] Presence updated.");
                };

                rpcClient.Initialize();

                while (!_stopLoop)
                {
                    if (_clientReady)
                    {
                        try
                        {
                            string Status = "Patrolling";

                            if (Settings.ShowInVehicle)
                            {
                                if (Game.LocalPlayer.Character.CurrentVehicle == null)
                                {
                                    Status = "On foot";
                                }
                                else
                                {
                                    Status = "Driving";
                                };
                            };

                            if (Functions.GetCurrentPullover() != null)
                            {
                                Status = "On a traffic stop";
                            }
                            else if (Functions.GetActivePursuit() != null)
                            {
                                Status = "In a pursuit";
                            }
                            else if (Functions.GetCurrentCallout() != null)
                            {
                                if (Settings.ShowCalloutName)
                                {
                                    Status = "Responding to a call - " + Functions.GetCalloutFriendlyName(Functions.GetCurrentCallout());
                                } else
                                {
                                    Status = "Responding to a call";
                                }
                                
                            }

                            String actualDepartment = Functions.GetCurrentAgencyScriptName();
                            String department = actualDepartment;

                            String[] validDepartments = { "lspd", "lssd", "fib", "noose", "sahp", "bcso", "lsfd", "nysp", "sapr", "sasp", "iaa", "doa" };

                            Vector3 playerLocation = Game.LocalPlayer.Character.Position;

                            if (!validDepartments.Contains(department.ToLower()))
                            {
                                department = "unknown";
                            }

                            if (Settings.ForceDepartmentLogo != "NONE")
                            {
                                department = Settings.ForceDepartmentLogo;
                            };

                            String details = Settings.DetailsLine;

                            IDictionary<string, string> countiesToName = new Dictionary<string, string>()
                            {
                                {"LosSantos","Los Santos"},
                                {"LosSantosCounty","Los Santos County"},
                                {"BlaineCounty","Blaine County"}
                            };

                            details = details.Replace("%STATUS%", Status);
                            details = details.Replace("%STREET_NAME%", World.GetStreetName(World.GetNextPositionOnStreet(playerLocation)));
                            details = details.Replace("%ZONE%", Functions.GetZoneAtPosition(playerLocation).RealAreaName);
                            details = details.Replace("%REGION%", countiesToName[Functions.GetZoneAtPosition(playerLocation).County.ToString()]);
                            details = details.Replace("%DEPARTMENT%", actualDepartment.ToUpper());

                            if (!Settings.DisableTimer)
                            {
                                CurrentRichPresence = new RichPresence()
                                {
                                    Details = details,
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = department,
                                        LargeImageText = "LSPDFR with BetterPresence v" + Settings.Version
                                    },
                                    Timestamps = new Timestamps()
                                    {
                                        Start = startTime
                                    }
                                };
                            }
                            else
                            {
                                CurrentRichPresence = new RichPresence()
                                {
                                    Details = details,
                                    Assets = new Assets()
                                    {
                                        LargeImageKey = department,
                                        LargeImageText = "LSPDFR with BetterPresence v" + Settings.Version
                                    }
                                };
                            };

                            rpcClient.SetPresence(CurrentRichPresence);
                        } catch (Exception) {
                            Game.LogVerbose("BetterPresence threw an exception while setting your presence! Crash [hopefully] prevented.");
                        }
                        
                    }

                    GameFiber.Sleep(1000);
                }

                _loopStarted = false;
                rpcClient.Dispose();
            }
        }

        public static void StopLoop()
        {
            if (_loopStarted)
            {
                _stopLoop = true;
            }
        }
    }
}
