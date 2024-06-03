using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Models;

namespace Heracles.Domain.UserExercises.Interfaces;

public interface IUserExerciseService
{
    Task<DomainResponse<QueryResponseDto<UserExercise>>> GetAsync(QueryRequestDto query);
    Task<DomainResponse<UserExercise>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(CreateUserExerciseDto dto);
    Task<DomainResponse<bool>> UpdateAsync(UpdateUserExerciseDto dto);
    Task<DomainResponse<bool>> DeleteAsync(int id);
}