﻿using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Systems.Depot;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedOutEventHandler : IEventHandler
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerDepotItemRepository _playerDepotItemRepository;
    private readonly DepotManager _depotManager;
    private readonly IGameServer _gameServer;

    public PlayerLoggedOutEventHandler(IPlayerRepository playerRepository,
        IPlayerDepotItemRepository playerDepotItemRepository,
        DepotManager depotManager, IGameServer gameServer)
    {
        _playerRepository = playerRepository;
        _playerDepotItemRepository = playerDepotItemRepository;
        _depotManager = depotManager;
        _gameServer = gameServer;
    }

    public void Execute(IPlayer player)
    {
        SavePlayer(player);
    }

    private void SavePlayer(IPlayer player)
    {

        _playerRepository.SavePlayer(player);
        _playerRepository.UpdatePlayerOnlineStatus(player.Id, false).Wait();
        SaveDepot(player);
    }

    private void SaveDepot(IPlayer player)
    {
        if (!_depotManager.Get(player.Id, out var depot)) return;
        _playerDepotItemRepository.Save(player, depot).Wait();

        _depotManager.Unload(player.Id);
    }
}