using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Tests
{
    [TestClass()]
    public class PlayerTests
    {
        [TestMethod()]
        public void CreateDefaultPlayerTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Assert.AreEqual(player.CurrentHitPoints, World.DEFAULT_CURRENT_HIT_POINTS);
            Assert.AreEqual(player.MaximumHitPoints, World.DEFAULT_MAXIMUM_HIT_POINTS);
            Assert.AreEqual(player.Gold, World.DEFAULT_GOLD);
            Assert.AreEqual(player.ExperiencePoints, World.DEFAULT_EXPERIENCE_POINTS);
        }

        [TestMethod()]
        public void CreatePlayerFromXmlStringTest()
        {
            string xmlPlayerData = "<Player><Stats><CurrentHitPoints>" + World.DEFAULT_CURRENT_HIT_POINTS + "</CurrentHitPoints><MaximumHitPoints>" + World.DEFAULT_MAXIMUM_HIT_POINTS + "</MaximumHitPoints><Gold>" + World.DEFAULT_GOLD + "</Gold><ExperiencePoints>" + World.DEFAULT_EXPERIENCE_POINTS + "</ExperiencePoints><CurrentLocation>" + World.LocationByID(World.LOCATION_ID_HOME) + "</CurrentLocation><CurrentWeapon>" + (Weapon)World.ItemByID(World.ITEM_ID_RUSTY_SWORD) + "</CurrentWeapon></Stats><InventoryItems><InventoryItem ID=\"1\" Quantity=\"1\" /></InventoryItems><PlayerQuests /></Player>";
            Player player = Player.CreatePlayerFromXmlString(xmlPlayerData);
            Assert.AreEqual(player.CurrentHitPoints, World.DEFAULT_CURRENT_HIT_POINTS);
            Assert.AreEqual(player.MaximumHitPoints, World.DEFAULT_MAXIMUM_HIT_POINTS);
            Assert.AreEqual(player.Gold, World.DEFAULT_GOLD);
            Assert.AreEqual(player.ExperiencePoints, World.DEFAULT_EXPERIENCE_POINTS);
            Assert.AreEqual(player.CurrentLocation, World.LocationByID(World.LOCATION_ID_HOME));
            Assert.AreEqual(player.CurrentWeapon, (Weapon)World.ItemByID(World.ITEM_ID_RUSTY_SWORD));
            Assert.AreEqual(player.Inventory.Count, 1);
        }

        [TestMethod()]
        public void FullyHealPlayerTest()
        {
            Player player = Player.CreateDefaultPlayer(1, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            int expectedHitPoints = player.MaximumHitPoints;
            player.FullyHealPlayer();
            Assert.AreEqual(player.CurrentHitPoints, expectedHitPoints);
        }

        [TestMethod()]
        public void AddExperiencePointsTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            player.AddExperiencePoints(10);
            Assert.AreEqual(player.ExperiencePoints, 10);
        }

        [TestMethod()]
        public void HasRequiredItemToEnterLocationTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Item item = new Item(999, "thing1", "things1");
            InventoryItem ii = new InventoryItem(item, 1);
            player.Inventory.Add(ii);
            Location location = new Location(998, "location", "location", item, null, null);
            Assert.IsTrue(player.HasRequiredItemToEnterLocation(location));
        }

        [TestMethod()]
        public void HasThisQuestTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Quest quest = new Quest(999, "quest", "quest", 1, 1);
            PlayerQuest playerQuest = new PlayerQuest(quest);
            playerQuest.IsCompleted = false;
            player.Quests.Add(playerQuest);
            Assert.IsTrue(player.HasThisQuest(quest));
        }

        [TestMethod()]
        public void CompletedThisQuestTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Quest quest = new Quest(999, "quest", "quest", 1, 1);
            PlayerQuest playerQuest = new PlayerQuest(quest);
            playerQuest.IsCompleted = true;
            player.Quests.Add(playerQuest);
            Assert.IsTrue(player.CompletedThisQuest(quest));
        }

        [TestMethod()]
        public void HasAllQuestCompletionItemsTest()
        {
            Quest quest = new Quest(999, "quest", "quest", 1, 1);
            Item questItem = new Item(998, "thing1", "things1");
            QuestCompletionItem questCompletionItem = new QuestCompletionItem(questItem, 1);
            List<QuestCompletionItem> list = new List<QuestCompletionItem>() { questCompletionItem };
            quest.QuestCompletionItems = list;

            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            InventoryItem inventoryItem = new InventoryItem(questItem, 1);
            player.Inventory.Add(inventoryItem);

            Assert.IsTrue(player.HasAllQuestCompletionItems(quest));
        }

        [TestMethod()]
        public void RemoveQuestCompletionItemsTest()
        {
            Quest quest = new Quest(999, "quest", "quest", 1, 1);
            Item questItem = new Item(998, "thing1", "things1");
            QuestCompletionItem questCompletionItem = new QuestCompletionItem(questItem, 1);
            List<QuestCompletionItem> list = new List<QuestCompletionItem>() { questCompletionItem };
            quest.QuestCompletionItems = list;

            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            InventoryItem inventoryItem1 = new InventoryItem(questItem, 1);
            player.Inventory.Add(inventoryItem1);

            player.RemoveQuestCompletionItems(quest);
            InventoryItem item = player.Inventory.SingleOrDefault(inventoryItem2 => inventoryItem2.Details.ID == questCompletionItem.Details.ID);
            Assert.AreEqual(item.Quantity, 0);
        }

        [TestMethod()]
        public void AddItemToInventoryTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Item item = new Item(999, "thing1", "things1");

            player.AddItemToInventory(item);
            Assert.AreEqual(player.Inventory.Count, 2);
            InventoryItem inventoryItem = player.Inventory.SingleOrDefault(ii => ii.Details.ID == 999);
            Assert.AreEqual(inventoryItem.Quantity, 1);

            player.AddItemToInventory(item);
            Assert.AreEqual(player.Inventory.Count, 2);
            inventoryItem = player.Inventory.SingleOrDefault(ii => ii.Details.ID == 999);
            Assert.AreEqual(inventoryItem.Quantity, 2);
        }

        [TestMethod()]
        public void RemoveItemFromInventoryTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Item item = new Item(999, "thing1", "things1");
            InventoryItem inventoryItem = new InventoryItem(item, 1);

            player.Inventory.Add(inventoryItem);
            player.RemoveItemFromInventory(item);
            inventoryItem = player.Inventory.SingleOrDefault(ii => ii.Details.ID == 999);
            Assert.AreEqual(inventoryItem.Quantity, 0);
            Assert.AreEqual(player.Inventory.Count, 2);
        }

        [TestMethod()]
        public void MarkQuestCompletedTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            Quest quest = new Quest(999, "quest", "quest", 1, 1);
            PlayerQuest playerQuest = new PlayerQuest(quest);
            playerQuest.IsCompleted = false;
            player.Quests.Add(playerQuest);
            player.MarkQuestCompleted(quest);
            Assert.IsTrue(playerQuest.IsCompleted);
        }

        [TestMethod()]
        public void ToXmlStringTest()
        {
            Player player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            string xmlDocument = player.ToXmlString();
            Assert.AreEqual(xmlDocument, "<Player><Stats><CurrentHitPoints>10</CurrentHitPoints><MaximumHitPoints>10</MaximumHitPoints><Gold>20</Gold><ExperiencePoints>0</ExperiencePoints><CurrentLocation>1</CurrentLocation><CurrentWeapon>1</CurrentWeapon></Stats><InventoryItems><InventoryItem ID=\"1\" Quantity=\"1\" /></InventoryItems><PlayerQuests /></Player>");
        }
    }
}