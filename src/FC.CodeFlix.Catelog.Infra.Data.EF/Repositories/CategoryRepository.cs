﻿using FC.CodeFlix.Catalog.Application.Exceptions;
using FC.CodeFlix.Catalog.Domain.Entity;
using FC.CodeFlix.Catalog.Domain.Repository;
using FC.CodeFlix.Catalog.Domain.SeedWork.SearchableRepository;
using Microsoft.EntityFrameworkCore;

namespace FC.CodeFlix.Catelog.Infra.Data.EF.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly CodeFlixCatelogDbContext _dbContext;
    private DbSet<Category> _categories =>
        _dbContext.Set<Category>();

    public CategoryRepository(
        CodeFlixCatelogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(Category aggregate, CancellationToken cancellationToken) =>
        await _categories.AddAsync(aggregate, cancellationToken);


    public Task DeleteAsync(Category aggregate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Category> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _categories.FindAsync(new object[] { id }, cancellationToken);

        NotFoundException.ThrowIfNull(
            category,
            string.Format(ConstantsMessages.CategoryIdNotFound, id));

        return category!;

    }

    public Task<SearchOutput<Category>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Category aggregate, CancellationToken cancellation) => Task.FromResult(_categories.Update(aggregate));
}
