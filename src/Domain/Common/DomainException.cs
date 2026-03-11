namespace ExcelDashboard.Domain.Common;

public class DomainException : Exception
{
    public DomainException(string code)
        : base(code)
    {
        Code = code;
    }

    public string Code { get; }
}

