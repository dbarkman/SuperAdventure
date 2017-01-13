using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class PlayerQuestTests
    {
        Quest quest = new Quest(999, "quest", "quest", 1, 1);

        [TestMethod()]
        public void PlayerQuestTest()
        {
            PlayerQuest playerQuest = new PlayerQuest(quest);
            Assert.AreEqual(playerQuest.Details, quest);
            Assert.IsFalse(playerQuest.IsCompleted);
        }
    }
}