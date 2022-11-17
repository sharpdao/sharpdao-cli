// See https://aka.ms/new-console-template for more information

using Cocona;
using SharpDao.Cli.Commands;
using SharpDao.Cli.Constants;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

DaoCommands.Configure(app);
IdentityCommands.Configure(app);
ProposalCommands.Configure(app);

if (!Directory.Exists(CommonConstants.BaseStorage))
    Directory.CreateDirectory(CommonConstants.BaseStorage);

if (!Directory.Exists(CommonConstants.DaoStorage))
    Directory.CreateDirectory(CommonConstants.DaoStorage);

if (!Directory.Exists(CommonConstants.IdentityStorage))
    Directory.CreateDirectory(CommonConstants.IdentityStorage);

app.Run();