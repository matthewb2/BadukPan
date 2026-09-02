using System;
using System.Drawing;
using System.Windows.Forms;

namespace BadukPan
{
    public partial class Form1
    {
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.D && lastStoneX >= 0)
            {
                moveNumbers[lastStoneX, lastStoneY] = 0;
                바둑판[lastStoneX, lastStoneY] = STONE.none;
                lastStoneX = lastStoneY = -1;
                flag = !flag;
                panel1.Invalidate();
            }
            else if (e.KeyCode == Keys.M)
            {
                bool start = !showMoveNumbers;
                showMoveNumbers = start;
                if (!showMoveNumbers)
                    moveCount = 0;
                ShowStatus(start ? "수순 기록 시작" : "수순 기록 해제");
                panel1.Invalidate();
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                for (int i = 0; i < 19; i++)
                    for (int j = 0; j < 19; j++)
                    {
                        바둑판[i, j] = STONE.none;
                        moveNumbers[i, j] = 0;
                    }
                moveCount = 0;
                lastStoneX = lastStoneY = -1;
                flag = false;
                panel1.Invalidate();
            }
            else if (e.KeyCode == Keys.D0 ||
                     e.KeyCode == Keys.NumPad0 ||
                     e.KeyCode == Keys.D1 ||
                     e.KeyCode == Keys.NumPad1 ||
                     e.KeyCode == Keys.D2 ||
                     e.KeyCode == Keys.NumPad2 ||
                     e.KeyCode == Keys.D3 ||
                     e.KeyCode == Keys.NumPad3)
            {
                LoadBackground(e.KeyCode);
                panel1.Invalidate();
            }
        }
private void LoadBackground(Keys key)
        {
            string file = null;
            if (key == Keys.D0 || key == Keys.NumPad0) file = null;
            else if (key == Keys.D1 || key == Keys.NumPad1) file = "bg1.jpg";
            else if (key == Keys.D2 || key == Keys.NumPad2) file = "bg2.jpg";
            else if (key == Keys.D3 || key == Keys.NumPad3) file = "bg3.jpg";

            if (background != null)
                background.Dispose();
            background = null;

            if (file == null)
                return;

            try
            {
                background = new Bitmap("../../Images/" + file);
            }
            catch
            {
                background = null;
            }
        }
    }
}