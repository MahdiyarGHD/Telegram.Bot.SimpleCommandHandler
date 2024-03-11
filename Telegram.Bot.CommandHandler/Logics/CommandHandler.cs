using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.SimpleCommandHandler.Models;
using Telegram.Bot.Types;

namespace Telegram.Bot.SimpleCommandHandler.Logics
{
    public class CommandHandler()
    {
        private Update _update;
        private ITelegramBotClient _botClient;
        private CancellationToken _cancellationToken;

        private readonly List<CommandHandle> CommandHandlers = [];
        public void RegisterCommandHandler(string command, Func<Update, ITelegramBotClient, string[], Task> handler, string? parameters = null)
        {
            var commandHandle = CommandHandlers.Where(x => x.Command.Equals(command)).FirstOrDefault();
            if (commandHandle is not null)
                CommandHandlers.Remove(commandHandle);

            CommandHandlers.Add(new CommandHandle
            {
                Command = command,
                Parameters = parameters,
                Action = handler
            });
        }

        public async Task Resolve(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _update = update;
            _botClient = botClient;
            _cancellationToken = cancellationToken;

            if (update.Message?.Text == null)
                return;

            await HandleCommand(update.Message.Text, cancellationToken);
        }

        private async Task HandleCommand(string messageText, CancellationToken cancellationToken)
        {
            var firstWord = messageText.Split(' ').FirstOrDefault();
            var commandHandle = CommandHandlers.Where(x => x.Command.Equals(firstWord)).FirstOrDefault();
            if (commandHandle is null)
                return;

            List<string> parameters = [];

            if (commandHandle.Parameters is not null)
            {
                string input = messageText.Trim(commandHandle.Command.ToCharArray()).Trim();

                string template = commandHandle.Parameters;

                string[] templateParts = template.Split(["%s"], StringSplitOptions.None);

                int startIndex = 0;
                bool missingPart = false;

                for (int i = 0; i < templateParts.Length; i++)
                {
                    string part = templateParts[i];
                    int index = input.IndexOf(part, startIndex);

                    if (index >= 0)
                    {
                        if (index > startIndex)
                        {
                            parameters.Add(input[startIndex..index]);
                        }

                        startIndex = index + part.Length;
                        missingPart = false;
                    }
                    else if (i != templateParts.Length - 1)
                    {
                        missingPart = true;
                    }
                }

                if (startIndex < input.Length && !missingPart)
                {
                    parameters.Add(input[startIndex..]);
                }
            }

            await commandHandle.Action(_update, _botClient, [.. parameters]);
        }
    }
}