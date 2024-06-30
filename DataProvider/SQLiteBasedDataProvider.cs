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
    public void Rate(long filePointer,float rating)
    {
        var sql = $"SELECT {Str.Total_Rating},{Str.Rating_Number} FROM {Str.FILE_Table} " +
            $"WHERE {Str.File_Pointer}==@{Str.File_Pointer}";
        var command = BuildSQL(sql).Add($"@{Str.File_Pointer}",filePointer);
        var reader=ReadOneLine(command);
        double totalRating = (double)reader[Str.Total_Rating];
        totalRating += rating;
        long rating_number=(long)reader[Str.Rating_Number];
        rating_number++;

        var sql2 = $"UPDATE {Str.FILE_Table} " +
            $"SET {Str.Total_Rating}=@{Str.Total_Rating},{Str.Rating_Number}=@{Str.Rating_Number} " +
            $"WHERE {Str.File_Pointer}=@{Str.File_Pointer}";
        var command2 = BuildSQL(sql2)
            .Add($"@{Str.Total_Rating}",totalRating)
            .Add($"@{Str.Rating_Number}",rating_number)
            .Add($"@{Str.File_Pointer}",filePointer)
            ;
        ExecuteSQL(command2);
    }
    /// <summary>
    /// 返回找到的50个
    /// </summary>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public IEnumerable<FileDetails> Search(string keyword)
    {
        int count = 0;
        const string KEY = "KEY";
        var sql = $"SELECT * FROM {Str.FILE_Table} WHERE " +
            $"','||{Str.Kch}||',' like '%'||@{KEY}||'%' OR " +
            $"','||{Str.Kcm}||',' like '%'||@{KEY}||'%' OR " +
            $"','||{Str.File_Name}||',' like '%'||@{KEY}||'%'";

        var command = BuildSQL(sql).Add($"@{KEY}",keyword);

        var reader=ReadLine(command);
        while (reader.Read())
        {
            if (count == 50)
            {
                break;
            }
            var detail=new FileDetails();
            detail.file_pointer =(long)reader[Str.File_Pointer];
            detail.file_name = (string)reader[Str.File_Name];
            detail.file_size = ((byte[])reader[Str.File_Blob]).Length;
            detail.kcm = (string)reader[Str.Kcm];
            detail.kch = (string)reader[Str.Kch];
            detail.upload_time = (long)reader[Str.Timestamp];
            var rating_number = (long)reader[Str.Rating_Number];
            if (rating_number == 0)
            {
                detail.rating = -1;
            }
            else
            {
                detail.rating = (float)((double)reader[Str.Total_Rating] / rating_number);
            }
            detail.rating_number = (long)reader[Str.Rating_Number];
            detail.details=(string)reader[Str.Details];

            yield return detail;
            count++;
        }
    }
    public (byte[], string) GetFile(long filePointer)
    {
        var sql = $"SELECT {Str.File_Blob},{Str.File_Name} FROM {Str.FILE_Table} WHERE {Str.File_Pointer}==@{Str.File_Pointer}";
        var command = BuildSQL(sql).Add($"{Str.File_Pointer}",filePointer);
        var reader=ReadOneLine(command);
        return ((byte[])reader[Str.File_Blob], (string)reader[Str.File_Name]);
    }
    public void Upload(string kcm, string kch, string fileName, string details, byte[] file)
    {
        var sql = $"INSERT INTO {Str.FILE_Table} " +
            $"({Str.File_Name},{Str.Timestamp},{Str.Kcm},{Str.Kch},{Str.File_Blob},{Str.Details}) VALUES " +
            $"(@{Str.File_Name},@{Str.Timestamp},@{Str.Kcm},@{Str.Kch},@{Str.File_Blob},@{Str.Details})";
        var command = BuildSQL(sql)
            .Add($"@{Str.File_Name}",fileName)
            .Add($"@{Str.Timestamp}", CurrentTime)
            .Add($"@{Str.Kcm}", kcm)
            .Add($"@{Str.Kch}", kch)
            .Add($"@{Str.File_Blob}", file)
            .Add($"@{Str.Details}",details)
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
        public const string Details = "Details";
        public const string File_Name = "File_Name";
        public const string File_Blob = "File_Bolb";
        public const string Total_Rating = "Rating_Total";
        public const string Rating_Number = "Rating_Number";
        public const string Timestamp = "Timestamp";
        public const string SQL_Build_File_Table = 
            $"CREATE TABLE IF NOT EXISTS {FILE_Table}(" +
                $"{File_Pointer} INTEGER PRIMARY KEY AUTOINCREMENT ," +
                $"{File_Name} TEXT ," +
                $"{Timestamp} INTEGER ," +
                $"{Kcm} TEXT ," +
                $"{Kch} TEXT DEFAULT ''," +
                $"{File_Blob} BLOB ," +
                $"{Details} TEXT DEFAULT ''," +
                $"{Total_Rating} REAL DEFAULT 0.0," +
                $"{Rating_Number} INTEGER DEFAULT 0" +
            $")";

    }
}
