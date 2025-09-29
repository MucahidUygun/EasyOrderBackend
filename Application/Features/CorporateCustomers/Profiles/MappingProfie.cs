using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.CorporateCustomers.Dtos.Requests;
using Application.Features.CorporateCustomers.Dtos.Responses;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.CorporateCustomers.Profiles;

public class MappingProfie : Profile
{
    public MappingProfie()
    {
        CreateMap<CorporateCustomer,CreateCorporateCustomerRequest>().ReverseMap();
        CreateMap<CorporateCustomer,CreatedCorporateCustomerResponse>().ReverseMap();
        CreateMap<CorporateCustomer, UpdatedCorporateCustomerResponse>().ReverseMap();
        CreateMap<CorporateCustomer,UpdateCorporateCustomerRequest>().ReverseMap();
        CreateMap<CorporateCustomer, DeletedCorporateCustomerResponse>().ReverseMap();
        CreateMap<CorporateCustomer, DeleteCorporateCustomerRequest>().ReverseMap();
    }
}
