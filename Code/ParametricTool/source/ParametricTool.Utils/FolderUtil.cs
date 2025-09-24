using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.Utils
{
    public class FolderUtil
    {
        public static string SelectSingleFolder(string title)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = title;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return null;
        }
    }
}
