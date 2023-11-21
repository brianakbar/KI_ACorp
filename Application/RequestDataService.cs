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
        _db.RequestData.Add(new RequestData()
        {
            RequesterId = requester.Id,
            RequestedId = requested.Id
        });

        _db.SaveChanges();
    }

    public async Task<IEnumerable<User>> GetAllRequestedUserAsync(int userId)
    {
        List<User> users = new();

        foreach (var data in _db.RequestData)
        {
            if (data.RequesterId != userId) continue;

            User? user = await _authService.FindUserAsync(userId);
            if (user == null) continue;

            users.Add(user);
        }

        return users;
    }
}