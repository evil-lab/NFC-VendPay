namespace UserManager.GUI
{
    partial class RegisterForm
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCardTag = new System.Windows.Forms.TextBox();
            this.groupUserData = new System.Windows.Forms.GroupBox();
            this.txtAddAmount = new System.Windows.Forms.TextBox();
            this.btnAddAmount = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOrganization = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.groupUserData.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Enabled = false;
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOk.Location = new System.Drawing.Point(157, 410);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(148, 44);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Регистрация";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.BtnOkClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.Location = new System.Drawing.Point(355, 410);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(148, 44);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(28, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 25);
            this.label3.TabIndex = 9;
            this.label3.Text = "Карта";
            // 
            // txtCardTag
            // 
            this.txtCardTag.Enabled = false;
            this.txtCardTag.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtCardTag.Location = new System.Drawing.Point(142, 12);
            this.txtCardTag.Name = "txtCardTag";
            this.txtCardTag.Size = new System.Drawing.Size(457, 33);
            this.txtCardTag.TabIndex = 8;
            this.txtCardTag.Text = "XX-XX-XX-XX";
            this.txtCardTag.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupUserData
            // 
            this.groupUserData.Controls.Add(this.txtAddAmount);
            this.groupUserData.Controls.Add(this.btnAddAmount);
            this.groupUserData.Controls.Add(this.label4);
            this.groupUserData.Controls.Add(this.txtAmount);
            this.groupUserData.Controls.Add(this.label2);
            this.groupUserData.Controls.Add(this.txtOrganization);
            this.groupUserData.Controls.Add(this.lblPhone);
            this.groupUserData.Controls.Add(this.txtPhone);
            this.groupUserData.Controls.Add(this.label1);
            this.groupUserData.Controls.Add(this.txtName);
            this.groupUserData.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupUserData.Location = new System.Drawing.Point(17, 66);
            this.groupUserData.Name = "groupUserData";
            this.groupUserData.Size = new System.Drawing.Size(600, 314);
            this.groupUserData.TabIndex = 10;
            this.groupUserData.TabStop = false;
            this.groupUserData.Text = "Данные";
            // 
            // txtAddAmount
            // 
            this.txtAddAmount.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtAddAmount.Location = new System.Drawing.Point(453, 233);
            this.txtAddAmount.Name = "txtAddAmount";
            this.txtAddAmount.Size = new System.Drawing.Size(129, 33);
            this.txtAddAmount.TabIndex = 16;
            this.txtAddAmount.Text = "100";
            this.txtAddAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnAddAmount
            // 
            this.btnAddAmount.Location = new System.Drawing.Point(323, 234);
            this.btnAddAmount.Name = "btnAddAmount";
            this.btnAddAmount.Size = new System.Drawing.Size(124, 33);
            this.btnAddAmount.TabIndex = 15;
            this.btnAddAmount.Text = "пополнить";
            this.btnAddAmount.UseVisualStyleBackColor = true;
            this.btnAddAmount.Click += new System.EventHandler(this.BtnAddAmountClick);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(11, 237);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 25);
            this.label4.TabIndex = 14;
            this.label4.Text = "Остаток";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // txtAmount
            // 
            this.txtAmount.Enabled = false;
            this.txtAmount.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtAmount.Location = new System.Drawing.Point(125, 234);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(189, 33);
            this.txtAmount.TabIndex = 13;
            this.txtAmount.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(11, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(155, 25);
            this.label2.TabIndex = 12;
            this.label2.Text = "Организация";
            // 
            // txtOrganization
            // 
            this.txtOrganization.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtOrganization.Location = new System.Drawing.Point(172, 152);
            this.txtOrganization.Name = "txtOrganization";
            this.txtOrganization.Size = new System.Drawing.Size(410, 33);
            this.txtOrganization.TabIndex = 11;
            this.txtOrganization.TextChanged += new System.EventHandler(this.DataTextChanged);
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblPhone.Location = new System.Drawing.Point(11, 96);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(108, 25);
            this.lblPhone.TabIndex = 10;
            this.lblPhone.Text = "Телефон";
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtPhone.Location = new System.Drawing.Point(125, 93);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(457, 33);
            this.txtPhone.TabIndex = 9;
            this.txtPhone.TextChanged += new System.EventHandler(this.DataTextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(11, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 25);
            this.label1.TabIndex = 8;
            this.label1.Text = "Ф.И.О";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.txtName.Location = new System.Drawing.Point(125, 34);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(457, 33);
            this.txtName.TabIndex = 7;
            this.txtName.TextChanged += new System.EventHandler(this.DataTextChanged);
            // 
            // RegisterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 466);
            this.Controls.Add(this.groupUserData);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtCardTag);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "RegisterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Регистрация";
            this.groupUserData.ResumeLayout(false);
            this.groupUserData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtCardTag;
        private System.Windows.Forms.GroupBox groupUserData;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOrganization;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Button btnAddAmount;
        private System.Windows.Forms.TextBox txtAddAmount;
    }
}