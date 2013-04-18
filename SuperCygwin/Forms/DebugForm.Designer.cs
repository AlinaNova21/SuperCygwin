namespace SuperCygwin.Forms
{
    partial class DebugForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioCygwin = new System.Windows.Forms.RadioButton();
            this.radioPutty = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 24);
            this.button1.TabIndex = 0;
            this.button1.Text = "Import";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioPutty);
            this.groupBox1.Controls.Add(this.radioCygwin);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(97, 67);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SSH Handler";
            // 
            // radioCygwin
            // 
            this.radioCygwin.AutoSize = true;
            this.radioCygwin.Location = new System.Drawing.Point(12, 19);
            this.radioCygwin.Name = "radioCygwin";
            this.radioCygwin.Size = new System.Drawing.Size(59, 17);
            this.radioCygwin.TabIndex = 3;
            this.radioCygwin.TabStop = true;
            this.radioCygwin.Text = "Cygwin";
            this.radioCygwin.UseVisualStyleBackColor = true;
            // 
            // radioPutty
            // 
            this.radioPutty.AutoSize = true;
            this.radioPutty.Location = new System.Drawing.Point(12, 42);
            this.radioPutty.Name = "radioPutty";
            this.radioPutty.Size = new System.Drawing.Size(49, 17);
            this.radioPutty.TabIndex = 3;
            this.radioPutty.TabStop = true;
            this.radioPutty.Text = "Putty";
            this.radioPutty.UseVisualStyleBackColor = true;
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 262);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DebugForm";
            this.Text = "DebugForm";
            this.Load += new System.EventHandler(this.DebugForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioPutty;
        private System.Windows.Forms.RadioButton radioCygwin;

    }
}