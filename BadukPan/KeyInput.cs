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
        }
    }
}