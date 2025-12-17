using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<SessionViewModel> GetAllSessions()
        {
            var sessions = _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategory().OrderByDescending(x=>x.StartDate);

            if (sessions is null || !sessions.Any())
                return [];
            
            var mappedSessions = _mapper.Map<IEnumerable<Session>,IEnumerable<SessionViewModel>>(sessions);

            foreach(var session in mappedSessions)
            {
                session.AvailableSlots = session.Capacity - _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id);
            }

            return mappedSessions;
        }

        public SessionViewModel? GetSessionById(int sessionId)
        {
            var session = _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(sessionId);

            if (session is null)
                return null;

            var mappedSessions = _mapper.Map<Session, SessionViewModel>(session);

            mappedSessions.AvailableSlots = session.Capacity - _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id);

            return mappedSessions;
        }

        public bool CreateSession(CreateSessionViewModel input)
        {
            if (!IsTrainerExist(input.TrainerId) || !IsCategoryExist(input.CategoryId) || !IsValidDateRange(input.StartDate,input.EndDate))
                return false;

            var session = _mapper.Map<CreateSessionViewModel,Session>(input);
            _unitOfWork.GetRepository<Session>().Add(session);
            return _unitOfWork.SaveChanges() > 0;
        }

        public bool UpdateSession(int id, UpdateSessionViewModel input)
        {
            var session = _unitOfWork.GetRepository<Session>().GetById(id);

            if (!IsSessionAvailableForUpdate(session) || !IsTrainerExist(input.TrainerId) || !IsValidDateRange(input.StartDate, input.EndDate))
                return false;

            _mapper.Map(input, session);
            session.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Session>().Update(session);
            return _unitOfWork.SaveChanges() > 0;
        }

        public UpdateSessionViewModel? GetSessionToUpdate(int id)
        {
            var session = _unitOfWork.GetRepository<Session>().GetById(id);

            if (session is null)
                return null;

            return _mapper.Map<UpdateSessionViewModel>(session);
        }

        public bool RemoveSession(int id)
        {
            var session = _unitOfWork.GetRepository<Session>().GetById(id);

            if(!IsSessionAvailableForRemove(session))
                return false;

            _unitOfWork.GetRepository<Session>().Delete(session);
            return _unitOfWork.SaveChanges() > 0;
        }

        #region Helper Methods

        private bool IsTrainerExist(int trainerId)
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetById(trainerId);

            return trainer is null ? false : true;
        }
        private bool IsCategoryExist(int categoryId)
        {
            var category = _unitOfWork.GetRepository<Category>().GetById(categoryId);

            return category is null ? false : true;
        }

        private bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return endDate > startDate && startDate > DateTime.UtcNow;
        }

        private bool IsSessionAvailableForUpdate(Session session)
        {
            if(session is null || session.EndDate < DateTime.UtcNow || session.StartDate <= DateTime.UtcNow)
                return false;

            var hasActiveBookings = _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id) > 0;
            if (hasActiveBookings)
                return false;

            return true;
        }

        private bool IsSessionAvailableForRemove(Session session)
        {
            if (session is null || session.StartDate > DateTime.UtcNow || (session.StartDate <= DateTime.UtcNow && session.EndDate > DateTime.UtcNow))
                return false;

            var hasActiveBookings = _unitOfWork.SessionRepository.GetCountOfBookedSlots(session.Id) > 0;
            if (hasActiveBookings)
                return false;

            return true;
        }

        #endregion
    }
}
