using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ElectronicsStore.DataAccess;
using ElectronicsStore.DataTransferObject;

namespace ElectronicsStore.BussinessLogic
{
    public class OrderDetailsService
    {
        private readonly IOrderDetailsRepository _repository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public OrderDetailsService(IMapper mapper)
        {
            _repository = new OrderDetailsRepository();
            _productRepository = new ProductRepository();
            _mapper = mapper;
        }


        //Tra cứu
        //Lấy danh sách chi tiết đơn hàng theo mã đơn hàng
        public List<OrderDetailsDTO> GetByOrderID(int orderId)
        {
            var entities = _repository.GetByOrderID(orderId);
            var dtos = _mapper.Map<List<OrderDetailsDTO>>(entities);

            // Gán thêm tên sản phẩm
            foreach (var dto in dtos)
            {
                dto.ProductName = _productRepository.GetById(dto.ProductID)?.ProductName;
            }

            return dtos;
        }

        //Thêm mới
        public void AddOrderDetails(List<OrderDetailsDTO> dtos)
        {
            var entities = _mapper.Map<List<Order_Details>>(dtos);
            _repository.AddRange(entities);
        }

        //Cập nhật
        public void UpdateOrderDetails(int orderId, List<OrderDetailsDTO> dtos)
        {
            _repository.DeleteByOrderID(orderId);
            var entities = _mapper.Map<List<Order_Details>>(dtos);
            _repository.AddRange(entities);
        }


    }
}
