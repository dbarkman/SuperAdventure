using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
                _player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
                //_player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
            }
            else
            {
                _player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
            }

            valueHitPoints.DataBindings.Add("Text", _player, "CurrentHitPoints");
            valueGold.DataBindings.Add("Text", _player, "Gold");
            valueExperience.DataBindings.Add("Text", _player, "ExperiencePoints");
            valueLevel.DataBindings.Add("Text", _player, "Level");

            dataGridViewInventory.RowHeadersVisible = false;
            dataGridViewInventory.AutoGenerateColumns = false;

            dataGridViewInventory.DataSource = _player.Inventory;

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Description"
            });

            dataGridViewInventory.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Quantity",
                DataPropertyName = "Quantity"
            });

            dataGridViewQuests.RowHeadersVisible = false;
            dataGridViewQuests.AutoGenerateColumns = false;

            dataGridViewQuests.DataSource = _player.Quests;

            dataGridViewQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Name",
                Width = 197,
                DataPropertyName = "Name"
            });

            dataGridViewQuests.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Done?",
                DataPropertyName = "IsCompleted"
            });

            comboBoxWeapons.DataSource = _player.Weapons;
            comboBoxWeapons.DisplayMember = "Name";
            comboBoxWeapons.ValueMember = "Id";

            if (_player.CurrentWeapon != null)
            {
                comboBoxWeapons.SelectedItem = _player.CurrentWeapon;
            }

            comboBoxWeapons.SelectedIndexChanged += comboBoxWeapons_SelectedIndexChanged;

            comboBoxPotions.DataSource = _player.Potions;
            comboBoxPotions.DisplayMember = "Name";
            comboBoxPotions.ValueMember = "Id";

            _player.PropertyChanged += PlayerOnPropertyChanged;

            MoveTo(_player.CurrentLocation);
        }

        private void PlayerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "Weapons")
            {
                comboBoxWeapons.DataSource = _player.Weapons;
                if (!_player.Weapons.Any())
                {
                    comboBoxWeapons.Visible = false;
                    buttonUseWeapon.Visible = false;
                }
            }
            if (propertyChangedEventArgs.PropertyName == "Potions")
            {
                comboBoxPotions.DataSource = _player.Potions;
                if (!_player.Potions.Any())
                {
                    comboBoxPotions.Visible = false;
                    buttonUsePotion.Visible = false;
                }
            }
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
                comboBoxWeapons.Visible = _player.Weapons.Any();
                buttonUseWeapon.Visible = _player.Weapons.Any();
                comboBoxPotions.Visible = _player.Potions.Any();
                buttonUsePotion.Visible = _player.Potions.Any();
            }
            else
            {
                _currentMonster = null;
                labelSelectAction.Visible = false;
                comboBoxWeapons.Visible = false;
                buttonUseWeapon.Visible = false;
                comboBoxPotions.Visible = false;
                buttonUsePotion.Visible = false;
            }
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
            _player = Player.CreateDefaultPlayer(World.DEFAULT_CURRENT_HIT_POINTS, World.DEFAULT_MAXIMUM_HIT_POINTS, World.DEFAULT_GOLD, World.DEFAULT_EXPERIENCE_POINTS);
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