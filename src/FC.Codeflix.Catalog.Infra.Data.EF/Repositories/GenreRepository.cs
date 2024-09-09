using FC.Codeflix.Catalog.Application.Exceptions;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.Domain.SeedWork.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
public class GenreRepository : IGenreRepository
{
    private readonly CodeflixCatelogDbContext _dbContext;
    private DbSet<Genre> _genres =>
        _dbContext.Set<Genre>();

    private DbSet<GenresCategories> _genresCategories =>
        _dbContext.Set<GenresCategories>();

    public GenreRepository(CodeflixCatelogDbContext dbContext)
        => _dbContext = dbContext;

    public async Task InsertAsync(Genre aggregate, CancellationToken cancellationToken)
    {
        await _genres.AddAsync(aggregate, cancellationToken);
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId =>
                    new GenresCategories(categoryId, aggregate.Id)
                );

            await _genresCategories.AddRangeAsync(relations, cancellationToken);
        }
    }

    public Task DeleteAsync(Genre aggregate, CancellationToken cancellationToken)
    {
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );

        _genres.Remove(aggregate);

        return Task.CompletedTask;
    }

    public async Task<Genre> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var genre = await _genres
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        NotFoundException.ThrowIfNull(
            genre,
            string.Format(ConstantsMessages.GenreIdNotFound, id)
        );

        var categoryIds = await _genresCategories
            .Where(relation => relation.Genre!.Id == genre!.Id)
            .Select(categoryId => categoryId.CategoryId)
            .ToListAsync(cancellationToken);

        categoryIds.ForEach(genre!.AddCategory);

        return genre;
    }


    public async Task<SearchOutput<Genre>> SearchAsync(SearchInput input, CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;

        var query = _genres.AsNoTracking();

        query = AddOrderToQuery(query, input.OrderBy, input.SearchOrder);

        if (string.IsNullOrEmpty(input.Search) is not true)
            query = query.Where(genre => genre.Name.Contains(input.Search));

        var total = await query.CountAsync(cancellationToken);

        var genres = await query
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync(cancellationToken);

        var genresIds = genres.Select(genre => genre.Id);

        var relations = await _genresCategories
            .Where(relation => genresIds.Contains(relation.GenreId))
            .ToArrayAsync(cancellationToken);

        var relationsByGenreIdGroup = relations.GroupBy(x => x.GenreId).ToList();
        relationsByGenreIdGroup.ForEach(relationGroup =>
        {
            var genre = genres.Find(genre => genre.Id == relationGroup.Key);
            if (genre is null) return;

            relationGroup.ToList().ForEach(relation => genre.AddCategory(relation.CategoryId));
        });

        return new SearchOutput<Genre>(input.Page, input.PerPage, genres, total);
    }

    public async Task UpdateAsync(Genre aggregate, CancellationToken cancellation)
    {
        _genres.Update(aggregate);
        _genresCategories.RemoveRange(
            _genresCategories.Where(x => x.GenreId == aggregate.Id)
        );
        if (aggregate.Categories.Count > 0)
        {
            var relations = aggregate.Categories
                .Select(categoryId =>
                    new GenresCategories(categoryId, aggregate.Id)
                );

            await _genresCategories.AddRangeAsync(relations, cancellation);
        }
    }

    private static IQueryable<Genre> AddOrderToQuery(
        IQueryable<Genre> query,
        string orderProperty,
        SearchOrder order
    )
    {
        var orderedQuery = (orderProperty.ToLower(), order) switch
        {
            ("name", SearchOrder.ASC) => query.OrderBy(x => x.Name).ThenBy(x => x.Id.ToString()),
            ("name", SearchOrder.DESC) => query.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id.ToString()),
            ("id", SearchOrder.ASC) => query.OrderBy(x => x.Id.ToString()),
            ("id", SearchOrder.DESC) => query.OrderByDescending(x => x.Id.ToString()),
            ("createdat", SearchOrder.ASC) => query.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.DESC) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name).ThenBy(x => x.Id.ToString())
        };

        return orderedQuery;
    }

    public Task<IReadOnlyList<Guid>> GetIdsListByIdsAsync(List<Guid> guids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Genre>> GetListByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
