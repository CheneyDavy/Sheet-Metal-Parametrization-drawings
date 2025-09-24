using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwEntry
{
    public class LogUtil
    {
        /// <summary>
        /// 日志目录
        /// </summary>
        internal static string LogDir { get; set; }
        /// <summary>
        /// 日志保留天数
        /// </summary>
        internal static int RemainDays { get; set; }
        /// <summary>
        /// 是否记录Console输出的内容
        /// </summary>
        internal static bool IncludeConsole { get; set; }
        internal static TextWriter ConsoleTextWriter { get; set; }



        /// <summary>
        /// 初始化日志监听
        /// </summary>
        /// <param name="logDir">日志所在文件夹</param>
        /// <param name="remainDays">日志保留天数</param>
        /// <param name="includeConsole">是否记录console打印的内容</param>
        public static string Init(string logDir, int remainDays = 7, bool includeConsole = true)
        {
            LogDir = logDir;
            RemainDays = remainDays;
            IncludeConsole = includeConsole;


            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
            else
            {
                if (RemainDays > 0)
                {
                    DeleteLogFile(LogDir, RemainDays);
                }
            }
            string logname = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff") + ".log";
            string logPath = Path.GetFullPath(Path.Combine(LogDir, logname));
            if (File.Exists(logPath))
            {
                File.Delete(logPath);
            }
            TextWriterTraceListener ttl = new SetTextWriterTraceListener(logPath);
            Trace.AutoFlush = true;
            Trace.Listeners.Add(ttl);
            if (IncludeConsole)
            {
                ConsoleTextWriter = Console.Out;
                Console.SetOut(new ConsoleTextWriter());
            }
            Trace.WriteLine("**************日志记录开始**************");
            if (IncludeConsole)
            {
                Console.WriteLine(string.Format("{0:r}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff"), "已开启记录Console功能"));
            }
            return logPath;
        }

        /// <summary>
        /// 删除7天前的日志
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteLogFile(string dir, int remainDays = 7, string suffix = "*.log", bool includeChildDir = false)
        {
            try
            {
                DirectoryInfo directory = new DirectoryInfo(dir);
                List<FileInfo> files = directory.EnumerateFiles(suffix, includeChildDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
                DateTime current = DateTime.Now;
                List<FileInfo> deleteList = new List<FileInfo>();
                foreach (FileInfo file in files)
                {
                    DateTime time = file.LastWriteTime;
                    TimeSpan span = current - time;
                    if (span.Days > remainDays)
                    {
                        deleteList.Add(file);
                    }
                }
                foreach (FileInfo file in deleteList)
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 关闭Console日志监听
        /// </summary>
        /// <param name="dir"></param>
        public static void CloseConsole()
        {
            try
            {
                if (IncludeConsole && ConsoleTextWriter != null)
                {
                    Console.Out.Close();
                    Console.SetOut(ConsoleTextWriter);
                }
            }
            catch (Exception)
            {
            }
        }
    }



    public class SetTextWriterTraceListener : TextWriterTraceListener
    {
        public SetTextWriterTraceListener(Stream myFile)
            : base(myFile)
        {

        }
        public SetTextWriterTraceListener(string myFile)
            : base(myFile)
        {

        }

        public SetTextWriterTraceListener(TextWriter writer) : base(writer)
        {
        }

        public override void Write(string x)
        {
            // Use whatever format you want here...
            base.Write(string.Format("{0:r}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff"), x));
        }


        public override void WriteLine(string x)
        {
            // Use whatever format you want here...
            base.WriteLine(string.Format("{0:r}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff"), x));
        }

        public override void WriteLine(string x, string category)
        {
            // Use whatever format you want here...
            base.WriteLine(string.Format("{0:r}: {1}>>>{2}", DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff"), category, x));
        }
    }

    public class ConsoleTextWriter : TextWriter
    {
        public override Encoding Encoding { get; }

        public override void Write(string x)
        {
            base.Write(x);
            Trace.Write(x);
        }


        public override void WriteLine(string x)
        {
            // Use whatever format you want here...
            base.Write(x);
            Trace.WriteLine(x);
        }

    }

}
