using System;
using System.Collections.Generic;
using Transbank.Responses.CommonResponses;
using Xunit;

namespace Transbank.Tests
{
    public class ResponseParsingHelperTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void SplitSegments_ShouldAddParseError_WhenPayloadIsBlank(string? payload)
        {
            List<string> parseErrors = new List<string>();

            string[] segments = ResponseParsingHelper.SplitSegments(payload, parseErrors);

            Assert.Empty(segments);
            Assert.Single(parseErrors);
        }

        [Fact]
        public void GetOptionalInt_ShouldReturnNullAndAddError_WhenValueIsInvalid()
        {
            List<string> parseErrors = new List<string>();
            string[] segments = { "0210", "abc" };

            int? value = ResponseParsingHelper.GetOptionalInt(segments, 1, "ResponseCode", parseErrors);

            Assert.Null(value);
            Assert.Single(parseErrors);
            Assert.Equal("ResponseCode is not a valid integer.", parseErrors[0]);
        }

        [Fact]
        public void GetOptionalCombinedDateTime_ShouldAddError_WhenOnlyOneSegmentExists()
        {
            List<string> parseErrors = new List<string>();
            string[] segments = { "1080", "90", "06102025" };

            DateTime? value = ResponseParsingHelper.GetOptionalCombinedDateTime(
                segments,
                2,
                3,
                "ddMMyyyyHHmmss",
                "RealDate",
                parseErrors);

            Assert.Null(value);
            Assert.Single(parseErrors);
            Assert.Equal("RealDate requires both date and time segments.", parseErrors[0]);
        }
    }
}
