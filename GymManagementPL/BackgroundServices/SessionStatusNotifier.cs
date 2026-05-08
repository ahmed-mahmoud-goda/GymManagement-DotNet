
using GymManagementBLL.Services.Interfaces;
using GymManagementPL.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GymManagementPL.BackgroundServices
{
    public class SessionStatusNotifier : BackgroundService
    {
        private readonly IHubContext<SessionHub> _hub;
        private readonly IServiceScopeFactory _scopeFactory;

        public SessionStatusNotifier(IHubContext<SessionHub> hub, IServiceScopeFactory scopeFactory)
        {
            _hub = hub;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                var sessions = await serviceManager.SessionService.GetChangingSessionsAsync();
                
                foreach (var session in sessions)
                {
                    string status = "";
                    if (session.Status == "Upcoming")
                        status = "Ongoing";
                    else if (session.Status == "Ongoing")
                        status = "Completed";
                    await _hub.Clients.All.SendAsync("SessionStatusChanged", session);
                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
