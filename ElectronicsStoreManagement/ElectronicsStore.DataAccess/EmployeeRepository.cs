using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.DataAccess
{
    public class EmployeeRepository: IEmployeeRepository
    {
        private readonly ElectronicsStoreContext _context;

        public EmployeeRepository()
        {
            _context = new ElectronicsStoreContext();
        }

        // Tra cứu
        public List<Employees> GetAll() => _context.Employee.ToList();

        public Employees? GetById(int id) => _context.Employee.Find(id);

        public Employees? GetbyUserName(string userName)
        {
            return _context.Employee.FirstOrDefault(e => e.UserName == userName);
        }


        //Thêm mới
        public void Add(Employees employee)
        {
            _context.Employee.Add(employee);
            _context.SaveChanges();
        }

        //Cập nhật
        public void Update(Employees employee)
        {
            var existingEmployee = _context.Employee.Find(employee.ID);
            if (existingEmployee != null)
            {
                // Cập nhật các thuộc tính cần thiết
                existingEmployee.FullName = employee.FullName;
                existingEmployee.EmployeeAddress = employee.EmployeeAddress;
                existingEmployee.EmployeePhone = employee.EmployeePhone;
                existingEmployee.Role = employee.Role;
                existingEmployee.Password = employee.Password;

                _context.Employee.Update(existingEmployee);
                _context.SaveChanges();
                _context.Entry(employee).Reload();

            }
            else
            {
                throw new Exception($"Employee with ID = {employee.ID} not found.");
            }

        }

        public void UpdatePassword(int id, string hashedPassword)
        {
            var employee = _context.Employee.Find(id);
            if (employee != null)
            {
                employee.Password = hashedPassword;
                _context.SaveChanges();
            }
            _context.Entry(employee).Reload();

        }


        //Xóa
        public void Delete(Employees employee)
        {
            _context.Employee.Remove(employee);
            _context.SaveChanges();
        }
    }
}
