using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using SolarService.Models;

namespace SolarService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly SolarContext db = new SolarContext();
        private bool powerInMW = true;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override async Task GetUsersAsync(EmptyRequest request, IServerStreamWriter<User> responseStream, ServerCallContext context)
        {
            List<User> users = db.Users.ToList();

            foreach (User user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }

        public override Task<SuccessResponse> RemoveUser(TelegramAuthorisedUser request, ServerCallContext context)
        {
            try
            {
                db.TelegramAuthorisedUsers.Remove(request);
                User temp = db.Users.Where(x => x.Login == request.UserLogin).First();
                db.Users.Remove(temp);
                return Task.FromResult(new SuccessResponse()
                {
                    Success = true
                });
            }
            catch
            {

                return Task.FromResult(new SuccessResponse()
                {
                    Success = false
                });
            }
        }

        public override async Task GetRolesAsync(EmptyRequest request, IServerStreamWriter<Role> responseStream, ServerCallContext context)
        {
            List<Role> roles = db.Roles.ToList();

            foreach (Role role in roles)
            {
                await responseStream.WriteAsync(role);
            }
        }

        public override async Task GetTelegramAuthorisedUsersAsync(EmptyRequest request, IServerStreamWriter<TelegramAuthorisedUser> responseStream, ServerCallContext context)
        {
            List<TelegramAuthorisedUser> users = db.TelegramAuthorisedUsers.ToList();

            foreach (TelegramAuthorisedUser user in users)
            {
                await responseStream.WriteAsync(user);
            }
        }

        public override async Task GetEventsAsync(EmptyRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventTypesAsync(EmptyRequest request, IServerStreamWriter<EventType> responseStream, ServerCallContext context)
        {
            List<EventType> eventTypes = db.EventTypes.ToList();

            foreach (EventType item in eventTypes)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByTypeAsync(EventsByTypeRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.EventTypeId == request.TypeId).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetStationsAsync(EmptyRequest request, IServerStreamWriter<SolarStation> responseStream, ServerCallContext context)
        {
            List<SolarStation> stations = db.SolarStations.ToList();

            foreach (SolarStation item in stations)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetAllInvertorsAsync(EmptyRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.ToList();

            foreach (Invertor item in invertors)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy *= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetInvertorsOnStationAsync(InvertorsOnStationRequest request, IServerStreamWriter<Invertor> responseStream, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.Where(x => x.StationId == request.StationId).ToList();

            foreach (Invertor item in invertors)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy *= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override Task<StationProducedEnergy> GetStationProducedEnergy(InvertorsOnStationRequest request, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.Where(x => x.StationId == request.StationId).ToList();
            double totalEnergyOnStation = 0;
            foreach (Invertor item in invertors)
            {
                totalEnergyOnStation += item.ProducedEnergy;
            }
            if (powerInMW)
            {
                return Task.FromResult(new StationProducedEnergy()
                {
                    Energy = totalEnergyOnStation * 1000
                });
            }
            else
            {
                return Task.FromResult(new StationProducedEnergy()
                {
                    Energy = totalEnergyOnStation
                });
            }
        }

        public override Task<StationProducedEnergy> GetTotalProducedEnergy(EmptyRequest request, ServerCallContext context)
        {
            List<Invertor> invertors = db.Invertors.ToList();
            double totalEnergy = 0;
            foreach (Invertor item in invertors)
            {
                totalEnergy += item.ProducedEnergy;
            }
            if (powerInMW)
            {
                return Task.FromResult(new StationProducedEnergy()
                {
                    Energy = totalEnergy * 1000
                });
            }
            else
            {
                return Task.FromResult(new StationProducedEnergy()
                {
                    Energy = totalEnergy
                });
            }
        }

        public override async Task GetErrorTypesAsync(EmptyRequest request, IServerStreamWriter<ErrorType> responseStream, ServerCallContext context)
        {
            List<ErrorType> errorTypes = db.ErrorTypes.ToList();

            foreach (ErrorType item in errorTypes)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetAllStationProducingStatisticsAsync(EmptyRequest request, IServerStreamWriter<StationProducingStatistic> responseStream, ServerCallContext context)
        {
            List<StationProducingStatistic> errorTypes = db.StationProducingStatistics.ToList();

            foreach (StationProducingStatistic item in errorTypes)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy *= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetStationProducingStatisticAsync(StationProducingStatisticRequest request, IServerStreamWriter<StationProducingStatistic> responseStream, ServerCallContext context)
        {
            List<StationProducingStatistic> errorTypes = db.StationProducingStatistics.Where(x => x.StationId == request.StationId).ToList();

            foreach (StationProducingStatistic item in errorTypes)
            {
                if (powerInMW)
                {
                    item.ProducedEnergy *= 1000;
                }
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByErrorCode(ErrorCodeRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.ErrorCode == request.Code).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByInvertor(InvertorRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.InvertorId == request.InvertorId).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override async Task GetEventsByStation(StationRequest request, IServerStreamWriter<Event> responseStream, ServerCallContext context)
        {
            List<Event> events = db.Events.Where(x => x.StationId == request.StationId).ToList();

            foreach (Event item in events)
            {
                await responseStream.WriteAsync(item);
            }
        }

        public override Task<SuccessResponse> ChangePowerMeasurementUnit(IsMWRequest request, ServerCallContext context)
        {
            try
            {
                powerInMW = request.IsMW;
                return Task.FromResult(new SuccessResponse()
                {
                    Success = true
                });
            }
            catch
            {
                return Task.FromResult(new SuccessResponse()
                {
                    Success = false
                });
            }
        }

    }
}

