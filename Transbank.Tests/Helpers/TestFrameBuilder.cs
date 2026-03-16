namespace Transbank.Tests.Helpers
{
    internal static class TestFrameBuilder
    {
        private const char STX = (char)0x02;
        private const char ETX = (char)0x03;

        internal static string BuildCommandFrame(string payload)
        {
            char lrc = ETX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            return $"{STX}{payload}{ETX}{lrc}";
        }

        internal static string BuildResponseFrame(string payload)
        {
            char lrc = STX;
            foreach (char c in payload)
            {
                lrc ^= c;
            }

            lrc ^= ETX;
            return $"{STX}{payload}{ETX}{lrc}";
        }

        internal static string BuildInvalidResponseFrame(string payload)
        {
            return $"{STX}{payload}{ETX}{(char)0x00}";
        }
    }
}
