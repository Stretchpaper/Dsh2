namespace Dsh
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();
            bot.RunAsync().ConfigureAwait(true).GetAwaiter().GetResult();
        }
    }
}
