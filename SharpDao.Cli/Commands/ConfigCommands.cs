using Cocona;

namespace SharpDao.Cli.Commands;

public static class ConfigCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("config", x =>
        {
            x.AddSubCommand("config", sub =>
            {
                sub.AddCommand("list", ListConfigCommand);
                sub.AddCommand("set", SetConfigCommand);
            });
        });
    }

    private static void ListConfigCommand([Argument] string dao)
    {
        //submits a dao identity to cardano
    }

    private static void SetConfigCommand([Argument] string dao)
    {
        //submits a dao identity to cardano
    }
}