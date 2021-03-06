﻿using BeauData;
using UnityEngine.Scripting;

namespace ProtoAqua.Shop
{
    public class ItemSet : ISerializedObject
    {
        public Item[] ItemData;

        [Preserve]
        public ItemSet() { }

        // Load the array of items from a given JSON file
        public void Serialize(Serializer ioSerializer)
        {
            ioSerializer.ObjectArray("ItemData", ref ItemData);
        }
    }
}
