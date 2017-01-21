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

        public List<Weapon> Weapons
        {
            get { return Inventory.Where(x => x.Details is Weapon).Select(x => x.Details as Weapon).ToList(); }
        }

        public List<HealingPotion> Potions
        {
            get { return Inventory.Where(x => x.Details is HealingPotion).Select(x => x.Details as HealingPotion).ToList(); }
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

        private void RaiseMessage(string message, bool addExtraNewLine = false)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new Engine.MessageEventArgs(message, addExtraNewLine));
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

        public void MarkQuestCompleted(Quest quest)
        {
            PlayerQuest playerQuest = Quests.SingleOrDefault(q => q.Details.ID == quest.ID);
            if (playerQuest != null) playerQuest.IsCompleted = true;
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

        public void MoveNorth() { }
        public void MoveEast() { }
        public void MoveSouth() { }
        public void MoveWest() { }

        public void UseWeapon(Weapon weapon)
        {
            PlayerAttacks();
            MonsterDefeated();
            MonsterAttacks();
            PlayerDefeated();
        }

        public void UsePotion(HealingPotion healingPotion)
        {
            PlayerDrinksPotion();
            MonsterAttacks();
            PlayerDefeated();
        }

        private void PlayerDrinksPotion()
        {
            HealingPotion potion = (HealingPotion)comboBoxPotions.SelectedItem;
            _player.CurrentHitPoints = (_player.CurrentHitPoints + potion.AmountToHeal);

            if (_player.CurrentHitPoints > _player.MaximumHitPoints)
            {
                _player.CurrentHitPoints = _player.MaximumHitPoints;
            }

            _player.RemoveItemFromInventory(potion);

            richTextBoxMessages.Text += "You drink a " + potion.Name + Environment.NewLine;
        }

        private void PlayerAttacks()
        {
            Weapon currentWeapon = (Weapon)comboBoxWeapons.SelectedItem;

            int damageToMonster = RandomNumberGenerator.NumberBetween(currentWeapon.MinimumDamage, currentWeapon.MaximumDamage);
            _currentMonster.CurrentHitPoints -= damageToMonster;

            richTextBoxMessages.Text += "You hit the " + _currentMonster.Name + " for " + damageToMonster.ToString() + " points." + Environment.NewLine;
        }

        private void MonsterDefeated()
        {
            richTextBoxMessages.Text += Environment.NewLine;
            richTextBoxMessages.Text += "You defeated the " + _currentMonster.Name + Environment.NewLine;
            richTextBoxMessages.Text += "You receive " + _currentMonster.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
            richTextBoxMessages.Text += "You receive " + _currentMonster.RewardGold.ToString() + " gold" + Environment.NewLine;

            _player.AddExperiencePoints(_currentMonster.RewardExperiencePoints);
            _player.Gold += _currentMonster.RewardGold;

            LootMonster();

            valueHitPoints.Text = _player.CurrentHitPoints.ToString();
            valueGold.Text = _player.Gold.ToString();
            valueExperience.Text = _player.ExperiencePoints.ToString();
            valueLevel.Text = _player.Level.ToString();

            richTextBoxMessages.Text += Environment.NewLine;

            FullyHealPlayer();
            SpawnNewMonster(_location);
        }

        private void MonsterAttacks()
        {
            int damageToPlayer = RandomNumberGenerator.NumberBetween(0, _currentMonster.MaximumDamage);
            _player.CurrentHitPoints -= damageToPlayer;

            richTextBoxMessages.Text += "The " + _currentMonster.Name + " did " + damageToPlayer.ToString() + " points of damage." + Environment.NewLine;
            richTextBoxMessages.Text += Environment.NewLine;

            valueHitPoints.Text = _player.CurrentHitPoints.ToString();
        }

        private void PlayerDefeated()
        {
            richTextBoxMessages.Text = "The " + _currentMonster.Name + " killed you." + Environment.NewLine;
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME), 1);
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
