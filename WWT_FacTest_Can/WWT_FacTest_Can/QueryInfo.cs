using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;

namespace WWT_FacTest
{
    public partial class QueryInfo : Form
    {
        public QueryInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //SQLiteDataReader reader = Data.sql.ReadFullTable("table_All");
            //while (reader.Read())
            //{
            //    //读取ID
            //    Trace.WriteLine("" + reader.GetString(reader.GetOrdinal("锁唯一ID")));
            //    //读取Name
            //    Trace.WriteLine("" + reader.GetString(reader.GetOrdinal("时间")));
            //    //读取Age
            //    Trace.WriteLine("" + reader.GetString(reader.GetOrdinal("结果")));
            //    //读取Email
            //    Trace.WriteLine(reader.GetString(reader.GetOrdinal("详细信息")));
            //}



            string TableName = "table_All";
            switch (comboBox1.Text)
            {
                case "全部记录":
                    TableName = "table_All";
                    break;
                case "测试通过记录":
                    TableName = "table_Right";
                    break;
                case "测试失败记录":
                    TableName = "table_Error";
                    break;
                default:
                    break;
            }

            //select * from table_All where CreateTime >= '2018-03-23 17:07:19' and CreateTime < '2019-08-02 00:00:00'

            SQLiteConnection dbConnection = new SQLiteConnection("data source=mydb.db");
            string cmd = "Select * From " + TableName+" where "+"CreateTime >= '"+dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss") +"' and CreateTime <= '"+dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            Trace.WriteLine(cmd);
            SQLiteDataAdapter mAdapter = new SQLiteDataAdapter(cmd, dbConnection);
            DataTable mTable = new DataTable(); // Don't forget initialize!
            mAdapter.Fill(mTable);

            // 绑定数据到DataGridView
            dataGridView1.DataSource = mTable;

            dataGridView1.Columns[dataGridView1.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].Width = 200;


            //string TableName = "table_All";
            //switch (comboBox1.Text)
            //{
            //    case "全部记录":
            //        TableName = "table_All";
            //        break;
            //    case "测试通过记录":
            //        TableName = "table_Right";
            //        break;
            //    case "测试失败记录":
            //        TableName = "table_Error";
            //        break;
            //    default:
            //        break;
            //}

            //SQLiteConnection dbConnection = new SQLiteConnection("data source=mydb.db");
            //SQLiteDataAdapter mAdapter = new SQLiteDataAdapter("SELECT * FROM "+TableName, dbConnection);
            //DataTable mTable = new DataTable(); // Don't forget initialize!
            //mAdapter.Fill(mTable);

            //// 绑定数据到DataGridView
            //dataGridView1.DataSource = mTable;

            //dataGridView1.Columns[dataGridView1.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //dataGridView1.Columns[1].Width = 200;
        }

        private void QueryInfo_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            dateTimePicker2.Value = DateTime.Now;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = "";
            string saveFileName = "";
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xlsx";
            saveDialog.Filter = "Excel文件|*.xlsx";
            saveDialog.FileName = fileName;
            saveDialog.ShowDialog();
            saveFileName = saveDialog.FileName;
            if (saveFileName.IndexOf(":") < 0) return; //被点了取消
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            if (xlApp == null)
            {
                MessageBox.Show("无法创建Excel对象，您的电脑可能未安装Excel");
                return;
            }
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook =
                        workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet =
                        (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];//取得sheet1 
                                                                                         //写入标题             
            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            { worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText; }
            //写入数值
            for (int r = 0; r < dataGridView1.Rows.Count; r++)
            {
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dataGridView1.Rows[r].Cells[i].Value;
                }
                System.Windows.Forms.Application.DoEvents();
            }
            worksheet.Columns.EntireColumn.AutoFit();//列宽自适应
            MessageBox.Show(fileName + "资料保存成功", "提示", MessageBoxButtons.OK);
            if (saveFileName != "")
            {
                try
                {
                    workbook.Saved = true;
                    workbook.SaveCopyAs(saveFileName);  //fileSaved = true;                 
                }
                catch (Exception ex)
                {//fileSaved = false;                      
                    MessageBox.Show("导出文件时出错,文件可能正被打开！\n" + ex.Message);
                }
            }
            xlApp.Quit();
            GC.Collect();//强行销毁 
        }
    }
}
