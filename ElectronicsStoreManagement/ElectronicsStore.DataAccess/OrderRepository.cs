using ElectronicsStore.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ElectronicsStore.DataAccess
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ElectronicsStoreContext _context;

        public OrderRepository()
        {
            _context = new ElectronicsStoreContext();
        }

        // Tra cứu với chi tiết
        public List<Orders> GetAllWithDetails()
        {
            return _context.Order
                .Include(o => o.Employee)
                .Include(o => o.Customer)
                .Include(o => o.ViewDetails)
                .ToList();
        }
        // Tra cứu không có chi tiết
        public List<Orders> GetAll() => _context.Order.ToList();

        // Tra cứu theo ID
        public Orders? GetById(int id) => _context.Order
            .Include(o => o.ViewDetails)
            .FirstOrDefault(o => o.ID == id);


        //Thêm mới
        public void Add(Orders entity)
        {
            _context.Order.Add(entity);
            _context.SaveChanges();
        }

        //Thêm mới đơn hàng không có chi tiết đơn hàng
        public int Insert(Orders order)
        {
            _context.Order.Add(order);
            _context.SaveChanges();
            return order.ID; // EF sẽ tự cập nhật ID sau khi SaveChanges nếu ID là identity
        }

        //Cập nhật
        public void Update(Orders order)
        {
            var existingOrder = _context.Order.Find(order.ID);
            if (existingOrder != null)
            {
                // Cập nhật các thuộc tính cần thiết
                existingOrder.Date = order.Date;
                existingOrder.EmployeeID = order.EmployeeID;
                existingOrder.CustomerID = order.CustomerID;
                existingOrder.Note = order.Note;
                //existingOrder.Order_Details = order.Order_Details;
                _context.Order.Update(existingOrder);
                _context.SaveChanges();
            }
            else
            {
                throw new Exception($"Order with ID = {order.ID} not found.");
            }    
                
        }

        //Xóa
        public void Delete(Orders order)
        {
            _context.Order.Remove(order);
            _context.SaveChanges();
        }

    }
}
