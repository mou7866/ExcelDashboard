using System.Globalization;

namespace ExcelDashboard.Domain.Common;

public sealed record Money
{
    public Money(decimal value, string currency)
    {
        if (value < 0) throw new DomainException("Money.Negative");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Money.CurrencyRequired");
        if (currency.Length != 3) throw new DomainException("Money.CurrencyFormat");
        Value = decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        Currency = currency.ToUpperInvariant();
    }

    public decimal Value { get; }

    public string Currency { get; }

    public static Money FromString(string value, string currency)
    {
        if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            throw new DomainException("Money.Parse");
        return new Money(parsed, currency);
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Value + other.Value, Currency);
    }

    void EnsureSameCurrency(Money other)
    {
        if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
            throw new DomainException("Money.CrossCurrency");
    }
}

