[Flags]
public enum BookingStatusFlags
{
    None = 0,
    Reserved = 1 << 0,
    Paid = 1 << 1,
    CheckedIn = 1 << 2,
    Cancelled = 1 << 3
}
