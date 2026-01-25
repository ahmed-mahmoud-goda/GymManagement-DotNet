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
        IEnumerable<TrainerViewModel> GetAllTrainers();
        TrainerViewModel? GetTrainerDetails(int trainerId);
        bool CreateTrainer(CreateTrainerViewModel model);
        TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId);
        bool UpdateTrainerDetails(int trainerId, TrainerToUpdateViewModel model);
        bool RemoveTrainer(int trainerId);
    }
}
