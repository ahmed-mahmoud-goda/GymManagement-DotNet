using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.Services.Specifications;
using GymManagementBLL.ViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync()
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

            var trainerViewModels = _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);

            return trainerViewModels;
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);

            if (trainer is null) return null;

            var trainerViewModel = _mapper.Map<TrainerViewModel>(trainer);

            return trainerViewModel;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model)
        {
            try
            {
                if(await IsEmailOrPhoneExistsAsync(model.Email,model.Phone))
                    return false;

                var trainer = _mapper.Map<Trainer>(model);

                await _unitOfWork.GetRepository<Trainer>().AddAsync(trainer);

                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);

            if (trainer is null) return null;

            var trainerToUpdate = _mapper.Map<TrainerToUpdateViewModel>(trainer);

            return trainerToUpdate;
        }

        public async Task<bool> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);

            if(trainer is null) return false;

            if (await IsEmailOrPhoneExistsAsync(model.Email,model.Phone,trainerId))
                return false;

            _mapper.Map(model,trainer);

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);

            if(trainer is null) return false;

            var sessionsSpecs = new SessionWithFilterSpecification(trainerId);
            var activeSessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(sessionsSpecs);

            if (activeSessions.Any())
            {
                return false;
            }

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        #region Helper Methods
        private async Task<bool> IsEmailOrPhoneExistsAsync(string email, string phone, int? id = null)
        {
            var trainerSpecs = new TrainerWithFilterSpecification(email, phone, id);
            var existingMember = await _unitOfWork.GetRepository<Trainer>().GetBySpecificationAsync(trainerSpecs);
            return existingMember != null;
        }
        #endregion
    }
}
