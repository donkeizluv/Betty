
using System.Threading.Tasks;
using Betty.EFModel;

namespace Betty.Service
{
    public interface IMailerService
    {
       Task MailNewBet(GameOdds game, Register bet);
    }
}