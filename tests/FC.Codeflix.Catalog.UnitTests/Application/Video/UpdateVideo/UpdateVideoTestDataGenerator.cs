using FC.Codeflix.Catalog.Application.UseCases.Video.UpdateVideo;
using FC.Codeflix.Catalog.Domain;
using System.Collections;

namespace FC.Codeflix.Catalog.UnitTests.Application.Video.UpdateVideo;
public class UpdateVideoTestDataGenerator : IEnumerable<object[]>
{

    public IEnumerator<object[]> GetEnumerator()
    {
        var fixture = new UpdateVideoTestFixture();
        var invalidInputList = new List<object[]>();
        var totalInvalidCases = 4;

        for (int i = 0; i < totalInvalidCases * 2; i++)
        {
            switch (i % totalInvalidCases)
            {
                case 0:
                    invalidInputList.Add(new object[] {
                         new UpdateVideoInput(
                            Guid.NewGuid(),
                            string.Empty,
                            fixture.GetValidDescription(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating(),
                            fixture.GetValidYearLauched(),
                            fixture.GetRandomBoolean()
                        ),
                        string.Format(ConstantsMessages.FIELD_NOT_EMPTY, "Title")
                    });
                    break;
                case 1:
                    invalidInputList.Add(new object[] {
                        new UpdateVideoInput(
                            Guid.NewGuid(),
                            fixture.GetValidTitle(),
                            string.Empty,
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating(),
                            fixture.GetValidYearLauched(),
                            fixture.GetRandomBoolean()
                        ),
                        string.Format(ConstantsMessages.FIELD_NOT_EMPTY, "Description")
                    });
                    break;
                case 2:
                    invalidInputList.Add(new object[] {
                        new UpdateVideoInput(
                           Guid.NewGuid(),
                            fixture.GetTooLongTitle(),
                            fixture.GetValidDescription(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating(),
                            fixture.GetValidYearLauched(),
                            fixture.GetRandomBoolean()
                        ),
                        string.Format(ConstantsMessages.FIELD_MAX_LENGHT, "Title", 255)
                    });
                    break;

                case 3:
                    invalidInputList.Add(new object[] {
                        new UpdateVideoInput(
                            Guid.NewGuid(),
                            fixture.GetValidTitle(),
                            fixture.GetTooLongDescription(),
                            fixture.GetRandomBoolean(),
                            fixture.GetValidDuration(),
                            fixture.GetRandomRating(),
                            fixture.GetValidYearLauched(),
                            fixture.GetRandomBoolean()
                        ),
                        string.Format(ConstantsMessages.FIELD_MAX_LENGHT, "Description", 4000)
                    });
                    break;
                default:
                    break;
            }
        }

        return invalidInputList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}