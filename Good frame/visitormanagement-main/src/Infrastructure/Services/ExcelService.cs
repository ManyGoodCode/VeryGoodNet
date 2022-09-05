using Microsoft.Extensions.Localization;
using System.Data;
using ClosedXML.Excel;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using CleanArchitecture.Blazor.Application.Common.Models;

namespace CleanArchitecture.Blazor.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IStringLocalizer<ExcelService> localizer;

        public ExcelService(IStringLocalizer<ExcelService> localizer)
        {
            this.localizer = localizer;
        }

        public async Task<byte[]> CreateTemplateAsync(IEnumerable<string> fields, string sheetName = "Sheet1")
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Properties.Author = "";
                IXLWorksheet ws = workbook.Worksheets.Add(sheetName);
                int colIndex = 1;
                int rowIndex = 1;
                foreach (string header in fields)
                {
                    IXLCell cell = ws.Cell(rowIndex, colIndex);
                    IXLFill fill = cell.Style.Fill;
                    fill.PatternType = XLFillPatternValues.Solid;
                    fill.SetBackgroundColor(XLColor.LightBlue);
                    IXLBorder border = cell.Style.Border;
                    border.BottomBorder = XLBorderStyleValues.Thin;
                    cell.Value = header;
                    colIndex++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    return await Task.FromResult(stream.ToArray());
                }
            }
        }

        public async Task<byte[]> ExportAsync<TData>(
            IEnumerable<TData> data,
            Dictionary<string, Func<TData, object>> mappers,
            string sheetName = "Sheet1")
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                workbook.Properties.Author = string.Empty;
                IXLWorksheet ws = workbook.Worksheets.Add(sheetName);
                int colIndex = 1;
                int rowIndex = 1;
                List<string> headers = mappers.Keys.Select(x => x).ToList();
                foreach (string header in headers)
                {
                    IXLCell cell = ws.Cell(rowIndex, colIndex);
                    IXLFill fill = cell.Style.Fill;
                    fill.PatternType = XLFillPatternValues.Solid;
                    fill.SetBackgroundColor(XLColor.LightBlue);
                    IXLBorder border = cell.Style.Border;
                    border.BottomBorder = border.BottomBorder = XLBorderStyleValues.Thin;
                    cell.Value = header;
                    colIndex++;
                }

                List<TData> dataList = data.ToList();
                foreach (TData item in dataList)
                {
                    colIndex = 1;
                    rowIndex++;
                    IEnumerable<object> result = headers.Select(header => mappers[header](item));
                    foreach (object value in result)
                    {
                        ws.Cell(rowIndex, colIndex++).Value = value;
                    }
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    return await Task.FromResult(stream.ToArray());
                }
            }
        }

        public async Task<IResult<IEnumerable<TEntity>>> ImportAsync<TEntity>(byte[] data, Dictionary<string, Func<DataRow, TEntity, object>> mappers, string sheetName = "Sheet1")
        {

            using (var workbook = new XLWorkbook(new MemoryStream(data)))
            {
                if (!workbook.Worksheets.Contains(sheetName))
                {
                    return await Result<IEnumerable<TEntity>>.FailureAsync(new string[] { string.Format(_localizer["Sheet with name {0} does not exist!"], sheetName) });
                }
                var ws = workbook.Worksheet(sheetName);
                var dt = new DataTable();
                var titlesInFirstRow = true;

                foreach (var firstRowCell in ws.Range(1, 1, 1, ws.LastCellUsed().Address.ColumnNumber).Cells())
                {
                    dt.Columns.Add(titlesInFirstRow ? firstRowCell.GetString() : $"Column {firstRowCell.Address.ColumnNumber}");
                }
                var startRow = titlesInFirstRow ? 2 : 1;
                var headers = mappers.Keys.Select(x => x).ToList();
                var errors = new List<string>();
                foreach (var header in headers)
                {
                    if (!dt.Columns.Contains(header))
                    {
                        errors.Add(string.Format(localizer["Header '{0}' does not exist in table!"], header));
                    }
                }
                if (errors.Any())
                {
                    return await Result<IEnumerable<TEntity>>.FailureAsync(errors);
                }
                var lastrow = ws.LastRowUsed();
                var list = new List<TEntity>();
                foreach (IXLRow row in ws.Rows(startRow, lastrow.RowNumber()))
                {
                    try
                    {
                        DataRow datarow = dt.Rows.Add();
                        var item = (TEntity)Activator.CreateInstance(typeof(TEntity));
                        foreach (IXLCell cell in row.Cells())
                        {
                            if (cell.DataType == XLDataType.DateTime)
                            {
                                datarow[cell.Address.ColumnNumber - 1] = cell.GetDateTime().ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                datarow[cell.Address.ColumnNumber - 1] = cell.Value.ToString();
                            }
                        }
                        headers.ForEach(x => mappers[x](datarow, item));
                        list.Add(item);
                    }
                    catch (Exception e)
                    {
                        return await Result<IEnumerable<TEntity>>.FailureAsync(new string[] { string.Format(_localizer["Sheet name {0}:{1}"], sheetName, e.Message) });
                    }
                }

                return await Result<IEnumerable<TEntity>>.SuccessAsync(list);
            }
        }
    }
}
