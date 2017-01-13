using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class QuestCompletionItemTests
    {
        Item item = new Item(999, "item", "items");
        int quantity = 1;

        [TestMethod()]
        public void QuestCompletionItemTest()
        {
            QuestCompletionItem questCompletionItem = new QuestCompletionItem(item, quantity);
            Assert.AreEqual(questCompletionItem.Details, item);
            Assert.AreEqual(questCompletionItem.Quantity, quantity);
        }
    }
}