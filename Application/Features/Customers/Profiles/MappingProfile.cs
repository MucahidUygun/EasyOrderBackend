using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Response;
using Application.Features.Customers.Dtos.Responses;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customers.Profiles;

public class MappingProfile: Profile
{
    public MappingProfile() 
    {
        CreateMap<Customer, CreatedCustomerResponse>().ReverseMap();
        CreateMap<Customer, CreateCustomerRequest>().ReverseMap();
        CreateMap<Customer,UpdateCustomerRequest>().ReverseMap();
        CreateMap<Customer,UpdatedCustomerResponse>().ReverseMap();
        CreateMap<Customer, DeleteCustomerRequest>().ReverseMap();
        CreateMap<Customer, DeletedCustomerResponse>().ReverseMap();
        CreateMap<Customer, GetByIdCustomerQueryResponse>().ReverseMap();
        CreateMap<Customer, GetByIdCustomerQueryRequest>().ReverseMap();
        CreateMap<Customer, GetListCustomerQueryResponse>().ReverseMap();
        CreateMap<IPaginate<Customer>, Paginate<GetListCustomerQueryResponse>>().ReverseMap();
    }
}
