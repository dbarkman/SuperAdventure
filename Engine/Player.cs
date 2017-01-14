using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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

        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int level) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;
            Level = level;

            Inventory = new List<InventoryItem>();
            Quests = new List<PlayerQuest>();
        }

        public static Player CreateDefaultPlayer(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int level)
        {
            Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();

                playerData.LoadXml(xmlPlayerData);

                int currentHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHitPoints").InnerText);
                int maximumHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaximumHitPoints").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/ExperiencePoints").InnerText);
                int level = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Level").InnerText);

                Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);

                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);

                foreach (XmlNode node in playerData.SelectNodes("/Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for (int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;

                    player.Quests.Add(playerQuest);
                }
                return player;
            }
            catch
            {
                return Player.CreateDefaultPlayer(10, 10, 20, 0, 1);
            }
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

        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            // Create the top-level XML node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            // Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);

            // Create the child nodes for the "Stats" node
            XmlNode currentHitPoints = playerData.CreateElement("CurrentHitPoints");
            currentHitPoints.AppendChild(playerData.CreateTextNode(this.CurrentHitPoints.ToString()));
            stats.AppendChild(currentHitPoints);

            XmlNode maximumHitPoints = playerData.CreateElement("MaximumHitPoints");
            maximumHitPoints.AppendChild(playerData.CreateTextNode(this.MaximumHitPoints.ToString()));
            stats.AppendChild(maximumHitPoints);

            XmlNode gold = playerData.CreateElement("Gold");
            gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
            stats.AppendChild(gold);

            XmlNode experiencePoints = playerData.CreateElement("ExperiencePoints");
            experiencePoints.AppendChild(playerData.CreateTextNode(this.ExperiencePoints.ToString()));
            stats.AppendChild(experiencePoints);

            XmlNode level = playerData.CreateElement("Level");
            level.AppendChild(playerData.CreateTextNode(this.Level.ToString()));
            stats.AppendChild(level);

            XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
            currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
            stats.AppendChild(currentLocation);

            // Create the "InventoryItems" child node to hold each InventoryItem node
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            // Create an "InventoryItem" node for each item in the player's inventory
            foreach (InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append(idAttribute);

                XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append(quantityAttribute);

                inventoryItems.AppendChild(inventoryItem);
            }

            // Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            // Create a "PlayerQuest" node for each quest the player has acquired
            foreach (PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append(idAttribute);

                XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append(isCompletedAttribute);

                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml; // The XML document, as a string, so we can save
        }
    }
}
