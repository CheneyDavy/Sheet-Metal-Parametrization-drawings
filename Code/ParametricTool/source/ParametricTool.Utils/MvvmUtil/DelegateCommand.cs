using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ParametricTool.Utils.MvvmUtil
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (this.CanExcuteFunc == null)
            {
                return true;
            }
            return this.CanExcuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            if (this.ExcuteAction == null)
            {
                return;
            }
            this.ExcuteAction(parameter);
        }
        public Action<object> ExcuteAction { get; set; }
        public Func<object, bool> CanExcuteFunc { get; set; }
    }
}
