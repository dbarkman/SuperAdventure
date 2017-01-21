﻿using System;
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

            _player.OnMessage += DisplayMessage;

            MoveTo(_player.CurrentLocation);
        }

        private void DisplayMessage(object sender, MessageEventArgs messageEventArgs)
        {
            richTextBoxMessages.Text += messageEventArgs.Message + Environment.NewLine;

            if (messageEventArgs.AddExtraNewLine)
            {
                richTextBoxMessages.Text += Environment.NewLine;
            }

            richTextBoxMessages.SelectionStart = richTextBoxMessages.Text.Length;
            richTextBoxMessages.ScrollToCaret();
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

            if (propertyChangedEventArgs.PropertyName == "CurrentLocation")
            {
                buttonGoNorth.Visible = (_player.CurrentLocation.LocationToNorth != null);
                buttonGoEast.Visible = (_player.CurrentLocation.LocationToEast != null);
                buttonGoSouth.Visible = (_player.CurrentLocation.LocationToSouth != null);
                buttonGoWest.Visible = (_player.CurrentLocation.LocationToWest != null);

                richTextBoxLocation.Text = _player.CurrentLocation.Name + Environment.NewLine;
                richTextBoxLocation.Text += _player.CurrentLocation.Description + Environment.NewLine;

                if (_player.CurrentLocation.MonsterLivingHere != null)
                {
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
        }

        private void buttonGoNorth_Click(object sender, EventArgs e)
        {
            _player.MoveNorth();
        }

        private void buttonGoEast_Click(object sender, EventArgs e)
        {
            _player.MoveEast();
        }

        private void buttonGoSouth_Click(object sender, EventArgs e)
        {
            _player.MoveSouth();
        }

        private void buttonGoWest_Click(object sender, EventArgs e)
        {
            _player.MoveWest();
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

            _player.FullyHealPlayer();

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

            if (_player.CurrentLocation.MonsterLivingHere != null)
            {
                SpawnMonster();
            }
        }

        private void SpawnMonster()
        {
            richTextBoxMessages.Text += "You see a " + _player.CurrentLocation.MonsterLivingHere.Name + Environment.NewLine;
            richTextBoxMessages.Text += Environment.NewLine;

            Monster standardMonster = World.MonsterByID(_player.CurrentLocation.MonsterLivingHere.ID);

            _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

            foreach (LootItem lootItem in standardMonster.LootTable)
            {
                _currentMonster.LootTable.Add(lootItem);
            }
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

        private void buttonUseWeapon_Click(object sender, EventArgs e)
        {
            Weapon weapon = (Weapon)comboBoxWeapons.SelectedItem;
            _player.UseWeapon(weapon);
        }

        private void buttonUsePotion_Click(object sender, EventArgs e)
        {
            HealingPotion potion = (HealingPotion)comboBoxPotions.SelectedItem;
            _player.UsePotion(potion);
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