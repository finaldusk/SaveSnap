using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SaveSnap
{
    public partial class Form1 : Form
    {
        Bitmap bmpOne, bmpTwo;
        bool firstTime=true;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = timer1.Interval.ToString();
            IDataObject data = Clipboard.GetDataObject();//从剪贴板中获取数据
            if (IsBitmap(data))
            {
                bmpOne = DataToMap(data);
                bmpTwo = bmpOne;
                MapToFile(bmpOne);
                firstTime = false;
            }
        }

        private bool IsBitmap(IDataObject data)
        {            
            if (data.GetDataPresent(typeof(Bitmap)))//判断是否是图片类型
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private Bitmap DataToMap(IDataObject data)
        {
            Bitmap bmp;
            bmp = (Bitmap)data.GetData(typeof(Bitmap));//将图片数据存到位图中
            this.pictureBox1.Image = bmp;//显示到程序窗口
            return bmp;
        }

        private void MapToFile(Bitmap bmp)
        {                       
                string nowtime = DateTime.Now.ToString("yyyyMMddhhmmss");
                string saveurl = "D:\\img\\" + nowtime + ".png";
                bmp.Save(@saveurl);//保存图片                       
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IDataObject data = Clipboard.GetDataObject();//从剪贴板中获取数据
            if (IsBitmap(data))
            {
                bmpOne = DataToMap(data);
                if (firstTime)
                {
                    bmpTwo = bmpOne;
                    firstTime = false;
                }

                if (!ImageEquals(bmpOne, bmpTwo))
                {
                    bmpTwo = bmpOne;
                    MapToFile(bmpTwo);
                }
               
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;

            this.WindowState = FormWindowState.Normal;
        }


        private bool ImageEquals(Bitmap bmpOne, Bitmap bmpTwo)
        {
            MemoryStream ms = new MemoryStream();
            bmpOne.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;

            bmpTwo.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());

            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
