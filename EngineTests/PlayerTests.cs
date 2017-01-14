using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Engine.Tests
{
    [TestClass()]
    public class PlayerTests
    {
        int currentHitPoints = 10;
        int maximumHitPoints = 10;
        int gold = 20;
        int experiencePoints = 0;
        int level = 1;

        [TestMethod()]
        public void PlayerTest()
        {
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
            Assert.AreEqual(player.CurrentHitPoints, currentHitPoints);
            Assert.AreEqual(player.MaximumHitPoints, maximumHitPoints);
            Assert.AreEqual(player.Gold, gold);
            Assert.AreEqual(player.ExperiencePoints, experiencePoints);
            Assert.AreEqual(player.Level, level);
        }

        [TestMethod()]
        public void FullyHealPlayerTest()
        {
            Player player = Player.CreateDefaultPlayer(1, maximumHitPoints, gold, experiencePoints, level);
            int expectedHitPoints = player.MaximumHitPoints;
            player.FullyHealPlayer();
            Assert.AreEqual(player.CurrentHitPoints, expectedHitPoints);
        }

        [TestMethod()]
        public void HasRequiredItemToEnterLocationTest()
        {
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
            Item item = new Item(999, "thing1", "things1");
            InventoryItem ii = new InventoryItem(item, 1);
            player.Inventory.Add(ii);
            Location location = new Location(998, "location", "location", item, null, null);
            Assert.IsTrue(player.HasRequiredItemToEnterLocation(location));
        }

        [TestMethod()]
        public void HasThisQuestTest()
        {
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
            Quest quest = new Quest(999, "quest", "quest", 1, 1);
            PlayerQuest playerQuest = new PlayerQuest(quest);
            playerQuest.IsCompleted = false;
            player.Quests.Add(playerQuest);
            Assert.IsTrue(player.HasThisQuest(quest));
        }

        [TestMethod()]
        public void CompletedThisQuestTest()
        {
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
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

            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
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

            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
            InventoryItem inventoryItem1 = new InventoryItem(questItem, 1);
            player.Inventory.Add(inventoryItem1);

            player.RemoveQuestCompletionItems(quest);
            InventoryItem item = player.Inventory.SingleOrDefault(inventoryItem2 => inventoryItem2.Details.ID == questCompletionItem.Details.ID);
            Assert.AreEqual(item.Quantity, 0);
        }

        [TestMethod()]
        public void AddItemToInventoryTest()
        {
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
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
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
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
            Player player = Player.CreateDefaultPlayer(currentHitPoints, maximumHitPoints, gold, experiencePoints, level);
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
            Assert.Fail();
        }

        [TestMethod()]
        public void CreateDefaultPlayerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CreatePlayerFromXmlStringTest()
        {
            Assert.Fail();
        }
    }
}