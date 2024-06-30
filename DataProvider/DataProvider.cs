namespace DataProvider;

[Obsolete]
public interface DataProvider
{
    /// <summary>
    /// 搜索文件中与关键词匹配的
    /// </summary>
    /// <param name="keywords"></param>
    /// <returns></returns>
    public IEnumerable<FileDetail> Search(string keyword);
    public (byte[],string) GetFile(long filePointer);
    public void Upload(string kcm,string kch,string fileName,string details,byte[] file);
    public void Rate(long filePointer,float rate);
}

public record FileDetail
{
    public string kcm { get; set; }
    public string kch { get; set; }
    public string file_name { get; set; }
    public string details { get; set; }
    public float rating { get; set; }
    public long rating_number { get; set; }
    public int file_size { get; set; }
    public long file_pointer { get; set; }
    public long upload_time { get; set; }
    public string uploader {  get; set; }
}
public record CommentDetail
{
    public string account { get; set; }
    public long file_pointer {  get; set; }
    public long timestamp {  get; set; }
    public string text {  get; set; }
    public float rating {  set; get; }
}