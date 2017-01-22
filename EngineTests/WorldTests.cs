using Microsoft.VisualStudio.TestTools.UnitTesting;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Tests
{
    [TestClass()]
    public class WorldTests
    {
        [TestMethod()]
        public void ItemByIDTest()
        {
            int id = World.ItemByID(World.ITEM_ID_RUSTY_SWORD).ID;
            Assert.AreEqual(id, 1);
        }

        [TestMethod()]
        public void MonsterByIDTest()
        {
            int id = World.MonsterByID(World.MONSTER_ID_RAT).ID;
            Assert.AreEqual(id, 1);
        }

        [TestMethod()]
        public void QuestByIDTest()
        {
            int id = World.QuestByID(World.QUEST_ID_CLEAR_ALCHEMIST_GARDEN).ID;
            Assert.AreEqual(id, 1);
        }

        [TestMethod()]
        public void LocationByIDTest()
        {
            int id = World.LocationByID(World.LOCATION_ID_HOME).ID;
            Assert.AreEqual(id, 1);
        }
    }
}