using QuantumHangar.HangarChecks;
using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace QuantumHangar.Commands;

/// <summary>
///     Command module for faction hangar admin commands.
/// </summary>
[Category("factionhangermod")]
public class FactionHangarAdminCommands : CommandModule
{
    /// <summary>
    ///     Adds or removes a player from the whitelist.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="targetNameOrSteamId">The target player's name or Steam ID.</param>
    [Command("whitelist", "Add or remove a player from whitelist")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Whitelist(string tag, string targetNameOrSteamId)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ChangeWhitelist(targetNameOrSteamId));
    }

    /// <summary>
    ///     Outputs the whitelisted Steam IDs.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    [Command("whitelisted", "Output the whitelisted steam ids")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Whitelist(string tag)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ListWhitelist());
    }

    /// <summary>
    ///     Changes the webhook.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="webhook">The new webhook URL.</param>
    [Command("webhook", "Change the webhook")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Webhook(string tag, string webhook)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ChangeWebhook(webhook));
    }

    /// <summary>
    ///     Saves the grid the player is looking at to the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="gridName">The name of the grid.</param>
    [Command("save", "Saves the grid you are looking at to hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void SaveGrid(string tag, string gridName)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.SaveGrid(gridName));
    }

    /// <summary>
    ///     Lists all the grids saved in the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    [Command("list", "Lists all the grids saved in your hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void ListGrids(string tag)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ListGrids());
    }

    /// <summary>
    ///     Loads the specified grid by index number.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="id">The grid ID.</param>
    /// <param name="loadNearPlayer">Whether to load the grid near the player.</param>
    [Command("load", "Loads the specified grid by index number")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Load(string tag, string id, bool loadNearPlayer = false)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.LoadGrid(id, loadNearPlayer));
    }

    /// <summary>
    ///     Removes the grid from the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="id">The grid ID.</param>
    [Command("remove", "removes the grid from your hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Remove(string tag, string id)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.RemoveGrid(id));
    }

    /// <summary>
    ///     Provides information about the current grid in the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="id">The grid ID (optional).</param>
    [Command("info", "Provides some info of the current grid in your hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Info(string tag, string id = "")
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.DetailedInfo(id));
    }
}

/// <summary>
///     Command module for faction hangar admin commands with alias.
/// </summary>
[Category("fhm")]
public class FactionHangarAdminCommandsAlias : CommandModule
{
    /// <summary>
    ///     Adds or removes a player from the whitelist.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="targetNameOrSteamId">The target player's name or Steam ID.</param>
    [Command("whitelist", "Add or remove a player from whitelist")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Whitelist(string tag, string targetNameOrSteamId)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ChangeWhitelist(targetNameOrSteamId));
    }

    /// <summary>
    ///     Outputs the whitelisted Steam IDs.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    [Command("whitelisted", "Output the whitelisted steam ids")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Whitelist(string tag)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ListWhitelist());
    }

    /// <summary>
    ///     Changes the webhook.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="webhook">The new webhook URL.</param>
    [Command("webhook", "Change the webhook")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Webhook(string tag, string webhook)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ChangeWebhook(webhook));
    }

    /// <summary>
    ///     Saves the grid the player is looking at to the hangar.
    /// </summary>
    /// <param name="gridName">The name of the grid.</param>
    [Command("save", "Saves the grid you are looking at to hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void SaveGrid(string gridName)
    {
        var user = new FactionAdminChecks(Context);
        await HangarCommandSystem.RunAdminTaskAsync(void () => user.SaveGrid(gridName));
    }

    /// <summary>
    ///     Lists all the grids saved in the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    [Command("list", "Lists all the grids saved in your hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void ListGrids(string tag)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.ListGrids());
    }

    /// <summary>
    ///     Loads the specified grid by index number.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="id">The grid ID.</param>
    /// <param name="loadNearPlayer">Whether to load the grid near the player.</param>
    [Command("load", "Loads the specified grid by index number")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Load(string tag, string id, bool loadNearPlayer = false)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.LoadGrid(id, loadNearPlayer));
    }

    /// <summary>
    ///     Removes the grid from the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="id">The grid ID.</param>
    [Command("remove", "removes the grid from your hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Remove(string tag, string id)
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.RemoveGrid(id));
    }

    /// <summary>
    ///     Provides information about the current grid in the hangar.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="id">The grid ID (optional).</param>
    [Command("info", "Provides some info of the current grid in your hangar")]
    [Permission(MyPromoteLevel.Admin)]
    public async void Info(string tag, string id = "")
    {
        var user = new FactionAdminChecks(tag, Context);
        await HangarCommandSystem.RunAdminTaskAsync(() => user.DetailedInfo(id));
    }
}