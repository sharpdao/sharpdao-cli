using System.Text.Json;
using CardanoSharp.Wallet;
using CardanoSharp.Wallet.Encoding;
using CardanoSharp.Wallet.Extensions;
using CardanoSharp.Wallet.Extensions.Models;
using Cocona;
using Spectre.Console;
using CardanoSharp.Wallet.Models.Keys;
using SharpDao.Cli.Constants;
using SharpDao.Core.Enums;
using SharpDao.Core.Models;
using SharpDao.Core.Storage;

namespace SharpDao.Cli.Commands;

public static class IdentityCommands
{
    public static void Configure(CoconaApp app)
    {
        app.AddSubCommand("identity", x =>
        {
            x.AddCommand("create", CreateIdentityCommand);
            x.AddCommand("set", SetConfigCommand);
        });
    }

    private static void CreateIdentityCommand()
    {
        var identity = new Identity();
        identity.Name = AnsiConsole.Ask<string>("[green]Name of Identity[/]?");
        if (File.Exists(Path.Combine(CommonConstants.IdentityStorage, $"{identity.Name}.json")))
        {
            AnsiConsole.Markup("[red bold]Identity with name already exists[/]");
            return;
        }

        var keyPair = KeyPair.GenerateKeyPair();

        identity.Vkey = Bech32.Encode(keyPair.PublicKey.Key, "vk");
        identity.Skey = Bech32.Encode(keyPair.PrivateKey.Key, "sk");
        
        identity.Did = Guid.NewGuid().ToString();
        
        StorageTypes storageType = AnsiConsole.Prompt(
            new SelectionPrompt<StorageTypes>()
                .Title("[green]Storage Type[/]:")
                .AddChoices(new[] {
                    StorageTypes.File
                }));
        

        switch (storageType)
        {
            case StorageTypes.File:
                FileStorage.SaveIdentityAsync(identity, Path.Combine(CommonConstants.IdentityStorage, $"{identity.GetId()}.json"));
                break;
        }

        var json = JsonSerializer.Serialize(
            identity, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
        Console.WriteLine(json);

        // var message = "SharpDao";
        // var messageByte = message.HexToByteArray();
        //
        // var skey = Bech32.Decode(identity.Skey, out _, out _);
        // var pvkey = new PrivateKey(skey, null);
        // var sig = pvkey.Sign(messageByte);
        //
        // var deVkey = Bech32.Decode(identity.Vkey, out _, out _);
        // var pubKey = new PublicKey(deVkey, null);
        // var verified = pubKey.Verify(messageByte, sig);
        // Console.WriteLine($"Verified: {verified}");
    }

    private static void SetConfigCommand()
    {
        //submits a dao identity to cardano
    }
}