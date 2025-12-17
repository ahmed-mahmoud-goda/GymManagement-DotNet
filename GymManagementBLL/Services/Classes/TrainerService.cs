using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModel;
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

        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var trainers = _unitOfWork.GetRepository<Trainer>().GetAll().ToList() ?? [];

            if (trainers is null || !trainers.Any()) return [];

            var trainerViewModels = _mapper.Map<IEnumerable<Trainer>,IEnumerable<TrainerViewModel>>(trainers);

            return trainerViewModels;
        }

        public TrainerViewModel? GetTrainerDetails(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            if (trainer is null) return null;

            var trainerViewModel = _mapper.Map<TrainerViewModel>(trainer);

            return trainerViewModel;
        }

        public bool CreateTrainer(CreateTrainerViewModel model)
        {
            try
            {
                if(IsEmailExists(model.Email) || IsPhoneExists(model.Phone))
                    return false;

                var trainer = _mapper.Map<Trainer>(model);

                _unitOfWork.GetRepository<Trainer>().Add(trainer);

                _unitOfWork.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            if (trainer is null) return null;

            var trainerToUpdate = _mapper.Map<TrainerToUpdateViewModel>(trainer);

            return trainerToUpdate;
        }

        public bool UpdateTrainerDetails(TrainerToUpdateViewModel model,int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            if(trainer is null) return false;

            if (IsEmailExists(model.Email) || IsPhoneExists(model.Phone))
                return false;

            _mapper.Map(model,trainer);

            _unitOfWork.GetRepository<Trainer>().Update(trainer);
            _unitOfWork.SaveChanges();

            return true;
        }

        public bool RemoveTrainer(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            if(trainer is null) return false; 

            var activeSessions = _unitOfWork.GetRepository<Session>().GetAll(x=>x.TrainerId == trainerId && x.StartDate > DateTime.Now);

            if (activeSessions.Any())
            {
                return false;
            }

            _unitOfWork.GetRepository<Trainer>().Delete(trainer);
            _unitOfWork.SaveChanges();

            return true;
        }

        #region Helper Methods
        private bool IsEmailExists(string email)
        {
            var existingMember = _unitOfWork.GetRepository<Trainer>().GetAll(x => x.Email == email);
            return existingMember is not null && existingMember.Any();
        }
        private bool IsPhoneExists(string phone)
        {
            var existingMember = _unitOfWork.GetRepository<Trainer>().GetAll(x => x.Phone == phone);
            return existingMember is not null && existingMember.Any();
        }
        #endregion
    }
}
