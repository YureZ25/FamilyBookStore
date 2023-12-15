namespace Data.Entities
{
    public struct ISBN
    {
        public enum Country : uint
        {
            English = 0,
            Japan = 4,
            Russia = 5,
            China = 7,
            Ukraine = 617,
        }

        private const ushort _prefix = 978;
        private readonly Country _country;
        private readonly byte[] _publisher;
        private readonly byte[] _publication;
        private readonly byte _checksum;

        private readonly bool _oldISBN;

        private const byte _length = 13;
        private const byte _prefixLength = 3;
        private readonly byte _countryLength;
        private readonly byte _publisherLength;
        private readonly byte _publicationLength;
        private const byte _checksumLength = 1;

        public static ISBN FromString(string ISBNString)
        {
            var splitted = ISBNString.Split('-');

            if (!splitted.All(s => s.All(c => char.IsDigit(c))))
            {
                throw new FormatException("ISBN must contain only digits and dashes");
            }

            if (splitted.Length == 5)
            {
                return new ISBN(
                    ushort.Parse(splitted[0]),
                    uint.Parse(splitted[1]),
                    splitted[2].ToCharArray().Select(c => (byte)char.GetNumericValue(c)).ToArray(),
                    splitted[3].ToCharArray().Select(c => (byte)char.GetNumericValue(c)).ToArray(),
                    byte.Parse(splitted[4]));
            }
            else if (splitted.Length == 4)
            {
                return new ISBN(
                    null,
                    uint.Parse(splitted[0]),
                    splitted[1].ToCharArray().Select(c => (byte)char.GetNumericValue(c)).ToArray(),
                    splitted[2].ToCharArray().Select(c => (byte)char.GetNumericValue(c)).ToArray(),
                    byte.Parse(splitted[3]));
            }
            else
            {
                throw new ArgumentException("ISBN must contain 5 or 4 (old) segments");
            }
        }

        public ISBN(ushort? prefix, uint country, byte[] publisher, byte[] publication, byte checksum)
        {
            _oldISBN = !prefix.HasValue;
            if (!_oldISBN && _prefix != prefix)
            {
                throw new ArgumentException($"Modern ISBN must have {_prefix} prefix");
            }

            if (!Enum.IsDefined((Country)country))
            {
                throw new ArgumentOutOfRangeException(nameof(country), "Invalid country code");
            }
            _country = (Country)country;
            _countryLength = GetLength(country);

            _publisherLength = (byte)publisher.Length;
            _publicationLength = (byte)publication.Length;

            if (_publisherLength + _publicationLength != _length - _prefixLength - _countryLength - _checksumLength)
            {
                throw new ArgumentException("Publisher or publication code is invalid");
            }
            _publisher = publisher;
            _publication = publication;

            _checksum = checksum;
            if (!Check())
            {
                throw new ArgumentException("ISBN is invalid");
            }
        }

        public override string ToString()
        {
            return $"{_prefix}-{(uint)_country}-{string.Concat(_publisher)}-{string.Concat(_publication)}-{_checksum}";
        }

        private bool Check()
        {
            if (!_oldISBN)
            {
                return GetEANChecksum() == _checksum;
            }
            else
            {
                return CheckOld();
            }
        }

        public bool CheckOld()
        {
            var numbers = GetNumbers();

            int i, s = 0, t = 0;

            for (i = 0; i < _length - _prefixLength; ++i)
            {
                t += numbers[i];
                s += t;
            }
            return s % 11 == 0;
        }

        private byte GetEANChecksum()
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
            byte[] numbers;

            if (!_oldISBN)
            {
                numbers = new byte[_length];

                for (byte i = _prefixLength; i > 0; i--, j++)
                {
                    uint divider = (uint)Math.Pow(10, i - 1);

                    numbers[j] = (byte)(_prefix / divider % 10);
                }
            }
            else
            {
                numbers = new byte[_length - _prefixLength];
            }

            for (byte i = _countryLength; i > 0; i--, j++)
            {
                uint divider = (uint)Math.Pow(10, i - 1);

                numbers[j] = (byte)((uint)_country / divider % 10);
            }

            for (int i = 0; i < _publisherLength; i++, j++)
            {
                numbers[j] = _publisher[i];
            }

            for (int i = 0; i < _publicationLength; i++, j++)
            {
                numbers[j] = _publication[i];
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
