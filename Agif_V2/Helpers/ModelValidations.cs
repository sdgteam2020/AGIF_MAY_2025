namespace Agif_V2.Helpers
{
    public class ModelValidations
    {
        public bool IsValidEmailDomain(string emailDomain)
        {
            var allowedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "gmail.com",
            "yahoo.com",
            "yahoo.co.in",
            "rediffmail.com",
            "outlook.com",
            "protonmail.com",
            "hotmail.com",
            "icloud.com",
            "zohomail.com"
        };

            return !string.IsNullOrWhiteSpace(emailDomain)
                   && allowedDomains.Contains(emailDomain);
        }
        public string CalculateSuffix(string armyNumber)
        {
            if (string.IsNullOrWhiteSpace(armyNumber))
                return string.Empty;

            armyNumber = armyNumber.PadLeft(8, '0');

            string weights = "98765432";
            int total = 0;

            for (int i = 0; i < 8; i++)
            {
                total += (armyNumber[i] - '0') * (weights[i] - '0');
            }

            int remainder = total % 11;

            return remainder switch
            {
                0 => "A",
                1 => "F",
                2 => "H",
                3 => "K",
                4 => "L",
                5 => "M",
                6 => "N",
                7 => "P",
                8 => "W",
                9 => "X",
                10 => "Y",
                _ => string.Empty
            };
        }
    }
}
