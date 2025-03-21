using Dapper;
using DevStore.Orders.API.Application.DTO;
using DevStore.Orders.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevStore.Orders.API.Application.Queries
{
    public interface IOrderQueries
    {
        Task<OrderDTO> GetLastOrder(Guid customerId);
        Task<IEnumerable<OrderDTO>> GetByCustomerId(Guid customerId);
        Task<OrderDTO> GetAuthorizedOrders();
    }

    public class OrderQueries : IOrderQueries
    {
        private readonly IOrderRepository _orderRepository;

        public OrderQueries(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderDTO> GetLastOrder(Guid customerId)
        {
            var order = await _orderRepository.GetLastOrder(customerId);

            if (order is null)
                return null;

            return MapOrder(order);
        }

        public async Task<IEnumerable<OrderDTO>> GetByCustomerId(Guid customerId)
        {
            var orders = await _orderRepository.GetCustomersById(customerId);

            return orders.Select(OrderDTO.ToOrderDTO);
        }

        public async Task<OrderDTO> GetAuthorizedOrders()
        {
            var orders = await _orderRepository.GetLastAuthorizedOrder();

            return MapOrder(orders);
        }

        private OrderDTO MapOrder(Order order)
        {
            var orderDto = new OrderDTO
            {
                Id = order.Id,
                Code = order.Code,
                CustomerId = order.CustomerId,
                Status = (int)order.OrderStatus,
                Date = order.DateAdded,
                Amount = order.Amount,
                Discount = order.Discount,
                HasVoucher = order.HasVoucher,
                Address = new AddressDto
                {
                    StreetAddress = order.Address.StreetAddress,
                    Neighborhood = order.Address.Neighborhood,
                    ZipCode = order.Address.ZipCode,
                    City = order.Address.City,
                    SecondaryAddress = order.Address.SecondaryAddress,
                    State = order.Address.State,
                    BuildingNumber = order.Address.BuildingNumber
                },
                OrderItems = order.OrderItems.Select(item => new OrderItemDTO
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    Name = item.ProductName,
                    Price = item.Price,
                    Image = item.ProductImage,
                    Quantity = item.Quantity
                }).ToList()
            };

            return orderDto;
        }

    }

}