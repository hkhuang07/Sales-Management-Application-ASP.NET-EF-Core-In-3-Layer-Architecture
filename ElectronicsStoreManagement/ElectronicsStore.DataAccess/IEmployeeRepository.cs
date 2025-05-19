using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.DataAccess
{
    public interface IEmployeeRepository
    {
        List<Employees> GetAll();
        Employees? GetById(int id);
        Employees? GetbyUserName(string userName);

        void Add(Employees employee);
        void Update(Employees employee);
        void UpdatePassword(int id, string hashedPassword);
        void Delete(Employees employee);
    }
}
