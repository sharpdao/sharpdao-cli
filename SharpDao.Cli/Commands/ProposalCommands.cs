using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization.Metadata;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions.Models;
using CardanoSharp.Wallet.Models.Keys;
using Cocona;
using Newtonsoft.Json;
using SharpDao.Cli.Constants;
using SharpDao.Core;
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
            x.AddCommand("execute", ExecuteProposalCommand);
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

                author = FileStorage.ReadAsync<Identity>(authorFile);
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

                dao = FileStorage.ReadAsync<Dao>(
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

                invitee = FileStorage.ReadAsync<Identity>(inviteeFile);
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
        
        switch (daoStorageType)
        {
            case StorageTypes.File:
                var proposalDir = Path.Combine(CommonConstants.DaoStorage, daoId, "proposals");

                Directory.CreateDirectory(Path.Combine(proposalDir, proposal.GetDidId()));

                FileStorage.SaveAsync<Proposal>(proposal,
                    Path.Combine(proposalDir, proposal.GetDidId(), "proposal.json"));
                
                Directory.CreateDirectory(Path.Combine(proposalDir, proposal.GetDidId(), "votes"));
                break;
        }
        
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

                author = FileStorage.ReadAsync<Identity>(authorFile);
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

                dao = FileStorage.ReadAsync<Dao>(
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
        
        switch (daoStorageType)
        {
            case StorageTypes.File:
                var proposalDir = Path.Combine(CommonConstants.DaoStorage, daoId, "proposals");

                Directory.CreateDirectory(Path.Combine(proposalDir, proposal.GetDidId()));

                FileStorage.SaveAsync<Proposal>(proposal,
                    Path.Combine(proposalDir, proposal.GetDidId(), "proposal.json"));
                
                Directory.CreateDirectory(Path.Combine(proposalDir, proposal.GetDidId(), "votes"));
                break;
        }
        
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

                author = FileStorage.ReadAsync<Identity>(authorFile);
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

                dao = FileStorage.ReadAsync<Dao>(
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
        
        switch (daoStorageType)
        {
            case StorageTypes.File:
                var proposalDir = Path.Combine(CommonConstants.DaoStorage, daoId, "proposals");

                Directory.CreateDirectory(Path.Combine(proposalDir, proposal.GetDidId()));

                FileStorage.SaveAsync<Proposal>(proposal,
                    Path.Combine(proposalDir, proposal.GetDidId(), "proposal.json"));
                
                Directory.CreateDirectory(Path.Combine(proposalDir, proposal.GetDidId(), "votes"));
                break;
        }
        
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

    private static void VoteProposalCommand([Argument] string authorDid)
    {
        var daoDid = AnsiConsole.Ask<string>("[green]Dao Did[/]:");
        var proposalDid = AnsiConsole.Ask<string>("[green]Proposal Did[/]:");
        StorageTypes proposalStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Proposal Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Proposal? proposal = null;
        string proposalDir = string.Empty;
        switch (proposalStorageType)
        {
            case StorageTypes.File:
                proposalDir = Path.Combine(CommonConstants.DaoStorage, daoDid.GetDidId(), "proposals", proposalDid.GetDidId());

                proposal = FileStorage.ReadAsync<Proposal>(
                    Path.Combine(proposalDir, "proposal.json"));
                break;
        }
        
        DateTime expirationDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        expirationDate = expirationDate.AddSeconds(int.Parse(proposal.Expires));

        AnsiConsole.Markup($"[grey]Expiration: [blue]{expirationDate.ToString("MM/dd/yyyy")}[/][/]");
        AnsiConsole.Markup($"[grey]Summary: [white]{proposal.Summary}[/][/]");
        ProposalChoice selectedChoice = AnsiConsole.Prompt(
            new SelectionPrompt<ProposalChoice>()
                .Title($"[green]{proposal.Title}[/]:")
                .AddChoices(proposal.PotentialChoices)
                .UseConverter(x =>
                {
                    return x.Choice;
                }));
        
        StorageTypes authorStorage = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Author Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Identity? author = null;
        switch (authorStorage)
        {
            case StorageTypes.File:
                author = FileStorage.ReadAsync<Identity>(
                    Path.Combine(CommonConstants.IdentityStorage, $"{authorDid.GetDidId()}.json"));
                break;
        }

        var vote = new ProposalVote()
        {
            Did = Guid.NewGuid().ToString(),
            ProposalDid = proposalDid,
            SelectedChoiceDids = new List<string>()
            {
                selectedChoice.Did
            },
            Voter = authorDid
        };
        
        var skey = new PrivateKey(Bech32.Decode(author.Skey, out _, out _), null);

        var bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, vote);
        var message = ms.ToArray();
        var sig = skey.Sign(message);

        var signedPayload = new SignedPayload<ProposalVote>()
        {
            Payload = vote,
            Signature = new SignedSignature()
            {
                SignerDid = author.Did,
                Sig = Bech32.Encode(sig, "sig")
            }
        };
        
        switch (authorStorage)
        {
            case StorageTypes.File:
                FileStorage.SaveAsync(signedPayload,
                    Path.Combine(proposalDir, "votes", $"{vote.GetDidId()}.json"));
                break;
        }
        
        Console.WriteLine(JsonSerializer.Serialize(signedPayload));
    }

    private static void ExecuteProposalCommand([Argument] string authorDid)
    {
        var daoDid = AnsiConsole.Ask<string>("[green]Dao Did[/]:");
        var proposalDid = AnsiConsole.Ask<string>("[green]Proposal Did[/]:");
        StorageTypes proposalStorageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Proposal Source[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));

        Dao? dao = null;
        string daoDir = string.Empty;
        switch (proposalStorageType)
        {
            case StorageTypes.File:
                daoDir = Path.Combine(CommonConstants.DaoStorage, daoDid.GetDidId());

                dao = FileStorage.ReadAsync<Dao>(
                    Path.Combine(daoDir, "dao.json"));
                break;
        }

        Proposal? proposal = null;
        string proposalDir = string.Empty;
        switch (proposalStorageType)
        {
            case StorageTypes.File:
                proposalDir = Path.Combine(CommonConstants.DaoStorage, daoDid.GetDidId(), "proposals", proposalDid.GetDidId());

                proposal = FileStorage.ReadAsync<Proposal>(
                    Path.Combine(proposalDir, "proposal.json"));
                break;
        }

        List<SignedPayload<ProposalVote>> votes = new List<SignedPayload<ProposalVote>>();
        switch (proposalStorageType)
        {
            case StorageTypes.File:
                var votesDir = Path.Combine(proposalDir, "votes");
                var voteFiles = Directory.EnumerateFiles(votesDir);
                foreach (var voteFile in voteFiles)
                {
                    var vote = FileStorage.ReadAsync<SignedPayload<ProposalVote>>(voteFile);
                    votes.Add(vote);
                }
                break;
        }
        
        //TODO add validations
        // - voters belong to dao
        // - proposal isnt expired
        // - enough members have voted
        
        AnsiConsole.WriteLine($"{proposal.Title}");
        AnsiConsole.WriteLine($"Choices:");
        List<TalliedVote> tallies = new List<TalliedVote>();
        foreach (var proposalPotentialChoice in proposal.PotentialChoices)
        {
            tallies.Add(new TalliedVote()
            {
                Choice = proposalPotentialChoice,
                Count = 0
            });
        }
        
        foreach (var proposalVote in votes)
        {
            AnsiConsole.WriteLine($"Vote is for Proposal? {proposal.Did == proposalVote.Payload.ProposalDid}");
            List<ProposalChoice> selectedChoices = new List<ProposalChoice>();
            foreach (var selectedChoiceDid in proposalVote.Payload.SelectedChoiceDids)
            {
                var c = proposal.PotentialChoices.FirstOrDefault(x => x.Did == selectedChoiceDid);
                if(c is not null) selectedChoices.Add(c);
            }

            foreach (var selectedChoice in selectedChoices)
            {
                var tally = tallies.FirstOrDefault(x => x.Choice.Did == selectedChoice.Did);
                if (tally is not null)
                {
                    tally.Count += 1;
                }
            }
        }
        
        foreach (var choice in tallies)
        {
            AnsiConsole.WriteLine($" - {choice.Choice.Choice}: {choice.Count}");
        }
        
    }
}