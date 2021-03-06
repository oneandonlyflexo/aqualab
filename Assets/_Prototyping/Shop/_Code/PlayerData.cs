﻿using System.Collections.Generic;
using UnityEngine;

namespace ProtoAqua.Shop
{
    public class PlayerData : MonoBehaviour
    {
        public List<Item> PlayerInventory { get; set; }
        public int PlayerCurrency { get; set; }

        // Initialize inventory and currency
        private void Awake()
        {
            PlayerInventory = new List<Item>();
            PlayerCurrency = 1000;
        }
    }
}
