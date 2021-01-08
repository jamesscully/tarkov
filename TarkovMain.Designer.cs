namespace TarkovAssistant
{
    partial class TarkovMain
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
            this.UIControlsHeader = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.UIControlsHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // UIControlsHeader
            // 
            this.UIControlsHeader.Controls.Add(this.button3);
            this.UIControlsHeader.Controls.Add(this.button2);
            this.UIControlsHeader.Controls.Add(this.button1);
            this.UIControlsHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.UIControlsHeader.Location = new System.Drawing.Point(0, 0);
            this.UIControlsHeader.Name = "UIControlsHeader";
            this.UIControlsHeader.Size = new System.Drawing.Size(800, 50);
            this.UIControlsHeader.TabIndex = 1;
            this.UIControlsHeader.TabStop = false;
            this.UIControlsHeader.Text = "Map Selection";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(169, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Interchange";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.MapButtonClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(88, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Shoreline";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.MapButtonClick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Customs";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.MapButtonClick);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 3;
            this.panel1.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.OnMapContainerLoad);
            // 
            // TarkovMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.UIControlsHeader);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "TarkovMain";
            this.ShowIcon = false;
            this.Text = "Tarkov Assistant - Main";
            this.Shown += new System.EventHandler(this.OnFormLoad);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            this.UIControlsHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox UIControlsHeader;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Panel panel1;
    }
}

