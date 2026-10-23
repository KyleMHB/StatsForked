using NUnit.Framework;
using RimWorld;

namespace Stats.Tests;

[TestFixture]
public sealed class QualityValidationTests
{
    [TestCase(QualityCategory.Awful)]
    [TestCase(QualityCategory.Poor)]
    [TestCase(QualityCategory.Normal)]
    [TestCase(QualityCategory.Good)]
    [TestCase(QualityCategory.Excellent)]
    [TestCase(QualityCategory.Masterwork)]
    [TestCase(QualityCategory.Legendary)]
    public void Defined_quality_values_from_table_configuration_are_supported(QualityCategory quality)
    {
        var configuration = new TableConfiguration(quality: (int)quality);
        Assert.That(ObjectTable.IsSupportedQuality(configuration.Quality), Is.True);
    }

    [TestCase(-1)]
    [TestCase(7)]
    [TestCase(255)]
    [TestCase(256)]
    [TestCase(258)]
    [TestCase(int.MinValue)]
    [TestCase(int.MaxValue)]
    public void Invalid_saved_quality_values_are_rejected_without_wrapping(int quality)
    {
        Assert.That(ObjectTable.IsSupportedQuality(quality), Is.False);
    }
}