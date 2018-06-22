
using System.Threading.Tasks;
using Betty.EFModel;

namespace Betty.Service
{
    public interface IMailerService
    {
       void MailNewBet(GameOdds game, Register bet);
       void MailCancelBet(Register bet);
    }
}