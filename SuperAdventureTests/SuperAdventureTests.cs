using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;
using System.Collections.Generic;
using System.Linq;

namespace SuperAdventure.Tests
{
    [TestClass()]
    public class SuperAdventureTests
    {
        [TestMethod()]
        public void RewardPlayerForCompletingQuest()
        {
            SuperAdventure superAdventure = new SuperAdventure();
            var privateObject = new PrivateObject(superAdventure);

            Quest quest = new Quest(999, "test", "test", 1, 1);
            Item questItem = new Item(998, "thing1", "things1");
            Item rewardItem = new Item(997, "thing2", "things2");
            QuestCompletionItem qci = new QuestCompletionItem(questItem, 1);
            List<QuestCompletionItem> list = new List<QuestCompletionItem>() { qci };
            quest.RewardItem = rewardItem;
            quest.QuestCompletionItems = list;
            Location location = new Location(996, "test", "test", null, quest, null);

            Player player = Player.CreateDefaultPlayer(10, 10, 0, 0, 1);
            InventoryItem ii = new InventoryItem(questItem, 1);
            player.Inventory.Add(ii);
            PlayerQuest pq = new PlayerQuest(quest);
            pq.IsCompleted = false;
            player.Quests.Add(pq);

            privateObject.SetField("_player", player);
            privateObject.SetField("_location", location);
            privateObject.Invoke("RewardPlayerForCompletingQuest", location);

            InventoryItem item = player.Inventory.SingleOrDefault(i => i.Details.ID == 998);
            Assert.AreEqual(item.Quantity, 0);
            item = player.Inventory.SingleOrDefault(i => i.Details.ID == 997);
            Assert.AreEqual(item.Quantity, 1);

            Assert.AreEqual(player.ExperiencePoints, 1);
            Assert.AreEqual(player.Gold, 1);

            Assert.IsTrue(pq.IsCompleted);
        }
    }
}