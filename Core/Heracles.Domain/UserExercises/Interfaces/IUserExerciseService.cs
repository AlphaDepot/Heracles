using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Models;

namespace Heracles.Domain.UserExercises.Interfaces;

public interface IUserExerciseService
{
    Task<ServiceResponse<QueryResponseDto<UserExercise>>> GetAsync(QueryRequestDto query);
    Task<ServiceResponse<UserExercise>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(CreateUserExerciseDto dto);
    Task<ServiceResponse<bool>> UpdateAsync(UpdateUserExerciseDto dto);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
}