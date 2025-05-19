using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.DataAccess
{
    public class OrderDetailsRepository : IOrderDetailsRepository
    {
        private readonly ElectronicsStoreContext _context;

        public OrderDetailsRepository()
        {
            _context = new ElectronicsStoreContext();
        }

        //Tra cứu
        //Lấy chi tiết đơn hàng theo ID
        public List<Order_Details> GetByOrderID(int orderId)
        {
            return _context.Order_Details.Where(x => x.OrderID == orderId).ToList();
        }
        //Lấy tất cả chi tiết đơn hàng
        public List<Order_Details> GetAll()
        {
            return _context.Order_Details.ToList();
        }

        //Thêm mới
        public void Insert(Order_Details detail)
        {
            _context.Order_Details.Add(detail);
            _context.SaveChanges();
        }

        public void AddRange(List<Order_Details> details)
        {
            _context.Order_Details.AddRange(details);
            _context.SaveChanges();
        }

        //Xóa theo orderID
        public void DeleteByOrderID(int orderId)
        {
            /*var oldDetails = _context.Order_Details.Where(x => x.OrderID == orderId);
            _context.Order_Details.RemoveRange(oldDetails);
            _context.SaveChanges();      */
            var oldDetails = _context.Order_Details
               .AsNoTracking()
               .Where(x => x.OrderID == orderId);
            _context.Order_Details.RemoveRange(oldDetails);
            _context.SaveChanges();
        }

   
    }

}
