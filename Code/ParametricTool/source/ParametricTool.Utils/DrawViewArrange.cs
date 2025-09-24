using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.Utils
{
    /// <summary>
    /// 视图自动排版计算
    /// </summary>
    public class DrawViewArrange
    {
        /// <summary>
        /// 主视图
        /// </summary>
        public ViewPara FrontView = null;
        /// <summary>
        /// 左视图
        /// </summary>
        public ViewPara LeftView = null;
        /// <summary>
        /// 右视图
        /// </summary>
        public ViewPara RightView = null;
        /// <summary>
        /// 俯视图
        /// </summary>
        public ViewPara TopView = null;
        /// <summary>
        /// 后视图
        /// </summary>
        public ViewPara BackView = null;
        /// <summary>
        /// 仰视图
        /// </summary>
        public ViewPara BottomView = null;
        /// <summary>
        /// 轴测图
        /// </summary>
        public ViewPara AxonometricView = null;
        /// <summary>
        /// 轴测图位置
        /// </summary>
        public AxonometricLocation AxonometricLocation = AxonometricLocation.RightBottom;
        /// <summary>
        /// 候选比例，如果不设置，则最后为计算的比例
        /// </summary>
        public List<double> CandidateScales { get; set; }
        /// <summary>
        /// 视图之间间距
        /// </summary>
        public double ViewGap = 0;
        /// <summary>
        /// 边上的视图与边界是否保留Gap
        /// </summary>
        public bool RetainGapViewOnEdge = true;
        /// <summary>
        /// 视图对齐是否考虑主视图与左视图高度不一致的情况，如果有请赋值  ByOrigin (视图坐标原点对齐)
        /// 默认ByBorder
        /// </summary>
        public AlignMethod AlignMethod = AlignMethod.ByBorder;
        /// <summary>
        /// 第一视角还是第三视角
        /// </summary>
        public Perspective Perspective;

        /// <summary>
        /// 视图整个区域宽度
        /// </summary>
        public double BorderWidth;

        /// <summary>
        /// 视图整个区域高度
        /// </summary>
        public double BorderHeight;


        /// <summary>
        /// 最终计算的比例
        /// </summary>
        public double Scale;

        public DrawViewArrange(double borderWidth, double borderHeight, double viewGap, bool retainGapViewOnEdge = true, List<double> candidateScales = null, AxonometricLocation axonometricLocation = AxonometricLocation.RightBottom, AlignMethod alignMethod = AlignMethod.ByBorder, Perspective perspective = Perspective.First)
        {
            BorderWidth = borderWidth;
            BorderHeight = borderHeight;
            CandidateScales = candidateScales;
            ViewGap = viewGap;
            RetainGapViewOnEdge = retainGapViewOnEdge;
            AxonometricLocation = axonometricLocation;
            AlignMethod = alignMethod;
            Perspective = perspective;
        }

        public void Computer()
        {
            #region 将视图摆放当作表格，求有几行几列
            ViewPara[,] viewParas = GetViewParaTable();
            int rowCount = viewParas.GetLength(0);
            int columnCount = viewParas.GetLength(1);
            #endregion

            #region 除去视图间空隙，计算真实的视图摆放区域长宽
            double viewRangeWidth = BorderWidth;
            double viewRangeHeight = BorderHeight;
            if (RetainGapViewOnEdge)
            {
                viewRangeWidth = viewRangeWidth - 2 * ViewGap;
                viewRangeHeight = viewRangeHeight - 2 * ViewGap;
            }
            viewRangeWidth = viewRangeWidth - (columnCount - 1) * ViewGap;
            viewRangeHeight = viewRangeHeight - (rowCount - 1) * ViewGap;
            #endregion

            #region 计算所有行中视图的总宽度和所有列中视图的总高度
            double viewTotalWidth = 0;
            double viewTotalHeight = 0;
            //宽度
            for (int i = 0; i < rowCount; i++)
            {
                double tmpWidth = 0;
                for (int j = 0; j < columnCount; j++)
                {
                    if (viewParas[i, j] != null)
                    {
                        tmpWidth += viewParas[i, j].Width;
                    }
                }
                if (tmpWidth > viewTotalWidth)
                {
                    viewTotalWidth = tmpWidth;
                }
            }
            //高度
            for (int i = 0; i < columnCount; i++)
            {
                double tmpHeight = 0;
                for (int j = 0; j < rowCount; j++)
                {
                    if (viewParas[j, i] != null)
                    {
                        tmpHeight += viewParas[j, i].Height;
                    }
                }
                if (tmpHeight > viewTotalHeight)
                {
                    viewTotalHeight = tmpHeight;
                }
            }
            #endregion

            #region 计算比例
            double scaleWidth = viewRangeWidth / viewTotalWidth;
            double scaleHeight = viewRangeHeight / viewTotalHeight;
            Scale = scaleWidth > scaleHeight ? scaleHeight : scaleWidth;
            //double fitScale = Scale;
            if (CandidateScales != null && CandidateScales.Any())
            {
                CandidateScales.Sort();
                if (Scale < CandidateScales.First())
                {
                    Scale = CandidateScales.First();
                }
                else if (Scale > CandidateScales.Last())
                {
                    Scale = CandidateScales.Last();
                }
                double tmpScale = 0;
                foreach (var item in CandidateScales)
                {
                    if (Scale < item)
                    {
                        Scale = tmpScale;
                        break;
                    }
                    tmpScale = item;
                }
            }
            #endregion

            #region 计算位置
            //每行高度
            List<double> maxRowHeights = new List<double>();

            //每列宽度
            List<double> maxColumnWidths = new List<double>();

            for (int i = 0; i < rowCount; i++)
            {
                //找出每行最高的视图高度
                double tmpHeight = 0;
                for (int j = 0; j < columnCount; j++)
                {
                    if (viewParas[i, j] != null && tmpHeight < viewParas[i, j].Height)
                    {
                        tmpHeight = viewParas[i, j].Height;
                    }
                }
                maxRowHeights.Add(tmpHeight * Scale);
            }

            for (int i = 0; i < columnCount; i++)
            {
                //找出每列最宽的视图宽度
                double tmpWidth = 0;
                for (int j = 0; j < rowCount; j++)
                {
                    if (viewParas[j, i] != null && tmpWidth < viewParas[j, i].Width)
                    {
                        tmpWidth = viewParas[j, i].Width;
                    }
                }
                maxColumnWidths.Add(tmpWidth * Scale);
            }

            double heightScale = viewRangeHeight / maxRowHeights.Sum();
            double widthScale = viewRangeWidth / maxColumnWidths.Sum();

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    double gapCount = 0;
                    if (RetainGapViewOnEdge)
                    {
                        gapCount++;
                    }
                    if (viewParas[i, j] != null)
                    {
                        double tmpWidth = 0;
                        for (int k = 0; k < j; k++)
                        {
                            tmpWidth += maxColumnWidths[k];
                        }
                        viewParas[i, j].CenterX = tmpWidth * widthScale + (gapCount + j) * ViewGap + maxColumnWidths[j] / 2 * widthScale;

                        double tmpHeight = 0;
                        for (int k = 0; k < i; k++)
                        {
                            tmpHeight += maxRowHeights[k];
                        }
                        viewParas[i, j].CenterY = BorderHeight - (tmpHeight * heightScale + (gapCount + i) * ViewGap + maxRowHeights[i] / 2 * heightScale);
                    }
                }
            }
            #endregion



        }

        private ViewPara[,] GetViewParaTable()
        {
            ViewPara[,] tmpViewParas = null;
            if (Perspective == Perspective.First)
            {
                //第一视角：第三视角未实现
                // XX  仰   XX
                // 右  主   左    后
                // XX  俯  
                ViewPara[,] tmpViewParas1 =
                {
                    { null,         BottomView,         null,           null},
                    { RightView,    FrontView,          LeftView,       BackView},
                    { null,         TopView,            null,           null}
                };
                tmpViewParas = tmpViewParas1;
            }
            else
            {
                ViewPara[,] tmpViewParas1 =
                {
                    { null,         BottomView,         null,           null},
                    { RightView,    FrontView,          LeftView,       BackView},
                    { null,         TopView,            null,           null}
                };
                tmpViewParas = tmpViewParas1;
            }
            if (AxonometricLocation == AxonometricLocation.RightTop)
            {
                tmpViewParas[0, 2] = AxonometricView;
            }
            else
            {
                tmpViewParas[2, 2] = AxonometricView;
            }
            return RemoveNullRowAndColumn(tmpViewParas);
        }

        private ViewPara[,] RemoveNullRowAndColumn(ViewPara[,] viewParas)
        {
            while (true)
            {
                bool needContinue = false;
                for (int i = 0; i < viewParas.GetLength(0); i++)
                {
                    if (IsNullRow(viewParas, i))
                    {
                        viewParas = RemoveRow(viewParas, i);
                        needContinue = true;
                        break;
                    }
                }
                if (needContinue)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            while (true)
            {
                bool needContinue = false;
                for (int i = 0; i < viewParas.GetLength(1); i++)
                {
                    if (IsNullCol(viewParas, i))
                    {
                        viewParas = RemoveColumn(viewParas, i);
                        needContinue = true;
                        break;
                    }
                }
                if (needContinue)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
            return viewParas;
        }

        private bool IsNullRow(ViewPara[,] viewParas, int rowIndex)
        {
            for (int i = 0; i < viewParas.GetLength(1); i++)
            {
                if (viewParas[rowIndex, i] != null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsNullCol(ViewPara[,] viewParas, int colIndex)
        {
            for (int i = 0; i < viewParas.GetLength(0); i++)
            {
                if (viewParas[i, colIndex] != null)
                {
                    return false;
                }
            }
            return true;
        }

        private ViewPara[,] RemoveRow(ViewPara[,] viewParas, int rowIndex)
        {
            int rowNum = viewParas.GetLength(0);
            int colNum = viewParas.GetLength(1);
            if (rowIndex < 0 || rowIndex >= rowNum)
            {
                return viewParas;
            }
            if (rowNum == 1)
            {
                return null;
            }
            ViewPara[,] result = new ViewPara[rowNum - 1, colNum];
            for (int i = 0; i < rowNum; i++)
            {
                if (i == rowIndex)
                {
                    continue;
                }
                else if (i < rowIndex)
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        result[i, j] = viewParas[i, j];
                    }
                }
                else
                {
                    for (int j = 0; j < colNum; j++)
                    {
                        result[i - 1, j] = viewParas[i, j];
                    }
                }
            }
            return result;
        }

        private ViewPara[,] RemoveColumn(ViewPara[,] viewParas, int colIndex)
        {
            int rowNum = viewParas.GetLength(0);
            int colNum = viewParas.GetLength(1);
            if (colIndex < 0 || colIndex >= colNum)
            {
                return viewParas;
            }
            if (colNum == 1)
            {
                return null;
            }
            ViewPara[,] result = new ViewPara[rowNum, colNum - 1];
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    if (colIndex > j)
                    {
                        result[i, j] = viewParas[i, j];
                    }
                    else if (colIndex == j)
                    {
                        continue;
                    }
                    else
                    {
                        result[i, j - 1] = viewParas[i, j];
                    }
                }
            }
            return result;
        }
    }

    public class ViewPara
    {
        #region 输入参数
        /// <summary>
        /// 视图对象
        /// </summary>
        public object View { get; set; }
        /// <summary>
        /// 从视图边界左下角到右下角长度
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// 从视图边界左下角到左上角高度
        /// </summary>
        public double Height { get; set; }
        /// <summary>
        /// 从视图边界左下角到视图模型的坐标原点X方向长(AlignMethod = AlignMethod.ByOrigin 才会用到)
        /// 考虑一些情况才设置的此参数:比如主视图和左视图Height不一致，或者两视图的此参数值不一致。统一按照坐标原点对齐处理
        /// </summary>
        public double ViewOriginX { get; set; }
        /// <summary>
        /// 从视图边界左下角到视图模型的坐标原点Y方向高(AlignMethod = AlignMethod.ByOrigin 才会用到)
        /// 考虑一些情况才设置的此参数:比如主视图和左视图Height不一致，或者两视图的此参数值不一致。统一按照坐标原点对齐处理
        /// </summary>
        public double ViewOriginY { get; set; }
        #endregion
        #region 输出参数
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        #endregion

        public ViewPara(double width, double height, object view)
        {
            Width = width;
            Height = height;
            View = view;
        }
    }
    /// <summary>
    /// 视图排布遵循第一视角还是第三视角
    /// 第一视角：(XX表示空白)
    /// XX  仰   XX
    /// 右  主   左    后
    /// XX  俯   
    /// </summary>
    public enum Perspective
    {
        /// <summary>
        /// 第一视角
        /// </summary>
        First,
        /// <summary>
        /// 第三视角
        /// </summary>
        Third
    }

    /// <summary>
    /// 轴测图在右上还是左下  
    /// </summary>
    public enum AxonometricLocation
    {
        RightTop,
        RightBottom
    }

    /// <summary>
    /// 视图对齐是否考虑主视图与左视图高度不一致的情况，如果有请赋值  ByOrigin (视图坐标原点对齐)
    /// </summary>
    public enum AlignMethod
    {
        ByBorder,
        ByOrigin
    }
}
