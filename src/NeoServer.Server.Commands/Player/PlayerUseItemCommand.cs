﻿using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Players;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Server.Commands.Player
{

    public class PlayerUseItemCommand : Command
    {
        private readonly Game game;
        private UseItemPacket useItemPacket;
        private readonly IPlayer player;

        public PlayerUseItemCommand(IPlayer player, Game game, UseItemPacket useItemPacket)
        {
            this.game = game;
            this.player = player;
            this.useItemPacket = useItemPacket;
        }

        public override void Execute()
        {

            var container = player.OpenContainerAt(useItemPacket.Location, useItemPacket.Index, out var alreadyOpened);

            if (game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection))
            {
                if (alreadyOpened)
                {
                    player.CloseContainer(container.Id);
                    connection.OutgoingPackets.Enqueue(new CloseContainerPacket(container.Id));
                }
                else
                {
                    connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container));
                }
                connection.Send();
            }

        }


    }
}
