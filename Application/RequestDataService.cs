using ACorp.Data;
using ACorp.Models;
using Microsoft.EntityFrameworkCore;

namespace ACorp.Application;

public class RequestDataService
{
    readonly ApplicationDbContext _db;
    private AuthService _authService;

    public RequestDataService(ApplicationDbContext db)
    {
        _db = db;
        _authService = new AuthService(db);
    }

    public void Request(User requester, User requested)
    {
        if (IsRequestedByUser(requested, requester)) return;

        _db.RequestData.Add(new RequestData()
        {
            RequesterId = requester.Id,
            RequestedId = requested.Id
        });

        _db.SaveChanges();
    }

    public async Task<IEnumerable<User>> GetAllRequestedUserAsync(User requester)
    {
        List<User> users = new();

        foreach (var data in _db.RequestData.ToList())
        {
            if (data.RequesterId != requester.Id) continue;

            User? user = await _authService.FindUserAsync(data.RequestedId);
            if (user == null) continue;

            users.Add(user);
        }

        return users;
    }

    bool IsRequestedByUser(User requested, User requester)
    {
        foreach (var data in _db.RequestData.ToList())
        {
            if (data.RequesterId == requester.Id && data.RequestedId == requested.Id) return true;
        }

        return false;
    }
}