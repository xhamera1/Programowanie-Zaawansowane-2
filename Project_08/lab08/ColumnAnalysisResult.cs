namespace Lab08;

public class ColumnAnalysisResult
{
    
    public string ColumnName { get; set; }
    public string DataType { get; set; } // "INTEGER", "REAL", "TEXT"
    public bool IsNullable { get; set; }

    public ColumnAnalysisResult()
    {
    }

    public ColumnAnalysisResult(string columnName, string dataType, bool isNullable)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        IsNullable = isNullable;
    }
}