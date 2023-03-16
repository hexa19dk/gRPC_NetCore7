using Dapper;
using Grpc.Core;
using GrpcNet7.Data;
using Microsoft.AspNetCore.Http.Metadata;

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

            using (var conn = _ctx.CreateConnection())
            {
                var list = await conn.QueryAsync<TransactionModel>(query);
                resp.Items.Add(list);
            }

            return await Task.FromResult(resp);
        }

        public override Task<TransactionModel> GetByIdTrans(TransId request, ServerCallContext context)
        {
            return base.GetByIdTrans(request, context);
        }

        public override async Task<ResponseMessage> CreateTransaction(TransactionModel request, ServerCallContext context)
        {
            try
            {
                var query = "insert into transaction (Rental_number, Trans_date, Destination, Price, Start_date, End_date, Warranty_type, VehicleId, UserId) values(@RentalNumber, @TransDate, @Destination, @Price, @StartDate, @EndDate, @WarrantyType, @UserId, @VehicleId)";

                using (var conn = _ctx.CreateConnection())
                {
                    await conn.ExecuteAsync(query, request);
                }
            }
            catch(Exception ex)
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
    }
}
