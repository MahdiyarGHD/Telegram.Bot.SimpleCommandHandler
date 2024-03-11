using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.Bot.SimpleCommandHandler.Models
{
    public class CommandHandle
    {
        public Func<Update, ITelegramBotClient, string[], Task> Action { get; set; }
        public string Command { get; set; }
        public string? Parameters { get; set; }
    }
}
