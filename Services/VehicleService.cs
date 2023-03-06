using Grpc.Core;
using GrpcNet7.Data;
using Microsoft.EntityFrameworkCore;
using GrpcNet7.Entities;

namespace GrpcNet7.Services
{
    public class VehicleService : VehicleServiceGrpc.VehicleServiceGrpcBase
    {
        private readonly ILogger<VehicleService> _logger;
        private readonly DataContext _ctx;

        public VehicleService(ILogger<VehicleService> logger, DataContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public override Task<VehicleResponse> GetVehicleList(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Received request to : VehicleList");
            VehicleResponse response = new VehicleResponse();

            try
            {
                var dbList = _ctx.Vehicles.ToList();
                List<VehicleList> VehicleModel = new List<VehicleList>();

                foreach (var db in dbList)
                {
                    VehicleModel.Add(new VehicleList()
                    {
                        Id = db.Id,
                        Name = db.Name,
                        Type = db.Type,
                        Description = db.Description,
                        Number = db.Number
                    });
                }

                response.Items.AddRange(VehicleModel);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            
            return Task.FromResult(response);
        }

        //public override Task<VehicleList> GetVehicleById(VehicleRequest request, ServerCallContext context)
        //{
        //    VehicleList veList = new VehicleList();
        //    {
        //        veList.Name = "Alphard";
        //        veList.Description = "Unit 1";
        //        veList.Number = "B1234BE";
        //        veList.Type = "VIP Unit";
        //    }

        //    return Task.FromResult(veList);
        //}

    }
}
