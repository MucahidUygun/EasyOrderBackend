using Application.Features.Auth.Dtos.Requests;
using AutoMapper;
using Core.Entities;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Profiles;

public class MappingProfie  : Profile
{
    public MappingProfie()
    {
        CreateMap<BaseRefreshToken,RefreshToken>().ReverseMap();
        CreateMap<Customer,RegisterCustomerCommandRequest>().ReverseMap();
    }
}
