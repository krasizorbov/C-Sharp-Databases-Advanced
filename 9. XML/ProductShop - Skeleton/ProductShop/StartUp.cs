using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var userPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\9. XML\ProductShop - Skeleton\ProductShop\Datasets\users.xml");
            var productPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\9. XML\ProductShop - Skeleton\ProductShop\Datasets\products.xml");
            var categoryPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\9. XML\ProductShop - Skeleton\ProductShop\Datasets\categories.xml");
            var categoryProductPath = File.ReadAllText(@"C:\Users\krasi\source\repos\Databases-Advanced\9. XML\ProductShop - Skeleton\ProductShop\Datasets\categories-products.xml");

            Mapper.Initialize(x => { x.AddProfile<ProductShopProfile>(); });

            using (ProductShopContext context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Console.WriteLine(ImportUsers(context, userPath));
                //Console.WriteLine(ImportProducts(context, productPath));
                //Console.WriteLine(ImportCategories(context, categoryPath));
                //Console.WriteLine(ImportCategoryProducts(context, categoryProductPath));
                //Console.WriteLine(GetProductsInRange(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetCategoriesByProductsCount(context));
                //Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSeliarizer = new XmlSerializer(typeof(ImportUserDTO[]), new XmlRootAttribute("Users"));

            var userDTO = (ImportUserDTO[])xmlSeliarizer.Deserialize(new StringReader(inputXml));

            var validUsers = new List<User>();

            foreach (var user in userDTO)
            {
                var u = Mapper.Map<User>(user);
                validUsers.Add(u);
            }

            context.Users.AddRange(validUsers);

            context.SaveChanges();

            return $"Successfully imported {validUsers.Count}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSeliarizer = new XmlSerializer(typeof(ImportProductDTO[]), new XmlRootAttribute("Products"));

            var productDTO = (ImportProductDTO[])xmlSeliarizer.Deserialize(new StringReader(inputXml));

            var validProducts = new List<Product>();

            foreach (var product in productDTO)
            {
                var p = Mapper.Map<Product>(product);
                validProducts.Add(p);
            }

            context.Products.AddRange(validProducts);

            context.SaveChanges();

            return $"Successfully imported {validProducts.Count}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSeliarizer = new XmlSerializer(typeof(ImportCategoryDTO[]), new XmlRootAttribute("Categories"));

            var categoryDTO = (ImportCategoryDTO[])xmlSeliarizer.Deserialize(new StringReader(inputXml));

            var validCategories = new List<Category>();

            foreach (var category in categoryDTO)
            {
                if (category.Name != null)
                {
                    var c = Mapper.Map<Category>(category);
                    validCategories.Add(c);
                }
            }

            context.Categories.AddRange(validCategories);

            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSeliarizer = new XmlSerializer(typeof(ImportCategoryProductDTO[]), new XmlRootAttribute("CategoryProducts"));

            var categoryProductDTO = (ImportCategoryProductDTO[])xmlSeliarizer.Deserialize(new StringReader(inputXml));

            var validCategoriesProducts = new List<CategoryProduct>();

            var categoryID = context.Categories.Select(x => x.Id);

            var productID = context.Products.Select(x => x.Id);

            foreach (var categoryProduct in categoryProductDTO)
            {
                if (categoryID.Contains(categoryProduct.CategoryId) && productID.Contains(categoryProduct.ProductId))
                {
                    var cp = Mapper.Map<CategoryProduct>(categoryProduct);
                    validCategoriesProducts.Add(cp);
                }
            }

            context.CategoryProducts.AddRange(validCategoriesProducts);

            context.SaveChanges();

            return $"Successfully imported {validCategoriesProducts.Count}";
        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var productsInRange = context.Products.Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(s => new ExportProductsInRangeDTO
                {
                    Name = s.Name,
                    Price = s.Price,
                    Buyer = s.Buyer.FirstName + " " + s.Buyer.LastName
                })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProductsInRangeDTO[]), new XmlRootAttribute("Products"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), productsInRange, namespaces);

            return sb.ToString().TrimEnd();
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersAndProducts = context.Users.Where(u => u.ProductsSold.Any(b => b.Buyer != null))
                .OrderBy(l => l.LastName)
                .ThenBy(f => f.FirstName)
                .Select(s => new ExportUsersAndProductsDTO
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    ProductDTO = s.ProductsSold.Select(p => new ProductDTO
                    {
                        Name = p.Name,
                        Price = p.Price
                    })
                    .ToArray()
                })
                .Take(5)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUsersAndProductsDTO[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), usersAndProducts, namespaces);

            return sb.ToString().TrimEnd();
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories.Select(c => new ExportCategoryDTO
            {
                Name = c.Name,
                Count = c.CategoryProducts.Count,
                AveragePrice = c.CategoryProducts.Average(x => x.Product.Price),
                TotalRevenue = c.CategoryProducts.Sum(x => x.Product.Price)
            })
            .OrderByDescending(p => p.Count)
            .ThenBy(t => t.TotalRevenue)
            .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCategoryDTO[]), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);

            return sb.ToString().TrimEnd();
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users.Where(u => u.ProductsSold.Any())
                .Select(u => new ExportUsersAndProducts88DTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProductsDTO = new SoldProductsDTO
                    {
                        Count = u.ProductsSold.Count,
                        ProductDTO = u.ProductsSold.Select(p => new ProductDTO
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(x => x.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(s => s.SoldProductsDTO.Count)
                .Take(10)
                .ToArray();

            var userCount = new ExportUsersAndProducts8DTO
            {
                Count = context.Users.Where(s => s.ProductsSold.Any(b => b.Buyer != null)).Count(),
                Users = users
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportUsersAndProducts8DTO), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });

            xmlSerializer.Serialize(new StringWriter(sb), userCount, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}