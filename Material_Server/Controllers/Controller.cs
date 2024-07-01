using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using DataProvider;
using System.IO;

namespace Material_Server.Controllers;


[ApiController]
[Route("/")]
public class ServerController : ControllerBase
{

    [HttpGet("search/{keyword}")]
    public IEnumerable<FileDetail>? Search(string keyword)
    {
        var result = Assets.DataProvider.Search(keyword);
        return result;
    }

    [HttpGet("user/{account}/files")]
    public IEnumerable<FileDetail>? GetFilesByUploader(string account)
    {
        var result = Assets.DataProvider.GetFileDetailsByUploader(account);
        return result;
    }
    [HttpGet("user/{account}/comments")]
    public IEnumerable<CommentDetail> GetCommentsByAccount(string account) {
        return Assets.DataProvider.GetCommentsByUser(account);
    }

    [HttpGet("file/{file_pointer}")]
    public FileResult GetFile(long file_pointer)
    {
        var (file,fileName) = Assets.DataProvider.GetFile(file_pointer);
        
        return File(file, "text/plain",fileName);
    }

    [HttpPost("upload")]
    public async Task<IResult> GetUpload([FromForm] UploadData data)
    {
        byte[] file=new byte[data.file.Length];
        var stream=data.file.OpenReadStream();
        await stream.ReadAsync(file);
        Assets.DataProvider.Upload(data.kcm,data.kch,data.file_name,data.details,file,data.uploader);
        return Results.Ok();
    }

    [HttpPost("rate")]
    public IResult Rate([FromQuery]long file_pointer, [FromQuery] float rating)
    {
        Assets.DataProvider.Rate(file_pointer, rating);
        return Results.Ok();
    }

    [HttpPost("comment")]
    public IResult Comment([FromForm] CommentData commentData)
    {
        Assets.DataProvider.Comment(commentData.account, commentData.file_pointer, commentData.text, commentData.rating); ;
        return Results.Ok();
    }

    [HttpGet("/comment/{filePointer}")]
    public IEnumerable<CommentDetail> GetCommentsByFilePointer(long filePointer)
    {
        return Assets.DataProvider.GetCommentsByFilePointer(filePointer);
    }
    [HttpGet("/recommend")]
    public IEnumerable<FileDetail> GetRecommendation
        (
        string keyword,
        float grades
        )
    {
        throw new NotImplementedException();
    }
}
public class UploadData
{
    public string kcm { get; set; }
    public string kch { get; set; }
    public string details { get; set; }
    public string file_name { get; set; }
    public string uploader {  get; set; }
    public IFormFile file { get; set; } // 用于接收文件
}
public class CommentData
{
    public string account {  get; set; }
    public long file_pointer { get; set; }
    public string text {  get; set; }
    public float rating {  get; set; }
}
