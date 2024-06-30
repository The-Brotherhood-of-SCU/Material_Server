namespace Material_Server;
using DataProvider;

public class Assets
{
    public static SQLiteBasedDataProvider DataProvider { get;private set; }=new SQLiteBasedDataProvider();
}
