using System;
using Transbank.Responses.CommonResponses;
using Xunit;

namespace Transbank.Tests
{
    public class IntermediateResponseTests
    {
        [Fact]
        public void IntermediateResponse_ShouldParseAllFields_FromSanitizedPayload()
        {
            var response = new IntermediateResponse("0900|84");

            Assert.Equal("0900", response.FunctionCode);
            Assert.Equal(84, response.ResponseCode);
            Assert.Equal("Opere tarjeta", response.ResponseMessage);
        }

        [Fact]
        public void IntermediateResponse_ShouldFail_WhenPayloadIsIncomplete()
        {
            var response = new IntermediateResponse("0900");

            Assert.Throws<IndexOutOfRangeException>(() => _ = response.ResponseCode);
        }
    }
}
