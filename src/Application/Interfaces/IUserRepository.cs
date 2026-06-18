using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRNAssessment.Domain.Entities;

namespace CRNAssessment.Application.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task UpdateAsync(User user);
}