using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.Services.Specifications;
using GymManagementBLL.ViewModels;
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
        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync()
        {
            var sessionSpecs = new SessionWithFilterSpecification();
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(sessionSpecs);

            if (sessions is null || !sessions.Any())
                return [];

            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in mappedSessions)
            {
                var bookingSpecs = new BookingWithFilterSpecification(false, sessionId: session.Id);
                session.AvailableSlots = session.Capacity - await _unitOfWork.GetRepository<Booking>().CountAsync(bookingSpecs);
            }

            return mappedSessions;
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId)
        {
            var sessionSpecs = new SessionWithFilterSpecification(sessionId,true);
            var session = await _unitOfWork.GetRepository<Session>().GetBySpecificationAsync(sessionSpecs);

            if (session is null)
                return null;

            var mappedSessions = _mapper.Map<Session, SessionViewModel>(session);

            var bookingSpecs = new BookingWithFilterSpecification(false, sessionId: session.Id);
            mappedSessions.AvailableSlots = session.Capacity - await _unitOfWork.GetRepository<Booking>().CountAsync(bookingSpecs);

            return mappedSessions;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel input)
        {
            if (!(await IsTrainerExist(input.TrainerId)) || !(await IsCategoryExist(input.CategoryId)) || !IsValidDateRange(input.StartDate, input.EndDate))
                return false;

            var session = _mapper.Map<CreateSessionViewModel, Session>(input);
            await _unitOfWork.GetRepository<Session>().AddAsync(session);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task<bool> UpdateSessionAsync(int id, UpdateSessionViewModel input)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id);

            if (!(await IsSessionAvailableForUpdate(session)) || !(await IsTrainerExist(input.TrainerId)) || !IsValidDateRange(input.StartDate, input.EndDate))
                return false;

            _mapper.Map(input, session);
            session.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.GetRepository<Session>().Update(session);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int id)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id);

            if (session is null)
                return null;

            return _mapper.Map<UpdateSessionViewModel>(session);
        }

        public async Task<bool> RemoveSessionAsync(int id)
        {
            var session = await _unitOfWork.GetRepository<Session>().GetByIdAsync(id);

            if (!(await IsSessionAvailableForRemove(session)))
                return false;

            _unitOfWork.GetRepository<Session>().Delete(session);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }
        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesDropDownAsync()
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerDropDownAsync()
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        #region Helper Methods

        private async Task<bool> IsTrainerExist(int trainerId)
        {

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);

            return trainer is null ? false : true;
        }
        private async Task<bool> IsCategoryExist(int categoryId)
        {
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(categoryId);

            return category is null ? false : true;
        }

        private bool IsValidDateRange(DateTime startDate, DateTime endDate)
        {
            return endDate > startDate && startDate > DateTime.UtcNow;
        }

        private async Task<bool> IsSessionAvailableForUpdate(Session? session)
        {
            if (session is null || session.EndDate < DateTime.UtcNow || session.StartDate <= DateTime.UtcNow)
                return false;

            var bookingSpecs = new BookingWithFilterSpecification(false, sessionId: session.Id);
            var hasActiveBookings = (await _unitOfWork.GetRepository<Booking>().CountAsync(bookingSpecs)) > 0;
            if (hasActiveBookings)
                return false;

            return true;
        }

        private async Task<bool> IsSessionAvailableForRemove(Session? session)
        {
            if (session is null || (session.StartDate <= DateTime.UtcNow && session.EndDate > DateTime.UtcNow))
                return false;

            var bookingSpecs = new BookingWithFilterSpecification(false, sessionId: session.Id);
            var hasActiveBookings = (await _unitOfWork.GetRepository<Booking>().CountAsync(bookingSpecs)) > 0;
            if (hasActiveBookings)
                return false;

            return true;
        }


        #endregion
    }
}
