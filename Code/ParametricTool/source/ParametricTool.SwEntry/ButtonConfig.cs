using ParametricTool.SwEntry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace ParametricTool.SwEntry
{
    /// <summary>
    /// 表示sw插件按钮配置
    /// </summary>
    public class ButtonConfig
    {
        /// <summary>
        /// dll所在路径
        /// </summary>
        [XmlIgnore]
        public string dllPath;
        /// <summary>
        /// 插件标题
        /// </summary>
        [XmlAttribute("Title")]
        public string AddinTitle;
        /// <summary>
        /// 插件描述
        /// </summary>
        [XmlAttribute("Description")]
        public string AddinDescription;
        /// <summary>
        /// 注册时决定是否再启动SW时加载插件
        /// </summary>
        [XmlAttribute("LoadAtStartup")]
        public bool LoadAtStartup;
        /// <summary>
        /// 日志文件夹(支持相对和绝对路径，相对路径相对于本dll)
        /// </summary>
        [XmlAttribute("LogDir")]
        public string LogDir;
        /// <summary>
        /// 菜单Tab页
        /// </summary>
        [XmlArray("CommandTabs")]
        [XmlArrayItem("CommandTab")]
        public List<CommandTab> CommandTabs;
        /// <summary>
        /// 从文件读取插件按钮配置
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ButtonConfig LoadConfig(string path)
        {
            ButtonConfig config = path.Ex_SerializeFile<ButtonConfig>();
            config.dllPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            config.LogDir = Path.GetFullPath(Path.Combine(config.dllPath, config.LogDir));
            if (config.CommandTabs == null)
            {
                config.CommandTabs = new List<CommandTab>();
            }
            config.CommandTabs.ForEach(tab =>
            {
                if (tab.Buttons == null)
                {
                    tab.Buttons = new List<CommandBtn>();
                }
                tab.Buttons.ForEach(button =>
                {
                    button.OwningTab = tab;
                    button.IconPath = Path.GetFullPath(Path.Combine(config.dllPath, button.IconPath));
                    button.AssemblyPath = Path.GetFullPath(Path.Combine(config.dllPath, button.AssemblyPath));
                });
            });
            return config;
        }
        /// <summary>
        /// 从与dll同名的xml文件读取插件按钮配置
        /// </summary>
        /// <returns></returns>
        public static ButtonConfig LoadConfig()
        {
            string configPath = Assembly.GetExecutingAssembly().Location + ".xml";
            return LoadConfig(configPath);
        }

        /// <summary>
        /// 根据按钮全名称获取Button对象
        /// </summary>
        /// <param name="fullName">命令全名称(Tab名称|按钮名称)</param>
        /// <returns></returns>
        public CommandBtn GetButtonFromFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentNullException(nameof(fullName));
            }
            string[] names = fullName.Split('|');
            if (names.Length != 2)
            {
                throw new Exception("解析按钮全名称失败，拆分后数量不为2！");
            }
            return this.CommandTabs.Find(x => x.Title == names[0]).Buttons.Find(x => x.Name == names[1]);
        }
    }
    /// <summary>
    /// 表示一个sw菜单tab页
    /// </summary>
    public class CommandTab
    {
        /// <summary>
        /// tab页标题(不能包含|)
        /// </summary>
        [XmlAttribute("Title")]
        public string Title;
        /// <summary>
        /// 包含的按钮
        /// </summary>
        [XmlArray("Buttons")]
        [XmlArrayItem("CommandBtn")]
        public List<CommandBtn> Buttons;
    }
    /// <summary>
    /// 表示一个sw菜单按钮
    /// </summary>
    public class CommandBtn
    {

        /// <summary>
        /// 按钮名称(不能包含|)
        /// 如果设置为 "---分割线---"  则表示一个分割线
        /// </summary>
        [XmlAttribute("Name")]
        public string Name;
        /// <summary>
        /// 图标路径(支持相对和绝对路径，相对路径相对于本dll)
        /// </summary>
        [XmlAttribute("IconPath")]
        public string IconPath;
        /// <summary>
        /// 按钮动作所在dll路径(支持相对和绝对路径，相对路径相对于本dll)
        /// </summary>
        [XmlAttribute("AssemblyPath")]
        public string AssemblyPath;
        /// <summary>
        /// 按钮动作所在类的全限定名
        /// </summary>
        [XmlAttribute("FullClassName")]
        public string FullClassName;
        /// <summary>
        /// 按钮动作所执行的方法
        /// 要求：
        /// 1.静态方法
        /// 2.第一个参数必须是ISldWorks
        /// 3.后面跟着string类型的参数
        /// 例如XXXX(ISldWorks iSwApp,params string[] args)
        /// </summary>
        [XmlAttribute("MethodName")]
        public string MethodName;
        /// <summary>
        /// 方法的string类型参数
        /// </summary>
        [XmlArray("ButtonArgs")]
        [XmlArrayItem("Arg")]
        public List<string> ButtonArgs;
        /// <summary>
        /// 按钮在哪些sw模块中可点击
        /// </summary>
        [XmlArray("SensitiveEnvs")]
        [XmlArrayItem("Env")]
        public List<ButtonEnv> SensitiveEnvs;

        /// <summary>
        /// 所属Tab页
        /// </summary>
        [XmlIgnore]
        public CommandTab OwningTab;

        /// <summary>
        /// 命令全名称(Tab名称|按钮名称)
        /// </summary>
        [XmlIgnore]
        public string FullName
        {
            get { return OwningTab.Title + "|" + Name; }
        }


    }
    /// <summary>
    /// 表示sw的应用模块
    /// </summary>
    public enum ButtonEnv
    {
        /// <summary>
        /// 装配
        /// </summary>
        Assembly,
        /// <summary>
        /// 零件
        /// </summary>
        Part,
        /// <summary>
        /// 制图
        /// </summary>
        Drawing,
        /// <summary>
        /// 未打开模型
        /// </summary>
        NoModel,
        /// <summary>
        /// 以下为SW的其他环境
        /// </summary>
        NONE,
        SDM,
        LAYOUT,
        IMPORTED_PART,
        IMPORTED_ASSEMBLY
    }

}
