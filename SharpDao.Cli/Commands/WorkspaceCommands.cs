using Cocona;
using SharpDao.Cli.Constants;
using Spectre.Console;

namespace SharpDao.Cli.Commands;

public static class WorkspaceCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("space", sub =>
        {
            sub.AddCommand("create", CreateWorkspaceCommand);
            sub.AddCommand("list", ListWorkspaceCommand);
            sub.AddCommand("remove", RemoveWorkspaceCommand);
        });
    }private static void CreateWorkspaceCommand([Argument] string dao, [Argument] string name)
    {
        var filePath = Path.Combine(CommonConstants.BaseStorage, dao, "workspaces", $"{name}.json");
        if (File.Exists(filePath))
        {
            AnsiConsole.Markup("[red bold]Workspace with name already exists[/]");
            return;
        }

        //TODO: Create Workspace model, fill it out, save it
        AnsiConsole.Markup("[green]Workspace created![/]");
    }

    private static void ListWorkspaceCommand([Argument] string dao)
    {
        var workspaces = Directory.EnumerateDirectories(CommonConstants.BaseStorage, dao);
        // Create a table
        var table = new Table();

        // Add some columns
        table.AddColumn(new TableColumn("Workspaces").Centered());

        // Add some rows
        foreach(var ws in workspaces)
            table.AddRow(new Markup($"[blue]{ws}[/]"));

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    private static void RemoveWorkspaceCommand([Argument] string dao)
    {
        var workspaces = Directory.EnumerateDirectories(CommonConstants.BaseStorage, dao);
        var workspace = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green]Set a Workspace[/]?")
                .AddChoices(workspaces));
        Directory.Delete(Path.Combine(CommonConstants.BaseStorage, workspace));
    }
}