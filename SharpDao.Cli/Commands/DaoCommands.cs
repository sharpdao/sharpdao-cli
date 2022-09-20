using Cocona;

namespace SharpDao.Cli.Commands;

public static class DaoCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("dao", x =>
        {
            x.AddCommand("create", CreateDaoCommand);
            x.AddCommand("modify", ModifyDaoCommand);
            x.AddCommand("register", ModifyDaoCommand);
        });
    }

    public static void CreateDaoCommand()
    {
        
    }

    public static void ModifyDaoCommand()
    {
        
    }
}