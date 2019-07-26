namespace UserManager.GUI
{
    partial class MainForm
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
            this.groupCard = new System.Windows.Forms.GroupBox();
            this.lblUser = new System.Windows.Forms.Label();
            this.lblOrganization = new System.Windows.Forms.Label();
            this.lblCardId = new System.Windows.Forms.Label();
            this.groupAmount = new System.Windows.Forms.GroupBox();
            this.lblAmount = new System.Windows.Forms.Label();
            this.groupCard.SuspendLayout();
            this.groupAmount.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupCard
            // 
            this.groupCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupCard.Controls.Add(this.lblUser);
            this.groupCard.Controls.Add(this.lblOrganization);
            this.groupCard.Controls.Add(this.lblCardId);
            this.groupCard.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupCard.Location = new System.Drawing.Point(12, 12);
            this.groupCard.Name = "groupCard";
            this.groupCard.Size = new System.Drawing.Size(578, 258);
            this.groupCard.TabIndex = 2;
            this.groupCard.TabStop = false;
            this.groupCard.Text = "Карта";
            // 
            // lblUser
            // 
            this.lblUser.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblUser.Font = new System.Drawing.Font("Verdana", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblUser.Location = new System.Drawing.Point(129, 106);
            this.lblUser.Name = "lblUser";
            this.lblUser.Size = new System.Drawing.Size(355, 59);
            this.lblUser.TabIndex = 7;
            this.lblUser.Text = "Ф.И.О";
            this.lblUser.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblOrganization
            // 
            this.lblOrganization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOrganization.AutoSize = true;
            this.lblOrganization.Font = new System.Drawing.Font("Verdana", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblOrganization.Location = new System.Drawing.Point(129, 179);
            this.lblOrganization.Name = "lblOrganization";
            this.lblOrganization.Size = new System.Drawing.Size(355, 59);
            this.lblOrganization.TabIndex = 6;
            this.lblOrganization.Text = "XX-XX-XX-XX";
            this.lblOrganization.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOrganization.Visible = false;
            // 
            // lblCardId
            // 
            this.lblCardId.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblCardId.Font = new System.Drawing.Font("Verdana", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCardId.Location = new System.Drawing.Point(129, 36);
            this.lblCardId.Name = "lblCardId";
            this.lblCardId.Size = new System.Drawing.Size(355, 59);
            this.lblCardId.TabIndex = 5;
            this.lblCardId.Text = "XX-XX-XX-XX";
            this.lblCardId.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupAmount
            // 
            this.groupAmount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupAmount.Controls.Add(this.lblAmount);
            this.groupAmount.Font = new System.Drawing.Font("Verdana", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupAmount.Location = new System.Drawing.Point(12, 276);
            this.groupAmount.Name = "groupAmount";
            this.groupAmount.Size = new System.Drawing.Size(578, 139);
            this.groupAmount.TabIndex = 3;
            this.groupAmount.TabStop = false;
            this.groupAmount.Text = "Средства";
            // 
            // lblAmount
            // 
            this.lblAmount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAmount.AutoSize = true;
            this.lblAmount.Font = new System.Drawing.Font("Verdana", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblAmount.Location = new System.Drawing.Point(236, 52);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(124, 59);
            this.lblAmount.TabIndex = 2;
            this.lblAmount.Text = "XXX";
            this.lblAmount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 427);
            this.Controls.Add(this.groupAmount);
            this.Controls.Add(this.groupCard);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Intems Lab";
            this.groupCard.ResumeLayout(false);
            this.groupCard.PerformLayout();
            this.groupAmount.ResumeLayout(false);
            this.groupAmount.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupCard;
        private System.Windows.Forms.GroupBox groupAmount;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblOrganization;
        private System.Windows.Forms.Label lblCardId;
    }
}