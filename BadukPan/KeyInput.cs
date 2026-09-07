using System;
using System.Drawing;
using System.Windows.Forms;

namespace BadukPan
{
    public partial class Form1
    {
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
if (e.KeyCode == Keys.D)
            {
                DeleteLastMove();
            }
            else if (e.KeyCode == Keys.M)
            {
                isMuted = !isMuted;
                ShowStatus(isMuted ? "소리 없음" : "소리 켜짐");
                panel1.Invalidate();
            }
            else if (e.KeyCode == Keys.P)
            {
                gridPointerEnabled = !gridPointerEnabled;
                ShowStatus(gridPointerEnabled ? "그리드 포인터 사용" : "그리드 포인터 숨김");
                panel1.Invalidate();
            }
else if (e.KeyCode == Keys.C && e.Control)
            {
                for (int i = 0; i < 19; i++)
                    for (int j = 0; j < 19; j++)
                    {
                        Board[i, j] = STONE.none;
                        moveNumbers[i, j] = 0;
                    }
                moveCount = 0;
                lastStoneX = lastStoneY = -1;
                flag = false;
                moveHistory.Clear();
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
private void DeleteLastMove()
        {
            for (int i = moveHistory.Count - 1; i >= 0; i--)
            {
                Point p = moveHistory[i];
                if (Board[p.X, p.Y] == STONE.none)
                    continue;

                moveNumbers[p.X, p.Y] = 0;
                moveHistory.RemoveAt(i);
                Board[p.X, p.Y] = STONE.none;
                flag = !flag;
                SetLastStoneToLastMove();
                panel1.Invalidate();
                return;
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