using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WWT_FacTest
{
    public partial class User_Magnetic_Confirm : Form
    {
        private int waitSecond = 20;
        public User_Magnetic_Confirm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.MagneticStatus = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Data.MagneticStatus = false;
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (waitSecond < 1)
            {
                this.Close();
            }
            else
            {
                waitSecond--;
                label1.Text = "左右移动的挡板，测试到地磁抖动会自动退出此窗口---倒计时：" + waitSecond + "秒";

                SerialFun.SendToPort(SerialFun.ComPortSend, "010420020004");//查询
                Thread.Sleep(100);
                if (Data.ReturnStr.Length > 26)
                {
                    if (Data.ReturnStr.Substring(18, 2) == "55")
                    {
                        Data.MagneticStatus = true;
                        waitSecond = 0;
                    }
                }
            }
        }

        private void User_Magnetic_Confirm_Load(object sender, EventArgs e)
        {
            Data.MagneticStatus = false;
        }
    }
}
