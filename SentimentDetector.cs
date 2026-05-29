namespace CyberBot
{

static class SentimentDetector
{
    public enum Sentiment { Neutral, Worried, Curious, Frustrated, Overwhelmed }

    public static Sentiment Detect(string input)
    {
        // Worried / anxious
        if (input.Contains("worried") || input.Contains("worry") ||
            input.Contains("scared") || input.Contains("afraid") ||
            input.Contains("nervous") || input.Contains("unsafe") ||
            input.Contains("danger") || input.Contains("terrified") ||
            input.Contains("concerned") || input.Contains("at risk"))
            return Sentiment.Worried;

        // Frustrated / angry
        if (input.Contains("frustrated") || input.Contains("annoyed") ||
            input.Contains("angry") || input.Contains("fed up") ||
            input.Contains("sick of") || input.Contains("hate") ||
            input.Contains("useless") || input.Contains("doesn't work") ||
            input.Contains("not working") || input.Contains("this is stupid"))
            return Sentiment.Frustrated;

        // Overwhelmed / confused
        if (input.Contains("overwhelmed") || input.Contains("confused") ||
            input.Contains("don't understand") || input.Contains("dont understand") ||
            input.Contains("too much") || input.Contains("complicated") ||
            input.Contains("lost") || input.Contains("no idea") ||
            input.Contains("what does") || input.Contains("what is"))
            return Sentiment.Overwhelmed;

        // Curious / interested
        if (input.Contains("curious") || input.Contains("interesting") ||
            input.Contains("tell me more") || input.Contains("explain") ||
            input.Contains("how does") || input.Contains("why does") ||
            input.Contains("wonder") || input.Contains("what if") ||
            input.Contains("can you explain") || input.Contains("fascinated"))
            return Sentiment.Curious;

        return Sentiment.Neutral;
    }

   
    public static string GetEmpathyPrefix(Sentiment sentiment, string name) => sentiment switch
    {
        Sentiment.Worried =>
            $"It's completely understandable to feel that way, {name}. " +
            $"Cybersecurity can feel daunting, but knowing about it puts you ahead of most people. " +
            $"Let me help you feel more secure! \n\n",

        Sentiment.Frustrated =>
            $"I hear you, {name} — it can be really frustrating dealing with security issues. " +
            $"Let's work through this together. \n\n",

        Sentiment.Overwhelmed =>
            $"No worries at all, {name} — cybersecurity has a lot of jargon! " +
            $"Let me break this down simply for you. \n\n",

        Sentiment.Curious =>
            $"Great curiosity, {name}! Asking questions is the best way to stay informed. \n\n",

        _ => string.Empty
    };
}
}
