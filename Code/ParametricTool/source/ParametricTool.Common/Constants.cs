using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.Common
{
    public class Constants
    {
        #region 读取配置文件的方法
        private static KeyValueConfigurationCollection GetAppSetttings()
        {
            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = ConfigPath;
            KeyValueConfigurationCollection appSetttings = ConfigurationManager.OpenMappedExeConfiguration(map, 0).AppSettings.Settings;
            return appSetttings;
        }
        private static string GetAppSetttingValue(string key)
        {
            if (!AppSetttings.AllKeys.Contains(key))
            {
                Trace.WriteLine("配置文件(" + ConfigPath + ")不包含" + key + "的定义。");
                throw new Exception("配置文件(" + ConfigPath + ")不包含" + key + "的定义。");
            }
            return AppSetttings[key].Value;
        }
        #endregion

        #region 自定义的常量
        /// <summary>
        /// 当前dll所在路径
        /// </summary>
        public static string DllPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString());
        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string ProjectPath = Path.GetFullPath(Path.Combine(DllPath, @"..\"));
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfigPath = Path.GetFullPath(Path.Combine(ProjectPath, @"config\app.config"));
        /// <summary>
        /// 配置文件设置
        /// </summary>
        private static KeyValueConfigurationCollection AppSetttings = GetAppSetttings();
        #endregion


        #region 配置文件中的设置
        /// <summary>
        /// 是否为测试模式，开发本地测试使用，部署时改为false
        /// </summary>
        public static bool TestMode = bool.Parse(GetAppSetttingValue("TestMode"));

        /// <summary>
        /// 工程图模板
        /// </summary>
        public static string DrawingTemplate = Path.GetFullPath(Path.Combine(ProjectPath, GetAppSetttingValue("DrawingTemplate")));

        /// <summary>
        /// 工程图间距
        /// </summary>
        public static double ViewGap = double.Parse( GetAppSetttingValue("ViewGap"));
        #endregion
    }
}
