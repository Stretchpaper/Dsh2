using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsh.Extennal_Classes
{
    public class GlobalClass
    {
        public string UserRes { get; set; }
        public string BotRes { get; set; }
        public List<string> bedWords = new List<string>();
        public GlobalClass()
        {
            var ranbot = new Random().Next(1, 4);
            switch (ranbot)
            {
                case 1:
                    BotRes = "Ножиці";
                    break;
                case 2:
                    BotRes = "Папір";
                    break;
                case 3:
                    BotRes = "Камінь";
                    break;
            }
        }
    }
}
