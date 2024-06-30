using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace DataProvider;

public class SQLiteDataProvider 
{
    protected SQLiteConnection dbConn;
    public SQLiteDataProvider(string databaseName)
    {
        try
        {
            dbConn = new($"Data Source={databaseName};Version=3;");
        }
        catch
        {
            SQLiteConnection.CreateFile(databaseName);
            dbConn = new($"Data Source={databaseName};Version=3;");
        }
        sqlBuilder = new SQLBuilder(dbConn);
        dbConn.Open();
    }

    public void Close()
    {
        dbConn.Close();
    }

    public void Flush()
    {

    }
    protected Task ExecuteSQL(string sql)
    {
        var command = new SQLiteCommand(sql, dbConn);
        return ExecuteSQL(command);


    }
    protected async Task ExecuteSQL(SQLiteCommand command)
    {
        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            throw new DatabaseException("Inner SQL exception");
        }

    }
    protected Task ExecuteSQL(_builder builder) {
        return ExecuteSQL(builder.Command);
    }
    protected SQLiteDataReader ReadLine(string sql)
    {
        SQLiteCommand command = new SQLiteCommand(sql, dbConn);
        return ReadLine(command);
    }

    protected SQLiteDataReader ReadLine(SQLiteCommand command)
    {
        return command.ExecuteReader();
    }
    protected SQLiteDataReader ReadLine(_builder builder)
    {
        return ReadLine(builder.Command);
    }


    protected Task<System.Data.Common.DbDataReader> ReadLineAsync(string sql)
    {
        SQLiteCommand command = new SQLiteCommand(sql, dbConn);
        return ReadLineAsync(command);
    }

    protected Task<System.Data.Common.DbDataReader>  ReadLineAsync(SQLiteCommand command)
    {
        return command.ExecuteReaderAsync();
    }
    protected Task<System.Data.Common.DbDataReader> ReadLineAsync(_builder builder)
    {
        return ReadLineAsync(builder.Command);
    }

    protected SQLiteDataReader ReadOneLine(string sql)
    {
        return ReadOneLine(new SQLiteCommand(sql, dbConn));
    }
    protected SQLiteDataReader ReadOneLine(SQLiteCommand command)
    {
        var a = ReadLine(command);
        a.Read();
        return a;
    }
    protected SQLiteDataReader ReadOneLine(_builder builder)
    {
        return ReadOneLine(builder.Command);
    }
    public void RemoveAll()
    {
        foreach (var table in GetAllTables())
        {
            ExecuteSQL($"DELETE FROM {table}");
        }
    }
    protected IEnumerable<string> GetAllTables()
    {
        var all_tables = "SELECT name FROM sqlite_master WHERE type='table' order by name";
        var result = ReadLine(all_tables);
        while (result.Read())
        {
            yield return result["name"].ToString()!;
        }

    }
    protected bool IsTableExists(string tableName)
    {
        var sql = $"SELECT name FROM sqlite_master WHERE name='{tableName}' AND type='table'";
        return ReadLine(sql).Read();
    }
    protected SQLBuilder sqlBuilder;


    protected _builder BuildSQL(string sql)
    {
        return sqlBuilder.build(sql);
    }
}
public class SQLBuilder
{
    SQLiteConnection conn;
    public SQLBuilder(SQLiteConnection dbConn)
    {
        this.conn = dbConn;
    }
    public _builder build(string sql)
    {
        return new _builder(sql, conn);
    }

}
public class _builder
{
    public SQLiteCommand Command { get; private set; }
    public _builder(string sql, SQLiteConnection connection)
    {
        Command = new SQLiteCommand(sql, connection);
    }
    public _builder Add(string parameter, object value)
    {
        Command.Parameters.AddWithValue(parameter, value);
        return this;
    }

}