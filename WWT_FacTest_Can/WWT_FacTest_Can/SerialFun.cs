using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WWT_FacTest
{
    class SerialFun
    {
        public static SerialPort ComPortSend;
        public static string SendToPort(SerialPort ComPort, string CmdStr)
        {
            byte[] temp = Function.StrToHexByte(CmdStr);
            byte[] CRC = Function.CRC16(temp);
            byte[] SendToPort = new byte[temp.Length + 5];
            SendToPort[0] = 0xeb;
            SendToPort[1] = 0x90;
            SendToPort[2] = 0x08;
            SendToPort[SendToPort.Length - 2] = CRC[1];
            SendToPort[SendToPort.Length - 1] = CRC[0];
            temp.CopyTo(SendToPort, 3);
            try
            {
                ComPort.Write(SendToPort, 0, SendToPort.Length);
            }
            catch (Exception ex)
            {
                MyLog.Error(ex.Message + "  串口发送失败");
                return "Error";
            }

            String str = null;
            for (int i = 0; i < SendToPort.Length; i++)
            {
                str += SendToPort[i].ToString("x2");
            }
            return str;

        }


    }
}
