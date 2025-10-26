namespace Services.Phones;

public interface IRealtyClassifier
{
    Task<ClassificationResult> ClassifyAsync(string text, CancellationToken ct = default);
}