using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Merlin.Networking.Messages.Request
{
    [Serializable]
    public struct GetInventoryItems : IMessage
    {
        public ItemType Type;

        public GetInventoryItems(ItemType type = ItemType.All)
        {
            Type = type;
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
