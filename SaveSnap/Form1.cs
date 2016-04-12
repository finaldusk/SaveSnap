using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace SaveSnap
{
    public partial class Form1 : Form
    {
        Bitmap bmpOne, bmpTwo;
        bool firstTime=true;

        [DllImport("user32")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint control, Keys vk);
        [DllImport("user32")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


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

        private void button1_Click(object sender, EventArgs e)
        {
            int Rate = Convert.ToInt32(textBox1.Text);
            timer1.Interval = Rate;
            timer2.Interval = Rate;
        }

        private void btnCutScreen_Click(object sender, EventArgs e)
        {
            cutscreen();        
        }

        private void cutscreen()
        {
            this.Visible = false;
            timer_cutscreen.Enabled = true;
        }

        private void timer_cutscreen_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{prtsc}");

            /*
            timer1.Enabled = false;

            Graphics myGraphics = this.CreateGraphics();
            Size s = Screen.PrimaryScreen.Bounds.Size;

            Bitmap bmp = new Bitmap(s.Width, s.Height);
            Graphics ScreenSnap = Graphics.FromImage(bmp);
            ScreenSnap.CopyFromScreen(Point.Empty, Point.Empty, s);
            ScreenSnap.Save();
            Bitmap snap = new Bitmap(s.Width,s.Height, ScreenSnap);
            ScreenSnap.Dispose();
            MapToFile(snap);
            bmpOne = snap;
            bmpTwo = bmpOne;
            Clipboard.SetDataObject(snap);
            timer1.Enabled = true;
            */


            this.Visible = true;            
            timer_cutscreen.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            timer1.Enabled = true;
            UnregisterHotKey(this.Handle, 888);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer2.Enabled = true;
            UnregisterHotKey(this.Handle, 888);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{prtsc}");
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            RegisterHotKey(this.Handle, 888, 0, Keys.PrintScreen);
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

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterHotKey(this.Handle, 888);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0312:    //这个是window消息定义的   注册的热键消息    
                    if (m.WParam.ToString().Equals("888"))  //如果是我们注册的那个热键   
                    {
                        UnregisterHotKey(this.Handle, 888);
                        SendKeys.Send("{prtsc}");
                        RegisterHotKey(this.Handle, 888, 0, Keys.PrintScreen);
                    }
                    break;
            }
            base.WndProc(ref m);
        }


    }
}
