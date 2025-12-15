using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Constants;

public static class AuthMessages
{
    public const string SectionName = "Auth";

    public const string BaseAuthUrl = "https://localhost:7064/api/Auth/";

    public const string EmailAuthenticatorDontExists = "EmailAuthenticator Dont Exists";
    public const string OtpAuthenticatorDontExists = "OtpAuthenticator Dont Exists";
    public const string AlreadyVerifiedOtpAuthenticatorIsExists = "Already Verified OtpAuthenticator Is Exists";
    public const string EmailActivationKeyDontExists = "Email ActivationKey Dont Exists";
    public const string UserDontExists = "User Dont Exists";
    public const string UserHaveAlreadyAAuthenticator = "User Have Already Authenticator";
    public const string RefreshDontExists = "Refresh Dont Exists";
    public const string InvalidRefreshToken = "Invalid RefreshToken";
    public const string UserMailAlreadyExists = "User Mail Already Exists";
    public const string PasswordDontMatch = "Password Dont Match";
    public const string PasswordResetRequestExpired = "Süresi Doldu!";
    public const string SendEmailForEmailActivate = "Emailinizi onaylamanız için e-posta gönderildi.";
    public const string RequestCannotBeNull = "Request cannot be null";
    public const string WrongPassword = "Girilen şifre doğru değildir!";
    public const string EmailCannotBeEmpty = "Email cannot be empty!";

    //CorporateCustomer
    public const string CompanyNameCannotBeNullOrEmpty = "Şirket ünvanı boş olamaz!";
    public const string TaxOfficeCannotBeNull = "Vergi dairesi boş olamaz!";
    public const string TaxNumberCannotBeNull = "Vergi numarası boş olamaz";

    //ForgotPassword
    public const string PasswordResetEmailSent = "Password reset e-mail sent.";
    public const string ActiveteAccount = "Please activate your account!";
    //Login
    public const string IpAdressDontExists = "IpAdress dont exists";
    //LogOut
    public const string LogOut = "Log out";
    public const string LogOutSuccess = "Log out is success";

}
