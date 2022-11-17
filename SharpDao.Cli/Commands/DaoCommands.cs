using System.ComponentModel;
using System.Text.Json;
using System.Xml;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Models.Keys;
using Cocona;
using SharpDao.Cli.Constants;
using SharpDao.Core.Enums;
using SharpDao.Core.Models;
using SharpDao.Core.Storage;
using Spectre.Console;

namespace SharpDao.Cli.Commands;

public static class DaoCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("dao", x =>
        {
            x.AddCommand("create", CreateDaoCommand);
            x.AddCommand("modify", ModifyDaoCommand);
            x.AddCommand("register", RegisterDaoCommand);
        });
    }

    private static void CreateDaoCommand()
    {
        //creates a dao record (file, db, etc)
        var dao = new Dao();
        
        dao.Name = AnsiConsole.Ask<string>("[green]Name[/]:");
        dao.AlternateName = AnsiConsole.Ask<string>("[green]Alternate Name[/]:");
        dao.Description = AnsiConsole.Ask<string>("[green]Short Description[/]:");
        dao.Logo = AnsiConsole.Ask<string>("[green]Logo[/] (public url please):");
        dao.Urls = AnsiConsole.Ask<string>("[green]Website/Github/Wiki Urls[/] (separate by ;):").Split((";"));
        dao.Quorum = AnsiConsole.Ask<string>("[green]Quorum[/] (1% - 100%):");
        dao.PassRate = AnsiConsole.Ask<string>("[green]Passing Rate[/] (1% - 100%):");
        dao.VotingDays = AnsiConsole.Ask<string>("[green]Voting Days[/]:");
        dao.ManagerDids = AnsiConsole.Ask<string>("[green]Founder Did(s)[/] (separate by ;):").Split(";");
        StorageTypes storageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Storage Type[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));
        
        var keyPair = KeyPair.GenerateKeyPair();

        dao.Vkey = Bech32.Encode(keyPair.PublicKey.Key, "vk");
        dao.Skey = Bech32.Encode(keyPair.PrivateKey.Key, "sk");
 
        dao.Did = Guid.NewGuid().ToString();
        
        switch (storageType)
        {
            case StorageTypes.File:
                var dirPath = Path.Combine(CommonConstants.DaoStorage, dao.GetId());
                if (Directory.Exists(dirPath))
                {
                    AnsiConsole.Markup("[red bold]DAO with name already exists[/]");
                    return;
                }
                
                Directory.CreateDirectory(dirPath);
                FileStorage.SaveDaoAsync(dao, Path.Combine(dirPath, "dao.json"));
                Directory.CreateDirectory(Path.Combine(dirPath, "workspaces"));
                break;
        }
        
        var json = JsonSerializer.Serialize(
            dao, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        AnsiConsole.Markup("[green bold]Saving Dao...[/]");
        AnsiConsole.WriteLine(json);
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