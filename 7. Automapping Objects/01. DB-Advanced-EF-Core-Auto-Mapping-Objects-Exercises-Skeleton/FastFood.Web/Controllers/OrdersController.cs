namespace FastFood.Web.Controllers
{
    using AutoMapper;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;

    using Data;
    using ViewModels.Orders;
    using FastFood.Models;
    using FastFood.Models.Enums;
    using AutoMapper.QueryableExtensions;

    public class OrdersController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public OrdersController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IActionResult Create()
        {
            var viewOrder = new CreateOrderViewModel
            {
                Items = this.context.Items.Select(x => x.Name).ToList(),
                Employees = this.context.Employees.Select(x => x.Name).ToList(),
            };

            return this.View(viewOrder);
        }

        [HttpPost]
        public IActionResult Create(CreateOrderInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Home");
            }
            var order = this.mapper.Map<Order>(model);

            order.DateTime = DateTime.Now;

            order.Type = Enum.Parse<OrderType>(model.OrderType);

            var employee = this.context.Employees.FirstOrDefault(x => x.Name == model.EmployeeName);

            order.EmployeeId = employee.Id;

            var item = this.context.Items.FirstOrDefault(x => x.Name == model.ItemName);

            order.OrderItems.Add(new OrderItem
            {
                ItemId = item.Id,
                Order = order,
                Quantity = model.Quantity
            });
            this.context.Orders.Add(order);
            this.context.SaveChanges();
            return this.RedirectToAction("All", "Orders");
        }

        public IActionResult All()
        {
            var orders = this.context.Orders
                .ProjectTo<OrderAllViewModel>(mapper.ConfigurationProvider)
                .ToList();
            return this.View(orders);
        }
    }
}
