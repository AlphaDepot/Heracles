using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExerciseHistories.DTOs;
using Heracles.Domain.UserExerciseHistories.Models;

namespace Heracles.Domain.UserExerciseHistories.Interfaces;

public interface IUserExerciseHistoryService
{
    Task<DomainResponse<UserExerciseHistory>> GetByIdAsync(int id);
    Task<DomainResponse<QueryResponse<UserExerciseHistory>>> GetAsync(QueryRequest query);
    Task<DomainResponse<int>> CreateAsync(UserExerciseHistory entity);
    Task<DomainResponse<bool>> UpdateAsync(UpdateUserExerciseHistoryDto entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}