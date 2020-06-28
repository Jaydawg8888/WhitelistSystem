using Facepunch.Extend;
using Oxide.Core.Libraries.Covalence;
using System;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Whitelist System", "Jamie Doggins", "1.0.2")]
    [Description("Allows only whitelisted players onto the server.")]
    public class WhitelistSystem: CovalencePlugin
    {
        #region Permission
        internal const string permUserWhitelisted = "whitelistsystem.whitelisted"; // To whitelist a user, add this permission to the individual. e.g. oxide.grant user [steamID] whitelistsystem.userwhitelisted.
        internal const string permWhitelistAdd = "whitelistsystem.grant";
        internal const string permWhitelistRemove = "whitelistsystem.revoke";
        internal const string permWhitelistToggle = "whitelistsystem.toggle";
        #endregion

        #region Localisation
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["CommandList"] = string.Join(Environment.NewLine, new[]
                {
                    "Whitelist system commands are:",
                    "\n /whitelist <grant> <steamID64> - Allows the specified user to connect to the server.",
                    "\n /whitelist <revoke> <steamID64> - Revokes the specified users whitelist.",
                    "\n /whitelist <toggle> <true/false> - Toggles the whitelist system on or off."
                }),

                // Sucess Handlers
                ["WhitelistGranted"] = "The specified user was whitelisted successfully.",
                ["WhitelistRevoked"] = "The specified users whitelist was revoked.",
                ["WhitelistToggle"] = "Whitelist system has been toggled {0}",

                // Kick Message
                ["RevokedKick"] = "Sorry, your whitelist has been revoked.",

                // Error Handlers
                ["NoPerm"] = "You lack the necessary permission for this command.",
                ["NotWhitelisted"] = "You are not whitelisted to join this server.",
                ["InvalidSteamID"] = "Invalid SteamID",
                ["InvalidSyntax"] = "Invalid command syntax, please use <u>/whitelist</u> or <u>/whitelist <help> </u> to list all valid commands.",
                ["MissingArgument"] = "You are missing a second value for this command."
            }, this);
        }
        #endregion

        #region Config
        protected override void LoadDefaultConfig()
        {
            Config["Whitelist System Enabled (true/false)"] = true;
            SaveConfig();
        }

        internal T GetConfig<T>(string configName, T defaultValue)
        {
            return Config[configName] == null ? defaultValue : (T)Convert.ChangeType(Config[configName], typeof(T));
        }

        private bool _bWhitelistEnabled;
        internal bool bWhitelistEnabled
        {
            get { return _bWhitelistEnabled; }
            set { _bWhitelistEnabled = value; }
        }
        #endregion

        void OnServerInitialized()
        {
            permission.RegisterPermission(permUserWhitelisted, this);
            permission.RegisterPermission(permWhitelistAdd, this);
            permission.RegisterPermission(permWhitelistRemove, this);
            permission.RegisterPermission(permWhitelistToggle, this);
            bWhitelistEnabled = GetConfig("Whitelist System Enabled (true/false)", true);
        }

        object CanUserLogin(string name, string id, string ipAddress)
        {
            if (bWhitelistEnabled)
            {
                if (permission.UserHasPermission(id, permUserWhitelisted))
                    return true;
                else
                    return lang.GetMessage("NotWhitelisted", this, id);
            }
            return true;

        }

        #region Commands
        public enum ValidCommands
        {
            grant,
            revoke,
            toggle,
            help
        }

        public ValidCommands? GetCommand(string arg)
        {
            try
            {
                ValidCommands command = (ValidCommands)Enum.Parse(typeof(ValidCommands), arg, true);
                return command;
            }
            catch
            {
                return null;
            }
        }

        [Command("whitelist", "wl")]
        void cmdWhitelist(IPlayer player, string command, string[] args)
        {
            if (args.Length == 0) // If the user types /whitelist and nothing else.
            {
                player.Reply(lang.GetMessage("CommandList", this, player.Id));
                return;
            }
            else if (GetCommand(args[0]) == null) // If the user types /whitelist <arg> and that arg isn't in our ValidCommands enum.
            {
                player.Reply(lang.GetMessage("InvalidSyntax", this, player.Id));
                return;
            }

            ValidCommands Command = (ValidCommands)GetCommand(args[0]);
            if(args.Length == 1 && (Command == ValidCommands.grant || Command == ValidCommands.revoke || Command == ValidCommands.toggle))
            {
                player.Reply(lang.GetMessage("MissingArgument", this, player.Id));
                return;
            }

            ulong targetSteamID;
            ulong.TryParse(args.Length == 2 ? args[1] : "0", out targetSteamID);
            if ((Command == ValidCommands.grant || Command == ValidCommands.revoke) && targetSteamID < 70000000000000000)
            {
                player.Reply(lang.GetMessage("InvalidSteamID", this, player.Id)); // If we aren't toggling the whitelist system, we need to check the SteamID length.
                return;
            }

            switch (Command)
            {
                case ValidCommands.help:
                    player.Reply(lang.GetMessage("CommandList", this, player.Id));
                    break;

                case ValidCommands.grant:
                    if (!permission.UserHasPermission(player.Id, permWhitelistAdd))
                    {
                        player.Reply(lang.GetMessage("NoPerm", this, player.Id));
                        return;
                    }

                    permission.GrantUserPermission(args[1], permUserWhitelisted, this);
                    player.Reply(lang.GetMessage("WhitelistGranted", this, player.Id));
                    break;

                case ValidCommands.revoke:
                    if (!permission.UserHasPermission(player.Id, permWhitelistRemove))
                    {
                        player.Reply(lang.GetMessage("NoPerm", this, player.Id));
                        return;
                    }
                    else if (BasePlayer.FindByID(targetSteamID) != null)
                    {
                        BasePlayer foundPlayer = BasePlayer.FindByID(targetSteamID);
                        foundPlayer.Kick(lang.GetMessage("RevokedKick", this, foundPlayer.UserIDString));
                    }

                    permission.RevokeUserPermission(args[1], permUserWhitelisted);
                    player.Reply(lang.GetMessage("WhitelistRevoked", this, player.Id));
                    break;

                case ValidCommands.toggle:
                    if (!permission.UserHasPermission(player.Id, permWhitelistToggle))
                    {
                        player.Reply(lang.GetMessage("NoPerm", this, player.Id));
                        return;
                    }

                    bool userInput;
                    if (args.Length == 2 && Boolean.TryParse(args[1], out userInput))
                    {
                        bWhitelistEnabled = userInput;
                        Config["Whitelist System Enabled (true/false)"] = userInput;
                        SaveConfig();
                        player.Reply(string.Format(lang.GetMessage("WhitelistToggle", this, player.Id), userInput == true ? "On" : "Off"));
                    }
                    else
                    {
                        player.Reply(lang.GetMessage("InvalidSyntax", this, player.Id));
                        break;
                    }
                    break;

                default:
                    player.Reply(lang.GetMessage("InvalidSyntax", this, player.Id));
                    break;
            }
        }
        #endregion
    }
}
