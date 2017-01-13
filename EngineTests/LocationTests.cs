using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class LocationTests
    {
        int id = 999;
        string name = "location";
        string description = "location";
        Item item = new Item(998, "item", "items");
        Quest quest = new Quest(997, "quest", "quest", 1, 1);
        Monster monster = new Monster(996, "monster", 10, 1, 1, 10, 10);

        [TestMethod()]
        public void LocationTest()
        {
            Location location = new Location(id, name, description, item, quest, monster);
            Assert.AreEqual(location.ID, id);
            Assert.AreEqual(location.Name, name);
            Assert.AreEqual(location.Description, description);
            Assert.AreEqual(location.ItemRequiredToEnter, item);
            Assert.AreEqual(location.QuestAvailableHere, quest);
            Assert.AreEqual(location.MonsterLivingHere, monster);
        }
    }
}