﻿using FC.Codeflix.Catalog.Domain.Exceptions;

namespace FC.Codeflix.Catalog.Domain.Validation;
public class DomainValidation
{
    public static void NotNull(object? target, string fieldName)
    {
        if (target == null)
            throw new EntityValidationException(string.Format(ConstantsMessages.fieldNotNull, fieldName));
    }

    public static void NotNullOrEmpty(string? target, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new EntityValidationException(string.Format(ConstantsMessages.fieldNotEmptyOrNull, fieldName));
    }

    public static void MinLenght(string target, int minLenght, string fieldName)
    {
        if (target.Length < minLenght)
            throw new EntityValidationException(
                string.Format(ConstantsMessages.fieldNotMinLenght, fieldName, minLenght));
    }

    public static void MaxLenght(string target, int maxLenght, string fieldName)
    {
        if (target.Length > maxLenght)
            throw new EntityValidationException(
                string.Format(ConstantsMessages.fieldNotMaxLenght, fieldName, maxLenght));
    }
}
