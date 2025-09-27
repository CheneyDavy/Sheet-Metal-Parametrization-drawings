using ParametricTool.Utils.MvvmUtil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.UI.Models
{
    public class ModelItemCollection : NotifyPropertyChanged
    {
        #region 绑定的属性
        #region 所有实例
        private ObservableCollection<ModelItem> items = new ObservableCollection<ModelItem>();
        /// <summary>
        /// 所有实例
        /// </summary>
        public ObservableCollection<ModelItem> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                OnPropertyChanged(nameof(Items));
            }
        }
        #endregion

        #region 第二列属性Title
        private string numTitle;
        /// <summary>
        /// 第二列属性Title
        /// </summary>
        public string NumTitle
        {
            get
            {
                return numTitle;
            }
            set
            {
                numTitle = value;
                OnPropertyChanged(nameof(NumTitle));
            }
        }
        #endregion

        #region 自定义属性Title
        private ObservableCollection<string> propTitles = new ObservableCollection<string>();
        /// <summary>
        /// 自定义属性Title
        /// </summary>
        public ObservableCollection<string> PropTitles
        {
            get
            {
                return propTitles;
            }
            set
            {
                propTitles = value;
                OnPropertyChanged(nameof(PropTitles));
            }
        }
        #endregion
        #endregion
    }
}
