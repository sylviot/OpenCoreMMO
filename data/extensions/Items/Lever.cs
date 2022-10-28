﻿using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.World.Map;
using NeoServer.Game.World.Models.Tiles;

namespace NeoServer.Extensions.Items;

public class Lever : BaseItem, IUsable
{
    public Lever(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
        metadata, location)
    {
    }

    public void Use(IPlayer player)
    {
        if (Map.Instance[Location] is not DynamicTile tile) return;

        SwitchLever(tile);
    }

    private void SwitchLever(DynamicTile dynamicTile)
    {
        var newLeverId = (ushort)(Metadata.TypeId == 1946 ? 1945 : 1946);
        var newLever = ItemFactory.Instance.Create(newLeverId, Location, null);

        dynamicTile.RemoveItem(this, 1, out _);
        dynamicTile.AddItem(newLever);
    }
}