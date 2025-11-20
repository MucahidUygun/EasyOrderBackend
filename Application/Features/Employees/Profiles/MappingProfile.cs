using Application.Features.Employees.Dtos.Requests;
using Application.Features.Employees.Dtos.Responses;
using AutoMapper;
using Core.Persistence.Paging;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employees.Profiles;

public class MappingProfile:Profile
{
    public MappingProfile()
    {
        CreateMap<Employee,CreateEmplooyeRequest>().ReverseMap();
        CreateMap<Employee,CreatedEmplooyeResponse>().ReverseMap();
        CreateMap<Employee,UpdateEmployeeReqeust>().ReverseMap();
        CreateMap<Employee, UpdatedEmplooyeResponse>().ReverseMap();
        CreateMap<Employee, GetByIdEmplooyeResponse>().ReverseMap();
        CreateMap<Employee, DeletedEmplooyeResponse>().ReverseMap();
        CreateMap<Employee, GetListEmplooyeResponse>().ReverseMap();
        CreateMap<IPaginate<Employee>,Paginate<GetListEmplooyeResponse>>().ReverseMap();
    }
}
