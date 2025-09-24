using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace ParametricTool.SwUtils
{
    public static class SheetUtil
    {
        public static IEnumerable<IView> Ex_GetViews(this ISheet sheet,bool exclusideFlatPatternView = true)
        {
            var tmp = sheet.GetViews();
            IView[] views = ((object[])tmp).Select(x => x as IView).ToArray();
            List<IView> result = new List<IView>();
            foreach (var item in views)
            {
                if (item.SuppressState!=0)
                {
                    continue;
                }
                if (item.GetVisible()!=true)
                {
                    continue;
                }

                if (exclusideFlatPatternView && item.IsFlatPatternView())
                {
                    continue;
                }
                result.Add(item);
            }
            return result;
        }

        public static IEnumerable<IView> Ex_GetAllViews(this ISheet sheet)
        {
            var tmp = sheet.GetViews();
            IView[] views = ((object[])tmp).Select(x => x as IView).ToArray();
            List<IView> result = new List<IView>();
            foreach (var item in views)
            {
                result.Add(item);
                
                int type = item.Type;
                
            }
            return result;
        }

        public static IEnumerable<IView> Ex_GetFlatPatternViews(this ISheet sheet)
        {
            var tmp = sheet.GetViews();
            IView[] views = ((object[])tmp).Select(x => x as IView).ToArray();
            List<IView> result = new List<IView>();
            foreach (var item in views)
            {
                if (item.SuppressState != 0)
                {
                    continue;
                }
                if (item.GetVisible() != true)
                {
                    continue;
                }
                if (!item.IsFlatPatternView())
                {
                    continue;
                }
                result.Add(item);
            }
            return result;
        }

        public static IView Ex_GetSheetView(this ISheet sheet)
        {
            var tmp = sheet.GetViews();
            IView[] views = ((object[])tmp).Select(x => x as IView).ToArray();
            foreach (var item in views)
            {
                if (item.SuppressState != 0)
                {
                    continue;
                }
                if (item.GetVisible() != true)
                {
                    continue;
                }

                if (item.Type == 1)
                {
                    return item;
                }
            }
            return null;
        }

        public static IEnumerable<IAnnotation> Ex_GetSheetAnnos(this ISheet sheet, DrawingDoc drawingDoc)
        {
            IView view = (IView)drawingDoc.GetFirstView();
            {
                var objs = (object[])view.GetAnnotations();

                if (objs != null)
                {
                    var annos = objs.Select(x => x as IAnnotation);
                    return annos;
                }
            }

            return new List<IAnnotation>();
        }
    }
}
