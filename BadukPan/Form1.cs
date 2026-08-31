using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BadukPan
{
    public partial class Form1 : Form
    {
        int margin = 40;
        int GridSize = 30;
        int DolSize = 28;
        int FeatureSize = 10;

        Pen pen;
        Brush wBrush, bBrush;

        enum STONE { none, black, white };
        STONE[,] 바둑판 = new STONE[19, 19];
        int[,] moveNumbers = new int[19, 19];
        int moveCount = 0;
        bool showMoveNumbers = false;
        bool flag = false;
        bool imageFlag = true;

        int mouseX = 0, mouseY = 0;
        int lastStoneX = -1, lastStoneY = -1;

        Timer statusTimer;
        Label statusLabel;

        public Form1()
        {
            InitializeComponent();

            // --- ✅ [1] 더블 버퍼링 설정 ---
            this.panel1.DoubleBuffered(true);

            this.panel1.Paint += panel1_Paint;
            this.panel1.MouseDown += panel1_MouseDown;
            this.KeyDown += Form1_KeyDown;
            this.KeyPreview = true;

            this.Text = "바둑판";
            this.BackColor = Color.FromArgb(225, 179, 104);

            pen = new Pen(Color.Black);
            bBrush = new SolidBrush(Color.Black);
            wBrush = new SolidBrush(Color.White);

            this.ClientSize = new Size(2 * margin + 18 * GridSize,
              2 * margin + 18 * GridSize + menuStrip1.Height);

            panel1.SetBounds(0, menuStrip1.Height, this.ClientSize.Width,
                             this.ClientSize.Height - menuStrip1.Height);
            panel1.BackColor = Color.Transparent;

            statusTimer = new Timer();
            statusTimer.Interval = 2000;
            statusTimer.Tick += StatusTimer_Tick;

            statusLabel = new Label();
            statusLabel.AutoSize = true;
            statusLabel.Font = new Font("굴림", 15f, FontStyle.Bold);
            statusLabel.ForeColor = Color.DarkRed;
            statusLabel.BackColor = Color.White;
            statusLabel.BorderStyle = BorderStyle.FixedSingle;
            statusLabel.Visible = false;
            panel1.Controls.Add(statusLabel);
        }

        private void ShowStatus(string text)
        {   
            statusLabel.Font = new Font("Arial", 15, FontStyle.Regular);
            statusLabel.Text = text;
            statusLabel.ForeColor = Color.Black;
            statusLabel.Location = new Point(
                (panel1.Width - statusLabel.PreferredWidth) / 2,
                (panel1.Height - statusLabel.PreferredHeight) / 2);
            statusLabel.Visible = true;
            statusLabel.BringToFront();
            statusLabel.BackColor = Color.White;
            statusTimer.Stop();
            statusTimer.Start();
        }

        private void StatusTimer_Tick(object sender, EventArgs e)
        {
            statusTimer.Stop();
            statusLabel.Visible = false;
        }

        private void 정보ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (About dlg = new About())
                dlg.ShowDialog(this);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            int x = (e.X - margin + GridSize / 2) / GridSize;
            int y = (e.Y - margin + GridSize / 2) / GridSize;

            mouseX = e.X;
            mouseY = e.Y;

            if (x < 0 || x > 18 || y < 0 || y > 18)
                return;

            // --- ✅ 우클릭: 돌 삭제 ---
            if (e.Button == MouseButtons.Right)
            {
                if (바둑판[x, y] != STONE.none)
                {
                    moveNumbers[x, y] = 0;
                    바둑판[x, y] = STONE.none;
                    panel1.Invalidate();
                }
                return;
            }

            if (바둑판[x, y] != STONE.none)
                return;

            STONE me = flag ? STONE.white : STONE.black;
            바둑판[x, y] = me;

            // --- ✅ [3] 상대방 돌이 사방으로 둘러싸이면(집 없음) 집어 올림 ---
            bool captured = false;
            CaptureOpponent(x, y, me, ref captured);

            // 자기 돌에 집이 없으면 두지 못함
            if (!captured && NoLiberty(x, y))
            {
                바둑판[x, y] = STONE.none;
                return;
            }

            if (showMoveNumbers)
            {
                moveCount++;
                moveNumbers[x, y] = moveCount;
            }

            flag = !flag;
            lastStoneX = x;
            lastStoneY = y;
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
                moveNumbers[cx, cy] = 0;

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
                g.DrawLine(pen, margin + i * GridSize, margin,
                                 margin + i * GridSize, margin + 18 * GridSize);
                g.DrawLine(pen, margin, margin + i * GridSize,
                                 margin + 18 * GridSize, margin + i * GridSize);
            }

            for (int x = 3; x <= 15; x += 6)
            {
                for (int y = 3; y <= 15; y += 6)
                {
                    g.FillEllipse(bBrush,
                        margin + GridSize * x - FeatureSize / 2,
                        margin + GridSize * y - FeatureSize / 2,
                        FeatureSize, FeatureSize);
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
                        margin + GridSize * x - DolSize / 2,
                        margin + GridSize * y - DolSize / 2,
                        DolSize, DolSize);

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

                    // 수순 번호 표시
                    if (showMoveNumbers && moveNumbers[x, y] > 0)
                    {
                        string num = moveNumbers[x, y].ToString();
                        Brush numBrush = (바둑판[x, y] == STONE.black) ? wBrush : bBrush;
                        using (Font numFont = new Font("굴림", 10f, FontStyle.Bold))
                        {
                            SizeF sz = g.MeasureString(num, numFont);
                            g.DrawString(num, numFont, numBrush,
                                margin + GridSize * x - sz.Width / 2,
                                margin + GridSize * y - sz.Height / 2);
                        }
                    }

                    // 마지막에 놓인 돌에 삼각형 표시
                    if (x == lastStoneX && y == lastStoneY)
                    {
                        int cx = margin + GridSize * x;
                        int cy = margin + GridSize * y;
                        Point[] tri = new Point[]
                        {
                        new Point(cx, cy - 8),
                        new Point(cx - 7, cy + 5),
                        new Point(cx + 7, cy + 5)
                        };
                        g.DrawPolygon(Pens.Red, tri);
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