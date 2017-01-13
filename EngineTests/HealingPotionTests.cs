using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class HealingPotionTests
    {
        int id = 999;
        string name = "healingPotion";
        string namePlural = "healingPotions";
        int amountToHeal = 10;

        [TestMethod()]
        public void HealingPotionTest()
        {
            HealingPotion healingPotion = new HealingPotion(id, name, namePlural, amountToHeal);
            Assert.AreEqual(healingPotion.ID, id);
            Assert.AreEqual(healingPotion.Name, name);
            Assert.AreEqual(healingPotion.NamePlural, namePlural);
            Assert.AreEqual(healingPotion.AmountToHeal, amountToHeal);
        }
    }
}