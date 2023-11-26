using Game;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(builder =>
    {
        builder.Sources.Clear();
        builder.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((hostContext, services) =>
    {
        var hiLoGameOptions = hostContext.Configuration.GetSection("HiLoGameConfig").Get<HiLoGameOptions>();

        _ = services
            .AddSingleton(hiLoGameOptions!)
            .AddSingleton<IMessage, ConsoleMessage>();
    })
    .Build();

_ = new HiLoGame(host.Services.GetRequiredService<HiLoGameOptions>(), host.Services.GetRequiredService<IMessage>(), new HiLoGameInfo());
