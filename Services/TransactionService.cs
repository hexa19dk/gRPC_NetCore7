using Dapper;
using Grpc.Core;
using GrpcNet7.Data;

namespace GrpcNet7.Services
{
    public class TransactionService : TransactionServiceGrpc.TransactionServiceGrpcBase
    {
        private readonly ILogger _logger;
        private readonly ICacheService _cache;
        private readonly DapperContext _ctx;
        
        public TransactionService(ILogger<TransactionService> logger, ICacheService cache, DapperContext ctx)
        {
            _logger = logger;
            _cache = cache;
            _ctx = ctx;
        }

        public override async Task<TransResponse> GetAllTrans(ReqEmpty request, ServerCallContext context)
        {
            TransResponse resp = new TransResponse();
            var query = "select * from transaction";
            var list = await _ctx.Connect().QueryAsync<TransactionModel>(query);
            resp.Items.Add(list);

            return await Task.FromResult(resp);
        }

        public override async Task<TransResponse> GetByIdTrans(TransId request, ServerCallContext context)
        {
            TransResponse respData = new TransResponse();
            var query = "select * from transaction where Id = " + request.Id;
            var isExist = await _ctx.Connect().QueryFirstOrDefaultAsync<TransactionModel>(query, new { request.Id });

            if (isExist == null)
            {
                _logger.LogError("data tidak ditemukan!");
            }

            respData.Items.Add(isExist);

            return await Task.FromResult(respData);
        }

        public override async Task<ResponseMessage> CreateTransaction(TransactionModel request, ServerCallContext context)
        {
            try
            {
                var query = "insert into transaction (RentalNumber, TransDate, Destination, Price, StartDate, EndDate, WarrantyType, VehicleId, UserId) values(@RentalNumber, @TransDate, @Destination, @Price, @StartDate, @EndDate, @WarrantyType, @UserId, @VehicleId)";

                await _ctx.Connect().ExecuteAsync(query, request);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                return await Task.FromResult(new ResponseMessage()
                {
                    Code = "400",
                    Desc = "Tambah data gagal"
                });
            }

            return await Task.FromResult(new ResponseMessage()
            {
                Code = "200",
                Desc = "Berhasil tambah data"
            });
        }

        public override async Task<ResponseMessage> UpdateTransaction(TransactionModel request, ServerCallContext context)
        {
            try
            {
                var query = "update transaction set RentalNumber=@RentalNumber, TransDate=@TransDate, Destination=@Destination, Price=@Price, StartDate=@StartDate, EndDate=@EndDate, WarrantyType=@WarrantyType, UserId=@UserId, VehicleId=@VehicleId where Id=@Id";

                await _ctx.Connect().ExecuteAsync(query, request);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex.ToString());
                return await Task.FromResult(new ResponseMessage()
                {
                    Code = "400",
                    Desc = "Update data gagal"
                });
            }

            return await Task.FromResult(new ResponseMessage()
            {
                Code = "200",
                Desc = "Update data berhasil"
            });
        }

        public override async Task<ResponseMessage> DeleteTransaction(TransId request, ServerCallContext context)
        {
            var query = "delete from transaction where id = " + request.Id;
            await _ctx.Connect().ExecuteAsync(query, new { request.Id });

            return await Task.FromResult(new ResponseMessage {
                Code = "200",
                Desc = "Hapus data berhasil"
            });
        }

        public override async Task<TransResponse> GetTransUserCar(TransId request, ServerCallContext context)
        {
            TransResponse resp = new TransResponse();

            try
            {
                var query = @"SELECT transaction.Id, transaction.RentalNumber, transaction.TransDate, transaction.Destination, transaction.Price, transaction.WarrantyType, transaction.VehicleId, vehicles.Id, vehicles.Name, vehicles.Type, vehicles.Description, vehicles.Number " +
               "FROM transaction " +
               "INNER JOIN vehicles ON transaction.VehicleId = vehicles.Id " +
               "WHERE transaction.Id = " + request.Id;

                var exist = (await _ctx.Connect().QueryAsync<TransactionModel, VehicleList, TransactionModel>(query, (transaction, vehicle) =>
                {
                    transaction.VehicleList = vehicle;
                    return transaction;
                },
                new { Id = request.Id },
                splitOn: "Id, VehicleId")).FirstOrDefault();

                resp.Items.Add(exist);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.ToString());
            }
            
            return await Task.FromResult(resp);
        }
    }
}
