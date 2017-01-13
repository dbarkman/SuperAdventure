using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class QuestTests
    {
        int id = 999;
        string name = "quest";
        string description = "quest";
        int rewardExperiencePoints = 1;
        int rewardGold = 1;

        [TestMethod()]
        public void QuestTest()
        {
            Quest quest = new Quest(id, name, description, rewardExperiencePoints, rewardGold);
            Assert.AreEqual(quest.ID, id);
            Assert.AreEqual(quest.Name, name);
            Assert.AreEqual(quest.Description, description);
            Assert.AreEqual(quest.RewardExperiencePoints, rewardExperiencePoints);
            Assert.AreEqual(quest.RewardGold, rewardGold);
        }
    }
}