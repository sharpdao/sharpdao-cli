using System.Text.Json.Serialization.Metadata;
using Cocona;
using Newtonsoft.Json;
using SharpDao.Cli.Constants;
using SharpDao.Core.Enums;
using SharpDao.Core.Extensions;
using SharpDao.Core.Models;
using SharpDao.Core.Proposals;
using SharpDao.Core.Storage;
using Spectre.Console;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;
using JsonSerializer = System.Text.Json.JsonSerializer;
using RequestMembershipProposal = SharpDao.Core.Models.RequestMembershipProposal;

namespace SharpDao.Cli.Commands;

public class ProposalCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("proposal", x =>
        {
            x.AddSubCommand("create", sub =>
            {
                sub.AddCommand("invite-member", InviteProposalCommand);
                sub.AddCommand("request-membership", RequestMembershipProposalCommand);
                sub.AddCommand("question", QuestionProposalCommand);
            });
            x.AddCommand("list", ListProposalCommand);
            x.AddCommand("vote", VoteProposalCommand);
        });
    }

    private static void InviteProposalCommand([Argument] string authorDid)
    {
        //Types
        // Invite Member to Dao
        StorageTypes authorStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Author Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Identity? author = null;
        switch (authorStorageType)
        {
            case StorageTypes.File:
                var authorId = authorDid.GetDidId();
                var authorFile = Path.Combine(CommonConstants.IdentityStorage, $"{authorId}.json");
                if (!File.Exists(authorFile))
                {
                    AnsiConsole.Markup("[red]Unable to find Identity[/]");
                    return;
                }

                author = FileStorage.ReadIdentityAsync(authorFile);
                break;
        }
        
        StorageTypes daoStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]DAO Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        var daoDid = AnsiConsole.Ask<string>("[green]DAO DID[/]:");
        var daoId = daoDid.GetDidId();
        Dao? dao = null;
        switch (daoStorageType)
        {
            case StorageTypes.File:
                var daoDir = Path.Combine(CommonConstants.DaoStorage, daoId);
                if (!Directory.Exists(daoDir))
                {
                    AnsiConsole.Markup("[red]Unable to find Dao[/]");
                    return;
                }

                dao = FileStorage.ReadDaoAsync(
                    Path.Combine(daoDir, "dao.json"));
                
                break;
        }
        var inviteeDid = AnsiConsole.Ask<string>("[green]New Member DID[/]:");
        StorageTypes inviteeStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Invitee Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Identity? invitee = null;
        switch (inviteeStorageType)
        {
            case StorageTypes.File:
                var inviteeId = inviteeDid.GetDidId();
                var inviteeFile = Path.Combine(CommonConstants.IdentityStorage, $"{inviteeId}.json");
                if (!File.Exists(inviteeFile))
                {
                    AnsiConsole.Markup("[red]Unable to find Identity[/]");
                    return;
                }

                invitee = FileStorage.ReadIdentityAsync(inviteeFile);
                break;
        }

        var expirationDate = DateTime.UtcNow.AddDays(int.Parse(dao.VotingDays));
        var unixTime = (Int32)(expirationDate.Subtract(new
            DateTime(1970, 1, 1))).TotalSeconds;
        var proposal = new InviteMemberProposal()
        {
            Did = Guid.NewGuid().ToString(),
            Title = $"DAO Invite Member for {dao.Name}",
            AuthorDids = new List<string>() { authorDid },
            InviteeDid = inviteeDid,
            DaoDid = daoDid,
            Summary = $"Do you approve the membership of {invitee.Name}",
            Expires = unixTime.ToString()
        };
        proposal.CreateChoices();
        
        Console.WriteLine(JsonSerializer.Serialize(proposal));
    }

    private static void RequestMembershipProposalCommand([Argument] string authorDid)
    {
        //Types
        // Request To Join Dao
        StorageTypes authorStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Author Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Identity? author = null;
        switch (authorStorageType)
        {
            case StorageTypes.File:
                var authorId = authorDid.GetDidId();
                var authorFile = Path.Combine(CommonConstants.IdentityStorage, $"{authorId}.json");
                if (!File.Exists(authorFile))
                {
                    AnsiConsole.Markup("[red]Unable to find Identity[/]");
                    return;
                }

                author = FileStorage.ReadIdentityAsync(authorFile);
                break;
        }
        
        StorageTypes daoStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]DAO Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        var daoDid = AnsiConsole.Ask<string>("[green]DAO DID[/]:");
        var daoId = daoDid.GetDidId();
        Dao? dao = null;
        switch (daoStorageType)
        {
            case StorageTypes.File:
                var daoDir = Path.Combine(CommonConstants.DaoStorage, daoId);
                if (!Directory.Exists(daoDir))
                {
                    AnsiConsole.Markup("[red]Unable to find Dao[/]");
                    return;
                }

                dao = FileStorage.ReadDaoAsync(
                    Path.Combine(daoDir, "dao.json"));
                
                break;
        }

        var expirationDate = DateTime.UtcNow.AddDays(int.Parse(dao.VotingDays));
        var unixTime = (Int32)(expirationDate.Subtract(new
            DateTime(1970, 1, 1))).TotalSeconds;
        var proposal = new RequestMembershipProposal()
        {
            Did = Guid.NewGuid().ToString(),
            Title = $"DAO Membership Request for {dao.Name}",
            AuthorDids = new List<string>() { authorDid },
            RequestorDid = authorDid,
            DaoDid = daoDid,
            Summary = $"Do you approve the membership of {author.Name}",
            Expires = unixTime.ToString()
        };
        proposal.CreateChoices();
        
        Console.WriteLine(JsonSerializer.Serialize(proposal));
    }

    private static void QuestionProposalCommand([Argument] string authorDid)
    {
        //Types
        // Question: Yes/No, Multiple Choice
        StorageTypes authorStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Author Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Identity? author = null;
        switch (authorStorageType)
        {
            case StorageTypes.File:
                var authorId = authorDid.GetDidId();
                var authorFile = Path.Combine(CommonConstants.IdentityStorage, $"{authorId}.json");
                if (!File.Exists(authorFile))
                {
                    AnsiConsole.Markup("[red]Unable to find Identity[/]");
                    return;
                }

                author = FileStorage.ReadIdentityAsync(authorFile);
                break;
        }
        
        StorageTypes daoStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]DAO Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        var daoDid = AnsiConsole.Ask<string>("[green]DAO DID[/]:");
        var daoId = daoDid.GetDidId();
        Dao? dao = null;
        switch (daoStorageType)
        {
            case StorageTypes.File:
                var daoDir = Path.Combine(CommonConstants.DaoStorage, daoId);
                if (!Directory.Exists(daoDir))
                {
                    AnsiConsole.Markup("[red]Unable to find Dao[/]");
                    return;
                }

                dao = FileStorage.ReadDaoAsync(
                    Path.Combine(daoDir, "dao.json"));
                
                break;
        }

        var question = AnsiConsole.Ask<string>("[green]Question[/]:");
        var details = AnsiConsole.Ask<string>("[green]Background Context[/]:");
        
        QuestionaireTypes mQuestionaireTypes = AnsiConsole.Prompt(
            new SelectionPrompt<QuestionaireTypes>()
                .Title("[green]Author Source[/]:")
                .AddChoices(new[] {
                    QuestionaireTypes.YesNo,
                    QuestionaireTypes.MultipleChoice
                }));
        
        var expirationDate = DateTime.UtcNow.AddDays(int.Parse(dao.VotingDays));
        var unixTime = (Int32)(expirationDate.Subtract(new
            DateTime(1970, 1, 1))).TotalSeconds;
        var proposal = new QuestionaireProposal()
        {
            Did = Guid.NewGuid().ToString(),
            Title = question,
            AuthorDids = new List<string>() { authorDid },
            DaoDid = daoDid,
            Summary = details,
            Expires = unixTime.ToString(),
            Type = mQuestionaireTypes
        };
        
        switch (proposal.Type)
        {
            case QuestionaireTypes.MultipleChoice:
                proposal.MultipleChoiceOptions = AnsiConsole
                    .Ask<string>("[green]Multiple Choice Options[/] (separate by ;):")
                    .Split(";")
                    .ToList();
                break;
        }
        
        proposal.CreateChoices();
        
        Console.WriteLine(JsonSerializer.Serialize(proposal));
    }

    private static void ListProposalCommand()
    {
        var daos = FileStorage.ReadAllDaoAsync(CommonConstants.DaoStorage);
        
        Dao dao = AnsiConsole.Prompt(
            new SelectionPrompt<Dao>()
                .Title("[green]Author Source[/]:")
                .AddChoices(daos)
                .UseConverter(x =>
                {
                    return x.Name;
                }));
        
    }

    private static void VoteProposalCommand()
    {
        
    }

    private static void ExecuteProposalCommand()
    {
        
    }
}