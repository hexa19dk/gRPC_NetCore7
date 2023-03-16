using Grpc.Core;
using GrpcNet7.Data;
using Microsoft.EntityFrameworkCore;
using GrpcNet7.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace GrpcNet7.Services
{
    public class VehicleService : VehicleServiceGrpc.VehicleServiceGrpcBase
    {
        private readonly ILogger<VehicleService> _logger;
        private readonly DataContext _ctx;
        private readonly IDistributedCache _dstCache;
        private readonly ICacheService _cacheService;

        public VehicleService(ILogger<VehicleService> logger, DataContext ctx, IDistributedCache dstCache, ICacheService cacheService)
        {
            _logger = logger;
            _ctx = ctx;
            _dstCache = dstCache;
            _cacheService = cacheService;
        }

        public override async Task<VehicleResponse> GetVehicleList(Empty request, ServerCallContext context)
        {
            _logger.LogInformation("Received request to : VehicleList");
            VehicleResponse response = new VehicleResponse();

            var cacheDt = _cacheService.GetData<IEnumerable<Vehicle>>("vehicles");
            if (cacheDt != null && cacheDt.Count() > 0)
            {
                var redisList = cacheDt.ToList();
                List<VehicleList> veModel = new List<VehicleList>();
                foreach (var temp in redisList)
                {
                    veModel.Add(new VehicleList()
                    {
                        Id = temp.Id,
                        Name = temp.Name,
                        Type = temp.Type,
                        Description = temp.Description,
                        Number = temp.Number
                    });
                }

                response.Items.AddRange(veModel);
            }
            else
            {
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

                    var expTime = DateTimeOffset.Now.AddMinutes(8.0);
                    _cacheService.SetData<List<VehicleList>>("vehicles", VehicleModel, expTime);
                    response.Items.AddRange(VehicleModel);
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.ToString());
                }
            }            

            return await Task.FromResult(response);
        }

        public override async Task<VehicleList> GetVehicleById(VehicleRequestId reqId, ServerCallContext context)
        {
            //var VeDt = _ctx.Vehicles.Find(reqId.Id);
            VehicleList resp = new VehicleList();
            var cacheDt = _cacheService.GetData<IEnumerable<Vehicle>>("vehicles");
            try
            {
                if (cacheDt != null)
                {
                    var caDt = cacheDt.FirstOrDefault(v => v.Id == reqId.Id);
                    resp.Id = caDt.Id;
                    resp.Name = caDt.Name;
                    resp.Type = caDt.Type;
                    resp.Description = caDt.Description;
                    resp.Number = caDt.Number;

                }
                else
                {
                    var VeDt = _ctx.Vehicles.Find(reqId.Id);
                    if (VeDt != null)
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
                }
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.ToString());
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

                var expTime = DateTimeOffset.Now.AddMinutes(8.0);
                _cacheService.SetData<Vehicle>("vehicles", veData, expTime);
                var stored = _ctx.Vehicles.AddAsync(veData);
                await _ctx.SaveChangesAsync();
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
            var vehicle = await _ctx.Vehicles.FirstOrDefaultAsync(ve => ve.Id == request.Id);
            var cacheDt = _cacheService.GetData<IEnumerable<Vehicle>>("vehicles");

            if (vehicle != null)
            {
                try
                {
                    //Data Mapping
                    vehicle.Id = request.Id;
                    vehicle.Name = request.Name;
                    vehicle.Type = request.Type;
                    vehicle.Description = request.Description;
                    vehicle.Number = request.Number;

                    //remove and set new cache data 
                    _cacheService.RemoveData("vehicles");
                    var expTime = DateTimeOffset.Now.AddMinutes(8.0);
                    _cacheService.SetData<Vehicle>("vehicles", vehicle, expTime);
                    
                    // updated to db
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
            var vehicle = await _ctx.Vehicles.FirstOrDefaultAsync(ve => ve.Id == request.Id);
            
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
