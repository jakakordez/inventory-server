using ClosedXML.Excel;
using System.Text;

namespace InventoryServer;

public class TableExporter<T>(IEnumerable<T> data)
{
    private readonly IEnumerable<T> data = data;
    private List<Tuple<string, Func<T, object?>, int>> columns = [];

    public void AddColumn(string name, Func<T, object?> valueFunction, int width = 20)
    {
        columns.Add(new Tuple<string, Func<T, object?>, int>(name, valueFunction, width));
    }

    public string ExportToCsv()
    {
        StringBuilder result = new StringBuilder();

        foreach (var column in columns)
        {
            result.Append(column.Item1);
            result.Append(',');
        }
        result.AppendLine();

        int index = 1;
        foreach (var row in data)
        {
            foreach (var column in columns)
            {
                string value;
                if (column.Item2 == null)
                {
                    value = index++.ToString();
                }
                else
                {
                    value = column.Item2(row)?.ToString() ?? "";
                }

                if (value.Contains(',')) result.Append("\"");
                result.Append(value);
                if (value.Contains(',')) result.Append("\"");

                result.Append(",");
            }
            result.AppendLine();
        }

        return result.ToString();
    }

    public byte[] ExportToExcel()
    {
        using (var outputStream = new MemoryStream())
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Inventory changes");

                int iCol = 1;

                foreach (var column in columns)
                {
                    worksheet.Column(iCol).Width = column.Item3;
                    var cell = worksheet.Cell(1, iCol++);
                    cell.Value = column.Item1;
                    cell.Style.Border.BottomBorder = XLBorderStyleValues.Medium;
                    cell.Style.Font.Bold = true;
                }

                int iRow = 2;
                foreach (var obj in data)
                {
                    iCol = 1;

                    foreach (var column in columns)
                    {
                        object value;
                        if (column.Item2 == null)
                        {
                            value = iRow - 1;
                        }
                        else
                        {
                            value = column.Item2(obj) ?? "";
                        }
                        worksheet.Cell(iRow, iCol++).Value = XLCellValue.FromObject(value);
                    }
                    iRow++;
                }

                workbook.SaveAs(outputStream);
            }
            return outputStream.ToArray();
        }
    }
}
