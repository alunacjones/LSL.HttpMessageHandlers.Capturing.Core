using FluentAssertions;
using LSL.ExecuteIf;

namespace LSL.HttpMessageHandlers.Capturing.Core.Tests;

public class OptionsHelperTests
{
    [TestCase(null, false, "{guid}")]
    [TestCase(null, true, null)]
    [TestCase("name", false, "name-{guid}")]
    [TestCase("name", true, "name-{guid}")]
    public void GivenBuildUniqueName_GivenInputs_ItShouldReturnTheExpectedResult(string originalName, bool preserveNull, string expectedOutput)
    {
        OptionsHelper.BuildUniqueName(originalName, preserveNull).ExecuteIf(
            expectedOutput is null,
            o => o.Should().BeNull(),
            o => o.Should().Match(expectedOutput.Replace("{guid}", "*-*-*-*-*")));
    }
}