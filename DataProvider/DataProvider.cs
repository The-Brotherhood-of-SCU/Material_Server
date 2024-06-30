namespace DataProvider;

public interface DataProvider
{
    /// <summary>
    /// 搜索文件中与关键词匹配的
    /// </summary>
    /// <param name="keywords"></param>
    /// <returns></returns>
    public IEnumerable<FileDetails> Search(string keyword);
    public byte[] GetFile(long filePointer);
    public void Upload(string kcm,string kch,string fileName,string details,byte[] file);
    public void Rate(long filePointer,float rate);
}

public record FileDetails
{
    public string kcm;
    public string kch;
    public string file_name;
    public string details;
    public float rating;
    public long rating_number;
    public int file_size;
    public long file_pointer;
    public long upload_time;
}