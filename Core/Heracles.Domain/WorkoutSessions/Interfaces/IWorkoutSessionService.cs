using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Models;

namespace Heracles.Domain.WorkoutSessions.Interfaces;

public interface IWorkoutSessionService
{
    Task<ServiceResponse<QueryResponseDto<WorkoutSession>>> GetAsync(QueryRequestDto query);
    
    Task<ServiceResponse<WorkoutSession>> GetByIdAsync(int id);
    Task<ServiceResponse<int>> CreateAsync(WorkoutSession entity);
    Task<ServiceResponse<bool>> UpdateAsync(WorkoutSession entity);
    Task<ServiceResponse<bool>> DeleteAsync(int id);
    
    Task<ServiceResponse<bool>> AddUserExerciseAsync(WorkoutSessionExerciseDto entity);
    Task<ServiceResponse<bool>> RemoveUserExerciseAsync(WorkoutSessionExerciseDto entity);
}