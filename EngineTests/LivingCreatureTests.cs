using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class LivingCreatureTests
    {
        int currentHitPoints = 10;
        int maximumHitPoints = 10;

        [TestMethod()]
        public void LivingCreatureTest()
        {
            LivingCreature livingCreature = new LivingCreature(currentHitPoints, maximumHitPoints);
            Assert.AreEqual(livingCreature.CurrentHitPoints, currentHitPoints);
            Assert.AreEqual(livingCreature.MaximumHitPoints, maximumHitPoints);
        }
    }
}