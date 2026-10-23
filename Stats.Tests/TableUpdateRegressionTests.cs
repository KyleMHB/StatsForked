using System;
using NUnit.Framework;

namespace Stats.Tests;

[TestFixture]
public sealed class TableUpdateRegressionTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void Failed_application_followed_by_frame_preparation_preserves_state_and_retries(bool throws)
    {
        using var session = new TableSession<int>();
        var metrics = new TableLayoutMetrics(new[] { 0 }, new[] { 20f });
        TableFrame<int> originalFrame = session.PrepareFrame(metrics).Frame;
        session.Queue(TableIntent.SetQuality(4));
        session.Queue(TableIntent.SetShowVariants(true));
        session.Queue(TableIntent.SetExpandedMultiValueCells(true));

        TableConfigurationTransitionResult rejected = session.StagePendingConfiguration(_ =>
            throws ? throw new InvalidOperationException("controlled application failure") : null);
        Assert.That(rejected.Applied, Is.False);
        Assert.That(session.HasRejectedConfiguration, Is.True);
        Assert.That(rejected.Diagnostics, Is.Not.Empty);

        // Exercise the drawing caller's next operation, even after staging failed.
        TableTransitionResult<int> frame = session.PrepareFrame(metrics);
        Assert.That(frame.Applied, Is.False);
        Assert.That(frame.Frame, Is.SameAs(originalFrame));
        Assert.That(session.Current.Quality, Is.Zero);
        Assert.That(session.Current.ShowVariants, Is.False);
        Assert.That(session.Current.ExpandMultiValueCells, Is.False);
        Assert.That(frame.Diagnostics, Is.EqualTo(rejected.Diagnostics));

        TableConfigurationTransitionResult retried = session.StagePendingConfiguration(configuration =>
            new TableConfigurationTransaction(configuration));
        Assert.That(retried.Applied, Is.True);
        Assert.That(session.HasRejectedConfiguration, Is.False);
        Assert.That(session.PrepareFrame(metrics).Applied, Is.True);
        Assert.That(session.Current.Quality, Is.EqualTo(4));
        Assert.That(session.Current.ShowVariants, Is.True);
        Assert.That(session.Current.ExpandMultiValueCells, Is.True);
    }
}
