using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels.SessionViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ISessionService
    {
        IEnumerable<SessionViewModel> GetAllSessions(); 
        SessionViewModel? GetSessionById(int sessionId);
        bool CreateSession(CreateSessionViewModel input);
        bool UpdateSession(int id,UpdateSessionViewModel input);
        UpdateSessionViewModel? GetSessionToUpdate(int id);
        bool RemoveSession(int id);
    }
}
