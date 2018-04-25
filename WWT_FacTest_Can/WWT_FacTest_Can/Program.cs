using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WWT_FacTest
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        //获取当前执行文件路径
        public static string GetStartupPath()
        {
            string path;
            path = Application.StartupPath;
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }
            return path;
        }
    }
}
