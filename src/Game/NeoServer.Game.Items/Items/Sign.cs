﻿using System;
using System.Collections.Generic;
using System.Globalization;
using NeoServer.Game.Common.Contracts.Inspection;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Items.Bases;

namespace NeoServer.Game.Items.Items
{
    public class Sign : BaseItem, ISign
    {
        public Sign(IItemType metadata, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(
            metadata, location)
        {
            attributes.TryGetValue(ItemAttribute.Text, out var text);
            Text = text?.ToString(CultureInfo.InvariantCulture);
        }

        public override string GetLookText(IInspectionTextBuilder inspectionTextBuilder, bool isClose = false)
        {
            var lookText = base.GetLookText(inspectionTextBuilder, isClose);

            return string.IsNullOrWhiteSpace(Text) ? lookText : $"{lookText} You read: {Text}";
        }

        public string Text { get; }

        public static bool IsApplicable(IItemType type)
        {
            return type.Attributes.HasAttribute(ItemAttribute.Text) &&
                   !type.Flags.Contains(ItemFlag.Useable) ||
                   type.Attributes.GetAttribute(ItemAttribute.Type) == "sign";
        }
    }
}