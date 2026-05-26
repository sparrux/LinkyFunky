namespace LinkyFunky.Application.Interfaces;

public interface IGuestService
{
    bool IsGuest { get; }
    Guid GuestIdOrDefault { get; }

    void SetGuest();
    void DeleteGuest();
}