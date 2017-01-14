using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class RandomNumberGeneratorTests
    {
        int minimumValue = 1;
        int maximumValue = 100;

        [TestMethod()]
        public void NumberBetweenTest()
        {
            int number = RandomNumberGenerator.NumberBetween(minimumValue, maximumValue);
            Assert.AreNotEqual(number, 0);
            Assert.IsNotNull(number);
            Assert.IsInstanceOfType(number, typeof(int));
        }
    }
}