

namespace wow
{
    partial class Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            this.panelBrowser = new System.Windows.Forms.Panel();
            this.panelNavigation = new System.Windows.Forms.Panel();
            this.buttonSystem = new System.Windows.Forms.Button();
            this.buttonConfig = new System.Windows.Forms.Button();
            this.buttonMap = new System.Windows.Forms.Button();
            this.buttonVideo = new System.Windows.Forms.Button();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.menuFolder = new Metro.MetroMenuContainer();
            this.buttonClose = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panelNavigation.SuspendLayout();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBrowser
            // 
            this.panelBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBrowser.Location = new System.Drawing.Point(65, 134);
            this.panelBrowser.Margin = new System.Windows.Forms.Padding(2);
            this.panelBrowser.Name = "panelBrowser";
            this.panelBrowser.Size = new System.Drawing.Size(900, 426);
            this.panelBrowser.TabIndex = 0;
            // 
            // panelNavigation
            // 
            this.panelNavigation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelNavigation.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panelNavigation.Controls.Add(this.buttonSystem);
            this.panelNavigation.Controls.Add(this.buttonConfig);
            this.panelNavigation.Controls.Add(this.buttonMap);
            this.panelNavigation.Controls.Add(this.buttonVideo);
            this.panelNavigation.Location = new System.Drawing.Point(-1, 0);
            this.panelNavigation.Margin = new System.Windows.Forms.Padding(0);
            this.panelNavigation.Name = "panelNavigation";
            this.panelNavigation.Size = new System.Drawing.Size(64, 561);
            this.panelNavigation.TabIndex = 1;
            // 
            // buttonSystem
            // 
            this.buttonSystem.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSystem.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonSystem.BackgroundImage")));
            this.buttonSystem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonSystem.FlatAppearance.BorderSize = 0;
            this.buttonSystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSystem.Location = new System.Drawing.Point(1, 497);
            this.buttonSystem.Name = "buttonSystem";
            this.buttonSystem.Size = new System.Drawing.Size(64, 64);
            this.buttonSystem.TabIndex = 3;
            this.buttonSystem.UseVisualStyleBackColor = true;
            // 
            // buttonConfig
            // 
            this.buttonConfig.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonConfig.BackgroundImage")));
            this.buttonConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonConfig.FlatAppearance.BorderSize = 0;
            this.buttonConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConfig.Location = new System.Drawing.Point(1, 263);
            this.buttonConfig.Name = "buttonConfig";
            this.buttonConfig.Size = new System.Drawing.Size(64, 64);
            this.buttonConfig.TabIndex = 2;
            this.buttonConfig.UseVisualStyleBackColor = true;
            // 
            // buttonMap
            // 
            this.buttonMap.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonMap.BackgroundImage")));
            this.buttonMap.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonMap.FlatAppearance.BorderSize = 0;
            this.buttonMap.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMap.Location = new System.Drawing.Point(1, 199);
            this.buttonMap.Name = "buttonMap";
            this.buttonMap.Size = new System.Drawing.Size(64, 64);
            this.buttonMap.TabIndex = 1;
            this.buttonMap.UseVisualStyleBackColor = true;
            this.buttonMap.Click += new System.EventHandler(this.buttonNavigation_Click);
            // 
            // buttonVideo
            // 
            this.buttonVideo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonVideo.BackgroundImage")));
            this.buttonVideo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonVideo.FlatAppearance.BorderSize = 0;
            this.buttonVideo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonVideo.Location = new System.Drawing.Point(1, 135);
            this.buttonVideo.Name = "buttonVideo";
            this.buttonVideo.Size = new System.Drawing.Size(64, 64);
            this.buttonVideo.TabIndex = 0;
            this.buttonVideo.UseVisualStyleBackColor = true;
            // 
            // panelMenu
            // 
            this.panelMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMenu.Controls.Add(this.button2);
            this.panelMenu.Controls.Add(this.buttonClose);
            this.panelMenu.Controls.Add(this.menuFolder);
            this.panelMenu.Location = new System.Drawing.Point(65, 4);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(900, 128);
            this.panelMenu.TabIndex = 2;
            // 
            // menuFolder
            // 
            this.menuFolder.BackColor = System.Drawing.SystemColors.Window;
            this.menuFolder.BorderWidth = 4;
            this.menuFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuFolder.ElementHeight = 128;
            this.menuFolder.Elements = null;
            this.menuFolder.ElementWidth = 128;
            this.menuFolder.Location = new System.Drawing.Point(0, 0);
            this.menuFolder.Name = "menuFolder";
            this.menuFolder.Size = new System.Drawing.Size(900, 128);
            this.menuFolder.TabIndex = 1;
            // wq t
            // buttonClose
            // 
            this.buttonClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonClose.BackgroundImage")));
            this.buttonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.buttonClose.FlatAppearance.BorderSize = 0;
            this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonClose.Location = new System.Drawing.Point(836, 0);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(64, 64);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("button2.BackgroundImage")));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(702, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 64);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 561);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelNavigation);
            this.Controls.Add(this.panelBrowser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form";
            this.Text = "Display";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_FormClosing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.panelNavigation.ResumeLayout(false);
            this.panelMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBrowser;
        private System.Windows.Forms.Panel panelNavigation;
        private System.Windows.Forms.Button buttonVideo;
        private System.Windows.Forms.Button buttonMap;
        private System.Windows.Forms.Button buttonConfig;
        private System.Windows.Forms.Button buttonSystem;
        private Metro.MetroMenuContainer menuFolder;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button buttonClose;
    }
}

