namespace Core.Interfaces.Shared.Services
{
    public interface IPhoneNumberService
    {
        bool IsPhoneNumberValid(string phoneNumber, int countryCode);
        bool IsPhoneNumberValid(string phoneNumebr, string regionCode);
        string GetRegionCode(int countryCode);
    }
}
