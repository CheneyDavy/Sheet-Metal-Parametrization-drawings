using ParametricTool.Utils.MvvmUtil;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.UI.Models
{
    public class ModelItem:NotifyPropertyChanged
    {
        #region 绑定的属性
        #region 序号
        private int index;
        /// <summary>
        /// 序号
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
                OnPropertyChanged(nameof(Index));
            }
        }
        #endregion

        #region 物料号
        private string number;
        /// <summary>
        /// 物料号
        /// </summary>
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged(nameof(Number));
            }
        }
        #endregion

        #region 是否需要出图
        private bool needDrawing;
        /// <summary>
        /// 是否需要出图
        /// </summary>
        public bool NeedDrawing
        {
            get
            {
                return needDrawing;
            }
            set
            {
                needDrawing = value;
                OnPropertyChanged(nameof(NeedDrawing));
            }
        }
        #endregion

        #region 属性值
        private ObservableCollection<string> propValues = new ObservableCollection<string>();
        /// <summary>
        /// 属性值
        /// </summary>
        public ObservableCollection<string> PropValues
        {
            get
            {
                return propValues;
            }
            set
            {
                propValues = value;
                OnPropertyChanged(nameof(PropValues));
            }
        }
        #endregion

        #region 是否成功
        private bool isSuccess;
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess
        {
            get
            {
                return isSuccess;
            }
            set
            {
                isSuccess = value;
                OnPropertyChanged(nameof(IsSuccess));
            }
        }
        #endregion

        #region 错误信息
        private string errorMsg;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMsg
        {
            get
            {
                return errorMsg;
            }
            set
            {
                errorMsg = value;
                OnPropertyChanged(nameof(ErrorMsg));
            }
        }
        #endregion

        #region 模型文件
        private string modelFile;
        /// <summary>
        /// 模型文件
        /// </summary>
        public string ModelFile
        {
            get
            {
                return modelFile;
            }
            set
            {
                modelFile = value;
                OnPropertyChanged(nameof(ModelFile));
            }
        }
        #endregion

        #region 模型对象
        private ModelDoc2 modelDoc;
        /// <summary>
        /// 模型对象
        /// </summary>
        public ModelDoc2 ModelDoc
        {
            get
            {
                return modelDoc;
            }
            set
            {
                modelDoc = value;
                OnPropertyChanged(nameof(ModelDoc));
            }
        }
        #endregion

        #region 三视图
        private string drawingFile1;
        /// <summary>
        /// 三视图
        /// </summary>
        public string DrawingFile1
        {
            get
            {
                return drawingFile1;
            }
            set
            {
                drawingFile1 = value;
                OnPropertyChanged(nameof(DrawingFile1));
            }
        }
        #endregion

        #region 三视图-dwg
        private string drawingFile2;
        /// <summary>
        /// 三视图-dwg
        /// </summary>
        public string DrawingFile2
        {
            get
            {
                return drawingFile2;
            }
            set
            {
                drawingFile2 = value;
                OnPropertyChanged(nameof(DrawingFile2));
            }
        }
        #endregion

        #region 三视图-pdf
        private string drawingFile3;
        /// <summary>
        /// 三视图-pdf
        /// </summary>
        public string DrawingFile3
        {
            get
            {
                return drawingFile3;
            }
            set
            {
                drawingFile3 = value;
                OnPropertyChanged(nameof(DrawingFile3));
            }
        }
        #endregion

        #region 展开图-dwg
        private string drawingFile4;
        /// <summary>
        /// 展开图-dwg
        /// </summary>
        public string DrawingFile4
        {
            get
            {
                return drawingFile4;
            }
            set
            {
                drawingFile4 = value;
                OnPropertyChanged(nameof(DrawingFile4));
            }
        }
        #endregion






        #endregion
    }

    public class ModelItemComparer : IEqualityComparer<ModelItem>
    {
        public bool Equals(ModelItem x, ModelItem y)
        {
            if (x == null && y != null)
            {
                return false;
            }
            if (x != null && y == null)
            {
                return false;
            }
            if (x == null && y == null)
            {
                return true;
            }
            if (x.Number == y.Number)
            {
                return true;
            }
            return false;
        }

        public int GetHashCode(ModelItem obj)
        {
            return obj.GetHashCode();
        }
    }
}
