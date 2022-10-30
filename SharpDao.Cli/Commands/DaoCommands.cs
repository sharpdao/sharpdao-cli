using System.Text.Json;
using System.Xml;
using Cocona;
using SharpDao.Core.Models;
using Spectre.Console;

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
        var dao = new Dao();
        
        dao.Name = AnsiConsole.Ask<string>("[green]Name[/]:");
        dao.AlternateName = AnsiConsole.Ask<string>("[green]Alternate Name[/]:");
        dao.Description = AnsiConsole.Ask<string>("[green]Short Description[/]:");
        dao.Logo = AnsiConsole.Ask<string>("[green]Logo[/] (public url please):");
        dao.Urls = (AnsiConsole.Ask<string>("[green]Website/Github/Wiki Urls[/] (separate by ;):")).Split((";"));
        dao.Quorum = AnsiConsole.Ask<string>("[green]Quorum[/] (1% - 100%):");
        dao.PassRate = AnsiConsole.Ask<string>("[green]Passing Rate[/] (1% - 100%):");
        dao.VotingDays = AnsiConsole.Ask<string>("[green]Voting Days[/]:");

        var json = JsonSerializer.Serialize(
            dao, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        Console.WriteLine(json);
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