using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace WWT_FacTest
{
    public partial class LifeTestForm : Form
    {
        int AlreadyRunNums = 0;
        bool threadOn = false;
        public LifeTestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(button1.Text=="开始寿命测试")
            {
                button1.Text = "停止";
                AlreadyRunNums = 0;
                threadOn = true;
                int nums = (int)this.numericUpDown1.Value;
                new Thread(() => { LifeTestThread(nums); }).Start();
            }
            else
            {
                button1.Text = "开始寿命测试";
                threadOn = false;
            }
        }

        private void LifeTestThread(int nums)
        {
            int runNum = 0;
            while(threadOn)
            {
                if (SerialFun.ComPortSend.IsOpen)
                {
                    if (runNum++ < nums)
                    {
                        String CmdStr1 = "01051002FF00";//升锁
                        SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr1);

                        Thread.Sleep(5000);

                        String CmdStr2 = "01051003FF00";//降锁
                        SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr2);

                        Thread.Sleep(5000);

                        AlreadyRunNums += 1;
                        textBox2.BeginInvoke(new Action(() => { textBox2.Text = AlreadyRunNums.ToString(); ; }));
                    }
                    else
                    {
                        MessageBox.Show("测试完成！！");
                        this.button1.BeginInvoke(new Action(() => { this.button1.Text = "开始寿命测试"; }));
                        break;
                    }

                }
                else
                {
                    MessageBox.Show("串口未正确打开，设置正确后重新开始测试！");
                    this.button1.BeginInvoke(new Action(() => { this.button1.Text = "开始寿命测试"; }));
                    break;
                }
            }


        }
    }
}
