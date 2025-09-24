using ParametricTool.UI.View;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParametricTool.Utils;

namespace ParametricTool.UI
{
    public class SwButtonEntry
    {
        /// <summary>
        /// 显示特征库窗口
        /// </summary>
        /// <param name="iSwApp"></param>
        public static void ShowParametricToolWindow(ISldWorks iSwApp, params string[] args)
        {
            SwUtils.Constants.iSwApp = iSwApp;

            {
                CheckResult check = LicenseUtil.Check();
                if (!check.IsPass)
                {
                    iSwApp.SendMsgToUser2(check.Message, (int)swMessageBoxIcon_e.swMbWarning, (int)swMessageBoxBtn_e.swMbOk);
                    return;
                }
            }
            
            try
            {
                ParametricToolWindow.GetInstance().Ex_Show();
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
                Trace.TraceError(err);
                iSwApp.SendMsgToUser2(ex.Message, (int)swMessageBoxIcon_e.swMbWarning, (int)swMessageBoxBtn_e.swMbOk);
            }
        }
    }
}
