using BuyCrypt.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyCrypt.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetOrCreateAsync(Guid userId);
        Task<UserResponse> UpdateAsync(Guid userId, UpdateUserRequest request);
        Task DeleteAsync(Guid userId);
    }

}
