using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider;

public class SQLiteBasedDataProvider : SQLiteDataProvider,DataProvider
{
    public SQLiteBasedDataProvider() :base("data.db"){ 
        ExecuteSQL(Str.SQL_Build_File_Table);
    }

    public IEnumerable<FileDetails> Search(string keyword)
    {
        const string KEY = "KEY";
        var sql = $"SELECT * FROM {Str.FILE_Table} WHERE " +
            $"FIND_IN_SET({Str.Kch},@{KEY}) OR" +
            $"FIND_IN_SET({Str.Kcm},@{KEY}) OR" +
            $"FIND_IN_SET({Str.File_Name},@{KEY})";

        var command = BuildSQL(sql).Add($"@{KEY}",keyword);

        var reader=ReadLine(command);
        while (reader.Read())
        {
            var detail=new FileDetails();
            detail.file_pointer =(string)reader[Str.File_Pointer];
            detail.file_name = (string)reader[Str.File_Name];
            detail.file_size = ((byte[])reader[Str.File_Blob]).Length;
            detail.kcm = (string)reader[Str.Kcm];
            detail.kch = (string)reader[Str.Kch];
            detail.timestamp = (long)reader[Str.Timestamp];

            yield return detail;
        }
    }
    public byte[] GetFile(string filePointer)
    {
        var sql = $"SELECT {Str.File_Blob} FROM {Str.FILE_Table} WHERE {Str.File_Pointer}==@{Str.File_Pointer}";
        var command = BuildSQL(sql).Add($"{Str.File_Pointer}",filePointer);
        var reader=ReadOneLine(command);
        return (byte[])reader[Str.File_Blob];
    }
    public void Upload(string kcm, string kch, string fileName, byte[] file)
    {
        var sql = $"INSERT INTO {Str.FILE_Table} " +
            $"({Str.File_Name},{Str.Timestamp},{Str.Kcm},{Str.Kch},{Str.File_Blob}) VALUES " +
            $"(@{Str.File_Name},@{Str.Timestamp},@{Str.Kcm},@{Str.Kch},@{Str.File_Blob})";
        var command = BuildSQL(sql)
            .Add($"@{Str.File_Name}",fileName)
            .Add($"@{Str.Timestamp}", CurrentTime)
            .Add($"@{Str.Kcm}", kcm)
            .Add($"@{Str.Kch}", kch)
            .Add($"@{Str.File_Blob}", file)
            ;

        ExecuteSQL(command);
    }
    private long CurrentTime {
        get { return (long)(DateTime.Now - ZERO_TIME).TotalSeconds; }
    }
    private DateTime ZERO_TIME=new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc);
    internal static class Str
    {
        public const string FILE_Table = "File_Table";
        public const string File_Pointer = "File_Pointer";
        public const string Kch = "kch";
        public const string Kcm = "kcm";
        public const string File_Name = "File_Name";
        public const string File_Blob = "File_Bolb";
        public const string Rating = "Rating";
        public const string Timestamp = "Timestamp";
        public const string SQL_Build_File_Table = 
            $"CREATE TABLE IF NOT EXISTS {FILE_Table}(" +
                $"{File_Pointer} INTEGER  AUTOINCREMENT" +
                $"{File_Name} TEXT" +
                $"{Timestamp} INTEGER " +
                $"{Kcm} TEXT" +
                $"{Kch} TEXT" +
                $"{File_Blob} BLOB" +
                $"{Rating} REAL" +
            $")";

    }
}
