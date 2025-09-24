using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using ParametricTool.SwUtils;
using ParametricTool.UI.Models;
using ParametricTool.UI.View;
using ParametricTool.Utils;
using ParametricTool.Utils.MvvmUtil;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swcommands;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Path = System.IO.Path;

namespace ParametricTool.UI.ViewModels
{
    public class ParametricToolViewModel : NotifyPropertyChanged
    {
        public ParametricToolWindow m_Window;

        public ParametricToolViewModel()
        {
            InitCommand = new DelegateCommand();
            InitCommand.ExcuteAction = new Action<object>(this.Init);

            SelectTemplateModelCommand = new DelegateCommand();
            SelectTemplateModelCommand.ExcuteAction = new Action<object>(this.SelectTemplateModel);

            SelectTemplateDrawingCommand = new DelegateCommand();
            SelectTemplateDrawingCommand.ExcuteAction = new Action<object>(this.SelectTemplateDrawing);

            SelectInstanceExcelCommand = new DelegateCommand();
            SelectInstanceExcelCommand.ExcuteAction = new Action<object>(this.SelectInstanceExcel);

            SelectOutputPathCommand = new DelegateCommand();
            SelectOutputPathCommand.ExcuteAction = new Action<object>(this.SelectOutputPath);

            TestCommand = new DelegateCommand();
            TestCommand.ExcuteAction = new Action<object>(this.Test);

            ApplyCommand = new DelegateCommand();
            ApplyCommand.ExcuteAction = new Action<object>(this.Apply);

            CancleCommand = new DelegateCommand();
            CancleCommand.ExcuteAction = new Action<object>(this.Cancle);
        }

        #region 绑定的属性
        #region 所选模板模型路径
        private string templateModel;
        /// <summary>
        /// 所选模板模型路径
        /// </summary>
        public string TemplateModel
        {
            get
            {
                return templateModel;
            }
            set
            {
                templateModel = value;
                OnPropertyChanged(nameof(TemplateModel));
            }
        }
        #endregion

        #region 所选模板工程图路径
        private string templateDrawing;
        /// <summary>
        /// 所选模板工程图路径
        /// </summary>
        public string TemplateDrawing
        {
            get
            {
                return templateDrawing;
            }
            set
            {
                templateDrawing = value;
                OnPropertyChanged(nameof(TemplateDrawing));
            }
        }
        #endregion

        #region 所选实例表格路径
        private string instanceExcel;
        /// <summary>
        /// 所选实例表格路径
        /// </summary>
        public string InstanceExcel
        {
            get
            {
                return instanceExcel;
            }
            set
            {
                instanceExcel = value;
                OnPropertyChanged(nameof(InstanceExcel));
            }
        }
        #endregion

        #region 所选输出路径
        private string outputPath;
        /// <summary>
        /// 所选输出路径
        /// </summary>
        public string OutputPath
        {
            get
            {
                return outputPath;
            }
            set
            {
                outputPath = value;
                OnPropertyChanged(nameof(OutputPath));
            }
        }
        #endregion

        #region 所有需要处理的实例
        private ModelItemCollection modelItemCollection = new ModelItemCollection();
        public ModelItemCollection ModelItemCollection
        {
            get
            {
                return modelItemCollection;
            }
            set
            {
                modelItemCollection = value;
                OnPropertyChanged(nameof(ModelItemCollection));
            }
        }
        #endregion
        #endregion

        #region 绑定的动作
        #region 窗口弹出后初始化动作
        public DelegateCommand InitCommand { get; set; }
        public void Init(object para)
        {
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("初始化异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("初始化异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 选择模板模型路径
        public DelegateCommand SelectTemplateModelCommand { get; set; }
        public void SelectTemplateModel(object para)
        {
            try
            {
                SelectTemplateModel();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("选择模板模型路径异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("选择模板模型路径异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 选择模板工程图路径
        public DelegateCommand SelectTemplateDrawingCommand { get; set; }
        public void SelectTemplateDrawing(object para)
        {
            try
            {
                SelectTemplateDrawing();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("选择模板工程图路径异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("选择模板工程图路径异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 选择实例表格路径
        public DelegateCommand SelectInstanceExcelCommand { get; set; }
        public void SelectInstanceExcel(object para)
        {
            try
            {
                SelectInstanceExcel();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("选择实例表格路径异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("选择实例表格路径异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 选择输出路径
        public DelegateCommand SelectOutputPathCommand { get; set; }
        public void SelectOutputPath(object para)
        {
            try
            {
                SelectOutputPath();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("选择输出路径异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("选择输出路径异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 测试
        public DelegateCommand TestCommand { get; set; }
        public void Test(object para)
        {
            try
            {
                Test();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("测试异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("测试异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 生成实例
        public DelegateCommand ApplyCommand { get; set; }
        public void Apply(object para)
        {
            try
            {
                Apply();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("生成实例异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("生成实例异常:" + ex.Message);
                }

            }
        }
        #endregion

        #region 取消
        public DelegateCommand CancleCommand { get; set; }
        public void Cancle(object para)
        {
            try
            {
                Cancle();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                if (ParametricTool.Common.Constants.TestMode)
                {
                    MessageBox.Show("取消异常:" + ex.ToString());
                }
                else
                {
                    MessageBox.Show("取消异常:" + ex.Message);
                }

            }
        }
        #endregion
        #endregion

        #region 自定义动作
        private void Init()
        {
            InitDataGrid();
            if (Common.Constants.TestMode)
            {
                TemplateModel = @"G:\BaiduSyncdisk\01.Projects\08.ParametricTool\workdir\temp\新建文件夹 (2)\风口包边法兰.SLDPRT";
                TemplateDrawing = @"G:\BaiduSyncdisk\01.Projects\08.ParametricTool\workdir\temp\新建文件夹 (2)\风口包边法兰.SLDDRW";
                InstanceExcel = @"G:\BaiduSyncdisk\01.Projects\08.ParametricTool\workdir\temp\E1-风口包边法兰 OK - 副本.xlsx";
                OutputPath = @"G:\BaiduSyncdisk\01.Projects\08.ParametricTool\workdir\temp\新建文件夹 (2)";
                InitModelItemCollection(InstanceExcel);
                FillDataGridColumns();
            }
        }

        private void SelectTemplateModel()
        {
            string tmp = FileUtil.SelectSingleFile("请选择模板模型", "Sw零件模型|*.sldprt");
            if (tmp != null)
            {
                TemplateModel = tmp;
            }
        }

        private void SelectTemplateDrawing()
        {
            string tmp = FileUtil.SelectSingleFile("请选择工程图模型", "Sw工程图模型|*.slddrw");
            if (tmp != null)
            {
                TemplateDrawing = tmp;
            }
        }

        private void SelectInstanceExcel()
        {
            string tmp = FileUtil.SelectSingleFile("请选择实例表格", "Excel文件|*.xlsx;*.xls");
            if (tmp != null)
            {
                #region 初始化ModelItems
                InitModelItemCollection(tmp);
                #endregion

                #region 初始化DataGrid
                InitDataGrid();
                FillDataGridColumns();
                #endregion

                InstanceExcel = tmp;
            }
        }

        private void InitModelItemCollection(string tmp)
        {
            DataTable dt = ExcelUtil.RenderDataTableFromExcel(tmp, 0, 0);
            ModelItemCollection = new ModelItemCollection();
            //第一行为序号，不用管
            //第二行固定为模型名称(界面上显示为物料号)
            for (int i = 2; i < dt.Columns.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(dt.Columns[i].ColumnName))
                {
                    continue;
                }
                if (dt.Columns[i].ColumnName == "是否需要出图")
                {
                    continue;
                }
                if (dt.Columns[i].ColumnName.ToLower().StartsWith("column"))
                {
                    continue;
                }
                ModelItemCollection.PropTitles.Add(dt.Columns[i].ColumnName);
            }

            int index = 1;
            foreach (DataRow row in dt.Rows)
            {
                ModelItem modelItem = new ModelItem();
                modelItem.Number = row[1]?.ToString();
                if (string.IsNullOrWhiteSpace(modelItem.Number))
                {
                    continue;
                }
                string needDwg = row["是否需要出图"]?.ToString();
                if (needDwg == "OK")
                {
                    modelItem.NeedDrawing = true;
                }
                else
                {
                    modelItem.NeedDrawing = false;
                }
                foreach (var title in ModelItemCollection.PropTitles)
                {
                    modelItem.PropValues.Add(row[title]?.ToString());
                }
                modelItem.Index = index;
                index++;
                ModelItemCollection.Items.Add(modelItem);
            }
        }

        private void SelectOutputPath()
        {
            string tmp = FolderUtil.SelectSingleFolder("请选择输出路径");
            if (tmp != null)
            {
                OutputPath = tmp;
            }
        }

        private void Apply()
        {
            #region 校验输入参数
            if (string.IsNullOrWhiteSpace(TemplateModel))
            {
                throw new Exception("请选择模板模型路径!");
            }
            if (string.IsNullOrWhiteSpace(InstanceExcel))
            {
                throw new Exception("请选择实例表格路径!");
            }
            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                throw new Exception("请选择输出路径!");
            }
            if (!File.Exists(TemplateModel))
            {
                throw new Exception("模板模型不存在，请重新选择!");
            }
            if (!File.Exists(InstanceExcel))
            {
                throw new Exception("实例表格不存在，请重新选择!");
            }
            if (!Directory.Exists(OutputPath))
            {
                try
                {
                    Directory.CreateDirectory(OutputPath);
                }
                catch (Exception ex)
                {
                    throw new Exception("创建输入路径失败(" + OutputPath + "):" + ex.Message);
                }

            }
            WindowState state = m_Window.WindowState;
            m_Window.WindowState = WindowState.Minimized;
            #endregion
            foreach (var item in ModelItemCollection.Items)
            {
                try
                {
                    Trace.WriteLine("开始创建:【" + item.Number + "】");
                    CreateItem(item);
                    item.IsSuccess = true;
                    item.ErrorMsg = "";
                    Trace.WriteLine("创建结束:【" + item.Number + "】");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("创建出错:【" + item.Number + "】" + "-->" + ex);
                    item.IsSuccess = false;
                    item.ErrorMsg = ex.Message;
                }
            }
            m_Window.WindowState = state;
            MessageBox.Show("生成结束!");
        }

        private void Test()
        {
            ModelDoc2 modelDoc = Constants.iSwApp.IActiveDoc2;
            DrawingDoc drawingDoc = modelDoc as DrawingDoc;

            List<Annotation> annotations = GetAnnotations(modelDoc);
            object tmp = drawingDoc.GetCurrentSheet();
            Sheet sheet = tmp as Sheet;
            IView[] views = sheet.Ex_GetViews().ToArray();
            Dictionary<IView, List<INote>> notes = new Dictionary<IView, List<INote>>();
            foreach (var view in views)
            {
                object[] tmp1 = (object[])view.GetNotes();
                if (tmp1 != null)
                {
                    INote[] tmpNotes = tmp1.Select(x => x as INote).ToArray();
                    if (tmpNotes != null)
                    {
                        notes.Add(view, tmpNotes.ToList());
                    }
                    else
                    {
                        notes.Add(view, new List<INote>());
                    }
                }
                else
                {
                    notes.Add(view, new List<INote>());
                }
            }


        }

        private void Cancle()
        {
            m_Window.Close();
        }
        #endregion

        #region 自定义方法
        public void InitDataGrid()
        {
            m_Window.m_DataGrid_ModelItems.Columns.Clear();
            m_Window.m_DataGrid_ModelItems.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "序号",
                    IsReadOnly = true,
                    Binding = new Binding(nameof(ModelItem.Index))
                });
            m_Window.m_DataGrid_ModelItems.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "物料号",
                    IsReadOnly = true,
                    Binding = new Binding(nameof(ModelItem.Number))
                });
            m_Window.m_DataGrid_ModelItems.Columns.Add(
                new DataGridCheckBoxColumn
                {
                    Header = "是否出图",
                    IsReadOnly = true,
                    Binding = new Binding(nameof(ModelItem.NeedDrawing))
                });
            m_Window.m_DataGrid_ModelItems.Columns.Add(
                new DataGridCheckBoxColumn
                {
                    Header = "是否完成",
                    IsReadOnly = true,
                    Binding = new Binding(nameof(ModelItem.IsSuccess)),
                });
            m_Window.m_DataGrid_ModelItems.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "错误信息",
                    IsReadOnly = true,
                    Binding = new Binding(nameof(ModelItem.ErrorMsg)),
                });
        }

        public void FillDataGridColumns()
        {
            int startColumn = 2;
            for (int i = 0; i < ModelItemCollection.PropTitles.Count; i++)
            {
                m_Window.m_DataGrid_ModelItems.Columns.Insert(startColumn,
                new DataGridTextColumn
                {
                    Header = ModelItemCollection.PropTitles[i],
                    IsReadOnly = true,
                    Binding = new Binding(nameof(ModelItem.PropValues) + "[" + i + "]")
                });
                startColumn++;
            }
        }

        public void CreateItem(ModelItem item)
        {
            //1.拷贝文件
            string templateFileExtension = Path.GetExtension(TemplateModel);
            item.ModelFile = Path.GetFullPath(Path.Combine(OutputPath, item.Number + templateFileExtension));
            item.DrawingFile1 = Path.GetFullPath(Path.Combine(OutputPath, item.Number + "-1.SLDDRW"));
            item.DrawingFile2 = Path.GetFullPath(Path.Combine(OutputPath, item.Number + "-1.dwg"));
            item.DrawingFile3 = Path.GetFullPath(Path.Combine(OutputPath, item.Number + "-1.pdf"));
            item.DrawingFile4 = Path.GetFullPath(Path.Combine(OutputPath, item.Number + "-2.dwg"));
            if (item.NeedDrawing)
            {
                Constants.iSwApp.CopyDocument(TemplateDrawing,item.DrawingFile1, new string[] {TemplateModel },new string[] { item.ModelFile},3);
            }
            else
            {
                File.Copy(TemplateModel, item.ModelFile, true);
            }
            //2.打开文件
            double[] border = new double[] { 0,0,0,0};
            List<Tuple<string, double[]>> notePoses = new List<Tuple<string, double[]>>();
            ModelDoc2 drawingDoc = null;
            if (item.NeedDrawing)
            {
                drawingDoc = FileOperation.OpenDoc(item.DrawingFile1, true);
                Constants.iSwApp.ActivateDoc(item.DrawingFile1);
                object tmp = (drawingDoc as DrawingDoc).GetCurrentSheet();
                Sheet sheet = tmp as Sheet;
                IView[] views = sheet.Ex_GetViews().ToArray();

                List<double[]> outlines = views.Select(x => (double[])x.GetOutline()).ToList();
                double borderXMin = outlines.Select(x => x[0]).Min() * 1000;
                double borderXMax = outlines.Select(x => x[2]).Max() * 1000;
                double borderYMin = outlines.Select(x => x[1]).Min() * 1000;
                double borderYMax = outlines.Select(x => x[3]).Max() * 1000;
                border[0] = borderXMin;
                border[1] = borderYMin;
                border[2] = borderXMax;
                border[3] = borderYMax;

                var annos = sheet.Ex_GetSheetAnnos(drawingDoc as DrawingDoc).ToList();
                foreach (var anno in annos)
                {
                    if (anno.GetType() == 6)
                    {
                        INote note = anno.GetSpecificAnnotation();
                        string text = note.GetText();
                        double[] pos = (double[])anno.GetPosition();
                        notePoses.Add(new Tuple<string, double[]>(text, pos));
                    }
                }
            }
            ModelDoc2 modelDoc = FileOperation.OpenDoc(item.ModelFile, true);
            Constants.iSwApp.ActivateDoc(item.ModelFile);
            item.ModelDoc = modelDoc;
            //3.设置参数
            ModelDocExtension swModDocExt = modelDoc.Extension;
            CustomPropertyManager swCustPropMgr = swModDocExt.CustomPropertyManager[""];
            List<string> existNames = new List<string>();
            object tmpNames = swCustPropMgr.GetNames();
            if (tmpNames != null)
            {
                existNames = ((string[])tmpNames).ToList();
            }
            for (int i = 0; i < ModelItemCollection.PropTitles.Count; i++)
            {
                string title = ModelItemCollection.PropTitles[i];
                string value = item.PropValues[i];
                int propType = (int)swCustomInfoType_e.swCustomInfoText;
                if (existNames.Contains(title))
                {
                    propType = swCustPropMgr.GetType2(title);
                    if (propType == (int)swCustomInfoType_e.swCustomInfoNumber)
                    {
                        propType = (int)swCustomInfoType_e.swCustomInfoDouble;
                    }
                }
                swCustPropMgr.Add3(title, propType, value, (int)swCustomPropertyAddOption_e.swCustomPropertyReplaceValue);
            }
            
            bool editRebuild3Result = modelDoc.EditRebuild3();
            Trace.WriteLine("EditRebuild3-->" + editRebuild3Result);
            bool forceRebuild33Result = modelDoc.ForceRebuild3(false);
            Trace.WriteLine("ForceRebuild3-->" + forceRebuild33Result);
            bool rebuildResult = modelDoc.Extension.Rebuild((int)swRebuildOptions_e.swForceRebuildAll);
            Trace.WriteLine("Rebuild-->" + rebuildResult);
            modelDoc.GraphicsRedraw2();
            
            modelDoc.ViewZoomtofit2();

            //4.保存
            FileOperation.SaveDoc(modelDoc);
            if (item.NeedDrawing)
            {
                FileOperation.SaveDoc(drawingDoc);
            }
            //5.工程图
            if (item.NeedDrawing)
            {
                var drawing = CreateDrawing(item, border, notePoses);
                
            }
            //关闭
            FileOperation.CloseDoc(drawingDoc,true);
            FileOperation.CloseDoc(modelDoc,false);
        }
        public DrawingDoc CreateDrawing(ModelItem item,double[] border, List<Tuple<string, double[]>> notePoses)
        {
            DrawingDoc drawingDoc = Constants.iSwApp.ActivateDoc(item.DrawingFile1) as DrawingDoc;
            CreateFoldDrawing(item, border, notePoses);
            Constants.iSwApp.ActivateDoc(item.ModelFile);
            CreateUnFoldDrawing(item);
            return drawingDoc;
        }

        public void CreateFoldDrawing(ModelItem item, double[] border, List<Tuple<string, double[]>> notePoses)
        {
            


            ModelDoc2 modelDoc = Constants.iSwApp.IActiveDoc2;
            DrawingDoc drawingDoc = modelDoc as DrawingDoc;
            object tmp = drawingDoc.GetCurrentSheet();
            Sheet sheet = tmp as Sheet;
            IView[] views = sheet.Ex_GetViews().ToArray();
            Dictionary<IView, List<INote>> notes = new Dictionary<IView, List<INote>>();
            
            foreach (var view in views)
            {
                object[] tmp1 = (object[])view.GetNotes();
                if (tmp1!=null)
                {
                    INote[] tmpNotes = tmp1.Select(x=>x as INote).ToArray();
                    if (tmpNotes != null)
                    {
                        notes.Add(view, tmpNotes.ToList());
                    }
                    else
                    {
                        notes.Add(view, new List<INote>());
                    }
                }
                else
                {
                    notes.Add(view, new List<INote>());
                }
            }
            List<INote> removeAnnos = new List<INote>();
            foreach (var viewnotes in notes)
            {
                foreach (var note in viewnotes.Value)
                {
                    if (string.IsNullOrWhiteSpace(note.GetText()))
                    {
                        removeAnnos.Add(note);
                    }
                }
            }
            double scale = ArrangeView(views, border);
            double[] props = (double[])sheet.GetProperties();
            sheet.SetProperties((int)props[0], (int)props[1], scale, 1, true, (int)props[5], (int)props[6]);
            IView[] flatPatternViews = sheet.Ex_GetFlatPatternViews().ToArray();
            foreach (var flatPatternView in flatPatternViews)
            {
                flatPatternView.ScaleRatio = new double[] { scale, 1 };
                flatPatternView.UpdateViewDisplayGeometry();
            }

            {
                var annos = sheet.Ex_GetSheetAnnos(drawingDoc).ToList();
                foreach (var anno in annos)
                {
                    if (anno.GetType() == 6)
                    {
                        INote note = anno.GetSpecificAnnotation();
                        string text = note.GetText();
                        var notePos = notePoses.FirstOrDefault(x => x.Item1 == text);
                        if (notePos != null)
                        {
                            anno.SetPosition2(notePos.Item2[0], notePos.Item2[1], notePos.Item2[2]);
                        }
                    }
                }
            }

            foreach (var view in views)
            {
                object[] tmp1 = (object[])view.GetNotes();
                if (tmp1 != null)
                {
                    INote[] tmpNotes = tmp1.Select(x => x as INote).ToArray();
                    if (tmpNotes != null)
                    {
                        foreach (INote note in tmpNotes)
                        {
                            string var = note.GetText();
                            if (!notes[view].Contains(note))
                            {
                                removeAnnos.Add(note);
                            }
                        }
                    }
                }
            }
            removeAnnos = removeAnnos.Distinct().ToList();
            if (removeAnnos.Any())
            {
                modelDoc.ClearSelection2(true);
                foreach (var anno in removeAnnos)
                {
                    anno.IGetAnnotation().Select3(true,null);
                }
                modelDoc.EditDelete();
            }

            SaveAsDwg(drawingDoc, item.DrawingFile2);
            SaveAsPdf(drawingDoc, item.DrawingFile3);
        }

        public List<Annotation> GetAnnotations(ModelDoc2 model)
        {
            List<Annotation> annotations = new List<Annotation>();
            Annotation anno = (Annotation)model.GetFirstAnnotation2();
            while (anno!=null)
            {
                annotations.Add(anno);
                anno = anno.GetNext3();
            }
            return annotations;
        }

        public void CreateUnFoldDrawing(ModelItem item)
        {
            //找到主视图，并摆正
            Face2 mainFace = GetMainFace(item.ModelDoc);
            if (mainFace == null)
            {
                throw new Exception("未找到主视图参考面!");
            }
            //展开
            UnFoldModel(item.ModelDoc);

            ISurface surface = mainFace.GetSurface();
            item.ModelDoc.ClearSelection2(true);
            (mainFace as Entity).Select4(false, null);
            //正视于选择的面
            item.ModelDoc.Extension.RunCommand((int)swCommands_e.swCommands_View_Normal_To, "");
            IDrawingDoc drawDoc = Constants.iSwApp.NewDocument(Common.Constants.DrawingTemplate, 0, 0, 0) as IDrawingDoc;
            if (drawDoc == null)
            {
                throw new Exception("未能成功创建工程图文件:" + Common.Constants.DrawingTemplate);
            }
            ModelDoc2 drawModel = (drawDoc as ModelDoc2);

            //插入平板视图
            SolidWorks.Interop.sldworks.View flatView = drawDoc.CreateFlatPatternViewFromModelView3(item.ModelFile, "默认", 0, 0, 0, true, false);
            flatView.ShowSheetMetalBendNotes = false;
            CleanView(drawModel, flatView);
            flatView.UpdateViewDisplayGeometry();

            SaveAsDwg(drawDoc, item.DrawingFile4);
            FileOperation.CloseDoc(drawModel);
        }

        public void CleanView(ModelDoc2 drawModel, SolidWorks.Interop.sldworks.View view)
        {
            CleanViewAnnotations(drawModel, view);
        }

        public void CleanViewAnnotations(ModelDoc2 drawModel, SolidWorks.Interop.sldworks.View view)
        {
            drawModel.ClearSelection2(true);
            object[] annos = view.GetAnnotations();
            if (annos!=null && annos.Length>0)
            {
                foreach (var item in annos)
                {
                    Annotation anno = item as Annotation;
                    anno.Select3(true,null);
                }
                drawModel.EditDelete();
            }
        }

        public void UnFoldModel(ModelDoc2 modelDoc)
        {
            //modelDoc.SetBendState(2);
            //modelDoc.EditRebuild3();
        }

        public Face2 GetMainFace(ModelDoc2 modelDoc)
        {
            IList<Face2> faces = FaceUtil.GetFaces(modelDoc);
            if (faces == null)
            {
                return null;
            }
            foreach (Face2 face in faces)
            {
                Entity entity = face as Entity;
                string name = entity.ModelName;
                if (name != null && name == "主视图")
                {
                    ISurface surface = face.GetSurface();
                    if (surface.IsPlane())
                    {
                        return face;
                    }
                    else
                    {
                        throw new Exception("主视图参考面设置为了非平面!");
                    }
                }
            }
            return null;
        }


        public void SaveAsDwg(IDrawingDoc drawingDoc, string dwgFile)
        {
            if (File.Exists(dwgFile))
            {
                File.Delete(dwgFile);
            }

            ModelDoc2 modelDoc = drawingDoc as ModelDoc2;
            modelDoc.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swViewDisplayHideAllTypes, true);
            modelDoc.EditRebuild3();
            modelDoc.WindowRedraw();
            Constants.iSwApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfOutputNoScale, 1);
            Constants.iSwApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDxfVersion, (int)swDxfFormat_e.swDxfFormat_R14);
            modelDoc.SaveAs3(dwgFile, 0, 0);
        }

        public void SaveAsPdf(IDrawingDoc drawingDoc, string pdfFile)
        {
            if (File.Exists(pdfFile))
            {
                File.Delete(pdfFile);
            }

            ModelDoc2 modelDoc = drawingDoc as ModelDoc2;
            modelDoc.SaveAs3(pdfFile, 0, 0);
        }

        #endregion

        public static double ArrangeView(IView[] views, double[] border)
        {
            foreach (var item in views)
            {
                item.UpdateViewDisplayGeometry();
            }
            List<double[]> postions = views.Select(x => (double[])x.Position).ToList();
            IView topView = views[postions.Select(x => x[1]).ToList().IndexOf(postions.Select(x => x[1]).Min())];
            IView leftView = views[postions.Select(x => x[0]).ToList().IndexOf(postions.Select(x => x[0]).Max())];
            IView frontView = null; ;
            foreach (var item in views)
            {
                if (item == topView)
                {
                    continue;
                }
                if (item == leftView)
                {
                    continue;
                }
                frontView = item;
                break;
            }
            //PageSetup ps = topView.Sheet.PageSetup;
            //double sheetScale = ps.Scale2;
            //List<double[]> outlines = views.Select(x => (double[])x.GetOutline()).ToList();
            double borderXMin = border[0];
            double borderXMax = border[2];
            double borderYMin = border[1];
            double borderYMax = border[3];
            double viewGap = 25;
            bool retainGapViewOnEdge = false;
            List<double> candidateScales = null;
            AxonometricLocation axonometricLocation = AxonometricLocation.RightBottom;
            AlignMethod alignMethod = AlignMethod.ByBorder;
            Perspective perspective = Perspective.First;
            
            

            ViewPara frontViewPara = GetViewPara(frontView);
            ViewPara leftViewPara = GetViewPara(leftView);
            ViewPara rightViewPara = GetViewPara(null);
            ViewPara topViewPara = GetViewPara(topView);
            ViewPara backViewPara = GetViewPara(null);
            ViewPara bottomViewPara = GetViewPara(null);
            ViewPara axonometricViewPara = GetViewPara(null);
            List<ViewPara> allViewParas = new List<ViewPara>();
            allViewParas.Add(frontViewPara);
            allViewParas.Add(leftViewPara);
            allViewParas.Add(rightViewPara);
            allViewParas.Add(topViewPara);
            allViewParas.Add(backViewPara);
            allViewParas.Add(bottomViewPara);
            allViewParas.Add(axonometricViewPara);
            


            DrawViewArrange drawViewArrange = new DrawViewArrange(borderXMax - borderXMin, borderYMax - borderYMin, viewGap, retainGapViewOnEdge, candidateScales, axonometricLocation, alignMethod, perspective);
            drawViewArrange.FrontView = frontViewPara;
            drawViewArrange.LeftView = leftViewPara;
            drawViewArrange.RightView = rightViewPara;
            drawViewArrange.TopView = topViewPara;
            drawViewArrange.BackView = backViewPara;
            drawViewArrange.BottomView = bottomViewPara;
            drawViewArrange.AxonometricView = axonometricViewPara;
            drawViewArrange.Computer();

            List<double[]> poses = views.Select(x=> (double[])x.Position).ToList();
            List<double[]> outlineses = views.Select(x=> (double[])x.GetOutline()).ToList();


            foreach (var item in allViewParas)
            {
                if (item != null)
                {
                    IView view = item.View as IView;
                    double[] center = new double[] { (item.CenterX + borderXMin) / 1000, (item.CenterY + borderYMin) / 1000 };
                    //int i = views.ToList().IndexOf(view);
                    //double[] pos = poses[i];
                    //double[] outlines = outlineses[i];
                    //double xCenter = (outlines[2] + outlines[0]) / 2;
                    //double yCenter = (outlines[3] + outlines[1]) / 2;
                    //double xGap = center[0] - xCenter;
                    //double yGap = center[1] - yCenter;

                    //view.Position = new double[] { pos[0] + xGap, pos[1] + yGap };
                    view.ScaleRatio = new double[] { drawViewArrange.Scale, 1 };
                    MoveViewByCenter(view, center);
                    
                    view.UpdateViewDisplayGeometry();
                }
            }
            return drawViewArrange.Scale;
        }

        public static ViewPara GetViewPara(IView view)
        {
            if (view == null)
            {
                return null;
            }
            
            double scale = view.ScaleRatio[0] / view.ScaleRatio[1];
            double[] outlines = (double[])view.GetOutline();
            double tmpWidth = outlines[2] - outlines[0];
            double tmpHeight = outlines[3] - outlines[1];
            return new ViewPara(tmpWidth * 1000 / scale, tmpHeight * 1000 / scale, view);
        }

        public static void MoveViewByCenter(IView view,double[] center)
        {
            double[] pos = (double[])view.Position;
            double[] outlines = (double[])view.GetOutline();
            double xCenter = (outlines[2] + outlines[0])/2;
            double yCenter = (outlines[3] + outlines[1])/2;

            double xGap = center[0] - xCenter;
            double yGap = center[1] - yCenter;

            view.Position = new double[] { pos[0] + xGap, pos[1] + yGap };
        }
    }
}
