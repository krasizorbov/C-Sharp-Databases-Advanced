using System;
using System.Collections.Generic;
using System.IO;
namespace CarDealer
{
    using AutoMapper;
    using CarDealer.Data;
    using CarDealer.DTO;
    using CarDealer.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var carsPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Car Dealer\CarDealer\Datasets\cars.json");
            var customersPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Car Dealer\CarDealer\Datasets\customers.json");
            var partsPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Car Dealer\CarDealer\Datasets\parts.json");
            var salesPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Car Dealer\CarDealer\Datasets\sales.json");
            var suppliersPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Car Dealer\CarDealer\Datasets\suppliers.json");

            //Console.WriteLine(ImportSuppliers(context, suppliersPath));
            //Console.WriteLine(ImportParts(context, partsPath));
            //Console.WriteLine(ImportCars(context, carsPath));
            //Console.WriteLine(ImportCustomers(context, customersPath));
            //Console.WriteLine(ImportSales(context, salesPath));
            //Console.WriteLine(GetOrderedCustomers(context));
            //Console.WriteLine(GetCarsFromMakeToyota(context));
            //Console.WriteLine(GetLocalSuppliers(context));
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));
            //Console.WriteLine(GetTotalSalesByCustomer(context));
            //Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        //09. Import Suppliers 
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var desSuppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            var validSuppliers = new List<Supplier>();

            foreach (var supplier in desSuppliers)
            {
                if (IsValid(supplier))
                {
                    validSuppliers.Add(supplier);
                }
            }
            context.AddRange(validSuppliers);

            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}.";
        }

        //10. Import Parts 
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var suppliersID = context.Suppliers.Select(x => x.Id).ToList();

            var desParts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            var validParts = new List<Part>();

            foreach (var part in desParts)
            {
                if (IsValid(part))
                {
                    if (suppliersID.Contains(part.SupplierId))
                    {
                        validParts.Add(part);
                    }
                }
            }
            context.AddRange(validParts);

            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        //11. Import Cars 
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var desCars = JsonConvert.DeserializeObject<CarsInsertDTO[]>(inputJson);

            var validCars = new List<Car>();

            var validPartCars = new List<PartCar>();

            foreach (var car in desCars)
            {
                if (IsValid(car))
                {
                    var newCar = new Car { Make = car.Make, Model = car.Model, TravelledDistance = car.TravelledDistance };
                    validCars.Add(newCar);
                    var partIds = car.PartsID.Distinct().ToList();

                    if (partIds == null)
                        continue;

                    partIds.ForEach(pid =>
                    {
                        var currentPair = new PartCar()
                        {
                            Car = newCar,
                            PartId = pid
                        };

                        newCar.PartCars.Add(currentPair);
                    }
                    );
                }
            }
            context.AddRange(validCars);

            context.SaveChanges();

            return $"Successfully imported {validCars.Count}.";
        }

        //12. Import Customers 
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var desCustomers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            var validCustomers = new List<Customer>();

            foreach (var customer in desCustomers)
            {
                if (IsValid(customer))
                {
                    validCustomers.Add(customer);
                }
            }

            context.AddRange(validCustomers);

            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var desSales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(desSales);

            int affectedRows = context.SaveChanges();

            return $"Successfully imported {affectedRows}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers.OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver).ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serCustomers = JsonConvert.SerializeObject(customers, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "dd/MM/yyyy",
                Formatting = Formatting.Indented
            });

            return serCustomers;
        }

        //15. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars.Where(x => x.Make == "Toyota")
                .Select(s => new
                {
                    Id = s.Id,
                    Make = s.Make,
                    Model = s.Model,
                    TravelledDistance = s.TravelledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(y => y.TravelledDistance)
                .ToList();

            var serCars = JsonConvert.SerializeObject(cars, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return serCars;
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(x => x.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            var serSuppliers = JsonConvert.SerializeObject(suppliers, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return serSuppliers;
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars.Select(s => new
            {
                car = new
                {
                    Make = s.Make,
                    Model = s.Model,
                    TravelledDistance = s.TravelledDistance
                },
                parts = s.PartCars.Select(p => new
                {
                    Name = p.Part.Name,
                    Price = $"{p.Part.Price:F2}"
                })
                .ToList()
            })
            .ToList();

            var serCars = JsonConvert.SerializeObject(cars, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return serCars;
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers.Where(c => c.Sales.Count >= 1)
                .Select(s => new
                {
                    fullName = s.Name,
                    boughtCars = s.Sales.Count,
                    spentMoney = s.Sales.Sum(m => m.Car.PartCars.Sum(p => p.Part.Price))
                })
                .OrderByDescending(e => e.spentMoney)
                .ThenBy(g => g.boughtCars)
                .ToList();

            var serCustomer = JsonConvert.SerializeObject(customers, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return serCustomer;
        }

        //19. Export Sales With Applied Discount 
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(x => new
                {
                    car = new
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    customerName = x.Customer.Name,
                    Discount = $"{x.Discount:F2}",
                    price = $"{x.Car.PartCars.Sum(y => y.Part.Price):F2}",
                    priceWithDiscount = $"{x.Car.PartCars.Sum(y => y.Part.Price) - (x.Car.PartCars.Sum(y => y.Part.Price) * (x.Discount / 100)):F2}",
                })
                .ToList();

            var serSales = JsonConvert.SerializeObject(sales, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            });

            return serSales;
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);

            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, results, true);
        }
    }
}