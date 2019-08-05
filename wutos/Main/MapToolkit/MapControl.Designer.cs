namespace MapToolkit
{
    public partial class MapControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ToolStripMenuItemSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemAddTracker = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemDeleteTracker = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripOperate = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemFront = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemBack = new System.Windows.Forms.ToolStripMenuItem();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.contextMenuStripOperate.SuspendLayout();
            this.SuspendLayout();
            // 
            // ToolStripMenuItemSelectAll
            // 
            this.ToolStripMenuItemSelectAll.Name = "ToolStripMenuItemSelectAll";
            this.ToolStripMenuItemSelectAll.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemSelectAll.Text = "全选";
            this.ToolStripMenuItemSelectAll.Click += new System.EventHandler(this.ToolStripMenuItemSelectAll_Click);
            // 
            // ToolStripMenuItemDelete
            // 
            this.ToolStripMenuItemDelete.Name = "ToolStripMenuItemDelete";
            this.ToolStripMenuItemDelete.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemDelete.Text = "删除";
            this.ToolStripMenuItemDelete.Click += new System.EventHandler(this.ToolStripMenuItemDelete_Click);
            // 
            // ToolStripMenuItemAddTracker
            // 
            this.ToolStripMenuItemAddTracker.Name = "ToolStripMenuItemAddTracker";
            this.ToolStripMenuItemAddTracker.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemAddTracker.Text = "添加控制点";
            this.ToolStripMenuItemAddTracker.Click += new System.EventHandler(this.ToolStripMenuItemAddTracker_Click);
            // 
            // ToolStripMenuItemDeleteTracker
            // 
            this.ToolStripMenuItemDeleteTracker.Name = "ToolStripMenuItemDeleteTracker";
            this.ToolStripMenuItemDeleteTracker.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemDeleteTracker.Text = "删除控制点";
            this.ToolStripMenuItemDeleteTracker.Click += new System.EventHandler(this.ToolStripMenuItemDeleteTracker_Click);
            // 
            // contextMenuStripOperate
            // 
            this.contextMenuStripOperate.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStripOperate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemSelectAll,
            this.ToolStripMenuItemDelete,
            this.ToolStripMenuItemAddTracker,
            this.ToolStripMenuItemDeleteTracker,
            this.ToolStripMenuItemCopy,
            this.ToolStripMenuItemPaste,
            this.ToolStripMenuItemFront,
            this.ToolStripMenuItemBack});
            this.contextMenuStripOperate.Name = "contextMenuStripOperate";
            this.contextMenuStripOperate.Size = new System.Drawing.Size(171, 228);
            // 
            // ToolStripMenuItemCopy
            // 
            this.ToolStripMenuItemCopy.Name = "ToolStripMenuItemCopy";
            this.ToolStripMenuItemCopy.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemCopy.Text = "复制";
            this.ToolStripMenuItemCopy.Click += new System.EventHandler(this.ToolStripMenuItemCopy_Click);
            // 
            // ToolStripMenuItemPaste
            // 
            this.ToolStripMenuItemPaste.Name = "ToolStripMenuItemPaste";
            this.ToolStripMenuItemPaste.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemPaste.Text = "粘贴";
            this.ToolStripMenuItemPaste.Click += new System.EventHandler(this.ToolStripMenuItemPaste_Click);
            // 
            // ToolStripMenuItemFront
            // 
            this.ToolStripMenuItemFront.Name = "ToolStripMenuItemFront";
            this.ToolStripMenuItemFront.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemFront.Text = "前置";
            this.ToolStripMenuItemFront.Click += new System.EventHandler(this.ToolStripMenuItemFront_Click);
            // 
            // ToolStripMenuItemBack
            // 
            this.ToolStripMenuItemBack.Name = "ToolStripMenuItemBack";
            this.ToolStripMenuItemBack.Size = new System.Drawing.Size(170, 28);
            this.ToolStripMenuItemBack.Text = "后置";
            this.ToolStripMenuItemBack.Click += new System.EventHandler(this.ToolStripMenuItemBack_Click);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(936, 225);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 0;
            this.toolTip.UseAnimation = false;
            this.toolTip.UseFading = false;
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaxZoom = 23;
            this.MinZoom = 1;
            this.Name = "MapControl";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Size = new System.Drawing.Size(1629, 879);
            this.Zoom = 13D;
            this.OnMarkerClick += new GMap.NET.WindowsForms.MarkerClick(this.DrawArea_OnMarkerClick);
            this.OnRouteDoubleClick += new GMap.NET.WindowsForms.RouteDoubleClick(this.DrawArea_OnRouteDoubleClick);
            this.OnRouteEnter += new GMap.NET.WindowsForms.RouteEnter(this.DrawArea_OnRouteEnter);
            this.OnRouteLeave += new GMap.NET.WindowsForms.RouteLeave(this.DrawArea_OnRouteLeave);
            this.OnMarkerEnter += new GMap.NET.WindowsForms.MarkerEnter(this.DrawArea_OnMarkerEnter);
            this.OnMarkerLeave += new GMap.NET.WindowsForms.MarkerLeave(this.DrawArea_OnMarkerLeave);
            this.OnPolygonEnter += new GMap.NET.WindowsForms.PolygonEnter(this.DrawArea_OnPolygonEnter);
            this.OnPolygonLeave += new GMap.NET.WindowsForms.PolygonLeave(this.DrawArea_OnPolygonLeave);
            this.OnMapDrag += new GMap.NET.MapDrag(this.DrawArea_OnMapDrag);
            this.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.DrawArea_OnMapZoomChanged);
            this.Load += new System.EventHandler(this.MapControl_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DrawArea_Paint);
            this.DoubleClick += new System.EventHandler(this.MapControl_DoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseDown);
            this.MouseHover += new System.EventHandler(this.MapControl_MouseHover);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DrawArea_MouseUp);
            this.contextMenuStripOperate.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemSelectAll;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDelete;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAddTracker;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemDeleteTracker;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripOperate;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemFront;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemBack;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemPaste;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
