using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var supplierPath = File.ReadAllText("../../../Datasets/suppliers.xml");
            var salePath = File.ReadAllText("../../../Datasets/sales.xml");
            var partPath = File.ReadAllText("../../../Datasets/parts.xml");
            var customerPath = File.ReadAllText("../../../Datasets/customers.xml");
            var carPath = File.ReadAllText("../../../Datasets/cars.xml");

            using (var context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Console.WriteLine(ImportSuppliers(context, supplierPath));
                //Console.WriteLine(ImportParts(context, partPath));
                //Console.WriteLine(ImportCars(context, carPath));
                //Console.WriteLine(ImportCustomers(context, customerPath));
                //Console.WriteLine(ImportSales(context, salePath));
                //Console.WriteLine(GetCarsWithDistance(context));
                //Console.WriteLine(GetCarsFromMakeBmw(context));
                //Console.WriteLine(GetLocalSuppliers(context));
                //Console.WriteLine(GetCarsWithTheirListOfParts(context));
                //Console.WriteLine(GetTotalSalesByCustomer(context));
                //Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        //09. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSuppliersDTO[]), new XmlRootAttribute("Suppliers"));

            var suppliersDTO = (ImportSuppliersDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var validSuppliers = new List<Supplier>();

            foreach (var supplier in suppliersDTO)
            {
                if (IsValid(supplier))
                {
                    var s = new Supplier 
                    {
                        Name = supplier.Name,
                        IsImporter = supplier.IsImporter
                    };
                    validSuppliers.Add(s);
                }
            }

            context.Suppliers.AddRange(validSuppliers);

            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartsDTO[]), new XmlRootAttribute("Parts"));

            var partsDTO = (ImportPartsDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var validParts = new List<Part>();

            var supplierID = context.Suppliers.Select(x => x.Id).ToList();

            foreach (var part in partsDTO)
            {
                if (IsValid(part))
                {
                    if (supplierID.Contains(part.SupplierId))
                    {
                        var p = new Part
                        {
                            Name = part.Name,
                            Price = part.Price,
                            Quantity = part.Quantity,
                            SupplierId = part.SupplierId
                        };
                        validParts.Add(p);
                    }   
                }
            }

            context.Parts.AddRange(validParts);

            context.SaveChanges();

            return $"Successfully imported {validParts.Count}";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var carsParsed = XDocument.Parse(inputXml).Root.Elements().ToList();

            var cars = new List<Car>();

            var partsID = context.Parts.Select(x => x.Id).ToArray();

            foreach (var car in carsParsed)
            {
                Car currentCar = new Car()
                {
                    Make = car.Element("make").Value,
                    Model = car.Element("model").Value,
                    TravelledDistance = Convert.ToInt64(car.Element("TraveledDistance").Value)
                };

                var partIds = new HashSet<int>();

                foreach (var partid in car.Element("parts").Elements())
                {
                    var pid = Convert.ToInt32(partid.Attribute("id").Value);
                    partIds.Add(pid);
                }

                foreach (var pid in partIds)
                {
                    if (!partsID.Contains(pid))
                        continue;

                    PartCar currentPair = new PartCar()
                    {
                        Car = currentCar,
                        PartId = pid
                    };

                    currentCar.PartCars.Add(currentPair);
                }
                cars.Add(currentCar);
            }

            context.Cars.AddRange(cars);

            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomersDTO[]), new XmlRootAttribute("Customers"));

            var customersDTO = (ImportCustomersDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var validCustomers = new List<Customer>();

            foreach (var customer in customersDTO)
            {
                if (IsValid(customer))
                {
                    var c = new Customer
                    {
                        Name = customer.Name,
                        BirthDate = customer.BirthDate,
                        IsYoungDriver = customer.IsYoungDriver
                    };
                    validCustomers.Add(c);
                }
            }

            context.Customers.AddRange(validCustomers);

            context.SaveChanges();

            return $"Successfully imported {validCustomers.Count}";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSalesDTO[]), new XmlRootAttribute("Sales"));

            var salesDTO = (ImportSalesDTO[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var validSales = new List<Sale>();

            var carsID = context.Cars.Select(x => x.Id).ToList();

            foreach (var sale in salesDTO)
            {
                if (IsValid(sale))
                {
                    if (carsID.Contains(sale.CarId))
                    {
                        var s = new Sale
                        {
                            CarId = sale.CarId,
                            CustomerId = sale.CustomerId,
                            Discount = sale.Discount
                        };
                        validSales.Add(s);
                    } 
                }
            }

            context.Sales.AddRange(validSales);

            context.SaveChanges();

            return $"Successfully imported {validSales.Count}";
        }

        //14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars.Where(p => p.TravelledDistance > 2000000)
                .Select(s => new ExportCars2000000DTO
                {
                    Make = s.Make,
                    Model = s.Model,
                    TravelledDistance = s.TravelledDistance
                })
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCars2000000DTO[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //15. Export Cars From Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars.Where(s => s.Make == "BMW")
                .Select(s => new ExportBMWCarsDTO
                {
                    Id = s.Id,
                    Model = s.Model,
                    TravelledDistance = s.TravelledDistance
                })
                .OrderBy(s => s.Model)
                .ThenByDescending(s => s.TravelledDistance)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportBMWCarsDTO[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), cars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(s => s.IsImporter == false)
                .Select(s => new ExportSuppliersAndPartsDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    Count = s.Parts.Count
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportSuppliersAndPartsDTO[]), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), suppliers, namespaces);

            return sb.ToString().TrimEnd();
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsAndParts = context.Cars
                .Select(s => new ExportCarsAndPartsDTO
                {
                    Make = s.Make,
                    Model = s.Model,
                    TravelledDistance = s.TravelledDistance,
                    PartsDTO = s.PartCars.Select(p => new PartsDTO 
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(f => f.Price)
                    .ToArray()
                })
                .OrderByDescending(f => f.TravelledDistance)
                .ThenBy(f => f.Model)
                .Take(5)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarsAndPartsDTO[]), new XmlRootAttribute("cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), carsAndParts, namespaces);

            return sb.ToString().TrimEnd();
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers.Where(c => c.Sales.Any())
                .Select(s => new ExportCustomersDTO
                {
                    Name = s.Name,
                    Count = s.Sales.Count,
                    Price = s.Sales.Sum(x => x.Car.PartCars.Sum(f => f.Part.Price))
                })
                .OrderByDescending(f => f.Price)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomersDTO[]), new XmlRootAttribute("customers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), customers, namespaces);

            return sb.ToString().TrimEnd();
        }

        //19. Export Sales With Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new ExportSalesDiscountDTO
                {
                    Car = new ExportCarsDTO 
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    Discount = $"{Math.Floor(s.Discount)}",
                    Name = s.Customer.Name,
                    Price = $"{s.Car.PartCars.Sum(y => y.Part.Price):F2}",
                    PriceDiscount = $"{s.Car.PartCars.Sum(y => y.Part.Price) - (s.Car.PartCars.Sum(y => y.Part.Price) * (s.Discount / 100))}".TrimEnd('0'),
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportSalesDiscountDTO[]), new XmlRootAttribute("sales"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), sales, namespaces);

            return sb.ToString().TrimEnd();
        }
        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);

            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, results, true);
        }
    }
}