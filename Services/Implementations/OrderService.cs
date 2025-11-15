using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Data;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Shared.Dtos;
using Shared.Dtos.OrderDtos;

namespace Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<BaseResponseDto> CreateOrderAsync(string userId, CheckoutDto checkoutDto)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product) 
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Giỏ hàng rỗng" };
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                    {
                        await transaction.RollbackAsync();
                        return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Sản phẩm ID {item.ProductId} không còn tồn tại." };
                    }
                    if (item.Product.Stock < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Sản phẩm '{item.Product.ProductName}' không đủ hàng (chỉ còn {item.Product.Stock})" };
                    }
                }

                decimal totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

                var order = new Order
                {
                    UserId = userId,
                    TotalAmount = totalAmount,
                    Status = "Pending",
                    PaymentStatus = "Unpaid", 
                    Note = checkoutDto.Note,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    FullName = checkoutDto.FullName,
                    Phone = checkoutDto.Phone,
                    Email = checkoutDto.Email
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); 

                foreach (var item in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Size = item.Size,
                        Price = item.Product.Price,
                        ProductName = item.Product.ProductName,
                        Subtotal = item.Product.Price * item.Quantity
                    };
                    _context.OrderItems.Add(orderItem);

                    var product = item.Product;
                    product.Stock -= item.Quantity;
                    _context.Products.Update(product);
                }

                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new BaseResponseDto { Result = ResultValue.Success, Message = "Đặt hàng thành công", Data = order.OrderId };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BaseResponseDto { Result = ResultValue.Failed, Message = $"Đặt hàng thất bại: {ex.Message}" };
            }
        }

        public async Task<BaseResponseDto> GetMyOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = orders };
        }

        public async Task<BaseResponseDto> GetMyOrderDetailsAsync(string userId, int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.UserId == userId && o.OrderId == orderId)
                .Include(o => o.OrderItems) 
                    .ThenInclude(oi => oi.Product) 
                .Select(o => new OrderDetailDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    Email = o.Email,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ImageUrl = oi.Product.ImageUrl, 
                        Quantity = oi.Quantity,
                        Size = oi.Size,
                        Price = oi.Price, 
                        Subtotal = oi.Subtotal
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy đơn hàng" };
            }
            return new BaseResponseDto { Result = ResultValue.Success, Data = order };
        }



        public async Task<BaseResponseDto> GetAllOrdersAsync()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Sum(oi => oi.Quantity)
                })
                .ToListAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Data = orders };
        }

        public async Task<BaseResponseDto> GetOrderDetailsForAdminAsync(int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Select(o => new OrderDetailDto
                {
                    OrderId = o.OrderId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    PaymentStatus = o.PaymentStatus,
                    Note = o.Note,
                    CreatedAt = o.CreatedAt,
                    FullName = o.FullName,
                    Phone = o.Phone,
                    Email = o.Email,
                    OrderItems = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.ProductName,
                        ImageUrl = oi.Product.ImageUrl,
                        Quantity = oi.Quantity,
                        Size = oi.Size,
                        Price = oi.Price,
                        Subtotal = oi.Subtotal
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy đơn hàng" };
            }
            return new BaseResponseDto { Result = ResultValue.Success, Data = order };
        }

        public async Task<BaseResponseDto> UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy đơn hàng" };
            }

            order.Status = status;
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật trạng thái đơn hàng thành công" };
        }

        public async Task<BaseResponseDto> UpdatePaymentStatusAsync(int orderId, string paymentStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return new BaseResponseDto { Result = ResultValue.NoData, Message = "Không tìm thấy đơn hàng" };
            }

            order.PaymentStatus = paymentStatus;
            order.UpdatedAt = DateTime.Now;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return new BaseResponseDto { Result = ResultValue.Success, Message = "Cập nhật trạng thái thanh toán thành công" };
        }
    }
}