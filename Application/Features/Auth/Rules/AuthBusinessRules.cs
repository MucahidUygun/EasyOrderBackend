using Core.Application.Rules;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Security.Hashing;
using Domain.Entities;
using Persistence.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Rules;

public class AuthBusinessRules : BaseBusinessRules
{
    private readonly IUserRepository _userRepository;

    public AuthBusinessRules(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task UserShouldBeExistsWhenSelected(User? user)
    {
        if (user is null)
             throw new BusinessException("Bu email ile kayıtlı kullanıcı bulunamadı!");
    }

    public async Task UserPasswordShouldBeMatch(User user,string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException("Yanlış Şifre");
    }

}
