using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Player : LivingCreature
    {
        public int Gold { get; set; }
        public int ExperiencePoints { get; set; }
        public int Level { get; set; }
        public Location CurrentLocation { get; set; }
        public List<InventoryItem> Inventory { get; set; }
        public List<PlayerQuest> Quests { get; set; }

        public Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int level) : 
            base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        public bool HasRequiredItemToEnterLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null)
            {
                return true;
            }
            return Inventory.Exists(inventoryItem => inventoryItem.Details.ID == location.ItemRequiredToEnter.ID);
        }

        public bool HasThisQuest(Quest quest)
        {
            return Quests.Exists(playerQuest => playerQuest.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            //return Quests.Exists(playerQuest => playerQuest.Details.ID == quest.ID);
            foreach (PlayerQuest playerQuest in Quests)
            {
                if (playerQuest.Details.ID == quest.ID)
                {
                    return playerQuest.IsCompleted; //<-- return this, not just true or false
                }
            }
            return false;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                if (!Inventory.Exists(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
                //bool foundItemInPlayersInventory = false;

                //foreach (InventoryItem ii in Inventory)
                //{
                //    if (ii.Details.ID == qci.Details.ID)
                //    {
                //        foundItemInPlayersInventory = true;

                //        if (ii.Quantity < qci.Quantity)
                //        {
                //            return false;
                //        }
                //    }
                //}

                //if (!foundItemInPlayersInventory)
                //{
                //    return false;
                //}
            }
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                foreach (InventoryItem ii in Inventory)
                {
                    if (ii.Details.ID == qci.Details.ID)
                    {
                        ii.Quantity -= qci.Quantity;
                        break;
                    }
                }
            }
        }

        public void AddItemToInventory(Item item)
        {
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == item.ID)
                {
                    ii.Quantity++;
                    return;
                }
            }
            Inventory.Add(new InventoryItem(item, 1));
        }

        public void RemoveItemFromInventory(Item item)
        {
            foreach (InventoryItem ii in Inventory)
            {
                if (ii.Details.ID == item.ID)
                {
                    ii.Quantity--;
                    return;
                }
            }
        }

        public void MarkQuestCompleted(Quest quest)
        {
            foreach (PlayerQuest pq in Quests)
            {
                if (pq.Details.ID == quest.ID)
                {
                    pq.IsCompleted = true;
                    return;
                }
            }
        }
    }
}
