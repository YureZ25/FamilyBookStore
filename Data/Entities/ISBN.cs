namespace Data.Entities
{
    public struct ISBN
    {
        public enum Country : uint
        {
            USA = 0,
            Japan = 4,
            Russia = 5,
            China = 7,
            Ukraine = 966,
        }

        private const ushort _prefix = 978;
        private readonly Country _country;
        private readonly uint _publisher;
        private readonly uint _publication;
        private readonly byte _checksum;

        public ISBN(uint country, uint publisher, uint publication, byte checksum)
        {
            if (!Enum.IsDefined((Country)country))
            {
                throw new ArgumentOutOfRangeException(nameof(country), "Invalid country code");
            }
            _country = (Country)country;

            if (GetNumberLength(publisher) + GetNumberLength(publication) != 8)
            {
                throw new ArgumentException("Publisher or publication code is invalid");
            }
            _publisher = publisher;
            _publication = publication;

            if (GetChecksum() != checksum)
            {
                throw new ArgumentException("ISBN is invalid");
            }
            _checksum = checksum;
        }

        private byte GetChecksum()
        {
            var nums = new List<byte>();

            byte publicationLen = GetNumberLength(_publication);
            for (byte i = 0; i < publicationLen; i++)
            {
                uint divider = (uint)Math.Pow(10, i);

                nums.Add((byte)(_publication / divider % 10));
            }

            byte publisherLen = GetNumberLength(_publisher);
            for (byte i = 0; i < publisherLen; i++)
            {
                uint divider = (uint)Math.Pow(10, i);

                nums.Add((byte)(_publisher / divider % 10));
            }

            uint countryInt = (uint)_country;
            byte countryLen = GetNumberLength(countryInt);
            for (byte i = 0; i < countryLen; i++)
            {
                uint divider = (uint)Math.Pow(10, i);

                nums.Add((byte)(countryInt / divider % 10));
            }

            byte prefixLen = GetNumberLength(_prefix);
            for (byte i = 0; i < prefixLen; i++)
            {
                uint divider = (uint)Math.Pow(10, i);

                nums.Add((byte)(_prefix / divider % 10));
            }

            int evenSum = 0, oddSum = 0;

            for (int i = 11; i >= 0; i--)
            {
                if (i % 2 == 1)
                {
                    oddSum += nums[i];
                }
                else
                {
                    evenSum += nums[i];
                }
            }

            int sum = oddSum + evenSum * 3;

            int ceiled = (int)Math.Ceiling(sum / 10d) * 10;

            return (byte)Math.Abs(ceiled - sum);
        }

        private byte GetNumberLength(ulong value)
        {
            byte valueLen = 0;
            while (value % Math.Pow(10, valueLen) != value)
            {
                valueLen++;
            }
            return valueLen;
        }
    }
}
