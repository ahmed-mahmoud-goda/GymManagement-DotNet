using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModel;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;
using GymManagementDAL.Entities.Enum;

namespace GymManagementBLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<TrainerViewModel> GetAllTrainers()
        {
            var trainers = _unitOfWork.GetRepository<Trainer>().GetAll();

            if (trainers is null || !trainers.Any()) return [];

            var trainerViewModels = trainers.Select(x => new TrainerViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Phone = x.Phone,
                Specialities = x.Specialities.ToString(),
            });

            return trainerViewModels;
        }

        public TrainerViewModel? GetTrainerDetails(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            if (trainer is null) return null;

            var trainerViewModel = new TrainerViewModel
            {
                Id = trainer.Id,
                Name = trainer.Name,
                Email = trainer.Email,
                Phone = trainer.Phone,
                DateOfBirth = trainer.DateOfBirth.ToShortDateString(),
                Specialities = trainer.Specialities.ToString(),
                Address = FormatAddress(trainer.Address),
            };

            return trainerViewModel;
        }

        public bool CreateTrainer(CreateTrainerViewModel model)
        {
            try
            {
                if(IsEmailExists(model.Email) || IsPhoneExists(model.Phone))
                    return false;

                var trainer = new Trainer
                {
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Address = new Address
                    {
                        BuildingNumber = model.BuildingNumber,
                        Street = model.Street,
                        City = model.City,
                    },
                    Specialities = model.Specialities
                };

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

            var trainerToUpdate = new TrainerToUpdateViewModel
            {
                Email = trainer.Email,
                Phone = trainer.Phone,
                BuildingNumber = trainer.Address.BuildingNumber,
                City = trainer.Address.City,
                Street = trainer.Address.Street,
                Specialities = trainer.Specialities.ToString()
            };

            return trainerToUpdate;
        }

        public bool UpdateTrainerDetails(TrainerToUpdateViewModel model,int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            if(trainer is null) return false;

            if (IsEmailExists(model.Email) || IsPhoneExists(model.Phone))
                return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.BuildingNumber = model.BuildingNumber;
            trainer.Address.Street = model.Street;
            trainer.Address.City = model.City;
            trainer.Specialities = (Specialities) Enum.Parse(typeof(Specialities),model.Specialities,true);

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

        private string FormatAddress(Address address)
        {
            if (address is null)
                return "N/A";
            else
                return $"{address.BuildingNumber}, {address.Street}, {address.City}";
        }

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
