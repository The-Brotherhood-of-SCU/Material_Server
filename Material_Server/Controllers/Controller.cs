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
    public IEnumerable<FileDetails>? Search(string keyword)
    {
        var result = Assets.DataProvider.Search(keyword);
        return result;
    }

    [HttpGet("user/{account}/files")]
    public IEnumerable<FileDetails>? GetFilesByUploader(string account)
    {
        var result = Assets.DataProvider.GetFileDetailsByUploader(account);
        return result;
    }
    [HttpGet("get_file/{file_pointer}")]
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
