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
    public partial class MetroMenuElement : UserControl
    {
        public MetroMenuElement()
        {
            InitializeComponent();
        }

        public string Title
        {
            get
            {
                return lbTitle.Text;
            }
            set
            {
                lbTitle.Text = value;
            }
        }

        public string Description
        {
            get
            {
                return lbDescription.Text;
            }
            set
            {
                lbDescription.Text = value;
            }
        }

        public Image Icon
        {
            get
            {
                return picIcon.Image;
            }
            set
            {
                picIcon.Image = value;
            }
        }

        bool selected;

        public event EventHandler SelectedChanged;

        public bool Selected
        {
            get
            {
                return selected;
            }

            set
            {
                if (selected == value) return;
                selected = value;
                if(SelectedChanged!=null)
                {
                    SelectedChanged(this, new EventArgs());
                }
                if (selected)
                {
                    this.BackColor = Color.FromArgb(51, 102, 153);
                }
                else
                {
                    this.BackColor = Color.FromArgb(51, 153, 153);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            foreach(Control child in Controls)
            {
                child.MouseEnter += Child_MouseEnter;
                child.MouseLeave += Child_MouseLeave;
                child.Click += Child_Click;
            }
            base.OnLoad(e);
        }

        private void Child_Click(object sender, EventArgs e)
        {
            Selected = true;
        }

        private void Child_MouseLeave(object sender, EventArgs e)
        {
            OnMounseLeave();
        }

        private void Child_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter();
        }

        private void MetroMenuElement_MouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter();
        }

        private void OnMouseEnter()
        {
            if (!Selected)
            {
                this.BackColor = Color.FromArgb(51, 153, 204);
            }
        }

        private void MetroMenuElement_MouseLeave(object sender, EventArgs e)
        {
            OnMounseLeave();
        }

        private void OnMounseLeave()
        {
            if (!Selected)
            {
                this.BackColor = Color.FromArgb(51, 153, 153);
            }
        }

        private void MetroMenuElement_Click(object sender, EventArgs e)
        {
            Selected = true;
        }
    }
}
