using System;
using System.Net;
using Rage;

namespace BetterPresence
{
    public class Settings
    {
        public static string DetailsLine = "%STATUS% in %STREET_NAME%, %ZONE% as %DEPARTMENT%";
        public static bool DisableTimer = false;
        public static bool ShowInVehicle = true;
        public static bool ShowCalloutName = true;
        public static int HourOffset = 0;
        public static string ForceDepartmentLogo = "NONE";

        public static void UpdateSettings()
        {
            var ini = new InitializationFile("Plugins/LSPDFR/BetterPresence.ini");
            ini.Create();

            DetailsLine = ini.ReadString("Settings", "DetailsLine", "%STATUS% in %STREET_NAME%, %ZONE% as %DEPARTMENT%");
            DisableTimer = ini.ReadBoolean("Settings", "DisableTimer", false);
            ShowInVehicle = ini.ReadBoolean("Settings", "ShowInVehicle", true);
            ShowCalloutName = ini.ReadBoolean("Settings", "ShowCalloutName", true);
            HourOffset = ini.ReadInt32("Settings", "HourOffset", 0);
            ForceDepartmentLogo = ini.ReadString("Settingfs", "ForceDepartmentLogo", "NONE");
        }

        public static bool CheckVersion()
        {
            Uri latestVersion = new Uri("https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId=41737&textOnly=1");
            WebClient webClient= new WebClient();
            string recieved = string.Empty;

            try
            {
                recieved = webClient.DownloadString(latestVersion);

                if (recieved != Version)
                {
                    Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~y~BetterPresence", "~w~Update available!", "~w~You are running ~r~" + Version + "~w~, and BetterPresence is on ~g~" + recieved);
                    return true;
                } else
                {
                    return true;
                }
            } catch (WebException)
            {
                Game.DisplayNotification("commonmenu", "mp_alerttriangle", "~r~BetterPresence", "~y~Update check failed!", "~w~BetterPresence could not check for updates! You are running version " + Version);
                return true;
            }
        }

        public static readonly string Version = "2.0.8";
    }
}
