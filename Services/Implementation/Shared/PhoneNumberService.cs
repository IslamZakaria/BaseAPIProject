using Core.Interfaces.Shared.Services;
using PhoneNumbers;

namespace Services.Implementation.Shared
{
    public class PhoneNumberService : IPhoneNumberService
    {
        private readonly PhoneNumberUtil _phoneNumberUtil;
        public PhoneNumberService()
        {
            _phoneNumberUtil = PhoneNumberUtil.GetInstance();
        }
        public string GetRegionCode(int countryCode)
        {
            return _phoneNumberUtil.GetRegionCodeForCountryCode(countryCode);
        }

        public bool IsPhoneNumberValid(string phoneNumber, int countryCode)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber))
                    return false;

                var regionCode = _phoneNumberUtil.GetRegionCodeForCountryCode(countryCode);

                if (string.IsNullOrEmpty(regionCode))
                    return false;

                var parsedPhoneNumber = _phoneNumberUtil.Parse(phoneNumber, regionCode);

                if (phoneNumber is null)
                    return false;

                return _phoneNumberUtil.IsValidNumberForRegion(parsedPhoneNumber, regionCode);
            }
            catch (NumberParseException e)
            {
                throw e;
            }
        }

        public bool IsPhoneNumberValid(string phoneNumber, string regionCode)
        {
            try
            {
                if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(regionCode))
                    return false;

                var parsedPhoneNumber = _phoneNumberUtil.Parse(phoneNumber, regionCode);

                if (phoneNumber is null)
                    return false;

                return _phoneNumberUtil.IsValidNumberForRegion(parsedPhoneNumber, regionCode);
            }
            catch (NumberParseException e)
            {
                throw e;
            }
        }
    }
}
