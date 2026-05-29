using System;
using System.Collections.Generic;

namespace CyberBot
{

/// <summary>
/// Validates user input and provides graceful fallback responses
/// for unexpected or unrecognised inputs.
/// </summary>
static class InputValidator
{
    // Varied default responses so repeated unknowns don't feel robotic
    private static readonly string[] DefaultResponses =
    [
        "I'm not sure I understood that. Could you try rephrasing? \n\n" +
        "Try asking about: passwords, phishing, scams, privacy, or safe browsing.\n" +
        "Or type 'help' to see all topics!",

        "Hmm, I didn't quite catch that. Could you ask in a different way? \n\n" +
        "I know a lot about:  passwords,  phishing,  scams,  privacy, and more!\n" +
        "Type 'help' to see the full list.",

        "I'm not sure what you mean — let's try again! \n\n" +
        "You can ask things like:\n" +
        "  • 'How do I make a strong password?'\n" +
        "  • 'What is phishing?'\n" +
        "  • 'How do I stay safe online?'",
    ];

    private static readonly Random _rng = new();

    /// <summary>Returns true if the input is null, empty, or only whitespace.</summary>
    public static bool IsEmpty(string? input) => string.IsNullOrWhiteSpace(input);

    /// <summary>Returns true if input exceeds a reasonable length limit.</summary>
    public static bool IsTooLong(string input, int maxLength = 500) =>
        input.Length > maxLength;

    /// <summary>
    /// Sanitises input — trims whitespace and enforces max length
    /// to prevent edge cases with very long strings.
    /// </summary>
    public static string Sanitise(string input, int maxLength = 500)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        input = input.Trim();
        return input.Length > maxLength ? input[..maxLength] : input;
    }

    public static string GetDefaultResponse(string name)
    {
        string response = DefaultResponses[_rng.Next(DefaultResponses.Length)];
        return $"I'm not sure I understood that, {name}. " + response[response.IndexOf(' ')..];
    }
}
}
