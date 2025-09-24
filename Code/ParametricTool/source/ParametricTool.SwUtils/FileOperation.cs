using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwUtils
{
    public class FileOperation
    {
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static ModelDoc2 OpenDoc(string strFile, bool visible)
        {
            if (strFile == null)
            {
                throw new ArgumentNullException(nameof(strFile));
            }
            if (!File.Exists(strFile))
            {
                throw new FileNotFoundException("文件不存在", strFile);
            }
            swDocumentTypes_e fileType = 0;
            //获取指定路径中文件的后缀名称
            string fileExtension = Path.GetExtension(strFile);
            //插入的模型是零件
            if (string.Equals(fileExtension, ".SLDPRT", StringComparison.CurrentCultureIgnoreCase))
            {
                fileType = swDocumentTypes_e.swDocPART;
            }
            else if (string.Equals(fileExtension, ".SLDLFP", StringComparison.CurrentCultureIgnoreCase))
            {
                fileType = swDocumentTypes_e.swDocPART;
            }
            else if (string.Equals(fileExtension, ".SLDASM", StringComparison.CurrentCultureIgnoreCase))
            {
                fileType = swDocumentTypes_e.swDocASSEMBLY;
            }
            else if (string.Equals(fileExtension, ".SLDDRW", StringComparison.CurrentCultureIgnoreCase))
            {
                fileType = swDocumentTypes_e.swDocDRAWING;
            }
            else
            {
                throw new Exception("试图打开不支持的文件类型:" + strFile);
            }
            swOpenDocOptions_e option = 0;
            bool currentVisible = Constants.iSwApp.GetDocumentVisible((int)fileType);
            if (!visible)
            {
                Constants.iSwApp.DocumentVisible(false, (int)fileType);
                option = swOpenDocOptions_e.swOpenDocOptions_Silent;
            }
            ModelDoc2 result = null;
            try
            {
                int loadError = 0;
                int loadWarnning = 0;
                result = Constants.iSwApp.OpenDoc6(strFile, (int)fileType, (int)option, "", ref loadError, ref loadWarnning);
            }
            finally
            {
                if (!visible && currentVisible)
                {
                    Constants.iSwApp.DocumentVisible(currentVisible, (int)fileType);
                }
            }
            return result;
        }


        /// <summary>
        /// 关闭文件(为支持新文件的关闭)
        /// </summary>
        /// <param name="modelDoc">需要关闭的文件</param>
        /// <param name="save">是否保存，默认为false，不保存</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static void CloseDoc(ModelDoc2 modelDoc, bool save = false)
        {
            if (save)
            {
                SaveDoc(modelDoc);
            }
            Constants.iSwApp.CloseDoc(modelDoc.GetPathName());
        }

        /// <summary>
        /// 关闭文件(为支持新文件的关闭)
        /// </summary>
        /// <param name="modelDoc">需要关闭的文件</param>
        /// <param name="save">是否保存，默认为false，不保存</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public static void SaveDoc(ModelDoc2 modelDoc)
        {
            if (modelDoc == null)
            {
                throw new ArgumentNullException(nameof(modelDoc));
            }
            bool boolstatus;
            int lErrors = 0;
            int lWarnings = 0;
            boolstatus = modelDoc.Save3((int)swSaveAsOptions_e.swSaveAsOptions_Silent, ref lErrors, ref lWarnings);
            if (!boolstatus)
            {
                string err = "模型保存失败,lErrors = " + lErrors + ";lWarnings = " + lWarnings;
                Trace.WriteLine(err);
                throw new Exception(err);
            }
        }
    }
}
