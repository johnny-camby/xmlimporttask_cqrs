
using AutoMapper;
using BusinessLogic.CQRS.FullAddresses.Dtos;
using BusinessLogic.Exceptions;
using Data.Repository.Entities;
using Data.Repository.Interfaces;
using MediatR;

namespace BusinessLogic.CQRS.Customers.Queries.GetDetails
{
    public class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQueryRequest, CustomerDetailVm>
    {
        private readonly IMapper _mapper;
        private readonly IXmlImporterRepository<CustomerEntity> _customerRepository;
        public GetCustomerDetailQueryHandler(IMapper mapper,
            IXmlImporterRepository<CustomerEntity> customerRepository)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDetailVm> Handle(GetCustomerDetailQueryRequest request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetAsync(request.Id);
            var customerDetail = _mapper.Map<CustomerDetailVm>(customer);
            customerDetail.Address = customer.FullAddress.Address;
            customerDetail.City = customer.FullAddress.City;
            customerDetail.Region = customer.FullAddress.Region;
            customerDetail.PostalCode = customer.FullAddress.PostalCode;
            customerDetail.Country = customer.FullAddress.Country;
            
            return customerDetail;
        }
    }
}
