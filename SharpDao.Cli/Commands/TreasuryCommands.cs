using Cocona;

namespace SharpDao.Cli.Commands;

public class TreasuryCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("treasury", x =>
        {
            x.AddCommand("init", InitTreasuryCommand);
            x.AddCommand("add-member", AddMemberTreasuryCommand);
            x.AddCommand("create", CreateTreasuryCommand);
            x.AddCommand("list", ListTreasuryCommand);
        });
    }

    private static void InitTreasuryCommand()
    {   
        //start building a treasury
    }

    private static void AddMemberTreasuryCommand()
    {
        //add a member to a treasury
    }

    private static void CreateTreasuryCommand()
    {
        //create the treasury into a policy script / multisig -> contract address
    }

    private static void ListTreasuryCommand()
    {
        //lists all treasuries
    }
}