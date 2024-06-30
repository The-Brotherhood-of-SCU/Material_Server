namespace Material_Server;
using DataProvider;

public class Assets
{
    public static void Init()
    {
        DataProvider = new SQLiteBasedDataProvider();
    }
    public static SQLiteBasedDataProvider DataProvider { get;private set; }
}
