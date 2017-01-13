using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Tests
{
    [TestClass()]
    public class WeaponTests
    {
        int id = 999;
        string name = "weapon";
        string namePlural = "weapons";
        int minimumDamage = 1;
        int maximumDamage = 10;

        [TestMethod()]
        public void WeaponTest()
        {
            Weapon weapon = new Weapon(id, name, namePlural, minimumDamage, maximumDamage);
            Assert.AreEqual(weapon.ID, id);
            Assert.AreEqual(weapon.Name, name);
            Assert.AreEqual(weapon.NamePlural, namePlural);
            Assert.AreEqual(weapon.MinimumDamage, minimumDamage);
            Assert.AreEqual(weapon.MaximumDamage, maximumDamage);
        }
    }
}