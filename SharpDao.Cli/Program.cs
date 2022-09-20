// See https://aka.ms/new-console-template for more information

using Cocona;
using SharpDao.Cli.Commands;

var builder = CoconaApp.CreateBuilder();

var app = builder.Build();

DaoCommands.Configure(app);

app.Run();