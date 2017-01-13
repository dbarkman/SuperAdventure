using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class ItemTests
    {
        int id = 999;
        string name = "item";
        string namePlural = "items";

        [TestMethod()]
        public void ItemTest()
        {
            Item item = new Item(id, name, namePlural);
            Assert.AreEqual(item.ID, id);
            Assert.AreEqual(item.Name, name);
            Assert.AreEqual(item.NamePlural, namePlural);
        }
    }
}