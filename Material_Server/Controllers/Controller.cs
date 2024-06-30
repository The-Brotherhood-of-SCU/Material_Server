using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using DataProvider;

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
    [HttpGet("get_file/{file_pointer}")]
    public IResult GetFile(long file_pointer)
    {
        return Results.Ok(Assets.DataProvider.GetFile(file_pointer));
    }

    [HttpPost("upload")]
    public async Task<IResult> GetUpload([FromForm] UploadData data)
    {
        byte[] file=new byte[data.file.Length];
        var stream=data.file.OpenReadStream();
        await stream.ReadAsync(file);
        Assets.DataProvider.Upload(data.kcm,data.kch,data.file_name,data.details,file);
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
    public IFormFile file { get; set; } // ���ڽ����ļ�
}