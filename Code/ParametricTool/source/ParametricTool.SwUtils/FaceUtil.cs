using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwUtils
{
    public class FaceUtil
    {
        /// <summary>
        /// 获取零件中的所有面
        /// </summary>
        public static IList<Face2> GetFaces(ModelDoc2 swModelDoc, bool includeSheet = false)
        {
            List<Face2> faceList = new List<Face2>();
            PartDoc swPartDoc = (PartDoc)swModelDoc;
            object[] partBodies = (object[])swPartDoc.GetBodies2((int)swBodyType_e.swAllBodies, true);
            if (partBodies != null)
            {
                foreach (Body2 body in partBodies)
                {
                    object[] faceObjs = (object[])body.GetFaces();
                    foreach (Face2 face in faceObjs)
                    {
                        if (!includeSheet && face.GetShellType() == 0)
                        {
                            continue;
                        }
                        faceList.Add(face);
                    }
                }
            }

            return faceList;
        }
        /// <summary>
        /// 获取零件中的所有面
        /// </summary>
        public static IList<Face2> GetFaces(ModelDoc2 swModelDoc)
        {
            return GetFaces(swModelDoc, false);
        }

        public static void SetFaceColor(Face2 face2, double r, double g, double b)
        {
            //获取模型
            ModelDoc2 swModelDoc = Constants.iSwApp.ActiveDoc;
            var vFaceProp = swModelDoc.MaterialPropertyValues;

            var vProps = face2.GetMaterialPropertyValues2(1, null);
            vProps[0] = r;
            vProps[1] = g;
            vProps[2] = b;
            vProps[3] = vFaceProp[3];
            vProps[4] = vFaceProp[4];
            vProps[5] = vFaceProp[5];
            vProps[6] = vFaceProp[6];
            vProps[7] = vFaceProp[7];
            vProps[8] = vFaceProp[8];

            face2.SetMaterialPropertyValues2(vProps, 1, null);
            vProps = null;
            vFaceProp = null;
        }

        /// <summary>
        /// 判断两个面是否相邻
        /// 此函数有问题，未找到更好的办法
        /// 问题:使用ClosestDistance必须前台打开模型，否则无法计算(返回-1)
        /// </summary>
        /// <param name="face1"></param>
        /// <param name="face2"></param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsAdjacent(ModelDoc2 model, IFace2 face1, IFace2 face2)
        {
            //((Entity)face1).Select4(false,null);
            //((Entity)face2).Select4(false,null);

            //创建两个点
            object point1;
            object point2;
            //找到最近距离
            double closestDistance = model.ClosestDistance(face1, face2, out point1, out point2);
            //MessageBox.Show(closestDistance.ToString());
            if (closestDistance == -1)
            {
                //表示没有求出来
                return IsAdjacent(face1, face2);
            }
            else
            {
                //如果最近距离小于一定阈值，这是由于SolidWorks本身可能存在一定误差
                if (Math.Abs(closestDistance * 1000) < 0.1)
                {
                    //相邻
                    return true;
                }
                else
                {
                    //否则，不相邻
                    return IsAdjacent(face1, face2);
                }
            }



        }

        /// <summary>
        /// 判断两个面是否相邻
        /// </summary>
        /// <param name="face1"></param>
        /// <param name="face2"></param>
        /// <returns></returns>
        public static bool IsAdjacent(IFace2 face1, IFace2 face2)
        {
            ////List<IEdge> edges1 = DynamicCast.CastList<IEdge>(face1.GetEdges());
            ////List<IEdge> edges2 = DynamicCast.CastList<IEdge>(face2.GetEdges());
            ////foreach (var e1 in edges1)
            ////{
            ////    foreach (var e2 in edges2)
            ////    {
            ////        if (e1 == e2)
            ////        {
            ////            return true;
            ////        }
            ////    }
            ////}
            return false;
        }
    }
}
