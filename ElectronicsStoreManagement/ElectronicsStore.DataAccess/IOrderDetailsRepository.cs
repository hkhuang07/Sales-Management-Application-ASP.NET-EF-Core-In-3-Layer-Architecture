using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElectronicsStore.DataAccess
{
    public interface IOrderDetailsRepository
    {
        List<Order_Details> GetByOrderID(int orderId);
        void DeleteByOrderID(int orderId);
        void AddRange(List<Order_Details> details);
        void Insert(Order_Details detail);


    }
}
