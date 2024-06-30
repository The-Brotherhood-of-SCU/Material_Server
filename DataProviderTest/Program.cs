using DataProvider;
using System.Text;


DataProvider.DataProvider provider = new SQLiteBasedDataProvider();
provider.Upload("english","114514","123",Encoding.UTF8.GetBytes("🤭黑黑黑"));