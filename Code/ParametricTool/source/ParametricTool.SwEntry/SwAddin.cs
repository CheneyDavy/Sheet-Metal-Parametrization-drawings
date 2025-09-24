using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using ParametricTool.SwEntry;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using SolidWorksTools;
using SolidWorksTools.File;

namespace ParametricTool.SwEntry
{
    /// <summary>
    /// Summary description for SwDrawOutputEntry.
    /// </summary>
    [Guid("067ACE4F-CB89-4DE9-A235-AB6C776A55E0"), ComVisible(true)]
    [SwAddin]
    public class SwAddin : ISwAddin
    {

        #region 属性&字段
        ISldWorks iSwApp = null;
        ICommandManager iCmdMgr = null;
        int addinID = 0;
        BitmapHandler iBmp;
        int registerID;
        private static SwAddin _swAddin;
        /// <summary>
        /// 配置的按钮
        /// </summary>
        private ButtonConfig _buttonConfig;
        /// <summary>
        /// 插件dll路径
        /// </summary>
        private readonly string _addinDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private System.Windows.Application app;
        /// <summary>
        /// 按钮动作所在程序集
        /// </summary>
        private Dictionary<string, Assembly> _asmDict;

        #region Event Handler Variables
        Hashtable openDocs = new Hashtable();
        SolidWorks.Interop.sldworks.SldWorks SwEventPtr = null;
        #endregion

        public UserPMPage ppage = null;

        public ISldWorks SwApp
        {
            get { return iSwApp; }
        }
        public ICommandManager CmdMgr
        {
            get { return iCmdMgr; }
        }

        public Hashtable OpenDocs
        {
            get { return openDocs; }
        }
        #endregion

        #region dll注册与取消注册事件
        [ComRegisterFunctionAttribute]
        public static void RegisterFunction(Type t)
        {

            try
            {
                ButtonConfig buttonConfig = ButtonConfig.LoadConfig();

                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                Microsoft.Win32.RegistryKey addinkey = hklm.CreateSubKey(keyname);
                addinkey.SetValue(null, 0);

                addinkey.SetValue("Description", buttonConfig.AddinDescription);
                addinkey.SetValue("Title", buttonConfig.AddinTitle);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                addinkey = hkcu.CreateSubKey(keyname);
                addinkey.SetValue(null, Convert.ToInt32(buttonConfig.LoadAtStartup), Microsoft.Win32.RegistryValueKind.DWord);


                #region 按照配置删除SW的tab页(按钮和Tab错乱时可参考下)
                ////if (buttonConfig.RemoveTabs == null)
                ////{
                ////    buttonConfig.RemoveTabs = new List<string>();
                ////}
                ////foreach (var tab in buttonConfig.CommandTabs)
                ////{
                ////    if (!buttonConfig.RemoveTabs.Contains(tab.Title))
                ////    {
                ////        buttonConfig.RemoveTabs.Add(tab.Title);
                ////    }
                ////}

                ////string sw = "SOFTWARE\\SolidWorks\\" + buttonConfig.SwVersion + "\\User Interface\\CommandManager";
                ////List<string> swContext = new List<string>();
                ////swContext.Add("AssyContext");
                ////swContext.Add("DrwContext");
                ////swContext.Add("PartContext");
                ////foreach (var context in swContext)
                ////{
                ////    Microsoft.Win32.RegistryKey assyTabs = hkcu.OpenSubKey(sw + "\\" + context);
                ////    string[] subNames = assyTabs.GetSubKeyNames();
                ////    assyTabs.Close();
                ////    foreach (var item in subNames)
                ////    {
                ////        bool delete = false;
                ////        string tabKeyName = sw + "\\" + context + "\\" + item;
                ////        Microsoft.Win32.RegistryKey assyTab = hkcu.OpenSubKey(tabKeyName);
                ////        object tabName = assyTab.GetValue("RefName");
                ////        if (tabName != null && buttonConfig.RemoveTabs.Contains(tabName.ToString()))
                ////        {
                ////            delete = true;
                ////        }
                ////        assyTab.Close();
                ////        if (delete)
                ////        {
                ////            Console.WriteLine("删除Tab页" + (tabName) + "--->HKEY_CURRENT_USER" + "\\" + tabKeyName);
                ////            hkcu.DeleteSubKeyTree(tabKeyName);
                ////        }
                ////    }
                ////}
                #endregion
            }
            catch (System.NullReferenceException nl)
            {
                Console.WriteLine("There was a problem registering this dll: SWattr is null. \n\"" + nl.Message + "\"");
                System.Windows.Forms.MessageBox.Show("There was a problem registering this dll: SWattr is null.\n\"" + nl.Message + "\"");
            }

            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);

                System.Windows.Forms.MessageBox.Show("There was a problem registering the function: \n\"" + e.Message + "\"");
            }
        }

        [ComUnregisterFunctionAttribute]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                hklm.DeleteSubKey(keyname);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                hkcu.DeleteSubKey(keyname);

                #region 按照配置删除SW的tab页(按钮和Tab错乱时可参考下)
                ////ButtonConfig buttonConfig = ButtonConfig.LoadConfig();
                ////if (buttonConfig.RemoveTabs == null)
                ////{
                ////    buttonConfig.RemoveTabs = new List<string>();
                ////}
                ////foreach (var tab in buttonConfig.CommandTabs)
                ////{
                ////    if (!buttonConfig.RemoveTabs.Contains(tab.Title))
                ////    {
                ////        buttonConfig.RemoveTabs.Add(tab.Title);
                ////    }
                ////}
                ////string sw = "SOFTWARE\\SolidWorks\\" + buttonConfig.SwVersion +"\\User Interface\\CommandManager";
                ////List<string> swContext = new List<string>();
                ////swContext.Add("AssyContext");
                ////swContext.Add("DrwContext");
                ////swContext.Add("PartContext");
                ////foreach (var context in swContext)
                ////{
                ////    Microsoft.Win32.RegistryKey assyTabs = hkcu.OpenSubKey(sw + "\\" + context);
                ////    string[] subNames = assyTabs.GetSubKeyNames();
                ////    assyTabs.Close();
                ////    foreach (var item in subNames)
                ////    {
                ////        bool delete = false;
                ////        string tabKeyName = sw + "\\" + context + "\\" + item;
                ////        Microsoft.Win32.RegistryKey assyTab = hkcu.OpenSubKey(tabKeyName);
                ////        object tabName = assyTab.GetValue("RefName");
                ////        if (tabName != null && buttonConfig.RemoveTabs.Contains(tabName.ToString()))
                ////        {
                ////            delete = true;
                ////        }
                ////        assyTab.Close();
                ////        if (delete)
                ////        {
                ////            Console.WriteLine("删除Tab页" + (tabName) + "--->HKEY_CURRENT_USER" + "\\" + tabKeyName);
                ////            hkcu.DeleteSubKeyTree(tabKeyName);
                ////        }
                ////    }
                ////}
                #endregion
            }
            catch (System.Exception e)
            {
                Console.WriteLine("异常:" + e.Message);
                //System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: \n\"" + e.Message + "\"");
            }
        }

        #endregion

        #region 插件加载与注销相关
        /// <summary>
        /// 构造函数，当sw加载插件时最先执行
        /// </summary>
        public SwAddin()
        {
            try
            {
                _buttonConfig = ButtonConfig.LoadConfig();
                #region 日志输出初始化，此处日志采用Trace方式
                string logPath = LogUtil.Init(_buttonConfig.LogDir);
                #endregion

                #region 许可初始化及验证

                #endregion

                #region 菜单配置初始化及加载相应的dll
                _asmDict = new Dictionary<string, Assembly>();
                LoadAssembly();
                #endregion

                #region 此处加载HandyControl，有时候按钮动作加载HC会出问题
                //之前遇到的问题:
                //描述:加载handycontrol提示 a strongly - name assembly required
                //项目其他dll都未设置强命名，handycontrol添加强明明后重新编译也不行
                string hcPath = Path.GetFullPath(Path.Combine(this._buttonConfig.dllPath, "HandyControl.dll"));
                Trace.WriteLine("开始加载HandyControl-->" + hcPath);
                if (File.Exists(hcPath))
                {
                    Assembly handy = Assembly.LoadFrom(hcPath);
                    if (handy != null)
                    {
                        Trace.WriteLine("加载HandyControl成功!");
                    }
                    else
                    {
                        Trace.WriteLine("加载HandyControl失败-->Assembly==null!");
                    }
                }
                else
                {
                    Trace.WriteLine("未找到文件!-->" + hcPath);
                }
                #endregion

                #region 未捕获异常设置
                app = System.Windows.Application.Current;
                app = app != null ? app : new System.Windows.Application();
                app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                var _currentDomain = AppDomain.CurrentDomain;
                ////_currentDomain.UnhandledException += (sender, e) =>
                ////{
                ////    Trace.WriteLine((e.ExceptionObject as Exception).ToString());
                ////    if (iSwApp != null)
                ////    {
                ////        iSwApp.SendMsgToUser($"发生了未预料的异常，请联系开发人员处理，异常信息存储在{logPath}");
                ////    }

                ////};
                ////app.DispatcherUnhandledException += (sender, e) =>
                ////{
                ////    Trace.WriteLine(e.Exception);
                ////    try
                ////    {
                ////        e.Handled = true;

                ////    }
                ////    catch
                ////    {
                ////    }
                ////    if (iSwApp != null)
                ////    {
                ////        iSwApp.SendMsgToUser($"发生了未预料的异常，请联系开发人员处理，异常信息存储在{logPath}");
                ////    }
                ////};
                #endregion

                #region dll加载错误解决
                AppDomain.CurrentDomain.ResourceResolve += ResourceResolve;
                AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;
                AppDomain.CurrentDomain.TypeResolve += AssemblyResolve;
                AppDomain.CurrentDomain.AssemblyLoad += AssemblyLoad;
                #endregion

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("插件初始化发生错误:" + ex.ToString());
            }
            _swAddin = this;
        }

        /// <summary>
        /// 插件连接至sw(执行完构造函数后，sw内部调用)
        /// </summary>
        /// <param name="ThisSW"></param>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public bool ConnectToSW(object ThisSW, int cookie)
        {
            iSwApp = (ISldWorks)ThisSW;
            addinID = cookie;

            //Setup callbacks
            iSwApp.SetAddinCallbackInfo(0, this, addinID);

            #region Setup the Command Manager
            iCmdMgr = iSwApp.GetCommandManager(cookie);
            AddCommandMgr();
            #endregion

            #region Setup the Event Handlers
            SwEventPtr = (SolidWorks.Interop.sldworks.SldWorks)iSwApp;
            openDocs = new Hashtable();
            AttachEventHandlers();
            #endregion

            #region 自定义属性页
            //AddPMP();
            #endregion

            return true;
        }

        /// <summary>
        /// 插件取消连接(sw内部调用，关闭sw或者手动关闭插件时)
        /// </summary>
        /// <returns></returns>
        public bool DisconnectFromSW()
        {
            //恢复标准输出
            LogUtil.CloseConsole();

            AppDomain.CurrentDomain.ResourceResolve -= ResourceResolve;
            AppDomain.CurrentDomain.AssemblyResolve -= AssemblyResolve;
            AppDomain.CurrentDomain.TypeResolve -= AssemblyResolve;
            AppDomain.CurrentDomain.AssemblyLoad -= AssemblyLoad;

            RemoveCommandMgr();
            RemovePMP();
            DetachEventHandlers();

            Marshal.ReleaseComObject(iCmdMgr);
            iCmdMgr = null;
            Marshal.ReleaseComObject(iSwApp);
            iSwApp = null;
            //The addin _must_ call GC.Collect() here in order to retrieve all managed code pointers 
            GC.Collect();
            GC.WaitForPendingFinalizers();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }
        #endregion

        #region 加载程序集相关
        /// <summary>
        /// 加载按钮动作所在程序集
        /// </summary>
        private void LoadAssembly()
        {
            foreach (var tab in _buttonConfig.CommandTabs)
            {
                foreach (var btn in tab.Buttons)
                {
                    var path = btn.AssemblyPath;
                    if (!File.Exists(path))
                    {
                        Trace.WriteLine($"不存在指定的程序集文件：{path}");
                    }
                    var asm = Assembly.LoadFrom(path);
                    if (!this._asmDict.ContainsKey(btn.AssemblyPath))
                    {
                        this._asmDict.Add(btn.AssemblyPath, asm);
                    }
                }
            }
        }

        /// <summary>
        /// 记录加载所有的dll加载过程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            try
            {
                Trace.WriteLine("加载dll-->" + args.LoadedAssembly.FullName);
                Trace.WriteLine("dll路径-->" + args.LoadedAssembly.Location);
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 有时候sw会只在sw安装路径加载dll，此处转换至插件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Assembly ResourceResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var path = args.Name.Replace("\\\\", "\\");
                string[] parts = path.Split('.');
                var asmName = Path.GetFileNameWithoutExtension(path);
                var appDomainAsms = AppDomain.CurrentDomain.GetAssemblies();
                var selfs = appDomainAsms.Where(x => x.FullName.Split(',')[0] == asmName).ToArray();
                if (selfs.Length > 0)
                {
                    return selfs[0];
                }
                if (File.Exists(path))
                {
                    return Assembly.LoadFile(path);
                }
                string file = args.RequestingAssembly?.Location;
                if (File.Exists(file))
                {
                    return Assembly.LoadFile(file);
                }
                file = $"{this._addinDir}\\{parts[0].Trim()}.dll";
                return Assembly.LoadFrom(file);
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine($"解析dll时出错：{ex}");
                return null;
            }
        }
        /// <summary>
        /// 有时候sw会只在sw安装路径加载dll，此处转换至插件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                var path = args.Name.Replace("\\\\", "\\");
                var asmName = Path.GetFileNameWithoutExtension(path);
                string[] parts = path.Split(',');
                if (parts[0].EndsWith(".resources") && !parts[2].EndsWith("neutral"))
                {
                    return null;
                };
                var appDomainAsms = AppDomain.CurrentDomain.GetAssemblies();
                var selfs = appDomainAsms.Where(x => x.FullName.Split(',')[0] == asmName).ToArray();
                if (selfs.Length > 0)
                {
                    return selfs[0];
                }
                string file = args.RequestingAssembly?.Location;
                if (File.Exists(file))
                {
                    return Assembly.LoadFile(file);
                }

                if (File.Exists(path))
                {
                    return Assembly.LoadFrom(path);
                }
                file = $"{AppDomain.CurrentDomain.BaseDirectory}\\{parts[0].Trim()}.dll";

                if (File.Exists(file))
                {
                    return Assembly.LoadFile(file);
                }

                file = $"{this._addinDir}\\{parts[0].Trim()}.dll";
                return Assembly.LoadFrom(file);
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine($"解析dll时出错：{ex.ToString()}");
                return null;
            }
        }
        #endregion

        #region UI添加按钮
        /// <summary>
        /// 添加菜单
        /// </summary>
        public void AddCommandMgr()
        {

            try
            {
                int tabIndex = 0;
                int buttonIndex = 0;
                if (_buttonConfig.CommandTabs is null || _buttonConfig.CommandTabs.Count == 0)
                {
                    int[] docTypes = new[] { 1, 2, 3 };
                    foreach (var type in docTypes)
                    {
                        var count = iCmdMgr.GetCommandTabCount(type);
                        for (int j = 0; j < count; j++)
                        {
                            var cmdTab = iCmdMgr.IGetCommandTabs(type, j);
                            //移除多余的tab
                            if (cmdTab != null)
                            {
                                bool res = iCmdMgr.RemoveCommandTab(cmdTab);
                            }
                        }
                    }

                }
                foreach (var tab in _buttonConfig.CommandTabs)
                {
                    tabIndex++;
                    int buttonCount = tab.Buttons.Where(x => x.Name != "---分割线---").Count();
                    ICommandGroup cmdGroup;
                    if (iBmp == null)
                        iBmp = new BitmapHandler();
                    Assembly thisAssembly;
                    string Title = tab.Title, ToolTip = tab.Title;
                    int[] docTypes = new int[]{(int)swDocumentTypes_e.swDocASSEMBLY,
                                       (int)swDocumentTypes_e.swDocDRAWING,
                                       (int)swDocumentTypes_e.swDocPART};
                    thisAssembly = System.Reflection.Assembly.GetAssembly(this.GetType());
                    int cmdGroupErr = 0;
                    bool ignorePrevious = false;

                    object registryIDs;
                    //get the ID information stored in the registry
                    bool getDataResult = iCmdMgr.GetGroupDataFromRegistry(tabIndex, out registryIDs);
                    var idList = new List<int>();
                    for (int i = 0; i < buttonCount; i++)
                    {
                        idList.Add(i);
                    }
                    int[] knownIDs = idList.ToArray();

                    if (registryIDs is null || (!CompareIDs((int[])registryIDs, knownIDs)))
                    {
                        ignorePrevious = true;
                    }
                    string[] mainIcons = new string[6];

                    cmdGroup = iCmdMgr.CreateCommandGroup2(tabIndex, Title, ToolTip, "", -1, ignorePrevious, ref cmdGroupErr);
                    try
                    {
                        List<string> icons = JoinImage(tab.Title, tab.Buttons.Where(x => x.Name != "---分割线---").Select(x => x.IconPath).ToList()).ToList();
                        cmdGroup.LargeIconList = icons[1];
                        cmdGroup.SmallIconList = icons[0];
                        cmdGroup.LargeMainIcon = icons[1];
                        cmdGroup.SmallMainIcon = icons[0];
                        //cmdGroup.LargeIconList = iBmp.CreateFileFromResourceBitmap("ParametricTool.SwEntry.ToolbarLarge.bmp", thisAssembly);
                        //cmdGroup.SmallIconList = iBmp.CreateFileFromResourceBitmap("ParametricTool.SwEntry.ToolbarSmall.bmp", thisAssembly);
                        //cmdGroup.LargeMainIcon = iBmp.CreateFileFromResourceBitmap("ParametricTool.SwEntry.MainIconLarge.bmp", thisAssembly);
                        //cmdGroup.SmallMainIcon = iBmp.CreateFileFromResourceBitmap("ParametricTool.SwEntry.MainIconSmall.bmp", thisAssembly);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.ToString());
                    }

                    int menuToolbarOption = (int)(swCommandItemType_e.swMenuItem | swCommandItemType_e.swToolbarItem);
                    buttonIndex = 0;
                    List<int> cmdIndexList = new List<int>();
                    foreach (var btn in tab.Buttons)
                    {
                        int cmdIndex = 0;
                        if (btn.Name == "---分割线---")
                        {
                            cmdGroup.AddSpacer2(-1, menuToolbarOption);
                        }
                        else
                        {
                            //按钮点击触发的函数及参数
                            string activeStr = $"ActivateBtton({btn.FullName})";
                            //按钮是否可用触发的函数及参数
                            string enableStr = $"EnableButton({btn.FullName})";
                            cmdIndex = cmdGroup.AddCommandItem2(btn.Name, -1, btn.Name, btn.Name, buttonIndex, activeStr, enableStr, 0, menuToolbarOption);
                            buttonIndex++;
                        }
                        cmdIndexList.Add(cmdIndex);
                    }

                    cmdGroup.HasToolbar = true;
                    cmdGroup.HasMenu = true;
                    cmdGroup.Activate();



                    bool bResult;

                    #region flyoutGroupID 添加，暂时无用

                    #endregion

                    foreach (int type in docTypes)
                    {
                        SolidWorks.Interop.sldworks.CommandTab cmdTab;

                        cmdTab = iCmdMgr.GetCommandTab(type, Title);

                        if (cmdTab != null & !getDataResult | ignorePrevious)//if tab exists, but we have ignored the registry info (or changed command group ID), re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
                        {
                            bool res = iCmdMgr.RemoveCommandTab(cmdTab);
                            cmdTab = null;
                        }

                        //if cmdTab is null, must be first load (possibly after reset), add the commands to the tabs
                        if (cmdTab == null)
                        {
                            cmdTab = iCmdMgr.AddCommandTab(type, Title);
                            int cmdCount = tab.Buttons.Count;
                            int index = -1;
                            List<List<int>> idGroups = new List<List<int>>();
                            List<int> ids = new List<int>();
                            foreach (var item in tab.Buttons)
                            {
                                index++;
                                if (item.Name == "---分割线---")
                                {
                                    if (ids.Count > 0)
                                    {
                                        idGroups.Add(ids.Select(x => x).ToList());
                                        ids = new List<int>();
                                    }
                                }
                                else
                                {
                                    ids.Add(cmdGroup.get_CommandID(cmdIndexList[index]));
                                }
                            }
                            if (ids.Count > 0)
                            {
                                idGroups.Add(ids);
                            }
                            Dictionary<CommandTabBox, int> tabBoxs = new Dictionary<CommandTabBox, int>();
                            for (int i = 0; i < idGroups.Count; i++)
                            {
                                CommandTabBox cmdBox = cmdTab.AddCommandTabBox();
                                cmdBox.AddCommands(idGroups[i].ToArray(), idGroups[i].Select(x => (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow).ToArray());
                                tabBoxs.Add(cmdBox, idGroups[i][0]);
                            }
                            for (int i = 1; i < tabBoxs.Count; i++)
                            {
                                KeyValuePair<CommandTabBox, int> tabBox = tabBoxs.ElementAt(i);
                                cmdTab.AddSeparator(tabBox.Key, tabBox.Value);
                            }

                            #region flyGroup暂时不用
                            #endregion

                        }

                    }

                    #region 添加右键菜单
                    //////// Create a third-party icon in the context-sensitive menus of faces in parts
                    //////// To see this menu, right click on any face in the part
                    //////var swFrame = iSwApp.Frame() as Frame;
                    //////bResult = swFrame.AddMenuPopupIcon3((int)swDocumentTypes_e.swDocPART, (int)swSelectType_e.swSelFACES, "third-party context-sensitive CSharp", addinID,
                    //////                                    "PopupCallbackFunction", "PopupEnable", "", cmdGroup.MainIconList);

                    //////// create and register the shortcut menu
                    //////registerID = iSwApp.RegisterThirdPartyPopupMenu();

                    //////// add a menu break at the top of the shortcut menu
                    //////bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Menu Break", addinID, "", "", "", "", "", (int)swMenuItemType_e.swMenuItemType_Break);

                    //////// add a couple of items to the shortcut menu
                    //////bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Test1", addinID, "TestCallback", "EnableTest", "", "Test1", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);
                    //////bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Test2", addinID, "TestCallback", "EnableTest", "", "Test2", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);

                    //////// add a separator bar to the shortcut menu
                    //////bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "separator", addinID, "", "", "", "", "", (int)swMenuItemType_e.swMenuItemType_Separator);

                    //////// add another item to the shortcut menu
                    //////bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "Test3", addinID, "TestCallback", "EnableTest", "", "Test3", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);

                    //////// add an icon to a menu bar of the shortcut menu
                    //////bResult = iSwApp.AddItemToThirdPartyPopupMenu2(registerID, (int)swDocumentTypes_e.swDocPART, "", addinID, "TestCallback", "EnableTest", "", "NoOp", mainIcons[0], (int)swMenuItemType_e.swMenuItemType_Default);
                    #endregion

                    thisAssembly = null;
                }
            }
            catch (System.Exception ex)
            {
                Trace.WriteLine($"{_buttonConfig.AddinTitle}插件菜单加载过程中出现了未预料的错误，请尝试卸载插件重新注册，或联系开发人员处理：{ex.ToString()}");
                System.Windows.Forms.MessageBox.Show($"{_buttonConfig.AddinTitle}插件菜单加载过程中出现了未预料的错误，请尝试卸载插件重新注册，或联系开发人员处理：{ex.ToString()}");
            }
        }

        #region 处理按钮图标
        /// <summary>
        /// 拼接图片
        /// </summary>
        /// <param name="title"></param>
        /// <param name="iconPaths"></param>
        /// <returns></returns>
        public List<string> JoinImage(string title, List<string> iconPaths)
        {
            List<string> result = new List<string>();
            List<Image> imageList = iconPaths.Select(x => GetImage(Path.GetFullPath(x))).ToList();
            //List<Image> iconImage20xList = imageList.Select(x => KiResizeImage(x, 20, 20)).ToList();
            //List<Image> iconImage32xList = imageList.Select(x => KiResizeImage(x, 32, 32)).ToList();
            //List<Image> iconImage40xList = imageList.Select(x => KiResizeImage(x, 40, 40)).ToList();
            //List<Image> iconImage64xList = imageList.Select(x => KiResizeImage(x, 64, 64)).ToList();
            //List<Image> iconImage96xList = imageList.Select(x => KiResizeImage(x, 96, 96)).ToList();
            //List<Image> iconImage128xList = imageList.Select(x => KiResizeImage(x, 128, 128)).ToList();

            List<Image> iconImage16xList = imageList.Select(x => KiResizeImage(x, 16, 16)).ToList();
            List<Image> iconImage24xList = imageList.Select(x => KiResizeImage(x, 24, 24)).ToList();

            //string iconImage20x = $"{_addinDir}\\icons\\" + title + "-iconImage20x.png";
            //string iconImage32x = $"{_addinDir}\\icons\\" + title + "-iconImage32x.png";
            //string iconImage40x = $"{_addinDir}\\icons\\" + title + "-iconImage40x.png";
            //string iconImage64x = $"{_addinDir}\\icons\\" + title + "-iconImage64x.png";
            //string iconImage96x = $"{_addinDir}\\icons\\" + title + "-iconImage96x.png";
            //string iconImage128x = $"{_addinDir}\\icons\\" + title + "-iconImage128x.png";

            string iconImage16x = $"{_addinDir}\\icons\\" + title + "-iconImage16x.png";
            string iconImage24x = $"{_addinDir}\\icons\\" + title + "-iconImage24x.png";

            //result.Add(iconImage20x);
            //result.Add(iconImage32x);
            //result.Add(iconImage40x);
            //result.Add(iconImage64x);
            //result.Add(iconImage96x);
            //result.Add(iconImage128x);

            result.Add(iconImage16x);
            result.Add(iconImage24x);

            foreach (var item in result)
            {
                if (File.Exists(item))
                {
                    File.Delete(item);
                }
            }
            //拼接并保存图片
            //CombinImage(iconImage20xList).Save(iconImage20x, ImageFormat.Png);
            //CombinImage(iconImage32xList).Save(iconImage32x, ImageFormat.Png);
            //CombinImage(iconImage40xList).Save(iconImage40x, ImageFormat.Png);
            //CombinImage(iconImage64xList).Save(iconImage64x, ImageFormat.Png);
            //CombinImage(iconImage96xList).Save(iconImage96x, ImageFormat.Png);
            //CombinImage(iconImage128xList).Save(iconImage128x, ImageFormat.Png);


            CombinImage(iconImage16xList).Save(iconImage16x, ImageFormat.Png);
            CombinImage(iconImage24xList).Save(iconImage24x, ImageFormat.Png);
            return result;
        }

        public static Image GetImage(string file)
        {
            try
            {
                return Image.FromFile(file);
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show(file + "文件读取错误!");
                return null;
            }

        }
        /// <summary>
        /// 改变图片像素
        /// </summary>
        /// <param name="bmp"></param>
        /// <param name="newW">需要改变的像素宽</param>
        /// <param name="newH">需要改变的像素高</param>
        /// <returns></returns>
        public static Image KiResizeImage(Image bmp, int newW, int newH)
        {
            try
            {
                Bitmap b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量 
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 合并图片
        /// </summary>
        /// <param name="imgs"></param>
        /// <returns></returns>
        public static Bitmap CombinImage(List<Image> imgs)
        {
            int width = imgs[0].Width;
            int height = imgs[0].Height;
            int count = imgs.Count;

            Bitmap bmp = new Bitmap(width * count, height);

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            for (int i = 0; i < count; i++)
            {
                g.DrawImage(imgs[i], i * width, 0, width, height);
            }
            GC.Collect();
            return bmp;
        }
        #endregion

        public void RemoveCommandMgr()
        {
            iBmp.Dispose();
            iCmdMgr.RemoveCommandGroup(0);
        }

        public bool CompareIDs(int[] storedIDs, int[] addinIDs)
        {
            List<int> storedList = new List<int>(storedIDs);
            List<int> addinList = new List<int>(addinIDs);

            addinList.Sort();
            storedList.Sort();

            if (addinList.Count != storedList.Count)
            {
                return false;
            }
            else
            {

                for (int i = 0; i < addinList.Count; i++)
                {
                    if (addinList[i] != storedList[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public Boolean AddPMP()
        {
            ppage = new UserPMPage(this);
            return true;
        }

        public Boolean RemovePMP()
        {
            ppage = null;
            return true;
        }

        #endregion

        #region UI回调

        #region 按钮执行方法
        /// <summary>
        /// 执行按钮命令
        /// </summary>
        /// <param name="btnFullName"></param>
        public void ActivateBtton(string btnFullName)
        {
            try
            {
                Trace.WriteLine($"{btnFullName}开始执行;");
                var btn = _buttonConfig.GetButtonFromFullName(btnFullName);
                if (this._asmDict.ContainsKey(btn.AssemblyPath))
                {
                    var assembly = _asmDict[btn.AssemblyPath];
                    Type tp = assembly.GetType(btn.FullClassName);
                    Trace.WriteLine("开始执行按钮命令");
                    ActivateBtton(btn, tp);
                    Trace.WriteLine("按钮命令执行完成");
                }
                else
                {
                    Trace.WriteLine($"未加载{btn.AssemblyPath}");
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"功能初始化过程中发生了未处理的异常，异常信息为:{ex.ToString()}");
                Trace.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 执行按钮命令
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="tp"></param>
        /// <returns></returns>
        public void ActivateBtton(CommandBtn btn, Type tp)
        {
            var tempArgs = new List<object>();
            tempArgs.Add(this.SwApp);
            if (btn.ButtonArgs != null)
            {
                tempArgs.Add(btn.ButtonArgs.ToArray());
            }
            else
            {
                tempArgs.Add(null);
            }
            object[] args = tempArgs.Count == 0 ? null : tempArgs.ToArray();
            InvokeStaticMethod(tp, btn.MethodName, args);
        }

        public static object InvokeStaticMethod(Type tp, string methodName, params Object[] args)
        {
            Trace.WriteLine("Type-->" + tp.FullName);
            Trace.WriteLine("MethodName-->" + methodName);
            MethodInfo method = tp.GetMethod(methodName);

            if (method == null)
            {
                string err = ("获取方法名称失败:" + methodName);
                Trace.WriteLine(err);
                Trace.WriteLine("dll路径-->" + tp.Assembly.Location);
                throw new Exception(err);
            }
            else
            {
                Trace.WriteLine("找到方法名!");
            }
            var obj = method.Invoke(null, args);
            return obj;
        }
        #endregion

        #region 按钮是否可用
        /// <summary>
        /// 按钮是否可用(SW内部调用，具体查看注册按钮时代)
        /// </summary>
        /// <param name="btnFullName">按钮的全名称，具体查看注册按钮时代码</param>
        /// <returns>0 不可用  1 可用</returns>
        public int EnableButton(string btnFullName)
        {
            var btn = _buttonConfig.GetButtonFromFullName(btnFullName);
            if (btn.SensitiveEnvs == null || btn.SensitiveEnvs.Count == 0)
            {
                return 0;
            }
            ButtonEnv currentEnv = ButtonEnv.NoModel;
            //判断当前sw环境
            if (iSwApp.ActiveDoc != null)
            {
                swDocumentTypes_e type = (swDocumentTypes_e)((ModelDoc2)iSwApp.ActiveDoc).GetType();
                switch (type)
                {
                    case swDocumentTypes_e.swDocNONE:
                        currentEnv = ButtonEnv.NONE;
                        break;
                    case swDocumentTypes_e.swDocPART:
                        currentEnv = ButtonEnv.Part;
                        break;
                    case swDocumentTypes_e.swDocASSEMBLY:
                        currentEnv = ButtonEnv.Assembly;
                        break;
                    case swDocumentTypes_e.swDocDRAWING:
                        currentEnv = ButtonEnv.Drawing;
                        break;
                    case swDocumentTypes_e.swDocSDM:
                        currentEnv = ButtonEnv.SDM;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                currentEnv = ButtonEnv.NoModel;
            }
            if (btn.SensitiveEnvs.Contains(currentEnv))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        #endregion

        #region 右键菜单回调
        public void PopupCallbackFunction()
        {
            bool bRet;

            bRet = iSwApp.ShowThirdPartyPopupMenu(registerID, 500, 500);
        }

        public int PopupEnable()
        {
            if (iSwApp.ActiveDoc == null)
                return 0;
            else
                return 1;
        }

        public void TestCallback()
        {
            Debug.Print("Test Callback, CSharp");
        }

        public int EnableTest()
        {
            if (iSwApp.ActiveDoc == null)
                return 0;
            else
                return 1;
        }
        #endregion

        #region 属性页回调
        public void ShowPMP()
        {
            if (ppage != null)
                ppage.Show();
        }

        public int EnablePMP()
        {
            if (iSwApp.ActiveDoc != null)
                return 1;
            else
                return 0;
        }
        #endregion

        #region flyoutCallback 暂时不用
        ////public void FlyoutCallback()
        ////{
        ////    FlyoutGroup flyGroup = iCmdMgr.GetFlyoutGroup(flyoutGroupID);
        ////    flyGroup.RemoveAllCommandItems();
        ////    flyGroup.AddCommandItem(System.DateTime.Now.ToLongTimeString(), "test", 0, "FlyoutCommandItem1", "FlyoutEnableCommandItem1");
        ////}

        ////public int FlyoutEnable()
        ////{
        ////    return 1;
        ////}

        ////public void FlyoutCommandItem1()
        ////{
        ////    iSwApp.SendMsgToUser("Flyout command 1");
        ////}

        ////public int FlyoutEnableCommandItem1()
        ////{
        ////    return 1;
        ////}
        #endregion

        #endregion

        #region 触发事件
        public bool AttachEventHandlers()
        {
            AttachSwEvents();
            //Listen for events on all currently open docs
            AttachEventsToAllDocuments();
            return true;
        }

        private bool AttachSwEvents()
        {
            try
            {
                SwEventPtr.ActiveDocChangeNotify += new DSldWorksEvents_ActiveDocChangeNotifyEventHandler(OnDocChange);
                SwEventPtr.DocumentLoadNotify2 += new DSldWorksEvents_DocumentLoadNotify2EventHandler(OnDocLoad);
                SwEventPtr.FileNewNotify2 += new DSldWorksEvents_FileNewNotify2EventHandler(OnFileNew);
                SwEventPtr.ActiveModelDocChangeNotify += new DSldWorksEvents_ActiveModelDocChangeNotifyEventHandler(OnModelChange);
                SwEventPtr.FileOpenPostNotify += new DSldWorksEvents_FileOpenPostNotifyEventHandler(FileOpenPostNotify);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }



        private bool DetachSwEvents()
        {
            try
            {
                SwEventPtr.ActiveDocChangeNotify -= new DSldWorksEvents_ActiveDocChangeNotifyEventHandler(OnDocChange);
                SwEventPtr.DocumentLoadNotify2 -= new DSldWorksEvents_DocumentLoadNotify2EventHandler(OnDocLoad);
                SwEventPtr.FileNewNotify2 -= new DSldWorksEvents_FileNewNotify2EventHandler(OnFileNew);
                SwEventPtr.ActiveModelDocChangeNotify -= new DSldWorksEvents_ActiveModelDocChangeNotifyEventHandler(OnModelChange);
                SwEventPtr.FileOpenPostNotify -= new DSldWorksEvents_FileOpenPostNotifyEventHandler(FileOpenPostNotify);
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }

        }

        public void AttachEventsToAllDocuments()
        {
            ModelDoc2 modDoc = (ModelDoc2)iSwApp.GetFirstDocument();
            while (modDoc != null)
            {
                if (!openDocs.Contains(modDoc))
                {
                    AttachModelDocEventHandler(modDoc);
                }
                else if (openDocs.Contains(modDoc))
                {
                    bool connected = false;
                    DocumentEventHandler docHandler = (DocumentEventHandler)openDocs[modDoc];
                    if (docHandler != null)
                    {
                        connected = docHandler.ConnectModelViews();
                    }
                }

                modDoc = (ModelDoc2)modDoc.GetNext();
            }
        }

        public bool AttachModelDocEventHandler(ModelDoc2 modDoc)
        {
            if (modDoc == null)
                return false;

            DocumentEventHandler docHandler = null;

            if (!openDocs.Contains(modDoc))
            {
                switch (modDoc.GetType())
                {
                    case (int)swDocumentTypes_e.swDocPART:
                        {
                            docHandler = new PartEventHandler(modDoc, this);
                            break;
                        }
                    case (int)swDocumentTypes_e.swDocASSEMBLY:
                        {
                            docHandler = new AssemblyEventHandler(modDoc, this);
                            break;
                        }
                    case (int)swDocumentTypes_e.swDocDRAWING:
                        {
                            docHandler = new DrawingEventHandler(modDoc, this);
                            break;
                        }
                    default:
                        {
                            return false; //Unsupported document type
                        }
                }
                docHandler.AttachEventHandlers();
                openDocs.Add(modDoc, docHandler);
            }
            return true;
        }

        public bool DetachModelEventHandler(ModelDoc2 modDoc)
        {
            DocumentEventHandler docHandler;
            docHandler = (DocumentEventHandler)openDocs[modDoc];
            openDocs.Remove(modDoc);
            modDoc = null;
            docHandler = null;
            return true;
        }

        public bool DetachEventHandlers()
        {
            DetachSwEvents();

            //Close events on all currently open docs
            DocumentEventHandler docHandler;
            int numKeys = openDocs.Count;
            object[] keys = new Object[numKeys];

            //Remove all document event handlers
            openDocs.Keys.CopyTo(keys, 0);
            foreach (ModelDoc2 key in keys)
            {
                docHandler = (DocumentEventHandler)openDocs[key];
                docHandler.DetachEventHandlers(); //This also removes the pair from the hash
                docHandler = null;
            }
            return true;
        }

        public int OnDocChange()
        {
            return 0;
        }

        public int OnDocLoad(string docTitle, string docPath)
        {
            return 0;
        }

        int FileOpenPostNotify(string FileName)
        {
            AttachEventsToAllDocuments();
            return 0;
        }

        public int OnFileNew(object newDoc, int docType, string templateName)
        {
            AttachEventsToAllDocuments();
            return 0;
        }

        public int OnModelChange()
        {
            return 0;
        }
        #endregion
    }

}
