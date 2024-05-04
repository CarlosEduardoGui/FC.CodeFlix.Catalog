﻿using FC.Codeflix.Catalog.UnitTests.Commom;
using Xunit;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Domain.Entity.Video;

[CollectionDefinition(nameof(VideoTestFixture))]
public class VideoTestFixtureCollection : ICollectionFixture<VideoTestFixture> { }

public class VideoTestFixture : BaseFixture
{
    public DomainEntity.Video GetValidVideo()
        => new(
            GetValidTitle(),
            GetValidDescription(),
            GetValidYearLauched(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidDuration()
        );

    public string GetTooLongTitle()
        => Faker.Lorem.Letter(400);

    public string GetValidTitle()
        => Faker.Lorem.Letter(100);

    public string GetValidDescription()
        => Faker.Commerce.ProductDescription();

    public int GetValidYearLauched()
        => Faker.Date.BetweenDateOnly(
            new DateOnly(1960, 1, 1),
            new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            ).Year;

    public int GetValidDuration()
        => new Random().Next(100, 300);
}
