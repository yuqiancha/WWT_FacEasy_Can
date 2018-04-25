using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WWT_FacTest
{
    public partial class UserConfirm : Form
    {
        private int waitSecond = 3;
        public UserConfirm()
        {
            InitializeComponent();
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
                label1.Text = "蜂鸣器测试结果，若不选择将在3s后默认工作正常！---倒计时：" + waitSecond + "秒";
                button1.Text = "蜂鸣器正常" + "(" + waitSecond + ")";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Data.AlarmStatus = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Data.AlarmStatus = false;
            this.Close();
        }

        private void UserConfirm_Load(object sender, EventArgs e)
        {
            Data.AlarmStatus = true;
        }
    }
}
