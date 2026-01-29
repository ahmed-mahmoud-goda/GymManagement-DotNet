using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(); 
        Task<SessionViewModel?> GetSessionByIdAsync(int sessionId);
        Task<bool> CreateSessionAsync(CreateSessionViewModel input);
        Task<bool> UpdateSessionAsync(int id,UpdateSessionViewModel input);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int id);
        Task<bool> RemoveSessionAsync(int id);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoriesDropDownAsync();
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerDropDownAsync();
    }
}
