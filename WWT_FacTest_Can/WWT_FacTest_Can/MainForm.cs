using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WWT_FacTest
{
    public partial class MainForm : Form
    {
        private DataTable dtLock = new DataTable();//单次测试记录
        private DataTable dtList = new DataTable();//历史测试记录

        public delegate void DeleUpdateGridView(int row, string result);
        public delegate void DeleUpdateGridView2(string SNID, string time, string result, string detail);
        public delegate void DeleUpdateTextBox(TextBox textbox, string str);

        public QueryInfo myQueryInfo = new QueryInfo();
        public LifeTestForm myLifeTest = new LifeTestForm();

        private void UpdateGridView(int row, string result)
        {
            dtLock.Rows[row]["测试结果"] = result;
        }

        private void UpdateGridView2(string SNID, string time, string result, string detail)
        {
            DataRow dr = dtList.NewRow();
            dr["SNID"] = (string)SNID;
            dr["时间"] = time;
            dr["结果"] = result;
            dr["详细信息"] = detail;
            dtList.Rows.Add(dr);
        }

        private void UpdateTextBox(TextBox textbox, string str)
        {
            textbox.Text = str;
        }

        public MainForm()
        {
            InitializeComponent();
            //启动日志
            MyLog.richTextBox1 = richTextBox1;
            MyLog.path = Program.GetStartupPath() + @"LogData\";
            MyLog.lines = 50;
            MyLog.start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.comboBox_SerialPortNum.Text = ConfigurationManager.AppSettings["serialportsend"];
            this.comboBox_SerialBaudrate.Text = ConfigurationManager.AppSettings["baudrate1"];
            this.comboBox_SerialDatabit.Text = ConfigurationManager.AppSettings["databit1"];
            this.comboBox_SerialStopbit.Text = ConfigurationManager.AppSettings["stopbit1"];
            this.comboBox_SerialParity.Text = ConfigurationManager.AppSettings["Parity1"];

            dtLock.Columns.Add("ID", typeof(Int32));
            dtLock.Columns.Add("测试项目", typeof(String));
            dtLock.Columns.Add("测试结果", typeof(String));
            for (int i = 0; i < 7; i++)
            {
                DataRow dr = dtLock.NewRow();
                dr["ID"] = i + 1;
                dr["测试结果"] = "尚未开始";
                dtLock.Rows.Add(dr);
            }
            dtLock.Rows[0]["测试项目"] = "升锁测试";
            dtLock.Rows[1]["测试项目"] = "降锁测试";
            dtLock.Rows[2]["测试项目"] = "蜂鸣器测试";
            dtLock.Rows[3]["测试项目"] = "超声测试1：无车情况";
            dtLock.Rows[4]["测试项目"] = "地磁测试1：无车情况";
            dtLock.Rows[5]["测试项目"] = "超声测试2：有车情况";
            dtLock.Rows[6]["测试项目"] = "地磁测试2：有车情况";

            dataGridView1.DataSource = dtLock;
            dataGridView1.AllowUserToAddRows = false;


            dtList.Columns.Add("SNID", typeof(Int32));
            dtList.Columns.Add("时间", typeof(String));
            dtList.Columns.Add("结果", typeof(String));
            dtList.Columns.Add("详细信息", typeof(String));

            dataGridView2.DataSource = dtList;
            dataGridView2.AllowUserToAddRows = false;

            btn_SerialOpen_Click(sender, e);

            Data.UniqueCode = ConfigurationManager.AppSettings["UniqueCode"];
            this.textBox_UniqueCode.Text = Data.UniqueCode;


            Data.sql = new SqLiteHelper("data source=mydb.db");
            //创建名为table1的数据表
            Data.sql.CreateTable("table_All", new string[] { "SNID", "CreateTime", "Result", "DetailInfo" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT" });
            Data.sql.CreateTable("table_Error", new string[] { "SNID", "CreateTime", "Result", "DetailInfo" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT" });
            Data.sql.CreateTable("table_Right", new string[] { "SNID", "CreateTime", "Result", "DetailInfo" }, new string[] { "TEXT", "TEXT", "TEXT", "TEXT" });


        }

        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void btn_SerialOpen_Click(object sender, EventArgs e)
        {
            if (btn_SerialOpen.Text == "打开端口")
            {
                SerialFun.ComPortSend = new SerialPort();
                SerialFun.ComPortSend.BaudRate = Convert.ToInt32(comboBox_SerialBaudrate.Text);
                SerialFun.ComPortSend.PortName = comboBox_SerialPortNum.Text;
                SerialFun.ComPortSend.DataBits = Convert.ToInt32(comboBox_SerialDatabit.Text);

                switch (comboBox_SerialStopbit.Text)
                {
                    case "1":
                        SerialFun.ComPortSend.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        SerialFun.ComPortSend.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        SerialFun.ComPortSend.StopBits = StopBits.Two;
                        break;
                    default:
                        MessageBox.Show("Error:停止位参数设置不正确", "Error");
                        break;
                }

                switch (comboBox_SerialParity.Text)
                {
                    case "无校验":
                        SerialFun.ComPortSend.Parity = Parity.None;
                        break;
                    case "偶校验":
                        SerialFun.ComPortSend.Parity = Parity.Even;
                        break;
                    case "奇校验":
                        SerialFun.ComPortSend.Parity = Parity.Odd;
                        break;
                    default:
                        MessageBox.Show("Error:校验位参数设置不正确", "Error");
                        break;
                }

                try
                {
                    SerialFun.ComPortSend.Open();
                    MyLog.Info("打开串口成功");

                    //事件注册
                    SerialFun.ComPortSend.DataReceived += ComPortSend_DataReceived; ;

                    SetConfigValue("serialportsend", SerialFun.ComPortSend.PortName);
                    SetConfigValue("baudrate1", SerialFun.ComPortSend.BaudRate.ToString());
                    SetConfigValue("databit1", SerialFun.ComPortSend.DataBits.ToString());
                    btn_SerialOpen.Text = "关闭端口";
                }
                catch (Exception ex)
                {
                    MyLog.Error("打开串口失败：" + ex.Message);
                }
            }
            else
            {
                try
                {
                    btn_SerialOpen.Text = "打开端口";
                    SerialFun.ComPortSend.Close();
                    //事件注销
                    SerialFun.ComPortSend.DataReceived -= ComPortSend_DataReceived;
                    MyLog.Info("关闭串口成功");
                }
                catch (Exception ex)
                {
                    MyLog.Error("关闭串口失败：" + ex.Message);
                }
            }
        }

        private void ComPortSend_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //            throw new NotImplementedException();
            Thread.Sleep(100);

            byte[] byteRead = new byte[SerialFun.ComPortSend.BytesToRead];
            SerialFun.ComPortSend.Read(byteRead, 0, byteRead.Length);

            string temp = "";
            for (int i = 0; i < byteRead.Length; i++)
            {
                temp += byteRead[i].ToString("x2");
            }
            Trace.WriteLine("串口收到:" + temp);
            Data.ReturnStr = temp;

        }

        private void comboBox_SerialPortNum_DropDown(object sender, EventArgs e)
        {
            string[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MyLog.Info("尝试选择串口,但是本机没有串口！");
            }
            comboBox_SerialPortNum.Items.AddRange(str);
            int count = comboBox_SerialPortNum.Items.Count;

            for (int i = 0; i < count; i++)
            {
                string str1 = comboBox_SerialPortNum.Items[i].ToString();
                for (int j = i + 1; j < count; j++)
                {
                    string str2 = comboBox_SerialPortNum.Items[j].ToString();
                    if (str1 == str2)
                    {
                        comboBox_SerialPortNum.Items.RemoveAt(j);
                        count--;
                        j--;
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            String CmdStr = "01051002FF00";//升锁
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String CmdStr = "01051003FF00";//降锁
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            String CmdStr = "01051004FF00";//报警
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            String CmdStr = "01051005FF00";//取消报警
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            String CmdStr = "01051001FF00";//复位
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            String CmdStr = "01051006FF00";//降锁休眠
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            String CmdStr = "010601050300";//超声测试
            Trace.WriteLine("执行超声测试指令:" + SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr));
        }

        private void button11_Click(object sender, EventArgs e)
        {
            String CmdStr = "010601050400";//地磁测试
            Trace.WriteLine("执行地磁测试指令:" + SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr));
        }

        private void button8_Click(object sender, EventArgs e)
        {
            String CmdStr = "010601050000";//退出测试
            SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr);
            Trace.WriteLine("执行退出测试指令:" + SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            String CmdStr = "010420020004";//查询指令
            Trace.WriteLine("执行查询指令:" + SerialFun.SendToPort(SerialFun.ComPortSend, CmdStr));
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            string time = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string detail = "";
            foreach (DataRow row in dtLock.Rows)
            {
                detail += row[2] + ",";
            }
            Data.sql.InsertValues("table_All", new string[] { Data.UniqueCode, time, "测试通过", detail });


            Data.UniqueCode = this.textBox_UniqueCode.Text;
            foreach (DataRow dr in dtLock.Rows)
            {
                dr[2] = "尚未开始";
            }
            new Thread(() => { Fun_Compare(); }).Start();
        }

        private void Fun_Compare()
        {
            DeleUpdateGridView myDeleUpdate = new DeleUpdateGridView(UpdateGridView);
            DeleUpdateGridView2 myDeleUpdate2 = new DeleUpdateGridView2(UpdateGridView2);
            DeleUpdateTextBox myDeleUpdateTextBox = new DeleUpdateTextBox(UpdateTextBox);
            String ret1 = null;
            bool result = true;


            if (SerialFun.ComPortSend.IsOpen)
            {
                try
                {
                    #region 升锁测试
                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "01051002FF00");//升锁
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 0, "升锁中...");
                    }
                    Thread.Sleep(5000);

                    SerialFun.SendToPort(SerialFun.ComPortSend, "010420020004");//查询
                    Thread.Sleep(500);

                    if (Data.ReturnStr.Substring(12, 2) == "55")
                    {
                        dataGridView1.Invoke(myDeleUpdate, 0, "升锁成功");
                    }
                    else
                    {
                        dataGridView1.Invoke(myDeleUpdate, 0, "升锁失败");
                        result = false;
                    }
                    #endregion

                    #region 降锁测试
                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "01051003FF00");//降锁
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 1, "降锁中...");
                    }
                    Thread.Sleep(5000);
                    SerialFun.SendToPort(SerialFun.ComPortSend, "010420020004");//查询
                    Thread.Sleep(500);
                    if (Data.ReturnStr.Substring(12, 2) == "ff")
                    {
                        dataGridView1.Invoke(myDeleUpdate, 1, "降锁成功");
                    }
                    else
                    {
                        dataGridView1.Invoke(myDeleUpdate, 1, "降锁失败");
                        result = false;
                    }
                    #endregion

                    #region 蜂鸣器测试
                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "01051004FF00");//报警
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 2, "报警中...");
                    }
                    Thread.Sleep(2000);

                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "01051005FF00");//取消报警
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 2, "取消报警...");
                    }
                    Thread.Sleep(500);

                    UserConfirm frm = new UserConfirm();
                    frm.ShowDialog();
                    if (Data.AlarmStatus)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 2, "蜂鸣器正常");
                    }
                    else
                    {
                        dataGridView1.Invoke(myDeleUpdate, 2, "蜂鸣器异常");
                        result = false;
                    }
                    #endregion

                    #region 无车情况下 超声测试以及地磁测试
                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "010601050300");//进入超声测试
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 3, "超声测试中...");
                    }

                    bool Normal_Ultrasonic_Tag = true;
                    for (int i = 0; i < 10; i++)
                    {
                        SerialFun.SendToPort(SerialFun.ComPortSend, "010420020004");//查询
                        Thread.Sleep(200);
                        if (Data.ReturnStr.Length > 26)
                        {
                            if (Data.ReturnStr.Substring(18, 2) != "00")
                            {
                                Normal_Ultrasonic_Tag = false;
                                MyLog.Error("无车超声测试时收到异常返回:" + Data.ReturnStr);
                                break;
                            }
                        }
                    }

                    if (Normal_Ultrasonic_Tag)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 3, "无车情况：超声正常");
                    }
                    else
                    {
                        byte temp_byte = Convert.ToByte(Data.ReturnStr.Substring(18, 2), 16);

                        bool temp_flag1 = false;
                        bool temp_flag2 = false;

                        if ((temp_byte & 0xf0) != 0)
                        {
                            temp_flag1 = true;
                        }
                        if ((temp_byte & 0x0f) != 0)
                        {
                            temp_flag2 = true;
                        }

                        if (temp_flag1 & temp_flag2)
                        {
                            dataGridView1.Invoke(myDeleUpdate, 3, "无车情况：超声正常");

                        }
                        else if ((!temp_flag1) & temp_flag2)
                        {
                            dataGridView1.Invoke(myDeleUpdate, 3, "无车情况：超声探头1异常");
                        }
                        else if (temp_flag1 & (!temp_flag2))
                        {
                            dataGridView1.Invoke(myDeleUpdate, 3, "无车情况：超声探头2异常");
                        }
                        else
                        {
                            dataGridView1.Invoke(myDeleUpdate, 3, "无车情况：超声探头1,2异常");
                        }

                        result = false;
                    }

                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出超声测试
                    Thread.Sleep(500);
                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出超声测试
                    Thread.Sleep(500);


                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "010601050400");//进入地磁测试
                    Thread.Sleep(2000);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 4, "地磁测试中...");
                    }

                    bool Normal_UltraMagnetic_Tag = true;
                    for (int i = 0; i < 10; i++)
                    {
                        SerialFun.SendToPort(SerialFun.ComPortSend, "010420020004");//查询
                        Thread.Sleep(200);
                        if (Data.ReturnStr.Length > 26)
                        {
                            if (Data.ReturnStr.Substring(18, 2) != "00")
                            {
                                Normal_UltraMagnetic_Tag = false;
                                MyLog.Error("无车地磁测试时收到异常返回:" + Data.ReturnStr);
                                break;
                            }
                        }
                    }

                    if (Normal_UltraMagnetic_Tag)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 4, "无车情况：地磁正常");
                    }
                    else
                    {
                        dataGridView1.Invoke(myDeleUpdate, 4, "无车情况：地磁异常");
                        result = false;
                    }

                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出地磁测试
                    Thread.Sleep(500);
                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出地磁测试
                    Thread.Sleep(500);

                    #endregion

                    #region 有车情况下，超声测试、地磁测试

                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "010601050300");//进入有车超声测试
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 5, "超声测试中...");
                    }

                    User_Ultrasonic_Confirm frm2 = new User_Ultrasonic_Confirm();
                    frm2.ShowDialog();

                    switch (Data.UltrasonicStatus)
                    {
                        case 0:
                            dataGridView1.Invoke(myDeleUpdate, 5, "有车情况：超声正常");
                            break;
                        case 1:
                            dataGridView1.Invoke(myDeleUpdate, 5, "有车情况：超声探头1异常");
                            result = false;
                            break;
                        case 2:
                            dataGridView1.Invoke(myDeleUpdate, 5, "有车情况：超声探头2异常");
                            result = false;
                            break;
                        case 3:
                            dataGridView1.Invoke(myDeleUpdate, 5, "有车情况：超声探头1,2异常");
                            result = false;
                            break;
                        case 4:
                            dataGridView1.Invoke(myDeleUpdate, 5, "485通讯异常");
                            result = false;
                            break;
                        default:
                            dataGridView1.Invoke(myDeleUpdate, 5, "有车情况：超声探头1,2异常");
                            result = false;
                            break;
                    }

                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出超声测试
                    Thread.Sleep(500);
                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出超声测试
                    Thread.Sleep(500);


                    ret1 = SerialFun.SendToPort(SerialFun.ComPortSend, "010601050400");//进入有车地磁测试
                    Thread.Sleep(500);
                    if (Data.ReturnStr == ret1)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 6, "地磁测试中...");
                    }
                    User_Magnetic_Confirm frm3 = new User_Magnetic_Confirm();
                    frm3.ShowDialog();

                    if (Data.MagneticStatus)
                    {
                        dataGridView1.Invoke(myDeleUpdate, 6, "有车情况：地磁正常");
                    }
                    else
                    {
                        dataGridView1.Invoke(myDeleUpdate, 6, "有车情况：地磁异常");
                        result = false;
                    }


                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出地磁测试
                    Thread.Sleep(500);
                    SerialFun.SendToPort(SerialFun.ComPortSend, "010601050000");//退出地磁测试
                    Thread.Sleep(500);

                    #endregion

                    if (result)
                    {
                        string timestr = string.Format("{0}-{1:D2}-{2:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        PrintMachine.PrintUniqueCode(timestr, Data.UniqueCode);

                        string time = string.Format("{0}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        string detail = "";
                        foreach (DataRow row in dtLock.Rows)
                        {
                            detail += row[2] + ",";
                        }
                        dataGridView2.Invoke(myDeleUpdate2, Data.UniqueCode, time, "测试通过", detail);

                        Data.sql.InsertValues("table_All", new string[] { Data.UniqueCode, time, "测试通过", detail });
                        Data.sql.InsertValues("table_Right", new string[] { Data.UniqueCode, time, "测试通过", detail });

                        int temp = 0;
                        int.TryParse(Data.UniqueCode, out temp);
                        temp = temp + 1;
                        Data.UniqueCode = Convert.ToString(temp, 10).PadLeft(8, '0');

                        this.textBox_UniqueCode.Invoke(myDeleUpdateTextBox, this.textBox_UniqueCode, Data.UniqueCode);

                        SetConfigValue("UniqueCode", Data.UniqueCode);

                    }
                    else
                    {
                        MyLog.Error("当前测试失败，请重新测试！");

                        string time = string.Format("{0}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        string detail = "";
                        foreach (DataRow row in dtLock.Rows)
                        {
                            detail += row[2] + ",";
                        }
                        dataGridView2.Invoke(myDeleUpdate2, Data.UniqueCode, time, "测试失败", detail);
                        Data.sql.InsertValues("table_All", new string[] { Data.UniqueCode, time, "测试失败", detail });
                        Data.sql.InsertValues("table_Error", new string[] { Data.UniqueCode, time, "测试失败", detail });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("串口错误，请重新配置串口！！");
                    MyLog.Error(ex.Message);
                }

            }
            else
            {
                MyLog.Error("串口未打开，测试失败，请重新测试！");
            }
            Trace.WriteLine("退出Fun_Compare");
        }


        private void button10_Click(object sender, EventArgs e)
        {
            PrintMachine.PrintUniqueCode("2000年1月1日", "99999999");
        }

        private void 串口配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 手动控制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.splitContainer1.Panel1Collapsed)
            {
                this.splitContainer1.Panel1Collapsed = false;
            }
            else
            {
                this.splitContainer1.Panel1Collapsed = true;
            }
        }

        private void btn_SerialConfig_Click(object sender, EventArgs e)
        {
            if (this.splitContainer3.Panel1Collapsed)
            {
                this.splitContainer3.Panel1Collapsed = false;
            }
            else
            {
                this.splitContainer3.Panel1Collapsed = true;
            }
        }

        private void 查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myQueryInfo != null)
            {
                myQueryInfo.Activate();
            }
            else
            {
                myQueryInfo = new QueryInfo();
            }
            myQueryInfo.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (myQueryInfo != null)
            {
                myQueryInfo.Activate();
            }
            else
            {
                myQueryInfo = new QueryInfo();
            }
            myQueryInfo.ShowDialog();
        }

        private void 寿命测试ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (myLifeTest != null)
            {
                myLifeTest.Activate();
            }
            else
            {
                myLifeTest = new LifeTestForm();
            }
            myLifeTest.ShowDialog();
        }
    }
}
