using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class InventoryItemTests
    {
        Item item = new Item(999, "item", "items");
        int quantity = 1;

        [TestMethod()]
        public void InventoryItemTest()
        {
            InventoryItem inventoryItem = new InventoryItem(item, quantity);
            Assert.AreEqual(inventoryItem.Details, item);
            Assert.AreEqual(inventoryItem.Quantity, quantity);
        }
    }
}