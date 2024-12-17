using System;
using LSPD_First_Response.Mod.API;
using Rage;

namespace BetterPresence
{
    public class Main : Plugin
    {
        public override void Initialize()
        {
            if (Settings.CheckVersion())
            {
                Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
                Settings.UpdateSettings();

                Game.Console.Print("[BetterPresence] Successfully loaded v" + Settings.Version + ". Go on duty to enable.");
            }
        }

        public override void Finally()
        {
            Presence.StopLoop();
        }

        public static void OnOnDutyStateChangedHandler(bool onDuty)
        {
            if (onDuty)
            {
                GameFiber.StartNew(delegate
                {
                    Presence.StartLoop();

                    Game.LogVerbose("BetterPresence" + Settings.Version + " enabled.");
                });
            } else
            {
                Presence.StopLoop();

                Game.LogVerbose("BetterPresence disabled.");
            }
        }
    }
}
