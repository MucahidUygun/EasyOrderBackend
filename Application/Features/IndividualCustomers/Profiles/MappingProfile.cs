using Application.Features.Customers.Dtos.Requests;
using Application.Features.Customers.Dtos.Response;
using Application.Features.Customers.Dtos.Responses;
using Application.Features.IndividualCustomers.Dtos.Requests;
using Application.Features.IndividualCustomers.Dtos.Responses;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.IndividualCustomers.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<IndividualCustomer, CreateIndividualCustomerRequest>().ReverseMap();
        CreateMap<IndividualCustomer, CreatedIndividualCustomerResponse>().ReverseMap();
        CreateMap<IndividualCustomer, UpdateIndividualCustomerRequest>().ReverseMap();
        CreateMap<IndividualCustomer, UpdatedIndividualCustomerResponse>().ReverseMap();
        CreateMap<IndividualCustomer, DeletedIndividualCustomerResponse>().ReverseMap();
        CreateMap<IndividualCustomer, DeleteIndividualCustomerRequest>().ReverseMap();
        CreateMap<IndividualCustomer, GetByIdIndividualCustomerQueryResponse>().ReverseMap();
        CreateMap<IndividualCustomer, GetByIdIndividualCustomerQueryRequest>().ReverseMap();
        CreateMap<IndividualCustomer, GetListIndividualCustomerQueryResponse>().ReverseMap();
        CreateMap<IPaginate<IndividualCustomer>, Paginate<GetListIndividualCustomerQueryResponse>>().ReverseMap();
    }
}