using ParametricTool.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ParametricTool.UI.View
{
    /// <summary>
    /// ParametricToolWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ParametricToolWindow : Window
    {
        public ParametricToolWindow()
        {
            InitializeComponent();

            ParametricToolViewModel vm = this.DataContext as ParametricToolViewModel;
            vm.m_Window = this;
        }

        //声明静态变量
        private static ParametricToolWindow window = null;

        //得到类实例的方法
        public static ParametricToolWindow GetInstance()
        {
            if (window == null || window.IsLoaded == false)
            {
                window = new ParametricToolWindow();
            }
            return window;
        }
    }
}
