using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace NS1
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        List<MyPoint> listPoints;
        Bitmap bm;
        Thread th;

        int sizeX;
        int sizeY;

        double x0;
        double y0;

        NeuroNet nn;

        bool isCanChange;

        public Form1()
        {
            InitializeComponent();
            FormClosing += CloseForm;

            listPoints = new List<MyPoint>();
        }

        private void CloseForm(object o, FormClosingEventArgs e)
        {
            th.Abort();
        }

        public double fAct(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (th == null)
            {
                int[] size = new int[3];
                size[0] = 3;
                size[1] = 3;
                size[2] = 2;
                nn = new NeuroNet(0.01, size, fAct);

                sizeX = pictureBox1.Width;
                sizeY = pictureBox1.Height;
               
                bm = new Bitmap(sizeX, sizeY);
                graphics = Graphics.FromImage(bm);

                x0 = (double)sizeX / 2;
                y0 = (double)sizeY / 2;
            }

            MyPoint mp;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //синий круг 1
                mp = new MyPoint((e.X - x0) / x0, (y0 - e.Y) / y0, 1);
            }
            else
            {
                //красный круг -1
                mp = new MyPoint((e.X - x0) / x0, (y0 - e.Y) / y0, -1);
            }
            listPoints.Add(mp);


            if (th == null)
            {
                th = new Thread(Learning);
                th.Start();
                isCanChange = true;
            }
            
        }

        private void Learning()
        {
            while (true)
            {
                if (isCanChange) //для синхронизации потоков
                {
                    for (int k = 0; k < 10000; k++) //обучаем Х раз на одних и тех же данных
                    {
                        for (int i = 0; i < listPoints.Count; i++)
                        {
                            double[] inp = new double[] { listPoints[i].x, listPoints[i].y , 1};//(double) r.Next(-500, 500) / 234.0};
                            nn.feedForward(inp); //рассчитываем результат

                            double[] targets = new double[] { listPoints[i].color, -listPoints[i].color };
                            nn.backpropagation(targets); //корректируем результат
                        }
                    }
                    ReDraw(); //отображаем результат обучения
                }
            }
        }

        private void ReDraw()
        {
            Random ra = new Random();
                for (int i = 0; i < sizeX; i++)
                {
                    for (int j = 0; j < sizeY; j++)
                    {
                        double[] inp = new double[] { (i - x0) / x0, (y0 - j) / y0, 1};//(double)ra.Next(-500, 500) / 323.0};
                        double[] res = nn.feedForward(inp);
                        try
                        {
                            bm.SetPixel(i, j, Color.FromArgb((byte)(255 * res[1]), 0, (byte) (255 * res[0])));
                        }
                        catch{}
                    }
                }
                for (int i = 0; i < listPoints.Count; i++)
                {
                    Color color = Color.FromArgb((byte)(125 - 125 * listPoints[i].color), 0, (byte)(125 + 125 * listPoints[i].color));
                    graphics.FillEllipse(Brushes.White, (int)(x0 + (listPoints[i].x * x0 - 5)), (int)(y0 - (listPoints[i].y * y0 + 5)), 10, 10);
                    graphics.FillEllipse(new SolidBrush(color), (int)(x0 + (listPoints[i].x * x0 - 4)), (int)(y0 - (listPoints[i].y * y0 + 4)), 8, 8);
                }
                
            
            pictureBox1.Image = (Image)bm;
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCanChange = false;
            Thread.Sleep(200);

            BinaryFormatter formatter = new BinaryFormatter();
            // получаем поток, куда будем записывать сериализованный объект
            using (FileStream fs = new FileStream("NeuroNetwork.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, nn);
            }

            isCanChange = true;
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCanChange = false;
            Thread.Sleep(200);

            BinaryFormatter formatter = new BinaryFormatter();
            // десериализация из файла people.dat
            using (FileStream fs = new FileStream("NeuroNetwork.dat", FileMode.OpenOrCreate))
            {
                nn = (NeuroNet)formatter.Deserialize(fs);
            }
            bm.Dispose();
            bm = new Bitmap(sizeX, sizeY);
            graphics = Graphics.FromImage(bm);
            pictureBox1.Image = (Image)bm;
            listPoints.Clear();

            isCanChange = true;
        }

        private void сохранитьПикчуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isCanChange = false;
            Thread.Sleep(1000);

                Random r = new Random();
                bm.Save("Image-" + r.Next().ToString() + ".bmp");
                isCanChange = true;
            
        }
    }
}
