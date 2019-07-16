namespace EncyrptionChecker
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.txtVenderID = new System.Windows.Forms.TextBox();
            this.txtAppID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblEncryptedResult = new System.Windows.Forms.Label();
            this.txtEncryptedResult = new System.Windows.Forms.TextBox();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Phone Number";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Device Vender ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Applicaiton ID";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.Location = new System.Drawing.Point(125, 40);
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.Size = new System.Drawing.Size(115, 20);
            this.txtPhoneNumber.TabIndex = 3;
            this.txtPhoneNumber.Text = "+923336699701";
            // 
            // txtVenderID
            // 
            this.txtVenderID.Location = new System.Drawing.Point(125, 66);
            this.txtVenderID.Name = "txtVenderID";
            this.txtVenderID.Size = new System.Drawing.Size(280, 20);
            this.txtVenderID.TabIndex = 4;
            this.txtVenderID.Text = "874B8453-A03B-495A-90F5-16B581E89330";
            // 
            // txtAppID
            // 
            this.txtAppID.Location = new System.Drawing.Point(125, 93);
            this.txtAppID.Name = "txtAppID";
            this.txtAppID.Size = new System.Drawing.Size(280, 20);
            this.txtAppID.TabIndex = 5;
            this.txtAppID.Text = "A8FDAA07-2C4A-4BC2-8091-D9AAABB925E3";
            this.txtAppID.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Encrypted Result: ";
            // 
            // lblEncryptedResult
            // 
            this.lblEncryptedResult.AutoSize = true;
            this.lblEncryptedResult.Location = new System.Drawing.Point(125, 126);
            this.lblEncryptedResult.Name = "lblEncryptedResult";
            this.lblEncryptedResult.Size = new System.Drawing.Size(0, 13);
            this.lblEncryptedResult.TabIndex = 7;
            // 
            // txtEncryptedResult
            // 
            this.txtEncryptedResult.Location = new System.Drawing.Point(125, 123);
            this.txtEncryptedResult.Name = "txtEncryptedResult";
            this.txtEncryptedResult.Size = new System.Drawing.Size(280, 20);
            this.txtEncryptedResult.TabIndex = 8;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(125, 166);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 9;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 201);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.txtEncryptedResult);
            this.Controls.Add(this.lblEncryptedResult);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAppID);
            this.Controls.Add(this.txtVenderID);
            this.Controls.Add(this.txtPhoneNumber);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.TextBox txtVenderID;
        private System.Windows.Forms.TextBox txtAppID;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblEncryptedResult;
        private System.Windows.Forms.TextBox txtEncryptedResult;
        private System.Windows.Forms.Button btnGenerate;

    }
}

