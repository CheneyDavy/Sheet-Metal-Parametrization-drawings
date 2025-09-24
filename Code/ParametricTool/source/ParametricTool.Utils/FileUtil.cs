using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ParametricTool.Utils
{
    public class FileUtil
    {
        /// <summary>
        /// 选择单个文件
        /// </summary>
        /// <param name="title">窗口标题</param>
        /// <param name="filter">过滤 例如 文本文档|*.txt</param>
        /// <param name=""></param>
        /// <returns></returns>
        public static string SelectSingleFile(string title,string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = title;
            dialog.Filter = filter;
            bool? hasSelect = dialog.ShowDialog();
            if (hasSelect.HasValue && hasSelect.Value)
            {
                return dialog.FileName;
            }
            return null;
        }
        
        
    }
}
