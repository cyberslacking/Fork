using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using License;

namespace RegisterApp
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        private void buttonBuild_Click(object sender, EventArgs e)
        {
            string fileName = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
            }
            else
            {
                return;
            }
            string localFileName = string.Concat(
                Environment.CurrentDirectory,
                Path.DirectorySeparatorChar,
                RegistFileHelper.ComputerInfofile);

            if (fileName != localFileName)
                File.Copy(fileName, localFileName, true);
            string computer = RegistFileHelper.ReadComputerInfoFile();
            EncryptionHelper help = new EncryptionHelper(EncryptionKeyEnum.KeyB);
            string md5String = help.GetMD5String(computer);
            string registInfo = help.EncryptString(md5String);
            RegistFileHelper.WriteRegistFile(registInfo);
            MessageBox.Show("注册码已生成");
        }
    }
}
