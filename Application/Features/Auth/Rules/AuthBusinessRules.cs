using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Constants;
using Application.Features.Auth.Dtos.Requests;
using Core.Application.Rules;
using Core.CrossCuttingConcerns.Expeptions.Types;
using Core.Entities;
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

    public void UserShouldBeExistsWhenSelected(User? user)
    {
        if (user is null)
             throw new BusinessException(AuthMessages.UserDontExists);
    }
    
    //ForgotPassword
    public void CheckForgotPasswordRequestIsNull(ForgotPasswordRequest? request)
    {
        if (request is null)
            throw new BusinessException(AuthMessages.EmailCannotBeEmpty);
    }
    public void CheckUserIsNull(User? user,string message)
    {
        if (user is null)
            throw new BusinessException(message);
    }
    public void CheckAccountIsActive(EmailAuthenticator? emailAuthenticator) 
    {
        if (emailAuthenticator is null)
            throw new BusinessException(AuthMessages.ActiveteAccount);
    }
    //Login
    public void LoginRequestIsNull(LoginCommandRequest? request)
    {
        if (request is null)
            throw new BusinessException(AuthMessages.RequestCannotBeNull);
    }
    public void LoginEmailIsHave(string? email)
    {
        if (email is null || email.Length == 0)
            throw new BusinessException(AuthMessages.EmailCannotBeEmpty);
    }
    public void UserPasswordShouldBeMatch(User user, string password)
    {
        if (!HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            throw new BusinessException(AuthMessages.WrongPassword);
    }
    //Logout
    public void RefreshTokenIsHave(string? refreshToken)
    {
        if (refreshToken is null)
            throw new BusinessException(AuthMessages.RefreshDontExists);
    }
    public void RefreshTokenIsHave(BaseRefreshToken? refreshToken)
    {
        if (refreshToken is null)
            throw new BusinessException(AuthMessages.InvalidRefreshToken);
        RefreshTokenIsHave(refreshToken.Token);
    }
    public void IpAdressIsHave(string? ipAdress) 
    {
        if (ipAdress is null)
            throw new BusinessException(AuthMessages.IpAdressDontExists);
    }

    //CorporateCustomer
    public void IsCorpateFieldNull(RegisterCorporateCustomerRequest? customer)
    {
        if (customer is null)
            throw new BusinessException(AuthMessages.RequestCannotBeNull);
        if (customer!.CompanyName is null || customer.CompanyName.Length == 0)
            throw new BusinessException(AuthMessages.CompanyNameCannotBeNullOrEmpty);
        if (customer.TaxOffice is null || customer.TaxOffice.Length == 0)
            throw new BusinessException(AuthMessages.TaxOfficeCannotBeNull);
        if (customer.TaxNumber is null || customer.TaxNumber.Length == 0)
            throw new BusinessException(AuthMessages.TaxNumberCannotBeNull);
    }
    public void IsCorporateCustomerRequestNull(RegisterCorporateCustomerRequest? request)
    {
        if (request is null)
            throw new BusinessException(AuthMessages.RequestCannotBeNull);
    }
    //Customer
    public void IsCustomerRequestNull(RegisterCustomerCommandRequest? request)
    {
        if (request is null)
            throw new BusinessException(AuthMessages.RequestCannotBeNull);
    }
}
