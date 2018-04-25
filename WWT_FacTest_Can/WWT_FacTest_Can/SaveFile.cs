using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WWT_FacTest
{
    class SaveFile
    {
        public static ReaderWriterLockSlim Lock_asyn_1 = new ReaderWriterLockSlim();
        public static ReaderWriterLockSlim Lock_asyn_2 = new ReaderWriterLockSlim();

        public static List<ReaderWriterLockSlim> myLockforAsync = new List<ReaderWriterLockSlim>();

        public static Queue<byte[]> DataQueue_Async = new Queue<byte[]>();

        public static List<Queue<byte[]>> DataQueueList = new List<Queue<byte[]>>();

        public static bool SaveOn = true;

        FileStream file_asyn1, file_asyn2;
        List<FileStream> myFileList_txt = new List<FileStream>();

        public static Queue<string> DataQueue_asyn_1 = new Queue<string>();
        public static Queue<string> DataQueue_asyn_2 = new Queue<string>();

        public static List<Queue<string>> DataQueue_asynList = new List<Queue<string>>();
        //在主页面初始化时，调用FileInit()和FileSaveStart(),分别初始化文件夹和开始存储

        /// <summary>
        /// 存储线程初始化
        /// </summary>
        public void FileInit()
        {
            Trace.WriteLine("Start FileInit!\n");

            string Save_path = Program.GetStartupPath() + @"SaveData\";

            FileCreateTxt(Program.GetStartupPath() + @"ID表\总表\", out file_asyn1);
            FileCreateTxt(Program.GetStartupPath() + @"ID表\单表\", out file_asyn2);

            DataQueue_asynList.Add(DataQueue_asyn_1);
            DataQueue_asynList.Add(DataQueue_asyn_2);


            myLockforAsync.Add(Lock_asyn_1);
            myLockforAsync.Add(Lock_asyn_2);
        }

        public void FileCreateTxt(string Path, out FileStream file)
        {
            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);

            string timestr = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            string filename = Path + timestr + ".txt";
            file = new FileStream(filename, FileMode.Create);
            myFileList_txt.Add(file);
        }

        public void FileSaveStart()
        {
            Trace.WriteLine("开启存盘线程");
            SaveOn = true;

            new Thread(() => { WriteToFileAsynC(1, file_asyn1, ref DataQueue_asyn_1, ref Lock_asyn_1); }).Start();
            new Thread(() => { WriteToFileAsynC(2, file_asyn2, ref DataQueue_asyn_2, ref Lock_asyn_2); }).Start();
        }

        public void FileClose()
        {
            SaveOn = false;
            Trace.WriteLine("Start FileClose!\n");
            Thread.Sleep(500);
            //           WriteFileThread.Abort();

            foreach (var item in myFileList_txt)
            {
                item.Close();
            }
        }



        private void WriteToFileAsynC(int key, object file, ref Queue<string> myQueue, ref ReaderWriterLockSlim myLock)
        {
            Trace.WriteLine("Start WriteToFileAsync Thread:" + key.ToString());
            FileStream myfile = (FileStream)file;
            StreamWriter bw = new StreamWriter(myfile);
            //      FileInfo fileInfo;
            while (SaveOn)
            {
                if (myQueue.Count() > 0)
                {
                    try
                    {
                        myLock.EnterReadLock();
                        bw.Write(myQueue.Dequeue());
                        bw.Flush();
                        myLock.ExitReadLock();

                        #region 分割文件，防止文件过大
                        long FileSizeMB = myfile.Length / (1024 * 1024);
                        if (FileSizeMB > 10)
                        {
                            myFileList_txt[key].Flush();

                            string Path2 = myFileList_txt[key].Name;
                            int count = Path2.LastIndexOf("\\");
                            Path2 = Path2.Substring(0, count + 1);

                            myFileList_txt[key].Close();

                            FileStream newFile;
                            string timestr = string.Format("{0}-{1:D2}-{2:D2} {3:D2}：{4:D2}：{5:D2}", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            string filename = Path2 + timestr + ".txt";
                            newFile = new FileStream(filename, FileMode.Create);

                            myFileList_txt.Remove(myFileList_txt[key]);
                            myFileList_txt.Insert(key, newFile);

                            break;
                            //break跳出循环会执行新线程
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        bw.Close();
                        Trace.WriteLine(myQueue.Count());
                        Trace.WriteLine(e.Message);
                        break;
                    }
                }
                else
                {
                    Thread.Sleep(200);
                    //                  Trace.WriteLine("Queue0 is empty!!");
                }
            }
            bw.Close();
            Trace.WriteLine("Leaving WriteToFileAsync:" + key.ToString());

            if (SaveOn) WriteToFileAsynC(key, myFileList_txt[key], ref myQueue, ref myLock);

        }

    }
}
