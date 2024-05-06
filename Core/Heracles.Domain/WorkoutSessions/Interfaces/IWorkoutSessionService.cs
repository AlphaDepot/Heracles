using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Models;

namespace Heracles.Domain.WorkoutSessions.Interfaces;

public interface IWorkoutSessionService
{
    Task<DomainResponse<QueryResponse<WorkoutSession>>> GetAsync(QueryRequest query);
    
    Task<DomainResponse<WorkoutSession>> GetByIdAsync(int id);
    Task<DomainResponse<int>> CreateAsync(WorkoutSession entity);
    Task<DomainResponse<bool>> UpdateAsync(WorkoutSession entity);
    Task<DomainResponse<bool>> DeleteAsync(int id);
    
    Task<DomainResponse<bool>> AddUserExerciseAsync(WorkoutSessionExerciseDto entity);
    Task<DomainResponse<bool>> RemoveUserExerciseAsync(WorkoutSessionExerciseDto entity);
}