using System.Collections.Generic;

namespace CyberBot
{

/// <summary>
/// Stores user details and favourite cybersecurity topics across the conversation.
/// </summary>
class UserMemory
{
    public string Name { get; set; } = "Friend";

    // The last main topic the user asked about (e.g. "phishing", "password")
    public string? LastTopic { get; set; }

    // The last full bot response for that topic (used for follow-ups)
    public string? LastResponse { get; set; }

    // Topics the user has shown interest in (mentioned more than once)
    private readonly Dictionary<string, int> _topicCounts = new();

    // Free-form facts the user shared ("I use public wifi a lot", "I'm worried about scams")
    private readonly List<string> _sharedFacts = new();

    /// <summary>Favourite topic = the one mentioned most often.</summary>
    public string? FavouriteTopic
    {
        get
        {
            string? fav = null;
            int max = 0;
            foreach (var kv in _topicCounts)
            {
                if (kv.Value > max) { max = kv.Value; fav = kv.Key; }
            }
            return max >= 2 ? fav : null; // only "favourite" after 2+ mentions
        }
    }

    public void RecordTopic(string topic)
    {
        LastTopic = topic;
        if (_topicCounts.ContainsKey(topic))
            _topicCounts[topic]++;
        else
            _topicCounts[topic] = 1;
    }

    public void AddFact(string fact)
    {
        if (!_sharedFacts.Contains(fact))
            _sharedFacts.Add(fact);
    }

    /// <summary>
    /// Returns a personalised memory nudge if we have something relevant to mention,
    /// or null if nothing to add.
    /// </summary>
    public string? GetMemoryNudge()
    {
        if (FavouriteTopic != null)
            return $"\n\n By the way — since you often ask about {FavouriteTopic}, " +
                   $"keeping up to date on that topic is a great habit, {Name}!";

        if (_sharedFacts.Count > 0)
            return $"\n\n Remembering what you mentioned earlier: {_sharedFacts[^1]}";

        return null;
    }
}
}
