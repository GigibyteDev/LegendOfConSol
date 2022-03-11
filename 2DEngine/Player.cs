using System;
using System.Collections.Generic;
using System.Linq;

namespace _2DEngine
{
    public class Player
    {
        public string Name { get; set; } = "Battler";
        public Vector2 Loc { get; set; } = new Vector2(Console.WindowWidth / 2, Console.WindowHeight / 2);
        public string[] Icon { get; set; } = new string[]
        {
            @" O",
            @"/|\",
            @"/ \",
        };

        public List<IItem> Inventory { get; set; } = new List<IItem>();

        public int MaxHealth { get; set; } = 100;
        public int Health { get; set; } = 100;
        public int Attack { get; set; } = 10;

        public int Randomness { get; set; } = 5;

        /// <summary>
        /// Moves Player Coords In Direction Sent As Char
        /// </summary>
        /// <param name="dir"></param>
        public void Walk(char dir)
        {
            switch (dir)
            {
                case 'u':
                    if (Loc.Y - 2 > 0)
                        Loc.Y -= 2;
                    break;
                case 'd':
                    if ((Loc.Y + 3) + 2 < Console.WindowHeight)
                        Loc.Y  += 2;
                    break;
                case 'l':
                    if ((Loc.X - 3) - 4 > 0)
                        Loc.X -= 4;
                    break;
                case 'r':
                    if ((Loc.X + 3) + 4 < Console.WindowWidth)
                        Loc.X += 4;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Adds Sent Item Interface to player inventory
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(IItem item)
        {
            if (item is Equippable equip)
            {
                bool exists = false;
                foreach (IItem invItem in Inventory)
                {
                    if (invItem is Equippable invEquip)
                    {
                        if (invEquip.Name == item.Name)
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                if (!exists)
                    Inventory.Add(equip);
            }
            else if (item is BattleItem bItem)
            {
                bool exists = false;
                for (int i = 0; i < Inventory.Count(); i++)
                {
                    if (Inventory[i] is BattleItem invBItem)
                    {
                        if (invBItem.Name == bItem.Name)
                        {
                            if (invBItem.Amt + bItem.Amt > 99)
                                invBItem.Amt = 99;
                            else
                                invBItem.Amt += bItem.Amt;
                            exists = true;
                            break;
                        }
                    }
                }
                if (!exists)
                    Inventory.Add(item);
            }
        }
        /// <summary>
        /// Adds Sent Item(s) By Name To Player Inventory (Item Must Already Exist)
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="amt"></param>
        public void AddItem(string itemName, int amt)
        {
            foreach(IItem item in Inventory)
            {
                if (item is BattleItem bItem)
                {
                    if (item.Name == itemName)
                    {
                        if (bItem.Amt + amt > 99)
                            bItem.Amt = 99;
                        else
                            bItem.Amt += amt;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Removes Item(s) by name from player inventory if they exist
        /// </summary>
        /// <param name="itemName"></param>
        /// <param name="amountToRemove"></param>
        public void RemoveItem(string itemName, int amountToRemove = 1)
        {
            foreach (IItem item in Inventory)
            {
                if (item is BattleItem bItem)
                {
                    if (bItem.Name == itemName)
                    {
                        if (bItem.Amt - amountToRemove < 0)
                            bItem.Amt = 0;
                        else
                            bItem.Amt -= amountToRemove;
                    }
                }
                else if (item is Equippable eItem)
                {
                    if (item.Name == itemName)
                    {
                        Inventory.Remove(eItem);
                    }
                }
            }
        }
    }
}
