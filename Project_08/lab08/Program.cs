using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.Sqlite; 
using System.Data;

namespace Lab08;

public class Program1
{
    public const string FILENAME = "data1.csv";
    public const char SEPARATOR = ',';
    public const string TABLE_NAME = "nazwatabeli";
    public const string DB_FILENAME = "database.sqlite";

    public static void Main(string[] args)
    {
        CsvReadResult? csvReadResult = ReadDataFromCsv(FILENAME, SEPARATOR);
        if (csvReadResult == null)
        {
            Console.WriteLine("CsvReadResult is null");
            return;
        }

        List<ColumnAnalysisResult>? columnAnalysisResults = GetDataTypes(csvReadResult.Headers, csvReadResult.Data);

        if (columnAnalysisResults == null)
        {
            Console.WriteLine("columnAnalisisResults is null");
            return;
        }

        foreach (var result in columnAnalysisResults)
        {
            Console.WriteLine($"Column: {result.ColumnName}, Type: {result.DataType}, Nullable: {result.IsNullable}");
        }

        string connectionString = $"Data Source={DB_FILENAME}";

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            try
            {
                connection.Open();

                bool tableCreated = CreateTableFromAnalisis(columnAnalysisResults, TABLE_NAME, connection);
                if (tableCreated)
                {
                    Console.WriteLine($"Table '{TABLE_NAME}' created successfully (or already existed)");
                }
                else
                {
                    Console.WriteLine($"Failed to create table '{TABLE_NAME}'");
                }

                bool dataInserted = InsertDataToTable(csvReadResult, TABLE_NAME, connection);
                if (dataInserted)
                {
                    Console.WriteLine("Data inserted successfully.");
                    PrintTableData(TABLE_NAME, connection);
                }
                else
                {
                    Console.WriteLine("Failed to insert data. Check previous error messages.");
                }
                
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"SQLite error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during database operations: {ex.Message}");
            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    Console.WriteLine("Database connection closed.");
                }
            }
        }
    }

    public static void PrintTableData(string? tableName, SqliteConnection connection)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            Console.WriteLine("Error: Table name cannot be null or empty for printing.");
            return;
        }
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Console.WriteLine("Error: SQLite connection is null or not open for printing.");
            return;
        }
        string selectSql = $"SELECT * FROM \"{tableName}\";";
        Console.WriteLine($"{selectSql}");

        try
        {
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = selectSql;
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        Console.WriteLine($"Table '{tableName}' exists but is empty or query returned no results.");
                        return;
                    }

                    int columnCount = reader.FieldCount;
                    List<string> columnNames = new List<string>();
                    List<int> columnWidths = new List<int>();
                    Console.Write("| ");
                    for (int i = 0; i < columnCount; i++)
                    {
                        string colName = reader.GetName(i);
                        columnNames.Add(colName);
                        columnWidths.Add(colName.Length);
                        Console.Write($"{colName} | ");
                    }

                    Console.WriteLine();

                    Console.Write("+-");
                    for (int i = 0; i < columnCount; i++)
                    {
                        Console.Write(new string('-', columnNames[i].Length) + "-+-");
                    }

                    Console.WriteLine();
                    int rowCount = 0;
                    while (reader.Read())
                    {
                        rowCount++;
                        Console.Write("| ");
                        for (int i = 0; i < columnCount; i++)
                        {

                            object value = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i);
                            string valueStr = value.ToString() ?? "NULL"; // Konwertuj na string


                            Console.Write($"{valueStr.PadRight(columnNames[i].Length)} | "); // PadRight dla wyrównania
                        }

                        Console.WriteLine();
                    }

                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Excpetion during selecting");
        }
    }


    public static CsvReadResult? ReadDataFromCsv(String? filename, Char? separator)
    {
        if (string.IsNullOrEmpty(filename))
        {
            Console.WriteLine("Error: Filename cannot be null or empty.");
            return null;
        }

        if (!File.Exists(filename))
        {
            Console.WriteLine($"Cannot find file : {filename}");
            return null;
        }

        if (separator == null)
        {
            Console.WriteLine("Error: Separator cannot be null.");
            return null;
        }

        CsvReadResult csvReadResult = new CsvReadResult();


        try
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                String? line;
                line = sr.ReadLine();
                if (line == null)
                {
                    Console.WriteLine($"Error: CSV file '{filename}' is empty or could not be read.");
                    return null;
                }

                string[] headerParts = line.Split(separator.Value);

                foreach (string header in headerParts)
                {
                    csvReadResult.Headers.Add(header.Trim());
                }

                if (csvReadResult.Headers.Count == 0 || csvReadResult.Headers.All(string.IsNullOrEmpty))
                {
                    Console.WriteLine($"Error: CSV file '{filename}' has an empty or invalid header line.");
                    return null;
                }

                int expectedColumnCount = csvReadResult.Headers.Count;
                int lineNumber = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }


                    string[] values = line.Split(separator.Value);

                    if (values.Length != expectedColumnCount)
                    {
                        Console.WriteLine(
                            $"Warning: Skipping line {lineNumber}. Expected {expectedColumnCount} columns, but found {values.Length}. Line content: \"{line}\"");
                        continue;
                    }

                    List<String> dataRow = new List<string>(values);

                    csvReadResult.Data.Add(dataRow);
                }
            }

            return csvReadResult;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"An IO error occurred while reading the file: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return null;
        }
    }

    public static List<ColumnAnalysisResult>? GetDataTypes(List<string>? headers, List<List<string>>? data)
    {
        if (headers == null || data == null || headers.Count == 0)
        {
            Console.WriteLine("Error: Invalid input data for analysis.");
            return null;
        }

        int columnCount = headers.Count;
        var analysisResults = new List<ColumnAnalysisResult>();

        for (int j = 0; j < columnCount; j++)
        {
            bool isNullable = false;
            bool isIntPossible = true;
            bool isRealPossible = true;

            foreach (var row in data)
            {
                string cellValue = row[j];

                if (string.IsNullOrEmpty(cellValue))
                {
                    isNullable = true;
                    continue;
                }

                if (isIntPossible)
                {
                    if (!int.TryParse(cellValue, out _))
                    {
                        isIntPossible = false;
                    }
                }

                if (isRealPossible)
                {

                    if (!double.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    {
                        isRealPossible = false;
                    }
                }
            }

            string? finalDataType;
            if (isIntPossible)
            {
                finalDataType = "INTEGER";
            }
            else if (isRealPossible)
            {
                finalDataType = "REAL";
            }
            else
            {
                finalDataType = "TEXT";
            }

            analysisResults.Add(new ColumnAnalysisResult
            {
                ColumnName = headers[j],
                DataType = finalDataType,
                IsNullable = isNullable
            });
        }

        return analysisResults;
    }


    public static bool CreateTableFromAnalisis(List<ColumnAnalysisResult>? analysisResults, String? tableName,
        SqliteConnection connection)
    {
        if (analysisResults == null || analysisResults.Count == 0)
        {
            Console.WriteLine("Error: Column analysis results are null or empty. Cannot create table.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            Console.WriteLine("Error: Table name cannot be null or empty.");
            return false;
        }

        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            Console.WriteLine("Error: SQLite connection is null or not open.");
            return false;
        }

        StringBuilder createTableSql = new StringBuilder();
        createTableSql.Append($"CREATE TABLE IF NOT EXISTS \"{tableName}\" (");

        bool firstColumn = true;
        foreach (var column in analysisResults)
        {
            if (!firstColumn)
            {
                createTableSql.Append(", ");
            }

            string? sqliteType;
            switch (column.DataType?.ToUpperInvariant())
            {
                case "INTEGER":
                    sqliteType = "INTEGER";
                    break;
                case "REAL":
                    sqliteType = "REAL";
                    break;
                case "TEXT":
                default:
                    sqliteType = "TEXT";
                    break;
            }

            createTableSql.Append($"\"{column.ColumnName}\" {sqliteType}");
            if (!column.IsNullable)
            {
                createTableSql.Append(" NOT NULL");
            }

            firstColumn = false;
        }

        if (firstColumn)
        {
            Console.WriteLine(
                "Error: No columns found to create the table (analysis results might be empty or contain only invalid columns previously).");
            return false;
        }

        createTableSql.Append(");");

        try
        {
            using (SqliteCommand command = connection.CreateCommand())
            {
                Console.WriteLine(createTableSql.ToString());
                command.CommandText = createTableSql.ToString();
                command.ExecuteNonQuery();
            }

            return true;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"SQLite error occurred while creating table '{tableName}': {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }

    }


    public static bool InsertDataToTable(CsvReadResult? csvData, String? tableName, SqliteConnection connection)
    {
        if (csvData == null || csvData.Headers == null || csvData.Data == null || csvData.Headers.Count == 0)
        {
            Console.WriteLine("Error: CSV data is null or empty. Cannot insert data.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(tableName))
        {
            Console.WriteLine("Error: Table name cannot be null or empty.");
            return false;
        }

        if (connection == null || connection.State != ConnectionState.Open)
        {
            Console.WriteLine("Error: SQLite connection is null or not open.");
            return false;
        }

        List<string> columnNames = csvData.Headers;
        List<string> paramNames = columnNames.Select((col, index) => $"@param{index}").ToList();

        string columnsSql = string.Join(", ", columnNames.Select(c => $"\"{c}\""));
        string paramsSql = string.Join(", ", paramNames);
        string insertSql = $"INSERT INTO \"{tableName}\" ({columnsSql}) VALUES ({paramsSql});";


        SqliteTransaction? transaction = null;
        try
        {
            transaction = connection.BeginTransaction();
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = insertSql;

                foreach (string paramName in paramNames)
                {
                    command.Parameters.Add(new SqliteParameter(paramName, null)); // Wartość zostanie ustawiona w pętli
                }

                int rowsInserted = 0;
                foreach (var dataRow in csvData.Data)
                {
                    if (dataRow.Count != columnNames.Count)
                    {
                        continue;
                    }

                    for (int i = 0; i < paramNames.Count; i++)
                    {
                        command.Parameters[paramNames[i]].Value =
                            string.IsNullOrEmpty(dataRow[i]) ? DBNull.Value : dataRow[i];
                    }

                    command.ExecuteNonQuery();
                    rowsInserted++;
                }
            }

            transaction.Commit();
            return true;
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"SQLite error occurred during data insertion into '{tableName}': {ex.Message}");
            try
            {
                transaction?.Rollback();
                Console.WriteLine("Transaction rolled back.");
            }
            catch
            {
                return false;
            }
        }

        return true;
    }
    
}

