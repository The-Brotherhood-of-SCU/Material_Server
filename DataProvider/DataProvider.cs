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
    public void Upload(string kcm,string kch,string fileName, byte[] file);
}

public record FileDetails
{
    public string kcm;
    public string kch;
    public string file_name;
    public int file_size;
    public long file_pointer;
    public long timestamp;
}