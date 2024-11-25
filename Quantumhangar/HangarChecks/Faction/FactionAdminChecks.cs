using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using QuantumHangar.Utils;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Character;
using Sandbox.Game.World;
using Torch.Commands;
using VRage.Game;
using VRage.Game.ModAPI;
using VRageMath;
using static QuantumHangar.Utils.CharacterUtilities;

namespace QuantumHangar.HangarChecks;

/// <summary>
///     Class responsible for performing various administrative checks and operations for factions.
/// </summary>
public class FactionAdminChecks
{
    static readonly Logger Log = LogManager.GetCurrentClassLogger();
    Chat _chat;
    CommandContext _ctx;
    MyFaction _faction;
    string _tag;

    /// <summary>
    ///     Initializes a new instance of the <see cref="FactionAdminChecks" /> class with a faction tag.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <param name="ctx">The command context.</param>
    public FactionAdminChecks(string tag, CommandContext ctx)
    {
        Initialize(ctx);
        _tag = tag;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="FactionAdminChecks" /> class without a faction tag.
    /// </summary>
    /// <param name="ctx">The command context.</param>
    public FactionAdminChecks(CommandContext ctx)
    {
        Initialize(ctx);
    }

    FactionHanger FactionsHanger { get; set; }
    static Settings Config => Hangar.Config;

    /// <summary>
    ///     Initializes the command context and chat.
    /// </summary>
    /// <param name="ctx">The command context.</param>
    void Initialize(CommandContext ctx)
    {
        var inConsole = TryGetAdminPosition(ctx.Player);
        _chat = new Chat(ctx, inConsole);
        _ctx = ctx;
    }

    /// <summary>
    ///     Checks if the admin position is valid.
    /// </summary>
    /// <param name="admin">The admin player.</param>
    /// <returns>True if the admin position is invalid, otherwise false.</returns>
    bool TryGetAdminPosition(IMyPlayer admin)
    {
        return admin?.GetPosition() == null;
    }

    /// <summary>
    ///     Initializes the hangar for the specified faction tag.
    /// </summary>
    /// <param name="tag">The faction tag.</param>
    /// <returns>True if the hangar was successfully initialized, otherwise false.</returns>
    bool InitHangar(string tag)
    {
        _faction = MySession.Static.Factions.TryGetFactionByTag(tag);
        if (_faction == null) return false;
        var id = _faction.Members.First().Key;
        var sid = MySession.Static.Players.TryGetSteamId(id);
        FactionsHanger = new FactionHanger(sid, new Chat(_ctx));
        return true;
    }

    /// <summary>
    ///     Performs the main checks required before executing any faction operations.
    /// </summary>
    /// <returns>True if all checks pass, otherwise false.</returns>
    bool PerformMainChecks()
    {
        if (!Config.PluginEnabled)
        {
            _chat?.Respond("Plugin is not enabled!");
            return false;
        }

        if (_faction == null)
        {
            _chat?.Respond("Faction not found!");
            return false;
        }

        if (FactionHanger.IsServerSaving(_chat))
        {
            _chat?.Respond("Server is saving or is paused!");
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Changes the webhook URL for the faction.
    /// </summary>
    /// <param name="webhook">The new webhook URL.</param>
    public void ChangeWebhook(string webhook)
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        FactionsHanger.ChangeWebhook(webhook);
        _chat?.Respond("Webhook changed.");
    }

    /// <summary>
    ///     Changes the whitelist status of a player.
    /// </summary>
    /// <param name="targetNameOrSteamId">The target player's name or Steam ID.</param>
    public void ChangeWhitelist(string targetNameOrSteamId)
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        if (TryGetPlayerSteamId(targetNameOrSteamId, _chat, out var steamId))
        {
            var result = FactionsHanger.ChangeWhitelist(steamId);
            _chat?.Respond(result ? "Player added to whitelist." : "Player removed from whitelist.");
        }
        else
        {
            _chat?.Respond("Couldn't find that player!");
        }
    }

    /// <summary>
    ///     Saves the specified grid.
    /// </summary>
    /// <param name="name">The name of the grid.</param>
    public async Task SaveGrid(string name)
    {
        var result = new GridResult(true);
        if (!result.GetGrids(_chat, (MyCharacter)_ctx.Player?.Character, name)) return;

        if (result.OwnerSteamId == 0)
        {
            _chat?.Respond("Unable to get major grid owner!");
            return;
        }

        _faction = MySession.Static.Factions.GetPlayerFaction(result.BiggestOwner);
        FactionsHanger = new FactionHanger(result.OwnerSteamId, _chat, true);

        if (!PerformMainChecks()) return;

        var gridData = result.GenerateGridStamp();
        FactionsHanger.SelectedFactionFile.FormatGridName(gridData);

        var val = await FactionsHanger.SaveGridsToFile(result, gridData.GridName, _faction.Members.First().Key);
        if (val)
        {
            FactionsHanger.SaveGridStamp(gridData);
            _chat?.Respond("Save Complete!");
            FactionsHanger.SendWebHookMessage($"Admin saved grid {gridData.GridName}");
        }
        else
        {
            _chat?.Respond("Saved Failed!");
        }
    }

    /// <summary>
    ///     Lists all whitelisted players for the faction.
    /// </summary>
    public void ListWhitelist()
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        FactionsHanger.ListAllWhitelisted();
    }

    /// <summary>
    ///     Lists all grids for the faction.
    /// </summary>
    public void ListGrids()
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        FactionsHanger.ListAllGrids();
    }

    /// <summary>
    ///     Provides detailed information about a specific grid.
    /// </summary>
    /// <param name="input">The grid identifier.</param>
    public void DetailedInfo(string input)
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        if (!FactionsHanger.ParseInput(input, out var id))
        {
            _chat.Respond($"Grid {input} could not be found!");
            return;
        }

        FactionsHanger.DetailedReport(id);
    }

    /// <summary>
    ///     Loads a grid for the faction.
    /// </summary>
    /// <param name="input">The grid identifier.</param>
    /// <param name="loadNearPlayer">Whether to load the grid near the player.</param>
    public void LoadGrid(string input, bool loadNearPlayer)
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        if (!PerformMainChecks()) return;

        if (!FactionsHanger.ParseInput(input, out var id))
        {
            _chat.Respond($"Grid {input} could not be found!");
            return;
        }

        if (!FactionsHanger.TryGetGridStamp(id, out var stamp)) return;

        if (!FactionsHanger.LoadGrid(stamp, out var grids, _faction.Members.First().Key))
        {
            Log.Error($"Loading grid {id} failed for Admin!");
            _chat.Respond("Loading grid failed! Report this to staff and check logs for more info!");
            return;
        }

        var myObjectBuilderCubeGrids = grids as MyObjectBuilder_CubeGrid[] ?? grids.ToArray();
        PluginDependencies.BackupGrid(myObjectBuilderCubeGrids.ToList(), _faction.Members.First().Key);

        var pos = _ctx.Player?.Character?.GetPosition();
        var spawnPos = DetermineSpawnPosition(stamp.GridSavePosition, pos.GetValueOrDefault(),
            out var keepOriginalPosition, loadNearPlayer);

        if (PluginDependencies.NexusInstalled && Config.NexusApi && NexusSupport.RelayLoadIfNecessary(spawnPos, id,
                loadNearPlayer, _chat, _ctx.Player!.SteamUserId, _faction.Members.First().Key,
                _ctx.Player!.GetPosition())) return;

        var sid = MySession.Static.Players.TryGetSteamId(_faction.Members.First().Key);
        var spawner = new ParallelSpawner(myObjectBuilderCubeGrids, _chat, sid, SpawnedGridsSuccessful);
        spawner.setBounds(stamp.BoundingBox, stamp.Box, stamp.MatrixTranslation);

        Log.Info("Attempting Grid Spawning @" + spawnPos);
        if (!spawner.Start(spawnPos, keepOriginalPosition)) return;
        _chat?.Respond("Spawning Complete!");
        FactionsHanger.RemoveGridStamp(id);
        FactionsHanger.SendWebHookMessage("Admin loaded grid {stamp.GridName}");
    }

    /// <summary>
    ///     Removes a grid from the faction.
    /// </summary>
    /// <param name="input">The grid identifier.</param>
    public void RemoveGrid(string input)
    {
        if (!InitHangar(_tag))
        {
            _chat?.Respond("That faction does not exist!");
            return;
        }

        if (!FactionsHanger.ParseInput(input, out var id))
        {
            _chat.Respond($"Grid {input} could not be found!");
            return;
        }

        if (FactionsHanger.RemoveGridStamp(id))
            _chat.Respond("Successfully removed grid!");
    }

    /// <summary>
    ///     Callback method for successful grid spawning.
    /// </summary>
    /// <param name="grids">The set of spawned grids.</param>
    void SpawnedGridsSuccessful(HashSet<MyCubeGrid> grids)
    {
        grids.BiggestGrid(out var biggestGrid);

        if (_ctx.Player == null) return;

        if (biggestGrid != null && _ctx.Player?.IdentityId != 0)
            new GpsSender().SendGps(biggestGrid.PositionComp.GetPosition(), biggestGrid.DisplayName,
                _ctx.Player.IdentityId);
    }

    /// <summary>
    ///     Determines the spawn position for a grid.
    /// </summary>
    /// <param name="gridPosition">The original grid position.</param>
    /// <param name="characterPosition">The character's position.</param>
    /// <param name="keepOriginalPosition">Whether to keep the original position.</param>
    /// <param name="playersSpawnNearPlayer">Whether to spawn near the player.</param>
    /// <returns>The determined spawn position.</returns>
    static Vector3D DetermineSpawnPosition(Vector3D gridPosition, Vector3D characterPosition,
        out bool keepOriginalPosition, bool playersSpawnNearPlayer = false)
    {
        switch (Config.LoadType)
        {
            case LoadType.ForceLoadNearOriginalPosition when gridPosition == Vector3D.Zero:
                Log.Info("Grid position is empty!");
                keepOriginalPosition = false;
                return characterPosition;
            case LoadType.ForceLoadNearOriginalPosition:
                Log.Info("Loading from grid save position!");
                keepOriginalPosition = true;
                return gridPosition;
            case LoadType.ForceLoadMearPlayer when characterPosition == Vector3D.Zero:
                keepOriginalPosition = true;
                return gridPosition;
            case LoadType.ForceLoadMearPlayer:
                keepOriginalPosition = false;
                return characterPosition;
        }

        if (playersSpawnNearPlayer)
        {
            keepOriginalPosition = false;
            return characterPosition;
        }

        keepOriginalPosition = true;
        return gridPosition;
    }
}