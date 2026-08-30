using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BadukPan
{
    public partial class Form1 : Form
    {
        int margin = 40;
        int 눈Size = 30;
        int 돌Size = 28;
        int 화점Size = 10;

        Pen pen;
        Brush wBrush, bBrush;

        enum STONE { none, black, white };
        STONE[,] 바둑판 = new STONE[19, 19];
        bool flag = false;
        bool imageFlag = true;

        int mouseX = 0, mouseY = 0;

        public Form1()
        {
            InitializeComponent();

            // --- ✅ [1] 더블 버퍼링 설정 ---
            this.panel1.DoubleBuffered(true);

            this.panel1.Paint += panel1_Paint;
            this.panel1.MouseDown += panel1_MouseDown;

            this.Text = "바둑판";
            this.BackColor = Color.FromArgb(225, 179, 104);

            pen = new Pen(Color.Black);
            bBrush = new SolidBrush(Color.Black);
            wBrush = new SolidBrush(Color.White);

            this.ClientSize = new Size(2 * margin + 18 * 눈Size,
              2 * margin + 18 * 눈Size + menuStrip1.Height);

            panel1.SetBounds(0, menuStrip1.Height, this.ClientSize.Width,
                             this.ClientSize.Height - menuStrip1.Height);
            panel1.BackColor = Color.Transparent;
        }

        private void 정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (About dlg = new About())
                dlg.ShowDialog(this);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = (e.X - margin + 눈Size / 2) / 눈Size;
            int y = (e.Y - margin + 눈Size / 2) / 눈Size;

            mouseX = e.X;
            mouseY = e.Y;

            if (x < 0 || x > 18 || y < 0 || y > 18)
                return;

            if (바둑판[x, y] != STONE.none)
                return;

            STONE me = flag ? STONE.white : STONE.black;
            바둑판[x, y] = me;

            // --- ✅ [3] 상대방 돌이 사방으로 둘러싸이면(집 없음) 집어 올림 ---
            bool captured = false;
            CaptureOpponent(x, y, me, ref captured);

            // 자기 돌에 집이 없으면(자살수) 두지 못함
            if (!captured && NoLiberty(x, y))
            {
                바둑판[x, y] = STONE.none;
                return;
            }

            flag = !flag;
            panel1.Invalidate();
        }

        private void CaptureOpponent(int x, int y, STONE me, ref bool captured)
        {
            STONE opp = (me == STONE.black) ? STONE.white : STONE.black;

            foreach (Point p in Neighbors(x, y))
            {
                if (바둑판[p.X, p.Y] != opp)
                    continue;

                if (NoLiberty(p.X, p.Y))
                {
                    RemoveGroup(p.X, p.Y);
                    captured = true;
                }
            }
        }

        private bool NoLiberty(int sx, int sy)
        {
            STONE color = 바둑판[sx, sy];
            bool[,] visited = new bool[19, 19];

            Queue<int> qx = new Queue<int>();
            Queue<int> qy = new Queue<int>();
            qx.Enqueue(sx);
            qy.Enqueue(sy);
            visited[sx, sy] = true;

            while (qx.Count > 0)
            {
                int cx = qx.Dequeue();
                int cy = qy.Dequeue();

                foreach (Point p in Neighbors(cx, cy))
                {
                    if (바둑판[p.X, p.Y] == STONE.none)
                        return false;

                    if (바둑판[p.X, p.Y] == color && !visited[p.X, p.Y])
                    {
                        visited[p.X, p.Y] = true;
                        qx.Enqueue(p.X);
                        qy.Enqueue(p.Y);
                    }
                }
            }
            return true;
        }

        private void RemoveGroup(int sx, int sy)
        {
            STONE color = 바둑판[sx, sy];
            bool[,] visited = new bool[19, 19];

            Queue<int> qx = new Queue<int>();
            Queue<int> qy = new Queue<int>();
            qx.Enqueue(sx);
            qy.Enqueue(sy);
            visited[sx, sy] = true;

            while (qx.Count > 0)
            {
                int cx = qx.Dequeue();
                int cy = qy.Dequeue();

                바둑판[cx, cy] = STONE.none;

                foreach (Point p in Neighbors(cx, cy))
                {
                    if (바둑판[p.X, p.Y] == color && !visited[p.X, p.Y])
                    {
                        visited[p.X, p.Y] = true;
                        qx.Enqueue(p.X);
                        qy.Enqueue(p.Y);
                    }
                }
            }
        }

        private IEnumerable<Point> Neighbors(int x, int y)
        {
            if (x > 0) yield return new Point(x - 1, y);
            if (y > 0) yield return new Point(x, y - 1);
            if (x < 18) yield return new Point(x + 1, y);
            if (y < 18) yield return new Point(x, y + 1);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DrawBoard(g);
            DrawStones(g);
        }

        private void DrawBoard(Graphics g)
        {
            for (int i = 0; i < 19; i++)
            {
                g.DrawLine(pen, margin + i * 눈Size, margin,
                                 margin + i * 눈Size, margin + 18 * 눈Size);
                g.DrawLine(pen, margin, margin + i * 눈Size,
                                 margin + 18 * 눈Size, margin + i * 눈Size);
            }

            for (int x = 3; x <= 15; x += 6)
            {
                for (int y = 3; y <= 15; y += 6)
                {
                    g.FillEllipse(bBrush,
                        margin + 눈Size * x - 화점Size / 2,
                        margin + 눈Size * y - 화점Size / 2,
                        화점Size, 화점Size);
                }
            }
        }

        private void DrawStones(Graphics g)
        {
            for (int x = 0; x < 19; x++)
            {
                for (int y = 0; y < 19; y++)
                {
                    if (바둑판[x, y] == STONE.none)
                        continue;

                    Rectangle r = new Rectangle(
                        margin + 눈Size * x - 돌Size / 2,
                        margin + 눈Size * y - 돌Size / 2,
                        돌Size, 돌Size);

                    if (!imageFlag)
                    {
                        if (바둑판[x, y] == STONE.black)
                            g.FillEllipse(bBrush, r);
                        else
                            g.FillEllipse(wBrush, r);
                    }
                    else
                    {
                        string imgPath = (바둑판[x, y] == STONE.black)
                            ? "../../Images/Go_b_no_bg.png"
                            : "../../Images/Go_w_no_bg.png";

                        try
                        {
                            using (Bitmap bmp = new Bitmap(imgPath))
                                g.DrawImage(bmp, r);
                        }
                        catch
                        {
                            if (바둑판[x, y] == STONE.black)
                                g.FillEllipse(bBrush, r);
                            else
                                g.FillEllipse(wBrush, r);
                        }
                    }
                }
            }
        }
    }

    // --- ✅ DoubleBuffered 확장 메서드 추가 ---
    public static class ControlExtensions
    {
        public static void DoubleBuffered(this Control c, bool enable)
        {
            System.Reflection.PropertyInfo aProp =
                typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, enable, null);
        }
    }
}