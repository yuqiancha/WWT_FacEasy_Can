using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WWT_FacTest
{
    public partial class User_Ultrasonic_Confirm : Form
    {
        private int waitSecond = 20;
        public User_Ultrasonic_Confirm()
        {
            InitializeComponent();
        }

        private void User_Ultrasonic_Confirm_Load(object sender, EventArgs e)
        {
            Data.UltrasonicStatus = 3;
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
                label1.Text = "请将挡板移到车位锁上方，测试到超声波会自动退出---倒计时：" + waitSecond + "秒";
                button2.Text = "超声异常" + "(" + waitSecond + ")";


                SerialFun.SendToPort(SerialFun.ComPortSend, "010420020004");//查询
                Thread.Sleep(100);

                if (Data.ReturnStr.Length > 26)
                {
                    byte Ultrasonic_return_b9 = Convert.ToByte(Data.ReturnStr.Substring(18, 2), 16);
                    Trace.WriteLine(Ultrasonic_return_b9);
                    bool Ultrasonic_result_S1 = false;
                    bool Ultrasonic_result_S2 = false;

                    if ((Ultrasonic_return_b9 & 0xf0) != 0)
                    {
                        Ultrasonic_result_S1 = true;
                    }
                    if ((Ultrasonic_return_b9 & 0x0f) != 0)
                    {
                        Ultrasonic_result_S2 = true;
                    }

                    if (Ultrasonic_result_S1 & Ultrasonic_result_S2)
                    {
                        Data.UltrasonicStatus = 0;
                        waitSecond = 0;
                    }
                    else if ((!Ultrasonic_result_S1) & Ultrasonic_result_S2)
                    {
                        Data.UltrasonicStatus = 1;
                    }
                    else if (Ultrasonic_result_S1 & (!Ultrasonic_result_S2))
                    {
                        Data.UltrasonicStatus = 2;
                    }
                    else
                    {
                        Data.UltrasonicStatus = 3;
                    }
                }
                else
                {
                    Data.UltrasonicStatus = 4;
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.UltrasonicStatus = 0;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Data.UltrasonicStatus = 3;
            this.Close();
        }
    }
}
