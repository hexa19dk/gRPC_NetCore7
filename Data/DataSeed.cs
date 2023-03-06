using GrpcNet7.Entities;

namespace GrpcNet7.Data
{
    public class DataSeed
    {
        private readonly DataContext _dtCtx;
        public DataSeed(DataContext dtCtx) 
        {
            _dtCtx = dtCtx;
        }

        public void Seed()
        {
            if (!_dtCtx.Vehicles.Any())
            {
                var vehicles = new List<Vehicle>()
                {
                    new Vehicle()
                    {
                        Name = "Toyota Avanza",
                        Type = "MPV",
                        Description = "Unit 1 Regular transport",
                        Number = "B12345BBG"
                    },
                    new Vehicle()
                    {
                        Name = "Honda Mobilio",
                        Type = "MPV",
                        Description = "Unit 2 Regular transport",
                        Number = "B23456BBG"
                    },
                    new Vehicle()
                    {
                        Name = "Mercedez E200",
                        Type = "Sedan",
                        Description = "Unit 3 Exclusive transport",
                        Number = "B34567BBG"
                    },
                    new Vehicle()
                    {
                        Name = "Toyota Transmover",
                        Type = "MPV",
                        Description = "Unit 4 Regular Transport",
                        Number = "B12345BBG"
                    },
                    new Vehicle()
                    {
                        Name = "Alphard",
                        Type = "MPV",
                        Description = "Unit 5 Exclusive transport",
                        Number = "B12345BBG"
                    },
                    new Vehicle()
                    {
                        Name = "Innova G Series",
                        Type = "MPV",
                        Description = "Unit 6 Gold transport",
                        Number = "B12345BBG"
                    },
                };

                _dtCtx.Vehicles.AddRange(vehicles);
                _dtCtx.SaveChanges();
            }           
        }
    }
}
