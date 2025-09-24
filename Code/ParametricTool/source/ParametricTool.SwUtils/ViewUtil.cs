using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwUtils
{
    public class ViewUtil
    {
        /// <summary>
        /// 移动视图，将三维模型中的某一点坐标与图纸页中的某一点坐标重合
        /// </summary>
        /// <param name="view"></param>
        /// <param name="modelPoint"></param>
        /// <param name="sheetPoint"></param>
        public static void MoveByModelPoint(IView view, double[] modelPoint, double[] sheetPoint)
        {
            double[] modelToSheet = PointMapUtil.ModelToSheet(view, modelPoint);
            double[] oldPosition = view.Position;
            double[] newPosition = new double[] { oldPosition[0] + (sheetPoint[0] - modelToSheet[0]),
                                                 oldPosition[1] + (sheetPoint[1] - modelToSheet[1]),
                                                    };
            view.Position = newPosition;
        }
        /// <summary>
        /// 移动视图，将View中的某一点坐标与图纸页中的某一点坐标重合
        /// </summary>
        /// <param name="view"></param>
        /// <param name="viewPoint"></param>
        /// <param name="sheetPoint"></param>
        public static void MoveByViewPoint(IView view, double[] viewPoint, double[] sheetPoint)
        {
            double[] viewToSheet = PointMapUtil.ViewToSheet(view, viewPoint);
            double[] oldPosition = view.Position;
            double[] newPosition = new double[] { oldPosition[0] + (sheetPoint[0] - viewToSheet[0]),
                                                 oldPosition[1] + (sheetPoint[1] - viewToSheet[1]),
                                                    };
            view.Position = newPosition;
        }







        /// <summary>
        /// 获取视图中实体的包围框大小，相对于视图中心的   左下角x 左下角y 右上角x 右上角y
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double[] GetEntitiesBox(IView view)
        {
            DrawingComponent drawingComponent = view.RootDrawingComponent;
            Component2 comp = drawingComponent.Component;
            double[] box = GetEntitiesBox(comp);
            if (box == null)
            {
                throw new Exception("获取包围盒失败,Component中无实体!");
            }
            double[] start = PointMapUtil.ModelToView(view, new double[] { box[0], box[1], box[2] });
            double[] end = PointMapUtil.ModelToView(view, new double[] { box[3], box[4], box[5] });
            box = new double[] { Math.Min(start[0], end[0]), Math.Min(start[1], end[1]), Math.Max(start[0], end[0]), Math.Max(start[1], end[1]) };
            return box;
        }

        public static Dictionary<IComponent2, double[]> compBoxDic = new Dictionary<IComponent2, double[]>();

        public static double[] GetEntitiesBox(IComponent2 comp)
        {
            if (comp == null)
            {
                throw new ArgumentNullException(nameof(comp));
            }
            if (compBoxDic.ContainsKey(comp))
            {
                return compBoxDic[comp];
            }
            string name = comp.Name2;
            double[] box = null;
            List<double> startX = new List<double>();
            List<double> startY = new List<double>();
            List<double> startZ = new List<double>();
            List<double> endX = new List<double>();
            List<double> endY = new List<double>();
            List<double> endZ = new List<double>();


            object infos = new object();
            IList<IBody2> bodies = DynamicCast.CastList<IBody2>(comp.GetBodies3((int)swBodyType_e.swSolidBody, out infos));
            if (bodies.Count > 0)
            {
                foreach (var item in bodies)
                {
                    double[] tmp = item.GetBodyBox();
                    double[] start = PointMapUtil.ComponentToAssembly(comp, new double[] { tmp[0], tmp[1], tmp[2] });
                    double[] end = PointMapUtil.ComponentToAssembly(comp, new double[] { tmp[3], tmp[4], tmp[5] });
                    startX.Add(new double[] { start[0], end[0] }.Min());
                    startY.Add(new double[] { start[1], end[1] }.Min());
                    startZ.Add(new double[] { start[2], end[2] }.Min());
                    endX.Add(new double[] { start[0], end[0] }.Max());
                    endY.Add(new double[] { start[1], end[1] }.Max());
                    endZ.Add(new double[] { start[2], end[2] }.Max());
                }
            }
            List<IComponent2> children = DynamicCast.CastList<IComponent2>(comp.GetChildren());
            if (children.Count > 0)
            {
                foreach (var item in children)
                {
                    double[] tmp = GetEntitiesBox(item);
                    if (tmp != null)
                    {
                        startX.Add(tmp[0]);
                        startY.Add(tmp[1]);
                        startZ.Add(tmp[2]);
                        endX.Add(tmp[3]);
                        endY.Add(tmp[4]);
                        endZ.Add(tmp[5]);
                    }
                }
            }
            if (startX.Count > 0)
            {
                box = new double[] { startX.Min(), startY.Min(), startZ.Min(), endX.Max(), endY.Max(), endZ.Max() };
            }
            if (!compBoxDic.ContainsKey(comp))
            {
                compBoxDic.Add(comp, box);
            }
            return box;

        }
    }
}
