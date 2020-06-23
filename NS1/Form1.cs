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

        //NN nn;
        NeuroNet nn;

        bool isCanChange;

        public Form1()
        {
            InitializeComponent();
            FormClosing += CloseForm;

            sizeX = pictureBox1.Width;
            sizeY = pictureBox1.Height;

            listPoints = new List<MyPoint>();
            bm = new Bitmap(sizeX, sizeY);
            graphics = Graphics.FromImage(bm);
            x0 = (double)sizeX / 2;
            y0 = (double)sizeY / 2;
            
            
            
            
        }

        private void CloseForm(object o, FormClosingEventArgs e)
        {
            th.Abort();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            MyPoint mp;
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //синий круг 1
                mp = new MyPoint(e.X - x0, y0 - e.Y, 1);
            }
            else
            {
                //красный круг -1
                mp = new MyPoint(e.X - x0, y0 - e.Y, -1);
            }
            listPoints.Add(mp);

           // isCanChange = true;
            isCanChange = false;
            Thread.Sleep(200);
            int[] size = new int[2];
            size[0] = 2;
            size[1] = 2;
            //nn = new NN(0.001, size);
            nn = new NeuroNet(0.01, size);
            isCanChange = true;

            if (th == null)
            {
                th = new Thread(Learning);
                th.Start();
            }
        }

        private void Learning()
        {
            while (true)
            {
                if (isCanChange)
                {
                    for (int k = 0; k < 1000; k++)
                    {
                        for (int i = 0; i < listPoints.Count; i++)
                        {
                            double[] inp = new double[] { listPoints[i].x, listPoints[i].y};
                            //double[] res = nn.feedForward(inp);


                            double[] targets = new double[] { listPoints[i].color, -listPoints[i].color };
                            //nn.backpropagation(targets);
                            nn.backpropagation(inp, targets);
                        }
                    }
                    ReDraw();
                }
                Thread.Sleep(100);
            }
        }

        private void ReDraw()
        {            
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    double[] inp = new double[] { i - x0, y0 - j};
                    double[] res = nn.feedForward(inp);
                    try
                    {
                        bm.SetPixel(i, j, Color.FromArgb((byte)(255 * res[1]), 0, (byte)(255 * res[0])));
                    }
                    catch{}
                }
            }
            for (int i = 0; i < listPoints.Count; i++)
            {
                Color color = Color.FromArgb((byte)(125 - 125 * listPoints[i].color), 0, (byte)(125 + 125 * listPoints[i].color));
                graphics.FillEllipse(Brushes.White, (int)(x0 + (listPoints[i].x - 5)), (int)(y0 - (listPoints[i].y + 5)), 10, 10);
                graphics.FillEllipse(new SolidBrush(color), (int)(x0 + (listPoints[i].x - 4)), (int)(y0 - (listPoints[i].y + 4)), 8, 8);
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
                //nn = (NN)formatter.Deserialize(fs);
            }
            bm.Dispose();
            bm = new Bitmap(sizeX, sizeY);
            graphics = Graphics.FromImage(bm);
            pictureBox1.Image = (Image)bm;
            listPoints.Clear();

            isCanChange = true;
        }
    }
}
