using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwUtils
{
    public class PointMapUtil
    {
        /// <summary>
        /// 将模型中的坐标映射到草图中
        /// </summary>
        /// <param name="sketch"></param>
        /// <param name="modelPoint">注意 单位为m</param>
        /// <returns></returns>
        public static double[] ModelToSketch(ISketch sketch, double[] modelPoint)
        {
            MathTransform sketchTransform = sketch.ModelToSketchTransform;
            IMathPoint mathPoint = ((IMathUtility)(Constants.iSwApp.GetMathUtility())).CreatePoint(modelPoint);

            IMathPoint sketchPoint = mathPoint.MultiplyTransform(sketchTransform);

            return sketchPoint.ArrayData;
        }

        /// <summary>
        /// 将草图中的坐标映射到模型中
        /// </summary>
        /// <param name="sketch"></param>
        /// <param name="modelPoint">注意 单位为m</param>
        /// <returns></returns>
        public static double[] SketchToModel(ISketch sketch, double[] modelPoint)
        {
            MathTransform sketchTransform = sketch.ModelToSketchTransform;

            sketchTransform = sketchTransform.IInverse();
            IMathPoint mathPoint = ((IMathUtility)(Constants.iSwApp.GetMathUtility())).CreatePoint(modelPoint);

            IMathPoint sketchPoint = mathPoint.MultiplyTransform(sketchTransform);

            return sketchPoint.ArrayData;
        }
        /// <summary>
        /// 将装配中的坐标映射到组件中
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="assemblyPoint">注意 单位为m</param>
        /// <returns></returns>
        public static double[] AssemblyToComponent(IComponent2 comp, double[] assemblyPoint)
        {
            MathTransform compTransform = comp.Transform2;
            IMathPoint mathPoint = ((IMathUtility)(Constants.iSwApp.GetMathUtility())).CreatePoint(assemblyPoint);

            IMathPoint sketchPoint = mathPoint.MultiplyTransform(compTransform.Inverse());

            return sketchPoint.ArrayData;
        }

        /// <summary>
        /// 将组件中的坐标映射到装配中
        /// </summary>
        /// <param name="comp"></param>
        /// <param name="compPoint">注意 单位为m</param>
        /// <returns></returns>
        public static double[] ComponentToAssembly(IComponent2 comp, double[] compPoint)
        {
            MathTransform compTransform = comp.Transform2;
            IMathPoint mathPoint = ((IMathUtility)(Constants.iSwApp.GetMathUtility())).CreatePoint(compPoint);

            IMathPoint sketchPoint = mathPoint.MultiplyTransform(compTransform);

            return sketchPoint.ArrayData;
        }
        /// <summary>
        /// 将模型中的点映射到视图中
        /// </summary>
        /// <param name="view"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static double[] ModelToView(IView view, double[] point)
        {
            double[] result = new double[3];
            MathTransform transform = view.ModelToViewTransform;
            IMathPoint mathPoint = ((IMathUtility)(Constants.iSwApp.GetMathUtility())).CreatePoint(point);
            IMathPoint viewPoint = mathPoint.MultiplyTransform(transform);
            double[] drawPoint = (double[])viewPoint.ArrayData;
            double[] pos = view.Position;
            result[0] = (drawPoint[0] - pos[0]) / view.ScaleDecimal;
            result[1] = (drawPoint[1] - pos[1]) / view.ScaleDecimal;
            result[2] = 0;
            return result;
        }

        public static double[] ModelToSheet(IView view, double[] point)
        {
            MathTransform transform = view.ModelToViewTransform;
            IMathPoint mathPoint = ((IMathUtility)(Constants.iSwApp.GetMathUtility())).CreatePoint(point);
            IMathPoint viewPoint = mathPoint.MultiplyTransform(transform);
            double[] drawPoint = (double[])viewPoint.ArrayData;
            return new double[] { drawPoint[0], drawPoint[1], drawPoint[2] };
        }

        public static double[] SheetToView(IView view, double[] point)
        {
            double[] result = new double[3];
            double[] pos = view.Position;
            result[0] = (point[0] - pos[0]) / view.ScaleDecimal;
            result[1] = (point[1] - pos[1]) / view.ScaleDecimal;
            result[2] = 0;
            return result;
        }

        public static double[] ViewToSheet(IView view, double[] point)
        {
            double[] result = new double[3];
            double[] pos = view.Position;
            result[0] = point[0] * view.ScaleDecimal + pos[0];
            result[1] = point[1] * view.ScaleDecimal + pos[1];
            result[2] = 0;
            return result;
        }
    }
}
