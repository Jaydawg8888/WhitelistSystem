# Configuration

* `Whitelist System Enabled` - > Toggles the whitelist on or off.

# Permissions
* `whitelistsystem.whitelisted` -> Allows the player to connect to the server.
* `whitelistsystem.grant` -> Allows the user to grant whitelist access to players.
* `whitelistsystem.revoke` -> Allows the user to revoke whitelist access.
* `whitelistsystem.toggle` -> Allows the user to toggle the whitelist system on or off.

# Chat/RCON Commands
* `/whitelist | /wl (shorthand)` or `/whitelist help` -> Displays all available commands.
* `/whitelist grant steamID64` -> Whitelists the specified player to the server.
* `/whitelist revoke steamID64` -> Revokes the players access to the server. Note: If the player is on the server, they will be kicked.
* `/whitelist toggle true/false` -> Toggles the whitelist system on or off.

# Localization
```
{
  "CommandList": "Whitelist commands are:\r\n\n /whitelist <grant> <steamID64> - Allows the specified user to connect to the server.\r\n\n /whitelist <revoke> <steamID64> - Revokes the specified users whitelist.\r\n\n /whitelist <toggle> <true/false> - Toggles the whitelist system on or off.",
  "WhitelistGranted": "The specified user was whitelisted successfully.",
  "WhitelistRevoked": "The specified users whitelist was revoked.",
  "WhitelistToggle": "Whitelist system has been toggled {0}",
  "RevokedKick": "Sorry, your whitelist has been revoked.",
  "NoPerm": "You lack the necessary permission for this command.",
  "NotWhitelisted": "You are not whitelisted to join this server.",
  "InvalidSteamID": "Invalid SteamID",
  "InvalidSyntax": "Invalid command syntax, please use <u>/whitelist</u> or <u>/whitelist <help> </u> to list all valid commands.",
  "MissingArgument": "You are missing a second value for this command."
}
```
# FAQ
1. Q: How do I whitelist myself?: A: You can whitelist yourself to the server by granting yourself the following permission `simplewhitelist.whitelisted` or running the following commaind in your servers console `/whitelist grant (your steamID64)`.