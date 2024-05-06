using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Models;

namespace Heracles.Domain.UserExercises.Interfaces;

public interface IUserExerciseService
{
    Task<DomainResponse<QueryResponse<UserExercise>>> GetAsync(QueryRequest query);
    Task<DomainResponse<UserExercise>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(CreateUserExerciseDto dto);
    Task<DomainResponse<bool>> UpdateAsync(UpdateUserExerciseDto dto);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}