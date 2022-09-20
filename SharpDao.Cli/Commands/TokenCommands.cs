using Cocona;

namespace SharpDao.Cli.Commands;

public class TokenCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("token", x =>
        {
            x.AddCommand("create", CreateTokenCommand);
            x.AddCommand("list", ListTokenCommand);
            x.AddCommand("mint", MintTokenCommand);
            x.AddCommand("send", SendTokenCommand);
        });
    }

    private static void CreateTokenCommand()
    {
        
    }

    private static void ListTokenCommand()
    {
        
    }

    private static void MintTokenCommand()
    {
        
    }

    private static void SendTokenCommand()
    {
        
    }
}