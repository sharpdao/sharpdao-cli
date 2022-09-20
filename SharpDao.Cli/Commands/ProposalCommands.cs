using Cocona;

namespace SharpDao.Cli.Commands;

public class ProposalCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("proposal", x =>
        {
            x.AddCommand("create", CreateProposalCommand);
            x.AddCommand("list", ListProposalCommand);
            x.AddCommand("vote", VoteProposalCommand);
            x.AddCommand("execute", ExecuteProposalCommand);
        });
    }

    private static void CreateProposalCommand()
    {
        
    }

    private static void ListProposalCommand()
    {
        
    }

    private static void VoteProposalCommand()
    {
        
    }

    private static void ExecuteProposalCommand()
    {
        
    }
}