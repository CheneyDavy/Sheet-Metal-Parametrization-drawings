using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Net.Http;
using System.IO.Pipes;

namespace ParametricTool.Utils
{
    public class ExcelUtil
    {

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>
        /// <param name="SheetName">要获取数据的工作表名称</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string strFileName, string SheetName, int HeaderRowIndex)
        {
            IWorkbook workbook = null;
            try
            {
                workbook = WorkbookFactory.Create(strFileName);
                return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close();
                }
            }

        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="strFileName">Excel文件全路径(服务器路径)</param>
        /// <param name="SheetIndex">要获取数据的工作表序号(从0开始)</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(string strFileName, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook = null;
            try
            {
                using (FileStream fileStream = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
                {
                    return RenderDataTableFromExcel(fileStream, SheetIndex, HeaderRowIndex);
                }


                //workbook = WorkbookFactory.Create(strFileName);
                //string SheetName = workbook.GetSheetName(SheetIndex);
                //return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
            }
            finally
            {
                //if (workbook != null)
                //{
                //    workbook.Close();
                //}
            }


        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="ExcelFileStream">Excel文件流</param>
        /// <param name="SheetName">要获取数据的工作表名称</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, string SheetName, int HeaderRowIndex)
        {
            //IWorkbook workbook = new HSSFWorkbook(ExcelFileStream);
            IWorkbook workbook = WorkbookFactory.Create(ExcelFileStream);
            ExcelFileStream.Close();
            return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="ExcelFileStream">Excel文件流</param>
        /// <param name="SheetIndex">要获取数据的工作表序号(从0开始)</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(Stream ExcelFileStream, int SheetIndex, int HeaderRowIndex)
        {
            IWorkbook workbook = WorkbookFactory.Create(ExcelFileStream);
            ExcelFileStream.Close();
            string SheetName = workbook.GetSheetName(SheetIndex);
            return RenderDataTableFromExcel(workbook, SheetName, HeaderRowIndex);
        }

        /// <summary>
        /// 从Excel中获取数据到DataTable
        /// </summary>
        /// <param name="workbook">要处理的工作薄</param>
        /// <param name="SheetName">要获取数据的工作表名称</param>
        /// <param name="HeaderRowIndex">工作表标题行所在行号(从0开始)</param>
        /// <returns></returns>
        public static DataTable RenderDataTableFromExcel(IWorkbook workbook, string SheetName, int HeaderRowIndex)
        {
            ISheet sheet = workbook.GetSheet(SheetName);
            //sheet.ForceFormulaRecalculation = true;
            DataTable table = new DataTable();
            try
            {
                IRow headerRow = sheet.GetRow(HeaderRowIndex);
                int cellCount = headerRow.LastCellNum;

                for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                    table.Columns.Add(column);
                }

                int rowCount = sheet.LastRowNum;

                #region 循环各行各列,写入数据到DataTable
                for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    DataRow dataRow = table.NewRow();
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        ICell cell = row.GetCell(j);
                        dataRow[j] = GetCellValue(cell);
                    }
                    table.Rows.Add(dataRow);
                    //dataRow[j] = row.GetCell(j).ToString();
                }
                #endregion
            }
            catch (System.Exception ex)
            {
                table.Clear();
                table.Columns.Clear();
                table.Columns.Add("出错了");
                DataRow dr = table.NewRow();
                dr[0] = ex.Message;
                table.Rows.Add(dr);
                return table;
            }
            finally
            {
                //sheet.Dispose();
                workbook = null;
                sheet = null;
            }
            #region 清除最后的空行
            for (int i = table.Rows.Count - 1; i > 0; i--)
            {
                bool isnull = true;
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (table.Rows[i][j] != null)
                    {
                        if (table.Rows[i][j].ToString() != "")
                        {
                            isnull = false;
                            break;
                        }
                    }
                }
                if (isnull)
                {
                    table.Rows[i].Delete();
                }
            }
            #endregion
            return table;
        }


        /// <summary>
        /// 读取合并单元格的值
        /// </summary>
        /// <param name="cell">查询的单元格</param>
        /// <returns>返回有数值的单元格</returns>
        private static ICell GetMergedCell(ICell cell)
        {
            if (cell.IsMergedCell)//是否是合并单元格
            {
                for (int i = 0; i < cell.Sheet.NumMergedRegions; i++)//遍历所有的合并单元格
                {
                    var cellRange = cell.Sheet.GetMergedRegion(i);
                    if (cell.ColumnIndex >= cellRange.FirstColumn && cell.ColumnIndex <= cellRange.LastColumn
                        && cell.RowIndex >= cellRange.FirstRow && cell.RowIndex <= cellRange.LastRow)//判断查询的单元格是否在合并单元格内
                    {
                        return cell.Sheet.GetRow(cellRange.FirstRow).GetCell(cellRange.FirstColumn);
                    }
                }
            }
            return cell;
        }

        /// <summary>
        /// 读取合并单元格的值
        /// </summary>
        /// <param name="cell">查询的单元格</param>
        /// <returns>返回有数值的单元格</returns>
        private static object GetCellValue(ICell cell)
        {
            if (cell == null)
            {
                return null;
            }
            else
            {
                cell = GetMergedCell(cell);
                switch (cell.CellType)
                {
                    case CellType.Blank:
                        return null;
                    case CellType.Boolean:
                        return cell.BooleanCellValue;
                    case CellType.Numeric:
                        return cell.NumericCellValue;
                    case CellType.String:
                        return cell.StringCellValue;
                    case CellType.Error:
                        return cell.ErrorCellValue;
                    case CellType.Formula:
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.Unknown:
                                return null;
                            case CellType.Numeric:
                                return cell.NumericCellValue;
                            case CellType.String:
                                return cell.StringCellValue;
                            case CellType.Formula:
                                return "=" + cell.CellFormula;
                            case CellType.Blank:
                                return null;
                            case CellType.Boolean:
                                return cell.BooleanCellValue;
                            case CellType.Error:
                                return cell.ErrorCellValue;
                            default:
                                return null;
                        }
                    default:
                        return "=" + cell.CellFormula;
                }
            }
        }
    }
}
