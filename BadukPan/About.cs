using System;
using System.Drawing;
using System.Windows.Forms;

namespace BadukPan
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            LoadLogo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadLogo()
        {
            try
            {
                using (Bitmap bmp = new Bitmap("../../Images/Goban_icon.png"))
                    pictureBox1.Image = new Bitmap(bmp);
            }
            catch
            {
                pictureBox1.Visible = false;
            }
        }
    }
}