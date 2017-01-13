using System.Collections.Generic;
using System.Linq;

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

        public void FullyHealPlayer()
        {
            CurrentHitPoints = MaximumHitPoints;
        }

        public bool HasRequiredItemToEnterLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null) return true;
            return Inventory.Exists(inventoryItem => inventoryItem.Details.ID == location.ItemRequiredToEnter.ID);
        }

        public bool HasThisQuest(Quest quest)
        {
            return Quests.Exists(playerQuest => playerQuest.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(q => q.Details.ID == quest.ID);
            if (playerQuest == null) return false;
            return playerQuest.IsCompleted;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                if (!Inventory.Exists(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                InventoryItem item = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);
                if (item != null) item.Quantity -= qci.Quantity;
            }
        }

        public void AddItemToInventory(Item item)
        {
            InventoryItem inventoryItem = Inventory.SingleOrDefault(ii => ii.Details.ID == item.ID);
            if (inventoryItem == null)
            {
                Inventory.Add(new InventoryItem(item, 1));
            }
            else
            {
                inventoryItem.Quantity++;
            }
        }

        public void RemoveItemFromInventory(Item item)
        {
            InventoryItem inventoryItem = Inventory.SingleOrDefault(ii => ii.Details.ID == item.ID);
            if (inventoryItem != null) inventoryItem.Quantity--;
        }

        public void MarkQuestCompleted(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(q => q.Details.ID == quest.ID);
            if (playerQuest != null) playerQuest.IsCompleted = true;
        }
    }
}
