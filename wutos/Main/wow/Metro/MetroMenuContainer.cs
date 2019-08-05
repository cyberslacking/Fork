using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Metro
{
    public partial class MetroMenuContainer : UserControl
    {
        public int BorderWidth { get; set; }
        public int ElementWidth { get; set; }
        public int ElementHeight { get; set; }

        public List<MetroMenuElement> Elements { get; set; }
        public event EventHandler<MenuElementEventArgs> ElementSelected;

        public MetroMenuContainer()
        {
            InitializeComponent();
            BorderWidth = 4;
            ElementWidth = 128;
            ElementHeight = 128;
        }

        private void SetElements()
        {
            if (Elements == null) return;
            int eWidth = BorderWidth + ElementWidth;
            int eHeight = BorderWidth + ElementHeight;
            int count = Elements.Count;
            
            int col = this.Size.Width / eWidth;

            if (col == 0) return;
            int row = (int)Math.Ceiling((double)count / col);

            Console.WriteLine("{0}x{1}",row,col);

            var k = 0;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (k >= count) break;
                    var child = Elements[k];
                    child.Location = new Point(j * eWidth  + BorderWidth, eHeight * i + BorderWidth);
                    child.Size = new Size(ElementWidth, ElementHeight);
                    Console.WriteLine("X:{0} Y:{1} W:{2} H:{3}", child.Location.X, child.Location.Y, child.Size.Width,child.Size.Height);
                    if (!this.Controls.Contains(child))
                    {
                        child.SelectedChanged += Child_SelectedChanged;
                        this.Controls.Add(child);
                    }
                    k++;
                }
            }

            //MessageBox.Show(this, "OK");
            Console.WriteLine("SIZE CHANGED");
        }

        private void MetroMenuContainer_Load(object sender, EventArgs e)
        {
            //SetLayout();

            SetElements();
        }

        private void SetLayout()
        {
            if (Elements == null) return;
            int eWidth = BorderWidth + ElementWidth;
            int count = Elements.Count;
            int col = this.Size.Width / eWidth;
            if (col == 0) return;
            int row = (int)Math.Ceiling((double)count / col);

            TableLayoutPanel layout = null;
            if (Controls.Count == 0)
            {
                layout = new TableLayoutPanel();
                this.Controls.Add(layout);
            }

            layout = Controls[0] as TableLayoutPanel;
            layout.BackColor = Color.Green;
            layout.Size = new Size(eWidth * col, row * eWidth);

            DynamicLayout(layout, row, col);

            var k = 0;

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (k >= count) break;
                    var child = Elements[k];
                    child.SelectedChanged += Child_SelectedChanged;
                    child.Dock = DockStyle.Fill;
                    layout.Controls.Add(child);
                    layout.SetRow(child, i);
                    layout.SetColumn(child, j);
                    k++;
                }
            }
        }

        private void Child_SelectedChanged(object sender, EventArgs e)
        {
            var selChild = sender as MetroMenuElement;
            if (selChild.Selected)
            {
                foreach (var child in Elements)
                {
                    if (child != selChild)
                        child.Selected = false;
                }
                if (ElementSelected != null)
                {
                    ElementSelected(this, new MenuElementEventArgs() { Element = selChild });
                }
            }
        }

        /// <summary>  
        /// 动态布局  
        /// </summary>  
        /// <param name="layoutPanel">布局面板</param>  
        /// <param name="row">行</param>  
        /// <param name="col">列</param>  
        private void DynamicLayout(TableLayoutPanel layoutPanel, int row, int col)
        {
            layoutPanel.RowCount = row; //设置分成几行  
            for (int i = 0; i < row; i++)
            {
                layoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            }
            layoutPanel.ColumnCount = col; //设置分成几列  
            for (int i = 0; i < col; i++)
            {
                layoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            }
        }

        private void MetroMenuContainer_SizeChanged(object sender, EventArgs e)
        {
            SetElements();
        }
    }
}
