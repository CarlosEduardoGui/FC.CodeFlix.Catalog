﻿using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Enum;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Commom;
using Moq;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.Common;

public class VideoUseCaseBaseFixture : BaseFixture
{
    public DomainEntity.Video GetValidVideo()
        => new(
            GetValidTitle(),
            GetValidDescription(),
            GetValidYearLauched(),
            GetRandomBoolean(),
            GetRandomBoolean(),
            GetValidDuration(),
            GetRandomRating()
        );

    public Rating GetRandomRating()
    {
        var values = Enum.GetValues<Rating>();
        var random = new Random();
        return (Rating)values.GetValue(random.Next(values.Length))!;
    }

    public string GetTooLongTitle()
        => Faker.Lorem.Letter(400);

    public string GetTooLongDescription()
        => Faker.Lorem.Letter(4001);

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

    public string GetValidImagePath()
        => Faker.Image.PlaceImgUrl();

    public string GetValidMediaPath()
    {
        var exampleMedias = new string[]
        {
            "https://wwww.googlestorage.com/file-example.mp4",
            "https://wwww.storage.com/file-example.mp4",
            "https://wwww.s3.com/file-example.mp4"
        };

        var random = new Random();
        return exampleMedias[random.Next(exampleMedias.Length)];
    }

    public Media GetValidMedia()
        => new(GetValidMediaPath());

    public Mock<IVideoRepository> GetRepository()
        => new();

    public Mock<ICategoryRepository> GetCategoryRepository()
        => new();

    public Mock<IGenreRepository> GetGenreRepository()
        => new();

    public Mock<ICastMemberRepository> GetCastMemberRepository() => new();

    public Mock<IStorageService> GetStorageService()
        => new();
}
