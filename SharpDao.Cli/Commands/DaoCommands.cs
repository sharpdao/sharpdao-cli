using Cocona;

namespace SharpDao.Cli.Commands;

public static class DaoCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("dao", x =>
        {
            x.AddCommand("create", CreateDaoCommand);
            x.AddCommand("list", ListDaoCommand);
            x.AddCommand("modify", ModifyDaoCommand);
            x.AddCommand("register", RegisterDaoCommand);
        });
    }

    private static void CreateDaoCommand()
    {
        //creates a dao record (file, db, etc)
    }

    private static void ListDaoCommand()
    {
        //lists all daos
    }

    private static void ModifyDaoCommand()
    {
        //modifies a dao record (file, db, etc)
    }

    private static void RegisterDaoCommand()
    {
        //submits a dao identity to cardano
    }
}