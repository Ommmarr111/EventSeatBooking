namespace EventSeatBooking.Domain.ValueObjects
{
    public class SeatNumber
    {
        public string Row { get; }
        public int Number { get; }

        private SeatNumber(string row, int number)
        {
            Row = row;
            Number = number;
        }

        public static SeatNumber Of(string row, int number)
        {
            if (string.IsNullOrWhiteSpace(row))
                throw new Exceptions.DomainException("Seat row cannot be empty.");

            if (number <= 0)
                throw new Exceptions.DomainException("Seat number must be positive.");

            return new SeatNumber(row.ToUpperInvariant(), number);
        }

        public override bool Equals(object? obj) =>
            obj is SeatNumber other && Row == other.Row && Number == other.Number;

        public override int GetHashCode() => HashCode.Combine(Row, Number);

        public override string ToString() => $"{Row}{Number}";
    }
}