using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Location _location;
        private Monster _currentMonster;
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";

        public SuperAdventure()
        {
            InitializeComponent();

            if (File.Exists(PLAYER_DATA_FILE_NAME))
            {
                _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer(10, 10, 20, 0, 1);
            }
            MoveTo(_player.CurrentLocation);

            UpdatePlayerStats();
        }

        private void UpdatePlayerStats()
        {
            valueHitPoints.Text = _player.CurrentHitPoints.ToString();
            valueGold.Text = _player.Gold.ToString();
            valueExperience.Text = _player.ExperiencePoints.ToString();
            valueLevel.Text = _player.Level.ToString();
        }

        private void buttonGoNorth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToNorth);
        }

        private void buttonGoEast_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToEast);
        }

        private void buttonGoSouth_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToSouth);
        }

        private void buttonGoWest_Click(object sender, EventArgs e)
        {
            MoveTo(_player.CurrentLocation.LocationToWest);
        }

        private void MoveTo(Location newLocation, int respawn = 0)
        {
            _location = newLocation;
            if (respawn == 0) richTextBoxMessages.Text = "";

            if (!_player.HasRequiredItemToEnterLocation(newLocation))
            {
                richTextBoxMessages.Text += "You must have an " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                return;
            }

            _player.CurrentLocation = newLocation;

            FullyHealPlayer();

            buttonGoNorth.Visible = (newLocation.LocationToNorth != null);
            buttonGoEast.Visible = (newLocation.LocationToEast != null);
            buttonGoSouth.Visible = (newLocation.LocationToSouth != null);
            buttonGoWest.Visible = (newLocation.LocationToWest != null);

            richTextBoxLocation.Text = newLocation.Name + Environment.NewLine;
            richTextBoxLocation.Text += newLocation.Description + Environment.NewLine;

            if (newLocation.QuestAvailableHere != null)
            {
                if (_player.HasThisQuest(newLocation.QuestAvailableHere))
                {
                    if (!_player.CompletedThisQuest(newLocation.QuestAvailableHere) && _player.HasAllQuestCompletionItems(newLocation.QuestAvailableHere))
                    {
                        RewardPlayerForCompletingQuest(newLocation);
                    }
                }
                else
                {
                    AssignPlayerQuest(newLocation);
                }
            }

            if (newLocation.MonsterLivingHere != null)
            {
                SpawnNewMonster(newLocation);
                labelSelectAction.Visible = true;
                ToggleWeaponButtons(true);
                TogglePotionButtons(true);
            }
            else
            {
                _currentMonster = null;
                labelSelectAction.Visible = false;
                ToggleWeaponButtons(false);
                TogglePotionButtons(false);
            }

            UpdateInventoryList();
            UpdateQuestList();
            UpdateWeaponList();
            UpdatePotionList();
        }

        private void FullyHealPlayer()
        {
            _player.FullyHealPlayer();
            valueHitPoints.Text = _player.CurrentHitPoints.ToString();
        }

        private void AssignPlayerQuest(Location newLocation)
        {
            richTextBoxMessages.Text += "You receive the " + newLocation.QuestAvailableHere.Name + " quest." + Environment.NewLine;
            richTextBoxMessages.Text += newLocation.QuestAvailableHere.Description + Environment.NewLine;
            richTextBoxMessages.Text += "To complete it, return with:" + Environment.NewLine;
            foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
            {
                if (qci.Quantity == 1)
                {
                    richTextBoxMessages.Text += qci.Quantity.ToString() + " " + qci.Details.Name + Environment.NewLine;
                }
                else
                {
                    richTextBoxMessages.Text += qci.Quantity.ToString() + " " + qci.Details.NamePlural + Environment.NewLine;
                }
            }

            _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
        }

        private void RewardPlayerForCompletingQuest(Location newLocation)
        {
            richTextBoxMessages.Text += "You completed the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;

            richTextBoxMessages.Text += "You receive: " + Environment.NewLine;
            richTextBoxMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
            richTextBoxMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
            richTextBoxMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;

            _player.RemoveQuestCompletionItems(newLocation.QuestAvailableHere);
            _player.AddExperiencePoints(newLocation.QuestAvailableHere.RewardExperiencePoints);
            _player.Gold += newLocation.QuestAvailableHere.RewardGold;
            _player.AddItemToInventory(newLocation.QuestAvailableHere.RewardItem);
            _player.MarkQuestCompleted(newLocation.QuestAvailableHere);
        }

        private void SpawnNewMonster(Location location)
        {
            richTextBoxMessages.Text += "You see a " + location.MonsterLivingHere.Name + Environment.NewLine;
            richTextBoxMessages.Text += Environment.NewLine;

            Monster standardMonster = World.MonsterByID(location.MonsterLivingHere.ID);

            _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

            foreach (LootItem lootItem in standardMonster.LootTable)
            {
                _currentMonster.LootTable.Add(lootItem);
            }
        }

        private void ToggleWeaponButtons(bool visible)
        {
            comboBoxWeapons.Visible = visible;
            buttonUseWeapon.Visible = visible;
        }

        private void TogglePotionButtons(bool visible)
        {
            comboBoxPotions.Visible = visible;
            buttonUsePotion.Visible = visible;
        }

        private void UpdateInventoryList()
        {
            dataGridViewInventory.RowHeadersVisible = false;
            dataGridViewInventory.ColumnCount = 2;
            dataGridViewInventory.Columns[0].Name = "Name";
            dataGridViewInventory.Columns[0].Width = 197;
            dataGridViewInventory.Columns[1].Name = "Quantity";
            dataGridViewInventory.Rows.Clear();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Quantity > 0)
                {
                    dataGridViewInventory.Rows.Add(new[] { inventoryItem.Details.Name, inventoryItem.Quantity.ToString() });
                }
            }
        }

        private void UpdateQuestList()
        {
            dataGridViewQuests.RowHeadersVisible = false;
            dataGridViewQuests.ColumnCount = 2;
            dataGridViewQuests.Columns[0].Name = "Name";
            dataGridViewQuests.Columns[0].Width = 197;
            dataGridViewQuests.Columns[1].Name = "Done?";
            dataGridViewQuests.Rows.Clear();

            foreach (PlayerQuest playerQuest in _player.Quests)
            {
                dataGridViewQuests.Rows.Add(new[] { playerQuest.Details.Name, playerQuest.IsCompleted.ToString() });
            }
        }

        private void UpdateWeaponList()
        {
            List<Weapon> weapons = new List<Weapon>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is Weapon)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        weapons.Add((Weapon)inventoryItem.Details);
                    }
                }
            }

            if (weapons.Count == 0)
            {
                ToggleWeaponButtons(false);
            }
            else
            {
                comboBoxWeapons.SelectedIndexChanged -= comboBoxWeapons_SelectedIndexChanged;
                comboBoxWeapons.DataSource = weapons;
                comboBoxWeapons.SelectedIndexChanged += comboBoxWeapons_SelectedIndexChanged;
                comboBoxWeapons.DisplayMember = "Name";
                comboBoxWeapons.ValueMember = "ID";

                if (_player.CurrentWeapon != null)
                {
                    comboBoxWeapons.SelectedItem = _player.CurrentWeapon;
                }
                else
                {
                    comboBoxWeapons.SelectedIndex = 0;
                }
            }
        }

        private void UpdatePotionList()
        {
            List<HealingPotion> healingPotions = new List<HealingPotion>();

            foreach (InventoryItem inventoryItem in _player.Inventory)
            {
                if (inventoryItem.Details is HealingPotion)
                {
                    if (inventoryItem.Quantity > 0)
                    {
                        healingPotions.Add((HealingPotion)inventoryItem.Details);
                    }
                }
            }

            if (healingPotions.Count == 0)
            {
                TogglePotionButtons(false);
            }
            else
            {
                comboBoxPotions.DataSource = healingPotions;
                comboBoxPotions.DisplayMember = "Name";
                comboBoxPotions.ValueMember = "ID";
                comboBoxPotions.SelectedIndex = 0;
            }
        }

        private void buttonUseWeapon_Click(object sender, EventArgs e)
        {
            PlayerAttacks();

            if (_currentMonster.CurrentHitPoints <= 0)
            {
                MonsterDefeated();
            }
            else
            {
                MonsterAttacks();

                if (_player.CurrentHitPoints <= 0)
                {
                    PlayerDefeated();
                }
            }

            ScrollToBottomOfMessages();
        }

        private void buttonUsePotion_Click(object sender, EventArgs e)
        {
            PlayerDrinksPotion();

            MonsterAttacks();

            if (_player.CurrentHitPoints <= 0)
            {
                PlayerDefeated();
            }

            valueHitPoints.Text = _player.CurrentHitPoints.ToString();
            UpdateInventoryList();
            UpdatePotionList();

            ScrollToBottomOfMessages();
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

            UpdateInventoryList();
            UpdateWeaponList();
            UpdatePotionList();

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
                _player.AddItemToInventory(inventoryItem.Details);

                if (inventoryItem.Quantity == 1)
                {
                    richTextBoxMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.Name + Environment.NewLine;
                }
                else
                {
                    richTextBoxMessages.Text += "You loot " + inventoryItem.Quantity.ToString() + " " + inventoryItem.Details.NamePlural + Environment.NewLine;
                }
            }
        }

        private void ScrollToBottomOfMessages()
        {
            richTextBoxMessages.SelectionStart = richTextBoxMessages.Text.Length;
            richTextBoxMessages.ScrollToCaret();
        }

        private void SuperAdventure_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
        }

        private void comboBoxWeapons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _player.CurrentWeapon = (Weapon)comboBoxWeapons.SelectedItem;
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            _player = Player.CreateDefaultPlayer(10, 10, 20, 0, 1);
            UpdatePlayerStats();
            MoveTo(_player.CurrentLocation);
        }

        private void buttonYes_Click(object sender, EventArgs e)
        {

        }

        private void buttonNo_Click(object sender, EventArgs e)
        {

        }
    }
}