using Application.Features.OperationClaims.Dtos.Requests;
using Application.Features.OperationClaims.Dtos.Responses;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.OperationClaims.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<OperationClaim,CreateOperationClaimCommandRequest>().ReverseMap();
        CreateMap<OperationClaim,CreatedOperationClaimCommandResponse>().ReverseMap();
    }
}
