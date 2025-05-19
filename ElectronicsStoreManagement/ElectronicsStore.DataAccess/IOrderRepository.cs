using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.DataAccess
{
    public interface IOrderRepository
    {
        List<Orders> GetAllWithDetails();
        List<Orders> GetAll();
        Orders? GetById(int id);
        void Add(Orders entity);
        int Insert(Orders order);
        void Update(Orders order);
        void Delete(Orders order);

        /*void DeleteOrderDetails(int orderId);
        void AddOrderDetails(List<Order_Details> details);
        void Save();   */
    }

}
