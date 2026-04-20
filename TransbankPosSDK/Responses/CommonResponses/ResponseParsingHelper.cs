using System;
using System.Collections.Generic;
using System.Globalization;

namespace Transbank.Responses.CommonResponses
{
    internal static class ResponseParsingHelper
    {
        internal static string[] SplitSegments(string response, ICollection<string> parseErrors)
        {
            if (string.IsNullOrWhiteSpace(response))
            {
                parseErrors.Add("Response payload is null, empty, or whitespace.");
                return Array.Empty<string>();
            }

            return response.Split('|');
        }

        internal static bool TryGetTrimmedSegment(string[] segments, int index, out string segment)
        {
            segment = string.Empty;
            if (segments == null || index < 0 || index >= segments.Length)
            {
                return false;
            }

            segment = (segments[index] ?? string.Empty).Trim();
            return true;
        }

        internal static string GetRequiredString(string[] segments, int index, string fieldName, ICollection<string> parseErrors)
        {
            if (!TryGetTrimmedSegment(segments, index, out string segment) || string.IsNullOrWhiteSpace(segment))
            {
                parseErrors.Add($"{fieldName} is missing or empty.");
                return string.Empty;
            }

            return segment;
        }

        internal static string GetOptionalString(string[] segments, int index)
        {
            return TryGetTrimmedSegment(segments, index, out string segment) ? segment : string.Empty;
        }

        internal static int? GetOptionalInt(string[] segments, int index, string fieldName, ICollection<string> parseErrors)
        {
            if (!TryGetTrimmedSegment(segments, index, out string segment) || string.IsNullOrWhiteSpace(segment))
            {
                return null;
            }

            if (int.TryParse(segment, out int value))
            {
                return value;
            }

            parseErrors.Add($"{fieldName} is not a valid integer.");
            return null;
        }

        internal static int? GetRequiredInt(string[] segments, int index, string fieldName, ICollection<string> parseErrors)
        {
            if (!TryGetTrimmedSegment(segments, index, out string segment) || string.IsNullOrWhiteSpace(segment))
            {
                parseErrors.Add($"{fieldName} is missing or empty.");
                return null;
            }

            if (int.TryParse(segment, out int value))
            {
                return value;
            }

            parseErrors.Add($"{fieldName} is not a valid integer.");
            return null;
        }

        internal static long? GetOptionalLong(string[] segments, int index, string fieldName, ICollection<string> parseErrors)
        {
            if (!TryGetTrimmedSegment(segments, index, out string segment) || string.IsNullOrWhiteSpace(segment))
            {
                return null;
            }

            if (long.TryParse(segment, out long value))
            {
                return value;
            }

            parseErrors.Add($"{fieldName} is not a valid long.");
            return null;
        }

        internal static long? GetRequiredLong(string[] segments, int index, string fieldName, ICollection<string> parseErrors)
        {
            if (!TryGetTrimmedSegment(segments, index, out string segment) || string.IsNullOrWhiteSpace(segment))
            {
                parseErrors.Add($"{fieldName} is missing or empty.");
                return null;
            }

            if (long.TryParse(segment, out long value))
            {
                return value;
            }

            parseErrors.Add($"{fieldName} is not a valid long.");
            return null;
        }

        internal static DateTime? GetOptionalDate(string[] segments, int index, string format, string fieldName, ICollection<string> parseErrors)
        {
            if (!TryGetTrimmedSegment(segments, index, out string segment) || string.IsNullOrWhiteSpace(segment))
            {
                return null;
            }

            if (DateTime.TryParseExact(segment, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out DateTime parsedDate))
            {
                return parsedDate;
            }

            parseErrors.Add($"{fieldName} does not match expected format {format}.");
            return null;
        }

        internal static DateTime? GetOptionalCombinedDateTime(
            string[] segments,
            int dateIndex,
            int timeIndex,
            string format,
            string fieldName,
            ICollection<string> parseErrors)
        {
            bool hasDate = TryGetTrimmedSegment(segments, dateIndex, out string dateSegment) && !string.IsNullOrWhiteSpace(dateSegment);
            bool hasTime = TryGetTrimmedSegment(segments, timeIndex, out string timeSegment) && !string.IsNullOrWhiteSpace(timeSegment);

            if (!hasDate && !hasTime)
            {
                return null;
            }

            if (!hasDate || !hasTime)
            {
                parseErrors.Add($"{fieldName} requires both date and time segments.");
                return null;
            }

            string combined = dateSegment + timeSegment;
            if (DateTime.TryParseExact(combined, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.NoCurrentDateDefault, out DateTime parsedDate))
            {
                return parsedDate;
            }

            parseErrors.Add($"{fieldName} does not match expected format {format}.");
            return null;
        }
    }
}
