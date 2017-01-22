using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;

namespace Engine
{
    public class Player : LivingCreature
    {
        private int _gold;
        private int _experiencePoints;
        private Location _currentLocation;
        private Monster _currentMonster;

        public event EventHandler<MessageEventArgs> OnMessage;

        public int Gold
        {
            get { return _gold; }
            set
            {
                _gold = value;
                OnPropertyChanged("Gold");
            }
        }

        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            private set
            {
                _experiencePoints = value;
                OnPropertyChanged("ExperiencePoints");
                OnPropertyChanged("Level");
            }
        }

        public int Level
        {
            get { return ((ExperiencePoints / 100) + 1); }
        }

        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }

        public Weapon CurrentWeapon { get; set; }

        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }

        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
        }

        public BindingList<InventoryItem> Inventory { get; set; }
        public BindingList<PlayerQuest> Quests { get; set; }

        private Player(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) : base(currentHitPoints, maximumHitPoints)
        {
            Gold = gold;
            ExperiencePoints = experiencePoints;

            Inventory = new BindingList<InventoryItem>();
            Quests = new BindingList<PlayerQuest>();
        }

        public static Player CreateDefaultPlayer(int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints)
        {
            Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints);
            player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));
            player.CurrentWeapon = (Weapon)World.ItemByID(World.ITEM_ID_RUSTY_SWORD);
            player.CurrentLocation = World.LocationByID(World.LOCATION_ID_HOME);

            return player;
        }

        public static Player CreatePlayerFromXmlString(string xmlPlayerData)
        {
            try
            {
                XmlDocument playerData = new XmlDocument();

                playerData.LoadXml(xmlPlayerData);

                int currentHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentHitPoints").InnerText);
                int maximumHitPoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/MaximumHitPoints").InnerText);
                int gold = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/Gold").InnerText);
                int experiencePoints = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/ExperiencePoints").InnerText);

                Player player = new Player(currentHitPoints, maximumHitPoints, gold, experiencePoints);

                int currentLocationID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentLocation").InnerText);
                player.CurrentLocation = World.LocationByID(currentLocationID);

                int currentWeaponID = Convert.ToInt32(playerData.SelectSingleNode("/Player/Stats/CurrentWeapon").InnerText);
                player.CurrentWeapon = (Weapon)World.ItemByID(currentWeaponID);

                foreach (XmlNode node in playerData.SelectNodes("/Player/InventoryItems/InventoryItem"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    int quantity = Convert.ToInt32(node.Attributes["Quantity"].Value);

                    for (int i = 0; i < quantity; i++)
                    {
                        player.AddItemToInventory(World.ItemByID(id));
                    }
                }

                foreach (XmlNode node in playerData.SelectNodes("/Player/PlayerQuests/PlayerQuest"))
                {
                    int id = Convert.ToInt32(node.Attributes["ID"].Value);
                    bool isCompleted = Convert.ToBoolean(node.Attributes["IsCompleted"].Value);

                    PlayerQuest playerQuest = new PlayerQuest(World.QuestByID(id));
                    playerQuest.IsCompleted = isCompleted;

                    player.Quests.Add(playerQuest);
                }
                return player;
            }
            catch
            {
                return Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            }
        }

        private void RaiseMessage(string message, bool addExtraNewLine = false, bool clearTextBox = false)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new Engine.MessageEventArgs(message, addExtraNewLine, clearTextBox));
            }
        }

        public void FullyHealPlayer()
        {
            CurrentHitPoints = MaximumHitPoints;
        }

        public void AddExperiencePoints(int experiencePoints)
        {
            ExperiencePoints += experiencePoints;
            MaximumHitPoints = (Level * World.PLAYER_HIT_POINT_LEVEL_MULTIPLIER);
        }

        public bool HasRequiredItemToEnterLocation(Location location)
        {
            if (location.ItemRequiredToEnter == null) return true;
            return Inventory.Any(inventoryItem => inventoryItem.Details.ID == location.ItemRequiredToEnter.ID);
        }

        public bool HasThisQuest(Quest quest)
        {
            return Quests.Any(playerQuest => playerQuest.Details.ID == quest.ID);
        }

        public bool CompletedThisQuest(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(q => q.Details.ID == quest.ID);
            if (playerQuest == null) return false;
            return playerQuest.IsCompleted;
        }

        public bool HasAllQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                if (!Inventory.Any(ii => ii.Details.ID == qci.Details.ID && ii.Quantity >= qci.Quantity))
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveQuestCompletionItems(Quest quest)
        {
            foreach (QuestCompletionItem qci in quest.QuestCompletionItems)
            {
                InventoryItem inventoryItem = Inventory.SingleOrDefault(ii => ii.Details.ID == qci.Details.ID);
                if (inventoryItem != null) RemoveItemFromInventory(inventoryItem.Details, qci.Quantity);
            }
        }

        public void MarkQuestCompleted(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(q => q.Details.ID == quest.ID);
            if (playerQuest != null) playerQuest.IsCompleted = true;
        }


        public void AddItemToInventory(Item item, int quantity = 1)
        {
            InventoryItem inventoryItem = Inventory.SingleOrDefault(ii => ii.Details.ID == item.ID);
            if (inventoryItem == null)
            {
                Inventory.Add(new InventoryItem(item, quantity));
            }
            else
            {
                inventoryItem.Quantity += quantity;
            }

            RaiseInventoryChangedEvent(item);
        }

        public void RemoveItemFromInventory(Item item, int quantity = 1)
        {
            InventoryItem inventoryItem = Inventory.SingleOrDefault(ii => ii.Details.ID == item.ID);
            if (inventoryItem != null) inventoryItem.Quantity -= quantity;
            if (inventoryItem.Quantity <= 0) Inventory.Remove(inventoryItem);

            RaiseInventoryChangedEvent(item);
        }

        private void RaiseInventoryChangedEvent(Item item)
        {
            if (item is Weapon)
            {
                OnPropertyChanged("Weapons");
            }
            if (item is HealingPotion)
            {
                OnPropertyChanged("Potions");
            }
        }

        public void Move(Location location)
        {
            MoveTo(location);
        }

        public void MoveNorth()
        {
            if (CurrentLocation.LocationToNorth != null)
            {
                MoveTo(CurrentLocation.LocationToNorth);
            }
        }

        public void MoveEast()
        {
            if (CurrentLocation.LocationToEast != null)
            {
                MoveTo(CurrentLocation.LocationToEast);
            }
        }

        public void MoveSouth()
        {
            if (CurrentLocation.LocationToSouth != null)
            {
                MoveTo(CurrentLocation.LocationToSouth);
            }
        }

        public void MoveWest()
        {
            if (CurrentLocation.LocationToWest != null)
            {
                MoveTo(CurrentLocation.LocationToWest);
            }
        }

        public void UseWeapon(Weapon weapon)
        {
            PlayerAttacks(weapon);

            if (_currentMonster.CurrentHitPoints <= 0)
            {
                MonsterDefeated();
            }
            else
            {
                MonsterAttacks();
            }

            if (CurrentHitPoints <= 0)
            {
                PlayerDefeated();
            }
        }

        public void UsePotion(HealingPotion healingPotion)
        {
            PlayerDrinksPotion(healingPotion);
            MonsterAttacks();

            if (CurrentHitPoints <= 0)
            {
                PlayerDefeated();
            }
        }

        private void MoveTo(Location newLocation, int respawn = 0)
        {
            if (!HasRequiredItemToEnterLocation(newLocation))
            {
                RaiseMessage("You must have an " + newLocation.ItemRequiredToEnter.Name + " to enter this location.", true);
                return;
            }

            CurrentLocation = newLocation;

            RaiseMessage("", false, true);

            FullyHealPlayer();

            if (newLocation.QuestAvailableHere != null)
            {
                if (HasThisQuest(newLocation.QuestAvailableHere))
                {
                    if (!CompletedThisQuest(newLocation.QuestAvailableHere) && HasAllQuestCompletionItems(newLocation.QuestAvailableHere))
                    {
                        RewardPlayerForCompletingQuest(newLocation);
                    }
                }
                else
                {
                    AssignPlayerQuest(newLocation);
                }
            }

            if (CurrentLocation.MonsterLivingHere != null)
            {
                SpawnNewMonster();
            }
            else
            {
                _currentMonster = null;
            }
        }

        private void AssignPlayerQuest(Location newLocation)
        {
            RaiseMessage("You receive the " + newLocation.QuestAvailableHere.Name + " quest.", true);
            RaiseMessage(newLocation.QuestAvailableHere.Description, true);
            RaiseMessage("To complete it, return with:", true);
            foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
            {
                if (qci.Quantity == 1)
                {
                    RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.Name, true);
                }
                else
                {
                    RaiseMessage(qci.Quantity.ToString() + " " + qci.Details.NamePlural, true);
                }
            }

            Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
        }

        private void RewardPlayerForCompletingQuest(Location newLocation)
        {
            RaiseMessage("You completed the '" + newLocation.QuestAvailableHere.Name + "' quest.", true);

            RaiseMessage("You receive: ", true);
            RaiseMessage(newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points", true);
            RaiseMessage(newLocation.QuestAvailableHere.RewardGold.ToString() + " gold", true);
            RaiseMessage(newLocation.QuestAvailableHere.RewardItem.Name, true);

            RemoveQuestCompletionItems(newLocation.QuestAvailableHere);
            AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
            Gold += newLocation.QuestAvailableHere.RewardGold;
            AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);
            MarkQuestCompleted(newLocation.QuestAvailableHere);
        }

        private void PlayerDrinksPotion(HealingPotion healingPotion)
        {
            CurrentHitPoints = (CurrentHitPoints + healingPotion.AmountToHeal);

            if (CurrentHitPoints > MaximumHitPoints)
            {
                CurrentHitPoints = MaximumHitPoints;
            }

            RemoveItemFromInventory(healingPotion);

            RaiseMessage("You drink a " + healingPotion.Name, true);
        }

        private void PlayerAttacks(Weapon weapon)
        {
            int damageToMonster = RandomNumberGenerator.NumberBetween(weapon.MinimumDamage, weapon.MaximumDamage);
            _currentMonster.CurrentHitPoints -= damageToMonster;

            RaiseMessage("You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points.", true);
        }

        private void MonsterDefeated()
        {
            RaiseMessage("", true);
            RaiseMessage("You defeated the " + _currentMonster.Name, true);
            RaiseMessage("You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points", true);
            RaiseMessage("You receive " + _currentMonster.RewardGold.ToString() + " gold", true);

            AddExperiencePoints(_currentMonster.RewardExperiencePoints);
            Gold += _currentMonster.RewardGold;

            LootMonster();

            RaiseMessage("", true);

            FullyHealPlayer();
            SpawnNewMonster();
        }

        private void SpawnNewMonster()
        {
            RaiseMessage("You see a " + CurrentLocation.MonsterLivingHere.Name, true);
            RaiseMessage("", true);

            Monster standardMonster = World.MonsterByID(CurrentLocation.MonsterLivingHere.ID);

            _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

            foreach (LootItem lootItem in standardMonster.LootTable)
            {
                _currentMonster.LootTable.Add(lootItem);
            }
        }

        private void MonsterAttacks()
        {
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);
            CurrentHitPoints -= damageToPlayer;

            RaiseMessage("The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage.", true);
            RaiseMessage("", true);
        }

        private void PlayerDefeated()
        {
            RaiseMessage("The " + _currentMonster.Name + " killed you.", true);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME), 1);
        }

        private void LootMonster()
        {
            List<InventoryItem> lootedItems = new List<InventoryItem>();

            foreach (LootItem lootItem in _currentMonster.LootTable)
            {
                if (RandomNumberGenerator.NumberBetween(1, 100) <= lootItem.DropPercentage)
                {
                    lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                }
            }

            if (lootedItems.Count == 0)
            {
                foreach (LootItem lootItem in _currentMonster.LootTable)
                {
                    if (lootItem.IsDefaultItem)
                    {
                        lootedItems.Add(new InventoryItem(lootItem.Details, 1));
                    }
                }
            }

            foreach (InventoryItem inventoryItem in lootedItems)
            {
                AddItemToInventory(inventoryItem.Details);

                if (inventoryItem.Quantity == 1)
                {
                    RaiseMessage("You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name, true);
                }
                else
                {
                    RaiseMessage("You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural, true);
                }
            }
        }

        public string ToXmlString()
        {
            XmlDocument playerData = new XmlDocument();

            // Create the top-level XML node
            XmlNode player = playerData.CreateElement("Player");
            playerData.AppendChild(player);

            // Create the "Stats" child node to hold the other player statistics nodes
            XmlNode stats = playerData.CreateElement("Stats");
            player.AppendChild(stats);

            // Create the child nodes for the "Stats" node
            XmlNode currentHitPoints = playerData.CreateElement("CurrentHitPoints");
            currentHitPoints.AppendChild(playerData.CreateTextNode(this.CurrentHitPoints.ToString()));
            stats.AppendChild(currentHitPoints);

            XmlNode maximumHitPoints = playerData.CreateElement("MaximumHitPoints");
            maximumHitPoints.AppendChild(playerData.CreateTextNode(this.MaximumHitPoints.ToString()));
            stats.AppendChild(maximumHitPoints);

            XmlNode gold = playerData.CreateElement("Gold");
            gold.AppendChild(playerData.CreateTextNode(this.Gold.ToString()));
            stats.AppendChild(gold);

            XmlNode experiencePoints = playerData.CreateElement("ExperiencePoints");
            experiencePoints.AppendChild(playerData.CreateTextNode(this.ExperiencePoints.ToString()));
            stats.AppendChild(experiencePoints);

            XmlNode currentLocation = playerData.CreateElement("CurrentLocation");
            currentLocation.AppendChild(playerData.CreateTextNode(this.CurrentLocation.ID.ToString()));
            stats.AppendChild(currentLocation);

            XmlNode currentWeapon = playerData.CreateElement("CurrentWeapon");
            currentWeapon.AppendChild(playerData.CreateTextNode(this.CurrentWeapon.ID.ToString()));
            stats.AppendChild(currentWeapon);

            // Create the "InventoryItems" child node to hold each InventoryItem node
            XmlNode inventoryItems = playerData.CreateElement("InventoryItems");
            player.AppendChild(inventoryItems);

            // Create an "InventoryItem" node for each item in the player's inventory
            foreach (InventoryItem item in this.Inventory)
            {
                XmlNode inventoryItem = playerData.CreateElement("InventoryItem");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = item.Details.ID.ToString();
                inventoryItem.Attributes.Append(idAttribute);

                XmlAttribute quantityAttribute = playerData.CreateAttribute("Quantity");
                quantityAttribute.Value = item.Quantity.ToString();
                inventoryItem.Attributes.Append(quantityAttribute);

                inventoryItems.AppendChild(inventoryItem);
            }

            // Create the "PlayerQuests" child node to hold each PlayerQuest node
            XmlNode playerQuests = playerData.CreateElement("PlayerQuests");
            player.AppendChild(playerQuests);

            // Create a "PlayerQuest" node for each quest the player has acquired
            foreach (PlayerQuest quest in this.Quests)
            {
                XmlNode playerQuest = playerData.CreateElement("PlayerQuest");

                XmlAttribute idAttribute = playerData.CreateAttribute("ID");
                idAttribute.Value = quest.Details.ID.ToString();
                playerQuest.Attributes.Append(idAttribute);

                XmlAttribute isCompletedAttribute = playerData.CreateAttribute("IsCompleted");
                isCompletedAttribute.Value = quest.IsCompleted.ToString();
                playerQuest.Attributes.Append(isCompletedAttribute);

                playerQuests.AppendChild(playerQuest);
            }

            return playerData.InnerXml; // The XML document, as a string, so we can save
        }
    }
}
