using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider;

public class SQLiteBasedDataProvider : SQLiteDataProvider
{
    public SQLiteBasedDataProvider() :base("data.db"){ 
        ExecuteSQL(Str.SQL_Build_File_Table);
        ExecuteSQL(Str2.SQL_Build_Comment_Table);
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
    public IEnumerable<FileDetail> Search(string keyword)
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
            yield return GetFileDetailByReader(reader);
            count++;
        }
    }

    private FileDetail GetFileDetailByReader(SQLiteDataReader reader)
    {
        var detail = new FileDetail();
        detail.file_pointer = (long)reader[Str.File_Pointer];
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
        detail.details = (string)reader[Str.Details];
        return detail;
    }
    public FileDetail GetFileDetailByFilePointer(long filePointer)
    {
        var detail = new FileDetail();
        var sql = $"SELECT * FROM {Str.FILE_Table} WHERE {Str.File_Pointer}==@{Str.File_Pointer}";
        var command=BuildSQL(sql).Add($"@{Str.File_Pointer}",filePointer);
        var reader=ReadOneLine(command);
        return GetFileDetailByReader(reader);
    }
    public IEnumerable<long> GetFilesByUploader(string uploader) {
        var sql = $"SELECT {Str.File_Pointer} FROM {Str.FILE_Table} " +
            $"WHERE {Str.Uploader}==@{Str.Uploader}";
        var command=BuildSQL(sql).Add($"@{Str.Uploader}",uploader);
        var reader=ReadLine(command);
        while (reader.Read()) { 
            yield return (long)reader[Str.File_Pointer];
        }
    }
    public IEnumerable<FileDetail> GetFileDetailsByUploader(string uploader)
    {
        var sql = $"SELECT * FROM {Str.FILE_Table} WHERE {Str.Uploader}==@{Str.Uploader}";
        var command=BuildSQL(sql).Add($"@{Str.Uploader}",uploader);
        var reader=ReadLine(command);
        while (reader.Read())
        {
            yield return GetFileDetailByReader(reader);
        }
    }
    public IEnumerable<FileDetail> GetRecommendation(string keyword, double grade)
    {
        // 查询所有文件的SQL语句
        var sql = $"SELECT * FROM {Str.FILE_Table}";
        var recommendations = new List<Tuple<FileDetail, double>>(); // 用于存储文件及其相似度的列表
        var reader = ReadLine(sql);

        // 遍历所有文件
        while (reader.Read())
        {
            var fileDetail = GetFileDetailByReader(reader); // 读取文件详情
            var similarity = CalculateSimilarity(keyword, fileDetail.file_name); // 计算关键词与文件名的相似度
            recommendations.Add(Tuple.Create(fileDetail, similarity)); // 将文件及其相似度添加到列表中
        }

        // 按相似度降序排序
        recommendations = recommendations.OrderByDescending(r => r.Item2).ToList();

        double threshold; // 相似度阈值
        if (grade >= 0.8) // 如果成绩较高
        {
            threshold = 0.3; // 推荐匹配度较低的资料
        }
        else // 如果成绩较低
        {
            threshold = 0.6; // 推荐匹配度较高的资料
        }

        // 返回相似度超过阈值的前五个文件
        return recommendations
            .Where(r => r.Item2 >= threshold)
            .Take(5)
            .Select(r => r.Item1);
    }

    // 计算两个字符串的相似度
    private double CalculateSimilarity(string keyword, string fileName)
    {
        int n = keyword.Length;
        int m = fileName.Length;
        if (n == 0) return m;
        if (m == 0) return n;

        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; i++)
        {
            d[i, 0] = i;
        }

        for (int j = 0; j <= m; j++)
        {
            d[0, j] = j;
        }

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (fileName[j - 1] == keyword[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }

        int levenshteinDistance = d[n, m];
        int maxLen = Math.Max(n, m);

        return 1.0 - (double)levenshteinDistance / maxLen;
    }
    public (byte[], string) GetFile(long filePointer)
    {
        var sql = $"SELECT {Str.File_Blob},{Str.File_Name} FROM {Str.FILE_Table} WHERE {Str.File_Pointer}==@{Str.File_Pointer}";
        var command = BuildSQL(sql).Add($"{Str.File_Pointer}",filePointer);
        var reader=ReadOneLine(command);
        return ((byte[])reader[Str.File_Blob], (string)reader[Str.File_Name]);
    }
    public void Upload(string kcm, string kch, string fileName, string details, byte[] file,string uploader)
    {
        var sql = $"INSERT INTO {Str.FILE_Table} " +
            $"({Str.File_Name},{Str.Timestamp},{Str.Kcm},{Str.Kch},{Str.File_Blob},{Str.Details},{Str.Uploader}) VALUES " +
            $"(@{Str.File_Name},@{Str.Timestamp},@{Str.Kcm},@{Str.Kch},@{Str.File_Blob},@{Str.Details},@{Str.Uploader})";
        var command = BuildSQL(sql)
            .Add($"@{Str.File_Name}",fileName)
            .Add($"@{Str.Timestamp}", CurrentTime)
            .Add($"@{Str.Kcm}", kcm)
            .Add($"@{Str.Kch}", kch)
            .Add($"@{Str.File_Blob}", file)
            .Add($"@{Str.Details}",details)
            .Add($"@{Str.Uploader}",uploader)
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
        public const string Uploader = "Uploader";
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
                $"{Rating_Number} INTEGER DEFAULT 0, " +
                $"{Uploader} TEXT DEFAULT 'unknown'" +
            $")";
    }
    //-----comment
    internal static class Str2{//for comment
        public const string Account = "Account";
        public const string Text = "Text";
        public const string File_Pointer = "File_Pointer";
        public const string Comment_Table = "Comment_Table";
        public const string Timestamp = "Timestamp";
        public const string Rating = "Rating";
        public const string SQL_Build_Comment_Table = 
            $"CREATE TABLE IF NOT EXISTS {Comment_Table}(" +
                $"{Account} TEXT," +
                $"{File_Pointer} INTEGER," +
                $"{Timestamp} INTEGER," +
                $"{Text} TEXT," +
                $"{Rating} REAL" +
            $")";
    }
    public void Comment(string account,long filePointer,string text,float rating)
    {
        var sql = $"INSERT INTO {Str2.Comment_Table} " +
            $"({Str2.Account},{Str2.File_Pointer},{Str2.Timestamp},{Str2.Text},{Str2.Rating})VALUES" +
            $"(@{Str2.Account},@{Str2.File_Pointer},@{Str2.Timestamp},@{Str2.Text},@{Str2.Rating})";
        var command = BuildSQL(sql)
            .Add($"@{Str2.Account}", account)
            .Add($"@{Str2.File_Pointer}", filePointer)
            .Add($"@{Str2.Timestamp}", CurrentTime)
            .Add($"@{Str2.Text}", text)
            .Add($"@{Str2.Rating}", rating)
            ;
        ExecuteSQL(command);
    }
    private CommentDetail GetCommentDetailByReader(SQLiteDataReader reader)
    {
        CommentDetail commentDetail = new CommentDetail();
        commentDetail.account =(string) reader[Str2.Account];
        commentDetail.text =(string) reader[Str2.Text];
        commentDetail.timestamp =(long) reader[Str2.Timestamp];
        commentDetail.file_pointer = (long)reader[Str2.File_Pointer];
        commentDetail.rating=(float)(double)reader[Str2.Rating];
        try
        {
            commentDetail.file_name =GetFileDetailByFilePointer(commentDetail.file_pointer).file_name;
        }
        catch (Exception) { }
        
        return commentDetail;
    }
    public IEnumerable<CommentDetail> GetCommentsByFilePointer(long filePointer)
    {
        var sql = $"SELECT * FROM {Str2.Comment_Table} WHERE {Str2.File_Pointer}=={filePointer}";
        var reader = ReadLine(sql);
        while (reader.Read())
        {
            yield return GetCommentDetailByReader(reader);
        }
    }
    public IEnumerable<CommentDetail> GetCommentsByUser(string account)
    {
        var sql = $"SELECT * FROM {Str2.Comment_Table} WHERE {Str2.Account}==@{Str2.Account}";
        var command = BuildSQL(sql).Add($"@{Str2.Account}",account);
        var reader = ReadLine(command);
        while (reader.Read())
        {
            yield return GetCommentDetailByReader(reader);
        }
    }
    
}
