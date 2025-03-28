// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;

namespace NetCa.Application.Common.Extensions;

/// <summary>
/// Excel Extension
/// </summary>
public static class ExcelExtensions
{
    /// <summary>
    /// Read Xlsx
    /// </summary>
    /// <param name="file"></param>
    /// <param name="checkValidation"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<DataTable> FromXlsx(
        this IFormFile file,
        bool checkValidation,
        CancellationToken cancellationToken
    )
    {
        var stream = new MemoryStream();
        var dataTable = new DataTable();

        await using (stream.ConfigureAwait(false))
        {
            await file.CopyToAsync(stream, cancellationToken).ConfigureAwait(false);

            using var wb = new XLWorkbook(stream);
            var firstRow = true;

            foreach (var row in wb.Worksheet(1).Rows())
            {
                if (row.IsEmpty())
                {
                    row.Cells().Clear();
                }

                if (!row.Cells().Any())
                {
                    break;
                }

                if (firstRow)
                {
                    foreach (var cell in row.Cells())
                    {
                        dataTable.Columns.Add(cell.Value.ToString());
                    }

                    firstRow = false;
                }
                else
                {
                    dataTable.Rows.Add();
                    var i = 0;

                    for (var j = 1; j <= dataTable.Columns.Count; j++)
                    {
                        var activeCell =
                            $"{(char)(PaginationConstants.CustomRuleAsciiAlphabet + j)}{dataTable.Rows.Count + 1}";

                        var value = row.Cells(activeCell)
                            .Select(x => x)
                            .FirstOrDefault()
                            ?.Value
                            .ToString();

                        if (!string.IsNullOrWhiteSpace(value) && checkValidation)
                        {
                            var combinePattern = RegexConstants
                                .Pattern
                                .Replace(
                                    "{pattern}",
                                    RegexConstants.CharPattern
                                        + RegexConstants.NumericPattern
                                        + " "
                                        + RegexConstants.SymbolPattern
                                );

                            var pattern = new Regex(combinePattern);

                            var isMatch = pattern.IsMatch(value);

                            if (!isMatch)
                            {
                                throw new BadRequestException(
                                    $"Only character [{RegexConstants.CharPattern}], numeric [{RegexConstants.NumericPattern}], and symbol [{RegexConstants.SymbolPattern}] can be accepted in cell {activeCell}"
                                );
                            }
                        }

                        dataTable.Rows[^1][i] = value ?? string.Empty;

                        i++;
                    }
                }
            }

            stream.Close();
        }

        return dataTable;
    }
}
