using Grpc.Core;
using GrpcNet7.Data;
using Microsoft.EntityFrameworkCore;
using GrpcNet7.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Validations;

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

        public override async Task<VehicleResponse> GetVehicleList(Empty request, ServerCallContext context)
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
            
            return await Task.FromResult(response);
        }

        public override async Task<VehicleList> GetVehicleById(VehicleRequestId reqId, ServerCallContext context)
        {
            var VeDt = _ctx.Vehicles.Find(reqId.Id);
            VehicleList resp = new VehicleList();

            _logger.LogInformation("Sending vehicle response");

            if(VeDt != null)
            {
                resp.Id = VeDt.Id;
                resp.Name = VeDt.Name;
                resp.Type = VeDt.Type;
                resp.Description = VeDt.Description;
                resp.Number = VeDt.Number;
            }
            else
            {
                _logger.LogError("Data not found");
            }

            return await Task.FromResult(resp);
        }

        public override async Task<RespMessage> CreateVehicle(VehicleList request, ServerCallContext context)
        {
            try
            {
                var veData = new Vehicle()
                {
                    Name = request.Name,
                    Type = request.Type,
                    Description = request.Description,
                    Number = request.Number,
                };
                var stored = _ctx.Vehicles.Add(veData);
                _ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }

            return await Task.FromResult(new RespMessage()
            {
                Code = "200",
                Desc = $"Data kendaraan {request.Name} Tipe {request.Type} berhasil disimpan!"
            });
        }

        public override async Task<RespMessage> UpdateVehicle(VehicleList request, ServerCallContext context)
        {
            var vehicle = await _ctx.Vehicles.Where(ve => ve.Id == request.Id).FirstOrDefaultAsync();

            if (vehicle != null)
            {
                try
                {
                    vehicle.Id = request.Id;
                    vehicle.Name = request.Name;
                    vehicle.Type = request.Type;
                    vehicle.Description = request.Description;
                    vehicle.Number = request.Number;

                    _ctx.Update(vehicle);
                    _ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                    return await Task.FromResult(new RespMessage()
                    {
                        Code = "400",
                        Desc = "Update data gagal"
                    });
                }
            }
            else
            {
                return await Task.FromResult(new RespMessage()
                {
                    Code = "404",
                    Desc = $"Data dengan id {request.Id} tidak ditemukan"
                });
            }

            return await Task.FromResult(new RespMessage() 
            {
                Code = "200",
                Desc = "Data berhasil terupdate"
            });
        }

        public override async Task<RespMessage> DeleteVehicle(VehicleRequestId request, ServerCallContext context)
        {
            var vehicle = await _ctx.Vehicles.Where(ve => ve.Id == request.Id).FirstOrDefaultAsync();
            
            if(vehicle != null)
            {
                try
                {
                    _ctx.Remove(vehicle);
                    _ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                    return await Task.FromResult(new RespMessage()
                    {
                        Code = "400",
                        Desc = "Delete failed"
                    });
                }
            }
            else
            {
                return await Task.FromResult(new RespMessage()
                {
                    Code = "404",
                    Desc = $"Data id {request.Id} not found"
                });
            }

            return await Task.FromResult(new RespMessage()
            {
                Code = "200",
                Desc = "Data successfully deleted"
            });
        }
    }
}
