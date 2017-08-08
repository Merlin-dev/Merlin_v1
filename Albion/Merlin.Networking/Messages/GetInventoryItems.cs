using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.Networking.Messages
{
    [Serializable]
    public struct GetInventoryItemsRequest : IMessage
    {
        public ItemType Type;

        public GetInventoryItemsRequest(ItemType type = ItemType.All)
        {
            Type = type;
        }
    }

    [Serializable]
    public struct GetInventoryItemsResponse : IMessage
    {
        public List<InventoryItem> Items;

        public GetInventoryItemsResponse(List<InventoryItem> items)
        {
            Items = items;
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public string Name;
        public int Count;
        public int MaxStack;

        public InventoryItem(string name, int count, int maxStack = 999)
        {
            Name = name;
            Count = count;
            MaxStack = maxStack;
        }
    }

    [Serializable, Flags]
    public enum ItemType
    {
        Tool,
        Item,

        All = Tool | Item
    }
}
