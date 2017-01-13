using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class LootItemTests
    {
        Item item = new Item(999, "item", "items");
        int dropPercentage = 50;
        bool isDefaultItem = true;

        [TestMethod()]
        public void LootItemTest()
        {
            LootItem lootItem = new LootItem(item, dropPercentage, isDefaultItem);
            Assert.AreEqual(lootItem.Details, item);
            Assert.AreEqual(lootItem.DropPercentage, dropPercentage);
            Assert.AreEqual(lootItem.IsDefaultItem, isDefaultItem);
        }
    }
}