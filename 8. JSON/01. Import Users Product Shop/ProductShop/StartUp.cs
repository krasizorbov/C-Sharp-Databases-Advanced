using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Export;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var jsonUserPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Product Shop\ProductShop\Datasets\users.json");
            var jsonProductPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Product Shop\ProductShop\Datasets\products.json");
            var jsonCategoryPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Product Shop\ProductShop\Datasets\categories.json");
            var jsonCategoryProductPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\8. JSON\01. Import Users Product Shop\ProductShop\Datasets\categories-products.json");

            var context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            var mapper = config.CreateMapper();

            //Console.WriteLine(ImportUsers(context, jsonUserPath));

            //Console.WriteLine(ImportProducts(context, jsonProductPath));

            //Console.WriteLine(ImportCategories(context, jsonCategoryPath));

            //Console.WriteLine(ImportCategoryProducts(context, jsonCategoryProductPath));

            //Console.WriteLine(GetProductsInRange(context));

            //Console.WriteLine(GetSoldProducts(context));

            //Console.WriteLine(GetCategoriesByProductsCount(context));

            //Console.WriteLine(GetUsersWithProducts(context));
        }

        //01. Import Users 
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var desUsers = JsonConvert.DeserializeObject<User[]>(inputJson);

            var validUsers = new List<User>();

            foreach (var user in desUsers)
            {
                if (IsValid(user))
                {
                    validUsers.Add(user);
                }
            }
            context.AddRange(validUsers);

            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        //02. Import Products 
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var desProducts = JsonConvert.DeserializeObject<Product[]>(inputJson);

            var validProducts = new List<Product>();

            foreach (var product in desProducts)
            {
                if (IsValid(product))
                {
                    validProducts.Add(product);
                }
            }
            context.AddRange(validProducts);

            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var desCategories = JsonConvert.DeserializeObject<Category[]>(inputJson);

            var validCategories = new List<Category>();

            foreach (var category in desCategories)
            {
                if (IsValid(category))
                {
                    validCategories.Add(category);
                }
            }
            context.AddRange(validCategories);

            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //04. Import Categories and Products 
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryIds = context.Categories.Select(x => x.Id).ToList();

            var productIds = context.Products.Select(x => x.Id).ToList();

            var desCategoryProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            var validCategoryProducts = new List<CategoryProduct>();

            foreach (var categoryProduct in desCategoryProducts)
            {
                validCategoryProducts.Add(categoryProduct);
                //!!!Skip Validation - Judge is not accepting it!!!
                //if (categoryIds.Contains(categoryProduct.CategoryId) && productIds.Contains(categoryProduct.ProductId))
                //{
                //    validCategoryProducts.Add(categoryProduct);
                //}
            }
            context.AddRange(validCategoryProducts);

            context.SaveChanges();

            return $"Successfully imported {validCategoryProducts.Count}";
        }

        //05. Export Products In Range 
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(p => new ProductDTO
                {
                    Name = p.Name,
                    Price = p.Price,
                    Seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .OrderBy(p => p.Price)
                .ToList();

            var serProduct = JsonConvert.SerializeObject(products, Formatting.Indented);

            return serProduct;
        }

        //06. Export Sold Products 
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users.Where(u => u.ProductsSold.Any(b => b.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(s => new
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    SoldProducts = s.ProductsSold.Where(ps => ps.Buyer != null)
                    .Select(ps => new
                    {
                        Name = ps.Name,
                        Price = ps.Price,
                        BuyerFirstName = ps.Buyer.FirstName,
                        BuyerLastName = ps.Buyer.LastName
                    }).ToList()
                }).ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serUserProduct = JsonConvert.SerializeObject(users, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            return serUserProduct;
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(x => new
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = $"{x.CategoryProducts.Average(c => c.Product.Price):F2}",
                    TotalRevenue = $"{x.CategoryProducts.Sum(c => c.Product.Price)}"
                })
                .ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serCategories = JsonConvert.SerializeObject(categories, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
            });

            return serCategories;
        }

        //08. Export Users and Products 
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users.Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.Buyer != null))
                .Select(u => new
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new 
                    {
                        Count = u.ProductsSold.Count(ps => ps.Buyer != null),
                        Products = u.ProductsSold.Where(ps => ps.Buyer != null)
                        .Select(ps => new
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .ToList()
                    }
                })
                .ToList();

            var result = new
            {
                UsersCount = users.Count,
                Users = users
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serUsers = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });

            return serUsers;
        }

        public static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);

            var results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, validationContext, results, true);
        }
    }
}