using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperAdventure;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperAdventure.Tests
{
    [TestClass()]
    public class SuperAdventureTests
    {
        [TestMethod()]
        public void SuperAdventureTest()
        {
        }

        [TestMethod()]
        public void FullyHealPlayerTest()
        {
            SuperAdventure superAdventure = new SuperAdventure();
            var privateObject = new PrivateObject(superAdventure);
            Player player = new Player(1,10,20,0,1);
            int expectedHitPoints = player.MaximumHitPoints;

            privateObject.SetField("_player", player);
            privateObject.Invoke("FullyHealPlayer");

            Assert.AreEqual(player.CurrentHitPoints, expectedHitPoints);
        }
    }
}