using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.Services.Specifications;
using GymManagementBLL.ViewModels;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementDAL.Data.Repositories.Interfaces;
using GymManagementDAL.Entities;

namespace GymManagementBLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MemberSessionViewModel> GetSessionsAsync()
        {
            var sessionsSpecs = new SessionWithFilterSpecification();
            var sessions = await _unitOfWork.GetRepository<Session>().GetAllAsync(sessionsSpecs);

            var mappedSessions = _mapper.Map<IEnumerable<BookingViewModel>>(sessions);

            foreach (var session in mappedSessions)
            {
                var bookingSpecs = new BookingWithFilterSpecification(false, sessionId: session.Id);
                session.AvailableSlots = session.Capacity - await _unitOfWork.GetRepository<Booking>().CountAsync(bookingSpecs);
            }
            return new MemberSessionViewModel
            {
                UpcomingSessions = mappedSessions
            .Where(s => s.StartDate > DateTime.Now)
            .ToList(),

                OngoingSessions = mappedSessions
            .Where(s => DateTime.Now >= s.StartDate && DateTime.Now <= s.EndDate)
            .ToList()
            };
        }
        public async Task<IEnumerable<MemberBookingViewModel>> GetMembersBookingsaAsync(int sessionId)
        {
            var bookingSpec = new BookingWithFilterSpecification(false,sessionId:sessionId);
            var bookings = await _unitOfWork.GetRepository<Booking>().GetAllAsync(bookingSpec);
            var mapped = _mapper.Map<IEnumerable<MemberBookingViewModel>>(bookings);
            return mapped;
        }
        public async Task<IEnumerable<MemberSelectViewModel>> GetMemberDropDownAsync(int sessionId)
        {
            var memberSpec = new MemberWithFilterSpecification(sessionId);
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(memberSpec);
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }
        public async Task<bool> CreateBookingAsync(CreateBookingViewModel input)
        {
            var sessionSpecs = new SessionWithFilterSpecification(input.SessionId, false);
            var session = await _unitOfWork.GetRepository<Session>().GetBySpecificationAsync(sessionSpecs);

            if (session is null)
                return false;

            var bookingSpecs = new BookingWithFilterSpecification(false, sessionId: input.SessionId);
            if((session.Capacity - await _unitOfWork.GetRepository<Booking>().CountAsync(bookingSpecs)) <= 0)
            {
                return false;
            }

            var booking = _mapper.Map<Booking>(input);
            await _unitOfWork.GetRepository<Booking>().AddAsync(booking);
            return (await _unitOfWork.SaveChangesAsync())>0;
        }
        public async Task<bool> CancelBookingAsync(int memberId,int sessionId)
        {
            var bookingSpec = new BookingWithFilterSpecification(false, memberId, sessionId);
            var booking = await _unitOfWork.GetRepository<Booking>().GetBySpecificationAsync(bookingSpec);
            if (booking is null) return false;
            _unitOfWork.GetRepository<Booking>().Delete(booking);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }
        public async Task<bool> AttendSessionAsync(int memberId,int sessionId)
        {
            var bookingSpec = new BookingWithFilterSpecification(false, memberId, sessionId);
            var booking = await _unitOfWork.GetRepository<Booking>().GetBySpecificationAsync(bookingSpec);
            if (booking is null) return false;
            booking.IsAttended = true;
            _unitOfWork.GetRepository<Booking>().Update(booking);
            return (await _unitOfWork.SaveChangesAsync()) > 0;
        }
    }
}
