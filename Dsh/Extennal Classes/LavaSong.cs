using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsh.Extennal_Classes
{
    internal class LavaSong
    {
        public LavaSong(LavalinkTrack track, DiscordChannel requestChannel)
        {
            this.lavaTrack = track;
            this.requestChannel = requestChannel;
        }

        public LavalinkTrack lavaTrack;
        public DiscordChannel requestChannel;
        public InteractionContext ctx;
    }
}
