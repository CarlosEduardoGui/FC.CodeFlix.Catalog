using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Validator;
public class VideoValidator : Validator
{
    private const int TITLE_MAX_LENGHT = 255;
    private const int DESCRIPTION_MAX_LENGHT = 4000;
    private readonly Video _video;

    public VideoValidator(Video video, ValidationHandler handler) : base(handler)
        => _video = video;

    public override void Validate()
    {
        ValidateTitle();
        ValidateDescription();
    }

    private void ValidateTitle()
    {
        if (string.IsNullOrEmpty(_video.Title))
            _handler.HandleError(string.Format(ConstantsMessages.FIELD_NOT_EMPTY, nameof(_video.Title)));

        if (_video.Title.Length > TITLE_MAX_LENGHT)
            _handler.HandleError(string.Format(ConstantsMessages.FIELD_MAX_LENGHT, nameof(_video.Title), TITLE_MAX_LENGHT));
    }

    private void ValidateDescription()
    {
        if (string.IsNullOrEmpty(_video.Description))
            _handler.HandleError(string.Format(ConstantsMessages.FIELD_NOT_EMPTY, nameof(_video.Description)));

        if (_video.Description.Length > DESCRIPTION_MAX_LENGHT)
            _handler.HandleError(string.Format(ConstantsMessages.FIELD_MAX_LENGHT, nameof(_video.Description), DESCRIPTION_MAX_LENGHT));
    }

}
