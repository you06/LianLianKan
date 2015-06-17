using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LianLianKan
{
    public partial class Form1 : Form
    {
        static llk L = new llk();
        class llk
        {
            private readonly string version = "v1.0.2";//标记类的版本号
            public int[,] container = new int[20, 8];
            Bitmap img1, img2, img3, img4, img5, decoration;
            private int typeCount = 5;
            private bool isGameStart = false;
            private bool isGameComplete = false;
            private bool isFocus = false;//是否被选中
            private Point pointToDraw;//选中的点
            private int lastm = -1;//上一个被选中的方格的坐标
            private int lastn = -1;
            static Random r = new Random();
            public llk()
            {
                //加载图片
                /* 从文件夹中引用图片
                 * img1 = new Bitmap("img/1.jpg");
                 * img2 = new Bitmap("img/2.jpg");
                 * img3 = new Bitmap("img/3.jpg");
                 * img4 = new Bitmap("img/4.jpg");
                 * img5 = new Bitmap("img/5.jpg");
                 * decoration = new Bitmap("img/decoration.png");
                 * */
                img1 = Properties.Resources._1;
                img2 = Properties.Resources._2;
                img3 = Properties.Resources._3;
                img4 = Properties.Resources._4;
                img5 = Properties.Resources._5;
                decoration = Properties.Resources.decoration;
            }
            public void reset()
            {
                //清空游戏数据
                for (int i = 0; i <= 19; i++)
                {
                    for (int j = 0; j <= 7; j++)
                    {
                        container[i, j] = 0;
                    }
                }
                //给二维数组随机赋值
                for (int i = 1; i <= 160; i++)
                {
                    int line_now = (i - 1) / 20;
                    //(i / 20 != 8) ? ((i - 1) / 20) : 7
                    int row_now = ((i % 20 - 1) != -1) ? (i % 20 - 1) : 19;
                    if (container[row_now, line_now] != 0 && i <= 160)
                    {
                        //若检测到该位置已被赋值,则跳过
                    }
                    else
                    {
                        //检测到该位置的值为0(初始值),则给找一个位置与该位置赋相同值
                        int type = r.Next(1, typeCount + 1);//type=1-typeCount,为该位置的方块类型
                        int m = r.Next(0, 20);
                        int n = r.Next(line_now, 8);//随机一个位置(跳过已经被赋值的行)
                        while (container[m, n] != 0 || (m == row_now && n == line_now))
                        {
                            //若检测到该位置已被赋值则重新随机位置,直到随机到的位置未被赋值
                            m = r.Next(0, 20);
                            n = r.Next(line_now, 8);//随机一个位置(跳过已经被赋值的行)
                        }
                        //将相同的值赋给两个位置
                        container[row_now, line_now] = type;
                        container[m, n] = type;
                    }
                }
            }
            public void drawcanvas(Graphics canvas)
            {
                for (int m = 0; m <= 19; m++)
                {
                    for (int n = 0; n <= 7; n++)
                    {
                        //判断该位置的图片类型绘制相对应的图片
                        switch (container[m, n])
                        {
                            case 0:
                                break;
                            case 1:
                                canvas.DrawImage(img1, 55 * m + 55, 70 * n + 70);
                                break;
                            case 2:
                                canvas.DrawImage(img2, 55 * m + 55, 70 * n + 70);
                                break;
                            case 3:
                                canvas.DrawImage(img3, 55 * m + 55, 70 * n + 70);
                                break;
                            case 4:
                                canvas.DrawImage(img4, 55 * m + 55, 70 * n + 70);
                                break;
                            case 5:
                                canvas.DrawImage(img5, 55 * m + 55, 70 * n + 70);
                                break;
                        }
                    }
                }
            }
            public void drawdiv(Graphics canvas)
            {
                //选中方格时，边缘高亮
                int Px = pointToDraw.X;
                int Py = pointToDraw.Y;
                int m = ((Px - 55 * (Px / 55)) <= 45) ? Px / 55 - 1 : -1;
                int n = ((Py - 70 * (Py / 70)) <= 60) ? Py / 70 - 1 : -1;
                if (m >= 0 && n >= 0 && m<= 19 && n<= 7) 
                {
                    if (canLink(lastm, lastn, m, n))
                    {
                        //上一个被选中的位置与本次选中的位置类型相同时,消除方块,将lastType重置为0,取消焦点
                        container[m, n] = 0;
                        container[lastm, lastn] = 0;
                        lastm = -1;
                        lastn = -1;
                        isFocus = false;
                    }
                    else if (container[m, n] != 0)
                    {
                        //上一个被选中的位置与本次选中的位置类型不同或不存在上一个被选中的位置时,(重新)设定上一个位置的信息,并显示选中当前位置
                        canvas.DrawImage(decoration, 55 * m + 55 - 5, 70 * n + 70 - 5);
                        lastm = m;
                        lastn = n;
                    }
                    else
                    {
                        lastm = -1;
                        lastn = -1;
                    }
                }
            }
            public bool IsGameStart
            {
                get { return isGameStart; }
                set { isGameStart = value; }
            }
            public bool IsGameComplete
            {
                get { return isGameComplete; }
                set { isGameComplete = value; }
            }
            public bool IsFocus
            {
                get { return isFocus; }
                set { isFocus = value; }
            }
            public Point PointToDraw
            {
                get { return pointToDraw; }
                set { pointToDraw = value; }
            }
            private bool canLink(int x1, int y1, int x2, int y2)
            {
                if (x1 < 0) return false;
                if (x1 > 19) return false;
                if (x2 < 0) return false;
                if (x2 > 19) return false;
                if (y1 < 0) return false;
                if (y1 > 7) return false;
                if (y2 < 0) return false;
                if (y2 > 7) return false;
                if (x1 == x2 && y1 == y2) return false;
                if (container[x1, y1] == container[x2, y2])
                {
                    if (x1 == 0 && x2 == 0) return true;//当两个位置在同一边时返回真
                    if (x1 == 19 && x2 == 19) return true;
                    if (y1 == 0 && y2 == 0) return true;
                    if (y1 == 7 && y2 == 7) return true;
                    //仅有一个位置在边上时的判定(虽然被囊括在同时延展到边上时返回能消除的情况中，但做了计算的优化)
                    if (x1 == 0)
                    {
                        for (int i = x2 - 1; i >= 0; i--)
                        {
                            if (container[i, y2] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (x1 == 19)
                    {
                        for (int i = x2 + 1; i <= 19; i--)
                        {
                            if (container[i, y2] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (x2 == 0)
                    {
                        for (int i = x1 - 1; i >= 0; i--)
                        {
                            if (container[i, y1] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (x2 == 19)
                    {
                        for (int i = x1 + 1; i <= 19; i--)
                        {
                            if (container[i, y1] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (y1 == 0)
                    {
                        for (int i = y2 - 1; i >= 0; i--)
                        {
                            if (container[x2, i] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (y1 == 7)
                    {
                        for (int i = x2 + 1; i <= 7; i--)
                        {
                            if (container[x2, i] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (y2 == 0)
                    {
                        for (int i = y1 - 1; i >= 0; i--)
                        {
                            if (container[x1, i] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    if (y2 == 7)
                    {
                        for (int i = x1 + 1; i <= 7; i--)
                        {
                            if (container[x1, i] != 0) break;
                            if (i == 0) return true;
                        }
                    }
                    bool[,] link1 = new bool[20, 8];
                    //做横向匹配
                    link1[x1, y1] = true;
                    link1[x2, y2] = true;
                    int pos;
                    if (x1 != 19)
                    {
                        pos = x1 + 1;
                        while (container[pos, y1] == 0)
                        {
                            link1[pos, y1] = true;
                            if (pos == 19) break;//pos等于19的时候跳出循环
                            pos++;
                        }
                    }
                    if (x1 != 0)
                    {
                        pos = x1 - 1;
                        while (container[pos, y1] == 0)
                        {
                            link1[pos, y1] = true;
                            if (pos == 0) break;//pos等于0的时候跳出循环
                            pos--;
                        }
                    }
                    if (x2 != 19)
                    {
                        pos = x2 + 1;
                        while (container[pos, y2] == 0)
                        {
                            link1[pos, y2] = true;
                            if (pos == 19) break;//pos等于19的时候跳出循环
                            pos++;
                        }
                    }
                    if (x2 != 0)
                    {
                        pos = x2 - 1;
                        while (container[pos, y2] == 0)
                        {
                            link1[pos, y2] = true;
                            if (pos == 0) break;//pos等于0的时候跳出循环
                            pos--;
                        }
                    }
                    if (y1 == y2)
                    {
                        for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
                        {
                            if (link1[i, y1]) break;
                            if (i == Math.Max(x1, x2)) return true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= 19; i++)
                        {
                            if (link1[i, y1] == true && link1[i, y2] == true)
                            {
                                if (i == 0 || i == 19) return true;//同时延展到边上时返回能消除
                                for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
                                {
                                    if (container[i, j] != 0 && j != Math.Max(y1, y2) && j != Math.Min(y1, y2)) break;
                                    if (j == Math.Max(y1, y2)) return true;
                                }
                            }
                        }
                    }
                    bool[,] link2 = new bool[20, 8];
                    //做纵向匹配
                    link2[x1, y1] = true;
                    link2[x2, y2] = true;
                    if (y1 != 7)
                    {
                        pos = y1 + 1;
                        while (container[x1, pos] == 0)
                        {
                            link2[x1, pos] = true;
                            if (pos == 7) break;//pos等于7的时候跳出循环
                            pos++;
                        }
                    }
                    if (y1 != 0)
                    {
                        pos = y1 - 1;
                        while (container[x1, pos] == 0)
                        {
                            link2[x1, pos] = true;
                            if (pos == 0) break;//pos等于0的时候跳出循环
                            pos--;
                        }
                    }
                    if (y2 != 7)
                    {
                        pos = y2 + 1;
                        while (container[x2, pos] == 0)
                        {
                            link2[x2, pos] = true;
                            if (pos == 7) break;//pos等于19的时候跳出循环
                            pos++;
                        }
                    }
                    if (y2 != 0)
                    {
                        pos = y2 - 1;
                        while (container[x2, pos] == 0)
                        {
                            link2[x2, pos] = true;
                            if (pos == 0) break;//pos等于0的时候跳出循环
                            pos--;
                        }
                    }
                    if (x1 == x2)
                    {
                        for (int i = Math.Min(y1, y2); i <= Math.Max(y1, y2); i++)
                        {
                            if (link1[x1, i]) break;
                            if (i == Math.Max(y1, y2)) return true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i <= 7; i++)
                        {
                            if (link2[x1, i] == true && link2[x2, i] == true)
                            {
                                if (i == 0 || i == 7) return true;//同时延展到边上时返回能消除
                                for (int j = Math.Min(x1, x2); j <= Math.Max(x1, x2); j++)
                                {
                                    if (container[j, i] != 0 && j != Math.Max(x1, x2) && j != Math.Min(x1, x2)) break;
                                    if (j == Math.Max(x1, x2)) return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
        }
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }
        private void canvas1_Click(object sender, EventArgs e)
        {
            //左上角的坐标为(0,25)
            Point formPoint = this.PointToClient(Control.MousePosition);
            formPoint.Y -= 25;//修复获取到的坐标
            L.IsFocus = true;//打开选中方格的开关
            L.PointToDraw = formPoint;//输入点击的点
            this.Refresh();//触发canvas1的重绘事件
        }
        private void canvas1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (L.IsFocus)
            {
                L.drawdiv(g);
            }
            if (L.IsGameStart)
            {
                L.drawcanvas(g);
            }
        }
        private void 开始ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            L.reset();//随机生成游戏
            L.IsGameStart = true;//打开游戏开始开关
            this.Refresh();//触发canvas1的重绘事件
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Copyright ©2015 LianLianKan Team. All Rights Reserved.\nPowered by 童牧，李袁紫薇，颜素媚，陈欣平.", "关于");
        }
    }
}
