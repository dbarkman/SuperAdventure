namespace SuperAdventure
{
    partial class SuperAdventure
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelHitPoints = new System.Windows.Forms.Label();
            this.valueHitPoints = new System.Windows.Forms.Label();
            this.labelGold = new System.Windows.Forms.Label();
            this.valueGold = new System.Windows.Forms.Label();
            this.labelExperience = new System.Windows.Forms.Label();
            this.valueExperience = new System.Windows.Forms.Label();
            this.labelLevel = new System.Windows.Forms.Label();
            this.valueLevel = new System.Windows.Forms.Label();
            this.labelSelectAction = new System.Windows.Forms.Label();
            this.comboBoxWeapons = new System.Windows.Forms.ComboBox();
            this.comboBoxPotions = new System.Windows.Forms.ComboBox();
            this.buttonUseWeapon = new System.Windows.Forms.Button();
            this.buttonUsePotion = new System.Windows.Forms.Button();
            this.buttonGoNorth = new System.Windows.Forms.Button();
            this.buttonGoEast = new System.Windows.Forms.Button();
            this.buttonGoSouth = new System.Windows.Forms.Button();
            this.buttonGoWest = new System.Windows.Forms.Button();
            this.dataGridViewInventory = new System.Windows.Forms.DataGridView();
            this.dataGridViewQuests = new System.Windows.Forms.DataGridView();
            this.richTextBoxLocation = new System.Windows.Forms.RichTextBox();
            this.richTextBoxMessages = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQuests)).BeginInit();
            this.SuspendLayout();
            // 
            // labelHitPoints
            // 
            this.labelHitPoints.AutoSize = true;
            this.labelHitPoints.Location = new System.Drawing.Point(18, 20);
            this.labelHitPoints.Name = "labelHitPoints";
            this.labelHitPoints.Size = new System.Drawing.Size(55, 13);
            this.labelHitPoints.TabIndex = 0;
            this.labelHitPoints.Text = "Hit Points:";
            // 
            // valueHitPoints
            // 
            this.valueHitPoints.AutoSize = true;
            this.valueHitPoints.Location = new System.Drawing.Point(110, 19);
            this.valueHitPoints.Name = "valueHitPoints";
            this.valueHitPoints.Size = new System.Drawing.Size(0, 13);
            this.valueHitPoints.TabIndex = 1;
            // 
            // labelGold
            // 
            this.labelGold.AutoSize = true;
            this.labelGold.Location = new System.Drawing.Point(18, 46);
            this.labelGold.Name = "labelGold";
            this.labelGold.Size = new System.Drawing.Size(32, 13);
            this.labelGold.TabIndex = 2;
            this.labelGold.Text = "Gold:";
            // 
            // valueGold
            // 
            this.valueGold.AutoSize = true;
            this.valueGold.Location = new System.Drawing.Point(110, 45);
            this.valueGold.Name = "valueGold";
            this.valueGold.Size = new System.Drawing.Size(0, 13);
            this.valueGold.TabIndex = 3;
            // 
            // labelExperience
            // 
            this.labelExperience.AutoSize = true;
            this.labelExperience.Location = new System.Drawing.Point(18, 74);
            this.labelExperience.Name = "labelExperience";
            this.labelExperience.Size = new System.Drawing.Size(63, 13);
            this.labelExperience.TabIndex = 4;
            this.labelExperience.Text = "Experience:";
            // 
            // valueExperience
            // 
            this.valueExperience.AutoSize = true;
            this.valueExperience.Location = new System.Drawing.Point(110, 73);
            this.valueExperience.Name = "valueExperience";
            this.valueExperience.Size = new System.Drawing.Size(0, 13);
            this.valueExperience.TabIndex = 5;
            // 
            // labelLevel
            // 
            this.labelLevel.AutoSize = true;
            this.labelLevel.Location = new System.Drawing.Point(18, 100);
            this.labelLevel.Name = "labelLevel";
            this.labelLevel.Size = new System.Drawing.Size(36, 13);
            this.labelLevel.TabIndex = 6;
            this.labelLevel.Text = "Level:";
            // 
            // valueLevel
            // 
            this.valueLevel.AutoSize = true;
            this.valueLevel.Location = new System.Drawing.Point(110, 99);
            this.valueLevel.Name = "valueLevel";
            this.valueLevel.Size = new System.Drawing.Size(0, 13);
            this.valueLevel.TabIndex = 7;
            // 
            // labelSelectAction
            // 
            this.labelSelectAction.AutoSize = true;
            this.labelSelectAction.Location = new System.Drawing.Point(617, 531);
            this.labelSelectAction.Name = "labelSelectAction";
            this.labelSelectAction.Size = new System.Drawing.Size(70, 13);
            this.labelSelectAction.TabIndex = 8;
            this.labelSelectAction.Text = "Select Action";
            // 
            // comboBoxWeapons
            // 
            this.comboBoxWeapons.FormattingEnabled = true;
            this.comboBoxWeapons.Location = new System.Drawing.Point(369, 559);
            this.comboBoxWeapons.Name = "comboBoxWeapons";
            this.comboBoxWeapons.Size = new System.Drawing.Size(121, 21);
            this.comboBoxWeapons.TabIndex = 9;
            // 
            // comboBoxPotions
            // 
            this.comboBoxPotions.FormattingEnabled = true;
            this.comboBoxPotions.Location = new System.Drawing.Point(369, 593);
            this.comboBoxPotions.Name = "comboBoxPotions";
            this.comboBoxPotions.Size = new System.Drawing.Size(121, 21);
            this.comboBoxPotions.TabIndex = 10;
            // 
            // buttonUseWeapon
            // 
            this.buttonUseWeapon.Location = new System.Drawing.Point(620, 559);
            this.buttonUseWeapon.Name = "buttonUseWeapon";
            this.buttonUseWeapon.Size = new System.Drawing.Size(75, 23);
            this.buttonUseWeapon.TabIndex = 11;
            this.buttonUseWeapon.Text = "Use";
            this.buttonUseWeapon.UseVisualStyleBackColor = true;
            this.buttonUseWeapon.Click += new System.EventHandler(this.buttonUseWeapon_Click);
            // 
            // buttonUsePotion
            // 
            this.buttonUsePotion.Location = new System.Drawing.Point(620, 593);
            this.buttonUsePotion.Name = "buttonUsePotion";
            this.buttonUsePotion.Size = new System.Drawing.Size(75, 23);
            this.buttonUsePotion.TabIndex = 12;
            this.buttonUsePotion.Text = "Use";
            this.buttonUsePotion.UseVisualStyleBackColor = true;
            this.buttonUsePotion.Click += new System.EventHandler(this.buttonUsePotion_Click);
            // 
            // buttonGoNorth
            // 
            this.buttonGoNorth.Location = new System.Drawing.Point(493, 433);
            this.buttonGoNorth.Name = "buttonGoNorth";
            this.buttonGoNorth.Size = new System.Drawing.Size(75, 23);
            this.buttonGoNorth.TabIndex = 13;
            this.buttonGoNorth.Text = "North";
            this.buttonGoNorth.UseVisualStyleBackColor = true;
            this.buttonGoNorth.Click += new System.EventHandler(this.buttonGoNorth_Click);
            // 
            // buttonGoEast
            // 
            this.buttonGoEast.Location = new System.Drawing.Point(573, 457);
            this.buttonGoEast.Name = "buttonGoEast";
            this.buttonGoEast.Size = new System.Drawing.Size(75, 23);
            this.buttonGoEast.TabIndex = 14;
            this.buttonGoEast.Text = "East";
            this.buttonGoEast.UseVisualStyleBackColor = true;
            this.buttonGoEast.Click += new System.EventHandler(this.buttonGoEast_Click);
            // 
            // buttonGoSouth
            // 
            this.buttonGoSouth.Location = new System.Drawing.Point(493, 487);
            this.buttonGoSouth.Name = "buttonGoSouth";
            this.buttonGoSouth.Size = new System.Drawing.Size(75, 23);
            this.buttonGoSouth.TabIndex = 15;
            this.buttonGoSouth.Text = "South";
            this.buttonGoSouth.UseVisualStyleBackColor = true;
            this.buttonGoSouth.Click += new System.EventHandler(this.buttonGoSouth_Click);
            // 
            // buttonGoWest
            // 
            this.buttonGoWest.Location = new System.Drawing.Point(412, 457);
            this.buttonGoWest.Name = "buttonGoWest";
            this.buttonGoWest.Size = new System.Drawing.Size(75, 23);
            this.buttonGoWest.TabIndex = 16;
            this.buttonGoWest.Text = "West";
            this.buttonGoWest.UseVisualStyleBackColor = true;
            this.buttonGoWest.Click += new System.EventHandler(this.buttonGoWest_Click);
            // 
            // dataGridViewInventory
            // 
            this.dataGridViewInventory.AllowUserToAddRows = false;
            this.dataGridViewInventory.AllowUserToDeleteRows = false;
            this.dataGridViewInventory.AllowUserToResizeRows = false;
            this.dataGridViewInventory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewInventory.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewInventory.Enabled = false;
            this.dataGridViewInventory.Location = new System.Drawing.Point(16, 130);
            this.dataGridViewInventory.MultiSelect = false;
            this.dataGridViewInventory.Name = "dataGridViewInventory";
            this.dataGridViewInventory.ReadOnly = true;
            this.dataGridViewInventory.RowHeadersVisible = false;
            this.dataGridViewInventory.Size = new System.Drawing.Size(312, 309);
            this.dataGridViewInventory.TabIndex = 17;
            // 
            // dataGridViewQuests
            // 
            this.dataGridViewQuests.AllowUserToAddRows = false;
            this.dataGridViewQuests.AllowUserToDeleteRows = false;
            this.dataGridViewQuests.AllowUserToResizeRows = false;
            this.dataGridViewQuests.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewQuests.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridViewQuests.Enabled = false;
            this.dataGridViewQuests.Location = new System.Drawing.Point(16, 446);
            this.dataGridViewQuests.MultiSelect = false;
            this.dataGridViewQuests.Name = "dataGridViewQuests";
            this.dataGridViewQuests.ReadOnly = true;
            this.dataGridViewQuests.RowHeadersVisible = false;
            this.dataGridViewQuests.Size = new System.Drawing.Size(312, 189);
            this.dataGridViewQuests.TabIndex = 18;
            // 
            // richTextBoxLocation
            // 
            this.richTextBoxLocation.Location = new System.Drawing.Point(347, 19);
            this.richTextBoxLocation.Name = "richTextBoxLocation";
            this.richTextBoxLocation.ReadOnly = true;
            this.richTextBoxLocation.Size = new System.Drawing.Size(360, 105);
            this.richTextBoxLocation.TabIndex = 19;
            this.richTextBoxLocation.Text = "";
            // 
            // richTextBoxMessages
            // 
            this.richTextBoxMessages.Location = new System.Drawing.Point(347, 130);
            this.richTextBoxMessages.Name = "richTextBoxMessages";
            this.richTextBoxMessages.ReadOnly = true;
            this.richTextBoxMessages.Size = new System.Drawing.Size(360, 286);
            this.richTextBoxMessages.TabIndex = 20;
            this.richTextBoxMessages.Text = "";
            // 
            // SuperAdventure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(719, 651);
            this.Controls.Add(this.richTextBoxMessages);
            this.Controls.Add(this.richTextBoxLocation);
            this.Controls.Add(this.dataGridViewQuests);
            this.Controls.Add(this.dataGridViewInventory);
            this.Controls.Add(this.buttonGoWest);
            this.Controls.Add(this.buttonGoSouth);
            this.Controls.Add(this.buttonGoEast);
            this.Controls.Add(this.buttonGoNorth);
            this.Controls.Add(this.buttonUsePotion);
            this.Controls.Add(this.buttonUseWeapon);
            this.Controls.Add(this.comboBoxPotions);
            this.Controls.Add(this.comboBoxWeapons);
            this.Controls.Add(this.labelSelectAction);
            this.Controls.Add(this.valueLevel);
            this.Controls.Add(this.labelLevel);
            this.Controls.Add(this.valueExperience);
            this.Controls.Add(this.labelExperience);
            this.Controls.Add(this.valueGold);
            this.Controls.Add(this.labelGold);
            this.Controls.Add(this.valueHitPoints);
            this.Controls.Add(this.labelHitPoints);
            this.Name = "SuperAdventure";
            this.Text = "SuperAdventure";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SuperAdventure_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewQuests)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelHitPoints;
        private System.Windows.Forms.Label valueHitPoints;
        private System.Windows.Forms.Label labelGold;
        private System.Windows.Forms.Label valueGold;
        private System.Windows.Forms.Label labelExperience;
        private System.Windows.Forms.Label valueExperience;
        private System.Windows.Forms.Label labelLevel;
        private System.Windows.Forms.Label valueLevel;
        private System.Windows.Forms.Label labelSelectAction;
        private System.Windows.Forms.ComboBox comboBoxWeapons;
        private System.Windows.Forms.ComboBox comboBoxPotions;
        private System.Windows.Forms.Button buttonUseWeapon;
        private System.Windows.Forms.Button buttonUsePotion;
        private System.Windows.Forms.Button buttonGoNorth;
        private System.Windows.Forms.Button buttonGoEast;
        private System.Windows.Forms.Button buttonGoSouth;
        private System.Windows.Forms.Button buttonGoWest;
        private System.Windows.Forms.DataGridView dataGridViewInventory;
        private System.Windows.Forms.DataGridView dataGridViewQuests;
        private System.Windows.Forms.RichTextBox richTextBoxLocation;
        private System.Windows.Forms.RichTextBox richTextBoxMessages;
    }
}

