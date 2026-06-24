using Transbank.Responses.CommonResponses;
using Xunit;

namespace Transbank.Tests
{
    public class IntermediateResponseTests
    {
        [Fact]
        public void BasicResponse_ShouldPreserveRawResponse_AndParseBaseFields()
        {
            var response = new BasicResponse("0210|00|597029414300");

            Assert.Equal("0210|00|597029414300", response.RawResponse);
            Assert.Equal("0210", response.FunctionCode);
            Assert.Equal(0, response.ResponseCode);
            Assert.True(response.HasValidParse);
            Assert.Empty(response.ParseErrors);
            Assert.True(response.Success);
        }

        [Fact]
        public void BasicResponse_ShouldCollectMultipleParseErrors_WhenPayloadHasSeveralProblems()
        {
            var response = new BasicResponse("|abc");

            Assert.Equal("|abc", response.RawResponse);
            Assert.False(response.HasValidParse);
            Assert.Empty(response.FunctionCode);
            Assert.Null(response.ResponseCode);
            Assert.Equal(2, response.ParseErrors.Count);
            Assert.Contains("FunctionCode is missing or empty.", response.ParseErrors);
            Assert.Contains("ResponseCode is not a valid integer.", response.ParseErrors);
            Assert.False(response.Success);
        }

        [Fact]
        public void LoadKeysResponse_ShouldBeInvalid_WhenRequiredFieldsAreMissing()
        {
            var response = new LoadKeysResponse("0810|00||");

            Assert.False(response.HasValidParse);
            Assert.Equal("0810|00||", response.RawResponse);
            Assert.Null(response.CommerceCode);
            Assert.Equal(string.Empty, response.TerminalId);
            Assert.Contains("CommerceCode is missing or empty.", response.ParseErrors);
            Assert.Contains("TerminalId is missing or empty.", response.ParseErrors);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("0900")]
        [InlineData("0900|")]
        [InlineData("|84")]
        [InlineData("0900|abc")]
        public void IntermediateResponse_ShouldKeepInvalidPayloadWithoutThrowing(string? payload)
        {
            var exception = Record.Exception(() => new IntermediateResponse(payload));
            var response = new IntermediateResponse(payload);

            Assert.Null(exception);
            Assert.False(response.HasValidParse);
            Assert.False(response.Success);
            Assert.Equal(payload, response.RawResponse);
            Assert.NotEmpty(response.ParseErrors);
        }

        [Fact]
        public void IntermediateResponse_ShouldTreatRecognizedIntermediateMessage_AsSuccessfulWhenParsed()
        {
            var response = new IntermediateResponse("0900|84");

            Assert.True(response.HasValidParse);
            Assert.True(response.Success);
            Assert.Equal("0900", response.FunctionCode);
            Assert.Equal(84, response.ResponseCode);
            Assert.Equal("Opere tarjeta", response.ResponseMessage);
            Assert.Equal("0900|84", response.RawResponse);
        }

        [Fact]
        public void IntermediateResponse_ShouldBeFlexibleForFutureIntermediateCodes()
        {
            var response = new IntermediateResponse("0900|123");

            Assert.True(response.HasValidParse);
            Assert.True(response.Success);
            Assert.Equal(123, response.ResponseCode);
            Assert.Equal("Mensaje no encontrado", response.ResponseMessage);
        }
    }
}
