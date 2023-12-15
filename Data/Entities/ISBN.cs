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

        private const byte _length = 13;
        private const byte _prefixLength = 3;
        private readonly byte _countryLength;
        private readonly byte _publisherLength;
        private readonly byte _publicationLength;
        private const byte _checksumLength = 1;

        public ISBN(uint country, uint publisher, uint publication, byte checksum)
        {
            if (!Enum.IsDefined((Country)country))
            {
                throw new ArgumentOutOfRangeException(nameof(country), "Invalid country code");
            }
            _country = (Country)country;
            _countryLength = GetLength(country);

            _publisherLength = GetLength(publisher);
            _publicationLength = GetLength(publication);

            if (_publisherLength + _publicationLength != _length - _prefixLength - _countryLength - _checksumLength)
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

        public override string ToString()
        {
            return $"{_prefix}-{(uint)_country}-{_publisher}-{_publication}-{_checksum}";
        }

        private byte GetChecksum()
        {
            var numbers = GetNumbers();

            int evenSum = 0, oddSum = 0;

            for (int i = 0; i < _length - _checksumLength; i++)
            {
                if ((i + 1) % 2 == 1)
                {
                    oddSum += numbers[i];
                }
                else
                {
                    evenSum += numbers[i];
                }
            }

            int sum = oddSum + evenSum * 3;

            int ceiled = (int)Math.Ceiling(sum / 10d) * 10;

            return (byte)Math.Abs(ceiled - sum);
        }

        private byte[] GetNumbers()
        {
            byte j = 0;
            byte[] numbers = new byte[_length];

            for (byte i = _prefixLength; i > 0; i--, j++)
            {
                uint divider = (uint)Math.Pow(10, i - 1);

                numbers[j] = (byte)(_prefix / divider % 10);
            }

            for (byte i = _countryLength; i > 0; i--, j++)
            {
                uint divider = (uint)Math.Pow(10, i - 1);

                numbers[j] = (byte)((uint)_country / divider % 10);
            }

            for (byte i = _publisherLength; i > 0; i--, j++)
            {
                uint divider = (uint)Math.Pow(10, i - 1);

                numbers[j] = (byte)(_publisher / divider % 10);
            }

            for (byte i = _publicationLength; i > 0; i--, j++)
            {
                uint divider = (uint)Math.Pow(10, i - 1);

                numbers[j] = (byte)(_publication / divider % 10);
            }

            numbers[j] = _checksum;

            return numbers;
        }

        private byte GetLength(ulong value)
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
