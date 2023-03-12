using System;

namespace Soda.Http.Exceptions;

public class SodaHttpException : Exception
{
    public SodaHttpException(string eMessage) : base(eMessage)
    {
    }
}