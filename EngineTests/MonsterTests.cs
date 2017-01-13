using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class MonsterTests
    {
        int id = 999;
        string name = "monster";
        int maximumDamage = 10;
        int rewardExperiencePoints = 1;
        int rewardGold = 1;
        int currentHitPoints = 10;
        int maximumHitPoints = 10;

        [TestMethod()]
        public void MonsterTest()
        {
            Monster monster = new Monster(id, name, maximumDamage, rewardExperiencePoints, rewardGold, currentHitPoints, maximumHitPoints);
            Assert.AreEqual(monster.ID, id);
            Assert.AreEqual(monster.Name, name);
            Assert.AreEqual(monster.MaximumDamage, maximumDamage);
            Assert.AreEqual(monster.RewardExperiencePoints, rewardExperiencePoints);
            Assert.AreEqual(monster.RewardGold, rewardGold);
            Assert.AreEqual(monster.CurrentHitPoints, currentHitPoints);
            Assert.AreEqual(monster.MaximumHitPoints, maximumHitPoints);
        }
    }
}