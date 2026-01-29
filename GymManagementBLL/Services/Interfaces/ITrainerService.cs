using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.ViewModels;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync();
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel model);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId);
        Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model);
        Task<bool> RemoveTrainerAsync(int trainerId);
    }
}
