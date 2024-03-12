# Telegram.Bot command handler
you can easily handle bot commands via this library.

Samples Usage:
```csharp
var botClient = new TelegramBotClient("BOT_TOKEN", httpClient);

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = [] 
};

CommandHandler commandHandler = new();
commandHandler.RegisterCommandHandler(command: "/find", parameters: "%s - %s", handler: MainCommandsController.FindCommand);

botClient.StartReceiving(
    updateHandler: commandHandler.Resolve,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

// ...

public class MainCommandsController
{
  public static async Task FindCommand(Update update, ITelegramBotClient botClient, string[] parameters)
  {
    var musicName = parameters[0];
    var artistName = parameters[1];
    _ = await botClient.SendTextMessageAsync(
        chatId: update?.Message?.Chat.Id,
        text: $"Music Name: {musicName} & Artist Name: {artistName}"
    );
  }
}
```
and when send a message like `/find Youngblood - 5 Seconds of Summer` to bot, you will receive `Music Name: Youngblood & Artist Name: 5 Seconds of Summer`.

Examples:
- https://github.com/MahdiyarGHD/LyricsFinderBot
