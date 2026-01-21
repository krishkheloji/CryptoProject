using AutoMapper;
using BuyCrypt.Application.DTOs;
using BuyCrypt.Application.Interfaces;
using BuyCrypt.Domain.Models;
using BuyCrypt.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class UserService : IUserService
{
    private readonly CryptoDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContext;

    public UserService(
        CryptoDbContext context,
        IMapper mapper,
        IHttpContextAccessor httpContext)
    {
        _context = context;
        _mapper = mapper;
        _httpContext = httpContext;
    }

    public async Task<UserResponse> GetOrCreateAsync(Guid userId)
    {
        var user = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
        {
            var claims = _httpContext.HttpContext!.User;

            var email = claims.FindFirst(ClaimTypes.Email)?.Value;
            var username = claims.FindFirst("username")?.Value;

            var createDto = new CreateUserRequest
            {
                FullName = username ?? "User",
                Email = email ?? "unknown@email.com",
                PreferredCurrency = "INR"
            };

            user = _mapper.Map<UserProfile>(createDto);

            typeof(UserProfile)
                .GetProperty(nameof(UserProfile.UserId))!
                .SetValue(user, userId);

            _context.UserProfiles.Add(user);
            await _context.SaveChangesAsync();
        }

        return _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse> UpdateAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId)
            ?? throw new Exception("User not found");

        _mapper.Map(request, user);

        typeof(UserProfile)
            .GetProperty(nameof(UserProfile.UpdatedAt))!
            .SetValue(user, DateTime.UtcNow);

        await _context.SaveChangesAsync();

        return _mapper.Map<UserResponse>(user);
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await _context.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (user == null) return;

        _context.UserProfiles.Remove(user);
        await _context.SaveChangesAsync();
    }
}
