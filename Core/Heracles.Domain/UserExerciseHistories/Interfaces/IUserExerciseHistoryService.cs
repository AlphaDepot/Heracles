using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExerciseHistories.DTOs;
using Heracles.Domain.UserExerciseHistories.Models;

namespace Heracles.Domain.UserExerciseHistories.Interfaces;

public interface IUserExerciseHistoryService
{
    Task<ServiceResponse<UserExerciseHistory>> GetByIdAsync(int id);
    Task<ServiceResponse<QueryResponseDto<UserExerciseHistory>>> GetAsync(QueryRequestDto query);
    Task<ServiceResponse<int>> CreateAsync(UserExerciseHistory entity);
    Task<ServiceResponse<bool>> UpdateAsync(UpdateUserExerciseHistoryDto entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}