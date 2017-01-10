using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Engine;

namespace SuperAdventure
{
    public partial class SuperAdventure : Form
    {
        private Player _player;
        private Monster _currentMonster;

        public SuperAdventure()
        {
            InitializeComponent();

            _player = new Player(10, 10, 20, 0, 1);
            MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
            _player.Inventory.Add(new InventoryItem(World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

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

        private void MoveTo(Location newLocation)
        {
            //Does the location have any required items
            if (newLocation.ItemRequiredToEnter != null)
            {
                // See if the player has the required item in their inventory
                bool playerHasRequiredItem = false;

                foreach (InventoryItem ii in _player.Inventory)
                {
                    if (ii.Details.ID == newLocation.ItemRequiredToEnter.ID)
                    {
                        // We found the required item
                        playerHasRequiredItem = true;
                        break; // Exit out of the foreach loop
                    }
                }

                if (!playerHasRequiredItem)
                {
                    // We didn't find the required item in their inventory, so display a message and stop trying to move
                    richTextBoxMessages.Text += "You must have a " + newLocation.ItemRequiredToEnter.Name + " to enter this location." + Environment.NewLine;
                    return;
                }
            }

            // Update the player's current location
            _player.CurrentLocation = newLocation;

            // Show/hide available movement buttons
            buttonGoNorth.Visible = (newLocation.LocationToNorth != null);
            buttonGoEast.Visible = (newLocation.LocationToEast != null);
            buttonGoSouth.Visible = (newLocation.LocationToSouth != null);
            buttonGoWest.Visible = (newLocation.LocationToWest != null);

            // Display current location name and description
            richTextBoxLocation.Text = newLocation.Name + Environment.NewLine;
            richTextBoxLocation.Text += newLocation.Description + Environment.NewLine;

            // Completely heal the player
            _player.CurrentHitPoints = _player.MaximumHitPoints;

            // Update Hit Points in UI
            valueHitPoints.Text = _player.CurrentHitPoints.ToString();

            // Does the location have a quest?
            if (newLocation.QuestAvailableHere != null)
            {
                // See if the player already has the quest, and if they've completed it
                bool playerAlreadyHasQuest = false;
                bool playerAlreadyCompletedQuest = false;

                foreach (PlayerQuest playerQuest in _player.Quests)
                {
                    if (playerQuest.Details.ID == newLocation.QuestAvailableHere.ID)
                    {
                        playerAlreadyHasQuest = true;

                        if (playerQuest.IsCompleted)
                        {
                            playerAlreadyCompletedQuest = true;
                        }
                    }
                }

                // See if the player already has the quest
                if (playerAlreadyHasQuest)
                {
                    // If the player has not completed the quest yet
                    if (!playerAlreadyCompletedQuest)
                    {
                        // See if the player has all the items needed to complete the quest
                        bool playerHasAllItemsToCompleteQuest = true;

                        foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                        {
                            bool foundItemInPlayersInventory = false;

                            // Check each item in the player's inventory, to see if they have it, and enough of it
                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                // The player has this item in their inventory
                                if (ii.Details.ID == qci.Details.ID)
                                {
                                    foundItemInPlayersInventory = true;

                                    if (ii.Quantity < qci.Quantity)
                                    {
                                        // The player does not have enough of this item to complete the quest
                                        playerHasAllItemsToCompleteQuest = false;

                                        // There is no reason to continue checking for the other quest completion items
                                        break;
                                    }

                                    // We found the item, so don't check the rest of the player's inventory
                                    break;
                                }
                            }

                            // If we didn't find the required item, set our variable and stop looking for other items
                            if (!foundItemInPlayersInventory)
                            {
                                // The player does not have this item in their inventory
                                playerHasAllItemsToCompleteQuest = false;

                                // There is no reason to continue checking for the other quest completion items
                                break;
                            }
                        }

                        // The player has all items required to complete the quest
                        if (playerHasAllItemsToCompleteQuest)
                        {
                            // Display message
                            richTextBoxMessages.Text += Environment.NewLine;
                            richTextBoxMessages.Text += "You complete the '" + newLocation.QuestAvailableHere.Name + "' quest." + Environment.NewLine;

                            // Remove quest items from inventory
                            foreach (QuestCompletionItem qci in newLocation.QuestAvailableHere.QuestCompletionItems)
                            {
                                foreach (InventoryItem ii in _player.Inventory)
                                {
                                    if (ii.Details.ID == qci.Details.ID)
                                    {
                                        // Subtract the quantity from the player's inventory that was needed to complete the quest
                                        ii.Quantity -= qci.Quantity;
                                        break;
                                    }
                                }
                            }

                            // Give quest rewards
                            richTextBoxMessages.Text += "You receive: " + Environment.NewLine;
                            richTextBoxMessages.Text += newLocation.QuestAvailableHere.RewardExperiencePoints.ToString() + " experience points" + Environment.NewLine;
                            richTextBoxMessages.Text += newLocation.QuestAvailableHere.RewardGold.ToString() + " gold" + Environment.NewLine;
                            richTextBoxMessages.Text += newLocation.QuestAvailableHere.RewardItem.Name + Environment.NewLine;
                            richTextBoxMessages.Text += Environment.NewLine;

                            _player.ExperiencePoints += newLocation.QuestAvailableHere.RewardExperiencePoints;
                            _player.Gold += newLocation.QuestAvailableHere.RewardGold;

                            // Add the reward item to the player's inventory
                            bool addedItemToPlayerInventory = false;

                            foreach (InventoryItem ii in _player.Inventory)
                            {
                                if (ii.Details.ID == newLocation.QuestAvailableHere.RewardItem.ID)
                                {
                                    // They have the item in their inventory, so increase the quantity by one
                                    ii.Quantity++;

                                    addedItemToPlayerInventory = true;

                                    break;
                                }
                            }

                            // They didn't have the item, so add it to their inventory, with a quantity of 1
                            if (!addedItemToPlayerInventory)
                            {
                                _player.Inventory.Add(new InventoryItem(newLocation.QuestAvailableHere.RewardItem, 1));
                            }

                            // Mark the quest as completed
                            // Find the quest in the player's quest list
                            foreach (PlayerQuest pq in _player.Quests)
                            {
                                if (pq.Details.ID == newLocation.QuestAvailableHere.ID)
                                {
                                    // Mark it as completed
                                    pq.IsCompleted = true;

                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    // The player does not already have the quest

                    // Display the messages
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
                    richTextBoxMessages.Text += Environment.NewLine;

                    // Add the quest to the player's quest list
                    _player.Quests.Add(new PlayerQuest(newLocation.QuestAvailableHere));
                }
            }

            // Does the location have a monster?
            if (newLocation.MonsterLivingHere != null)
            {
                richTextBoxMessages.Text += "You see a " + newLocation.MonsterLivingHere.Name + Environment.NewLine;

                // Make a new monster, using the values from the standard monster in the World.Monster list
                Monster standardMonster = World.MonsterByID(newLocation.MonsterLivingHere.ID);

                _currentMonster = new Monster(standardMonster.ID, standardMonster.Name, standardMonster.MaximumDamage,
                    standardMonster.RewardExperiencePoints, standardMonster.RewardGold, standardMonster.CurrentHitPoints, standardMonster.MaximumHitPoints);

                foreach (LootItem lootItem in standardMonster.LootTable)
                {
                    _currentMonster.LootTable.Add(lootItem);
                }

                comboBoxWeapons.Visible = true;
                comboBoxPotions.Visible = true;
                buttonUseWeapon.Visible = true;
                buttonUsePotion.Visible = true;
            }
            else
            {
                _currentMonster = null;

                comboBoxWeapons.Visible = false;
                comboBoxPotions.Visible = false;
                buttonUseWeapon.Visible = false;
                buttonUsePotion.Visible = false;
            }

            // Refresh player's inventory list
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

            // Refresh player's quest list
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

            // Refresh player's weapons combobox
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
                // The player doesn't have any weapons, so hide the weapon combobox and "Use" button
                comboBoxWeapons.Visible = false;
                buttonUseWeapon.Visible = false;
            }
            else
            {
                comboBoxWeapons.DataSource = weapons;
                comboBoxWeapons.DisplayMember = "Name";
                comboBoxWeapons.ValueMember = "ID";

                comboBoxWeapons.SelectedIndex = 0;
            }

            // Refresh player's potions combobox
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
                // The player doesn't have any potions, so hide the potion combobox and "Use" button
                comboBoxPotions.Visible = false;
                buttonUsePotion.Visible = false;
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

        }

        private void buttonUsePotion_Click(object sender, EventArgs e)
        {

        }
    }
}