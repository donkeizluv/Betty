using System.Threading.Tasks;
using Betty.DTO;
using Betty.ViewModels;

namespace Betty.Service
{
    public interface IBetService
    {
        Task<BetVM> GetVM();
        Task Cancel(CancelDto cancel);
        Task Create(BetDto bet);
    }
}