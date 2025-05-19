using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using ElectronicsStore.DataAccess;
using ElectronicsStore.DataTransferObject;
using BC=BCrypt.Net.BCrypt;

namespace ElectronicsStore.BussinessLogic
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;

        public EmployeeService(IMapper mapper)
        {
            _repository = new EmployeeRepository();
            _mapper = mapper;
        }

        //Kiểm tra dữ liệu đầu vào
        private void Validate(EmployeeDTO dto, bool isNew = true)
        {
            if (dto == null)
                throw new ArgumentException("Employee data cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.FullName) || dto.FullName.Length > 200)
                throw new ArgumentException("Employee name cannot be blank and must be max 200 characters.");

            if (!IsValidUserName(dto.UserName))
                throw new ArgumentException("Username must be 6–32 characters long, letters, digits or underscores only.");

            if (string.IsNullOrWhiteSpace(dto.EmployeeAddress) || dto.EmployeeAddress.Length > 200)
                throw new ArgumentException("Address cannot be blank and must be max 200 characters.");

            if (!IsValidPhone(dto.EmployeePhone))
                throw new ArgumentException("Invalid phone number. Example: 0901234567.");

            /*if (isNew && string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required for new employee.");*/
           
            if (string.IsNullOrEmpty(dto.Role.ToString()))
                throw new ArgumentException("Role is required.");
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var regex = new Regex(@"^(0|\+84)(\d{9})$"); // supports 090xxxxxxx, +849xxxxxxxx
            return regex.IsMatch(phone);
        }

        private bool IsValidUserName(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            var regex = new Regex(@"^[a-zA-Z0-9_]{6,32}$");
            return regex.IsMatch(username);
        }

        //Tra cứu
        public List<EmployeeDTO> GetAll()
        {
            var list = _repository.GetAll();
            return _mapper.Map<List<EmployeeDTO>>(list);
        }

        public EmployeeDTO GetById(int id)
        {
            var entity = _repository.GetById(id);
            if (entity == null) throw new Exception($"Employee not found with ID = {id}.");
            return _mapper.Map<EmployeeDTO>(entity);
        }

        public EmployeeDTO GetByFullName(string fullname)
        {
            var entity = _repository.GetAll().FirstOrDefault(e => e.FullName == fullname);
            if (entity == null) throw new Exception($"Employee not found with FullName = {fullname}.");
            return _mapper.Map<EmployeeDTO>(entity);
        }

        public EmployeeDTO? GetByUserName(string userName)
        {
            var employee = _repository.GetbyUserName(userName); // DAL
            if (employee == null) return null;
            return _mapper.Map<EmployeeDTO>(employee);
        }


        //Thêm mới
        public void Add(EmployeeDTO dto)
        {
            Validate(dto,isNew:true);
            var entity = _mapper.Map<Employees>(dto);
            entity.Password = BC.HashPassword(dto.Password);
            _repository.Add(entity);
        }

        //Cập nhật
        public void Update(int id, EmployeeDTO dto)
        {
            Validate(dto,isNew:false);

            var entity = _repository.GetById(id);
            if (entity == null)
                throw new Exception($"Employee not found with ID = {id}.");

            entity.FullName = dto.FullName;
            entity.UserName = dto.UserName;
            entity.EmployeeAddress = dto.EmployeeAddress;
            entity.EmployeePhone = dto.EmployeePhone;
            entity.Role = dto.Role;

            // Optional password update
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                entity.Password = BC.HashPassword(dto.Password);
            }

            _repository.Update(entity);
        }

        public void UpdatePassword(int id, string hashedPassword)
        {
            _repository.UpdatePassword(id, hashedPassword);
        }


        //Xóa
        public void Delete(int id)
        {
            var entity = _repository.GetById(id);
            if (entity == null)
                throw new Exception($"Manufacturer not found with ID = {id}.");

            _repository.Delete(entity);
        }
    }
}

