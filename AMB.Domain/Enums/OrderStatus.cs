namespace AMB.Domain.Enums
{
    public enum OrderStatus
    {
        Draft = 1,
        SentToKDS = 2,
        Preparing = 3,
        OnHold = 4,
        Ready = 5,
        Served = 6,
        Cancelled = 7
    }
}