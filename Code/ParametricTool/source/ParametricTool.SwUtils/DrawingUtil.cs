using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ParametricTool.SwUtils
{
    public static class DrawingUtil
    {
        public static IEnumerable<IAnnotation> Ex_GetAnnos(this IModelDoc2 drawingDoc)
        {
            List<IAnnotation> result = new List<IAnnotation>();
            IAnnotation anno = drawingDoc.GetFirstAnnotation2();
            while (anno != null)
            {
                result.Add(anno);
                anno = anno.GetNext2();
            }
            return result;
        }
    }
}
