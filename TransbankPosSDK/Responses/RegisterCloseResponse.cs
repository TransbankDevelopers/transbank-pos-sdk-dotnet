using Transbank.POS.Utils;

namespace Transbank.POS.Responses
{
    public class RegisterCloseResponse : LoadKeysResponse
    {
        public RegisterCloseResponse(LoadKeyCloseResponse cresponse) : base(cresponse) {}
    }
}
