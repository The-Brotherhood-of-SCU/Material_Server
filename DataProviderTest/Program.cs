using DataProvider;
using System.Text;


var provider = new SQLiteBasedDataProvider();

//provider.RemoveAll();

provider.Upload("english","114514","123","??",Encoding.UTF8.GetBytes("🤭黑黑黑"));



var pointer = provider.Search("114").First().file_pointer;

var file_ = provider.GetFile(pointer);

Console.WriteLine(Encoding.UTF8.GetString(file_));