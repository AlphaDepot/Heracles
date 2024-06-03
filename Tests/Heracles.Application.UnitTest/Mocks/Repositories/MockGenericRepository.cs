using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.Abstractions.Interfaces;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockBaseRepository<T, TRepository> where T : BaseEntity where TRepository : class, IGenericRepository<T>
 {
     
     protected readonly List<T> Entities;
     protected readonly Mock<TRepository> MockRepo;

     protected MockBaseRepository(List<T> entities)
     {
         Entities = entities;
         MockRepo = new Mock<TRepository>();
         Setup();
     }

     private void Setup()
    {
        
        MockRepo.Setup(r => r.GetAsync(It.IsAny<QueryableEntityDto<T>>()))
            .ReturnsAsync((QueryableEntityDto<T> queryableDto) =>
            {
                var queryable = Entities.AsQueryable();
                if (queryableDto.Filter != null)
                {
                    queryable = queryable.Where(queryableDto.Filter);
                }
                if (queryableDto.Sorter != null)
                {
                    queryable = queryableDto.Sorter(queryable);
                }
                var result =  queryable.Skip((queryableDto.PageNumber - 1) * queryableDto.PageSize).Take(queryableDto.PageSize).ToList();
                
                return new QueryResponseDto<T>()
                {
                    Data =  result,
                    TotalPages = result.Count,
                    PageSize = queryableDto.PageSize,
                    PageNumber = queryableDto.PageNumber
                };
                
            });
        
        MockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!
           .ReturnsAsync((int id) => Entities.FirstOrDefault(q => q.Id == id ));

        
        MockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<string[]>()))!
            .ReturnsAsync((int id, string[] includeProperties) => Entities.FirstOrDefault(q => q.Id == id ));
        
        MockRepo.Setup(r => r.CreateAsync(It.IsAny<T>()))
            .ReturnsAsync((T entity) =>
            {
                entity.Id = Entities.Count + 1;
                Entities.Add(entity);
                return entity.Id;
            });
        
        MockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => Entities.Any(q => q.Id == id));
        
        MockRepo.Setup(r => r.UpdateAsync(It.IsAny<T>()))
            .ReturnsAsync((T entity) =>
            {
                var index = Entities.FindIndex(q => q.Id == entity.Id);
                Entities[index] = entity;
                return 1; // number of rows affected
            });
        
        MockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var entity = Entities.FirstOrDefault(q => q.Id == id);
                if (entity != null) Entities.Remove(entity);
                return 1; // number of rows affected
            });
    }
}