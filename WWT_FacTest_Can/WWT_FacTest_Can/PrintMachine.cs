using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WWT_FacTest
{
    class PrintMachine
    {
        const uint IMAGE_BITMAP = 0;
        const uint LR_LOADFROMFILE = 16;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr LoadImage(IntPtr hinst, string lpszName, uint uType,
           int cxDesired, int cyDesired, uint fuLoad);
        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int DeleteObject(IntPtr ho);
        const string szSavePath = "D:\\Argox";
        const string szSaveFile = "D:\\Argox\\PPLB_Example.Prn";
        const string sznop1 = "nop_front\r\n";
        const string sznop2 = "nop_middle\r\n";
        [DllImport("Winpplb.dll")]
        public static extern int B_Bar2d_Maxi(int x, int y, int cl, int cc, int pc, string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Bar2d_PDF417(int x, int y, int w, int v, int s, int c, int px,
            int py, int r, int l, int t, int o, string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Bar2d_PDF417_N(int x, int y, int w, int h, string pParameter, string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Bar2d_DataMatrix(int x, int y, int r, int l, int h, int v, string data);
        [DllImport("Winpplb.dll")]
        public static extern void B_ClosePrn();
        [DllImport("Winpplb.dll")]
        public static extern int B_CreatePrn(int selection, string filename);
        [DllImport("Winpplb.dll")]
        public static extern int B_Del_Form(string formname);
        [DllImport("Winpplb.dll")]
        public static extern int B_Del_Pcx(string pcxname);
        [DllImport("Winpplb.dll")]
        public static extern int B_Draw_Box(int x, int y, int thickness, int hor_dots,
            int ver_dots);
        [DllImport("Winpplb.dll")]
        public static extern int B_Draw_Line(char mode, int x, int y, int hor_dots, int ver_dots);
        [DllImport("Winpplb.dll")]
        public static extern int B_Error_Reporting(char option);
        [DllImport("Winpplb.dll")]
        public static extern IntPtr B_Get_DLL_Version(int nShowMessage);
        [DllImport("Winpplb.dll")]
        public static extern int B_Get_DLL_VersionA(int nShowMessage);
        [DllImport("Winpplb.dll")]
        public static extern int B_Get_Graphic_ColorBMP(int x, int y, string filename);
        [DllImport("Winpplb.dll")]
        public static extern int B_Get_Graphic_ColorBMPEx(int x, int y, int nWidth, int nHeight,
            int rotate, string id_name, string filename);
        [DllImport("Winpplb.dll")]
        public static extern int B_Get_Graphic_ColorBMP_HBitmap(int x, int y, int nWidth, int nHeight,
           int rotate, string id_name, IntPtr hbm);
        [DllImport("Winpplb.dll")]
        public static extern int B_Get_Pcx(int x, int y, string filename);
        [DllImport("Winpplb.dll")]
        public static extern int B_Initial_Setting(int Type, string Source);
        [DllImport("Winpplb.dll")]
        public static extern int B_WriteData(int IsImmediate, byte[] pbuf, int length);
        [DllImport("Winpplb.dll")]
        public static extern int B_ReadData(byte[] pbuf, int length, int dwTimeoutms);
        [DllImport("Winpplb.dll")]
        public static extern int B_Load_Pcx(int x, int y, string pcxname);
        [DllImport("Winpplb.dll")]
        public static extern int B_Open_ChineseFont(string path);
        [DllImport("Winpplb.dll")]
        public static extern int B_Print_Form(int labset, int copies, string form_out, string var);
        [DllImport("Winpplb.dll")]
        public static extern int B_Print_MCopy(int labset, int copies);
        [DllImport("Winpplb.dll")]
        public static extern int B_Print_Out(int labset);
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Barcode(int x, int y, int ori, string type, int narrow,
            int width, int height, char human, string data);
        [DllImport("Winpplb.dll")]
        public static extern void B_Prn_Configuration();
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Text(int x, int y, int ori, int font, int hor_factor,
            int ver_factor, char mode, string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Text_Chinese(int x, int y, int fonttype, string id_name,
            string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Text_TrueType(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Text_TrueType_W(int x, int y, int FHeight, int FWidth,
            string FType, int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut,
            string id_name, string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_Select_Option(int option);
        [DllImport("Winpplb.dll")]
        public static extern int B_Select_Option2(int option, int p);
        [DllImport("Winpplb.dll")]
        public static extern int B_Select_Symbol(int num_bit, int symbol, int country);
        [DllImport("Winpplb.dll")]
        public static extern int B_Select_Symbol2(int num_bit, string csymbol, int country);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Backfeed(char option);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Backfeed_Offset(int offset);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_CutPeel_Offset(int offset);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_BMPSave(int nSave, string strBMPFName);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Darkness(int darkness);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_DebugDialog(int nEnable);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Direction(char direction);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Form(string formfile);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Labgap(int lablength, int gaplength);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Labwidth(int labwidth);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Originpoint(int hor, int ver);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Prncomport(int baud, char parity, int data, int stop);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Prncomport_PC(int nBaudRate, int nByteSize, int nParity,
            int nStopBits, int nDsr, int nCts, int nXonXoff);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_Speed(int speed);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_ProcessDlg(int nShow);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_ErrorDlg(int nShow);
        [DllImport("Winpplb.dll")]
        public static extern int B_GetUSBBufferLen();
        [DllImport("Winpplb.dll")]
        public static extern int B_EnumUSB(byte[] buf);
        [DllImport("Winpplb.dll")]
        public static extern int B_CreateUSBPort(int nPort);
        [DllImport("Winpplb.dll")]
        public static extern int B_ResetPrinter();
        [DllImport("Winpplb.dll")]
        public static extern int B_GetPrinterResponse(byte[] buf, int nMax);
        [DllImport("Winpplb.dll")]
        public static extern int B_TFeedMode(int nMode);
        [DllImport("Winpplb.dll")]
        public static extern int B_TFeedTest();
        [DllImport("Winpplb.dll")]
        public static extern int B_CreatePort(int nPortType, int nPort, string filename);
        [DllImport("Winpplb.dll")]
        public static extern int B_Execute_Form(string form_out, string var);
        [DllImport("Winpplb.dll")]
        public static extern int B_Bar2d_QR(int x, int y, int model, int scl, char error,
            char dinput, int c, int d, int p, string data);
        [DllImport("Winpplb.dll")]
        public static extern int B_GetNetPrinterBufferLen();
        [DllImport("Winpplb.dll")]
        public static extern int B_EnumNetPrinter(byte[] buf);
        [DllImport("Winpplb.dll")]
        public static extern int B_CreateNetPort(int nPort);
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Text_TrueType_Uni(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            byte[] data, int format);
        [DllImport("Winpplb.dll")]
        public static extern int B_Prn_Text_TrueType_UniB(int x, int y, int FSize, string FType,
            int Fspin, int FWeight, int FItalic, int FUnline, int FStrikeOut, string id_name,
            byte[] data, int format);
        [DllImport("Winpplb.dll")]
        public static extern int B_GetUSBDeviceInfo(int nPort, byte[] pDeviceName,
            out int pDeviceNameLen, byte[] pDevicePath, out int pDevicePathLen);
        [DllImport("Winpplb.dll")]
        public static extern int B_Set_EncryptionKey(string encryptionKey);
        [DllImport("Winpplb.dll")]
        public static extern int B_Check_EncryptionKey(string decodeKey, string encryptionKey,
            int dwTimeoutms);

        public static void PrintUniqueCode(String str_time,String str_code)
        {
            //Test code start
            // open port.
            int nLen, ret, sw;
            byte[] pbuf = new byte[128];
            string strmsg;
            IntPtr ver;
            System.Text.Encoding encAscII = System.Text.Encoding.ASCII;
            System.Text.Encoding encUnicode = System.Text.Encoding.Unicode;

            // dll version.
            ver = PrintMachine.B_Get_DLL_Version(0);

            // search port.
            nLen = PrintMachine.B_GetUSBBufferLen() + 1;
            strmsg = "DLL ";
            strmsg += Marshal.PtrToStringAnsi(ver);
            strmsg += "\r\n";
            if (nLen > 1)
            {
                byte[] buf1, buf2;
                int len1 = 128, len2 = 128;
                buf1 = new byte[len1];
                buf2 = new byte[len2];
                PrintMachine.B_EnumUSB(pbuf);
                PrintMachine.B_GetUSBDeviceInfo(1, buf1, out len1, buf2, out len2);
                sw = 1;
                if (1 == sw)
                {
                    ret = PrintMachine.B_CreatePrn(12, encAscII.GetString(buf2, 0, len2));// open usb.
                }
                else
                {
                    ret = PrintMachine.B_CreateUSBPort(1);// must call B_GetUSBBufferLen() function fisrt.
                }
                if (0 != ret)
                {
                    //    strmsg += "Open USB fail!";
                    MyLog.Error("Open USB fail!");
                }
                else
                {
                    strmsg += "Open USB:\r\nDevice name: ";
                    strmsg += encAscII.GetString(buf1, 0, len1);
                    strmsg += "\r\nDevice path: ";
                    strmsg += encAscII.GetString(buf2, 0, len2);
                    //sw = 2;
                    if (2 == sw)
                    {
                        //Immediate Error Report.
                        PrintMachine.B_WriteData(1, encAscII.GetBytes("^ee\r\n"), 5);//^ee
                        ret = PrintMachine.B_ReadData(pbuf, 4, 1000);
                    }
                    MyLog.Info("打码机打开成功，开始打印二维码！");


                    // sample setting.
                    PrintMachine.B_Set_DebugDialog(1);
                    PrintMachine.B_Set_Originpoint(0, 0);
                    PrintMachine.B_Select_Option(2);
                    PrintMachine.B_Set_Darkness(8);
                    PrintMachine.B_Del_Pcx("*");// delete all picture.
                                                //draw box.
                                                //B_Draw_Box(330, 20, 4, 880, 450);//画一个框
                    PrintMachine.B_Prn_Text_TrueType_W(500, 40, 80, 30, "Times New Roman", 1, 400, 0, 0, 0, "AA", "位位通");
                    PrintMachine.B_Prn_Text_TrueType_W(350, 140, 40, 20, "Times New Roman", 1, 400, 0, 0, 0, "AB", "合格证 " + str_time);
                    PrintMachine.B_Prn_Barcode(350, 220, 0, "9", 4, 10, 160, 'B', str_code);//印出一個條碼和加上跳號功能                                                                        
                    PrintMachine.B_Print_Out(1);// copy 2.
                    PrintMachine.B_ClosePrn();

                    MyLog.Info("二维码打印成功！");

                }
            }
            else
            {
                MyLog.Error("未检测到打码机，请确认打码机连接正常，指示灯亮！");
                //System.IO.Directory.CreateDirectory(szSavePath);
               // ret = PrintMachine.B_CreatePrn(0, szSaveFile);// open file.
            }

            //if (0 != ret)
            //    return;         
        }

    }
}
