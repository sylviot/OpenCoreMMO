﻿using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Items
{
    public class ContainerItem : MoveableItem, IContainerItem, IItem
    {
        public event RemoveItem OnItemRemoved;
        public event AddItem OnItemAdded;
        public byte SlotsUsed { get; private set; }
        public IContainerItem Parent { get; private set; }
        public bool HasParent => Parent != null;
        public byte Capacity { get; }
        public List<IItem> Items { get; }
        public IItem this[int index] => Items[index];

        public ContainerItem(IItemType type) : base(type)
        {
            Capacity = type.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Capacity);

            Items = new List<IItem>(Capacity);
        }

        public void SetParent(IContainerItem container)
        {
            Parent = container;
        }

        public static bool IsApplicable(IItemType type) => type.Group == Enums.ItemGroup.GroundContainer ||
            type.Attributes.GetAttribute(Enums.ItemAttribute.Type)?.ToLower() == "container";

        public bool GetContainerAt(byte index, out IContainerItem container)
        {
            container = null;
            if (Items[index] is IContainerItem)
            {
                container = Items[index] as IContainerItem;
                return true;
            }

            return false;
        }

        public bool TryAddItem(IItem item, byte? slot = null)
        {
            if (SlotsUsed + 1 >= Capacity)
            {
                return false;
            }

            if (slot != null && Items.Count > slot && Items[slot.Value] is IContainerItem container)
            {
                container.TryAddItem(item);
                return true;
            }

            Items.Insert(0, item);
            SlotsUsed++;
            OnItemAdded?.Invoke(item);
            return true;
        }
        public void MoveItem(byte fromSlotIndex, byte toSlotIndex)
        {
            if (GetContainerAt(toSlotIndex, out var container))
            {
                var item = RemoveItem(fromSlotIndex);
                container.TryAddItem(item);
                if (item is IContainerItem itemContainer)
                {
                    itemContainer.SetParent(container);
                }
            }
            if(fromSlotIndex > toSlotIndex)
            {
                var item = RemoveItem(fromSlotIndex);
                TryAddItem(item); 
            }
        }
        public IItem RemoveItem(byte slotIndex)
        {
            var item = Items[slotIndex];

            Items.RemoveAt(slotIndex);

            SlotsUsed--;

            OnItemRemoved?.Invoke(slotIndex, item);
            return item;
        }
    }
}
