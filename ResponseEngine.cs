using System;
using System.Collections.Generic;

namespace CyberBot
{

/// <summary>
/// Core response engine. Uses a dictionary-driven keyword map and
/// named response pools for clean OOP structure and easy expansion.
/// </summary>
class ResponseEngine
{
    private static readonly Random _rng = new();

    // ── Topic metadata ─────────────────────────────────────────────────────
    // Maps keyword fragments → topic key. Ordered by specificity (longer first).
    private static readonly (string Fragment, string Topic)[] KeywordMap =
    {
        ("passphrase",       "password"),
        ("password",         "password"),
        ("phishing",         "phishing"),
        ("phish",            "phishing"),
        ("scam",             "scam"),
        ("fraud",            "scam"),
        ("swindle",          "scam"),
        ("privacy",          "privacy"),
        ("personal data",    "privacy"),
        ("data protection",  "privacy"),
        ("two-factor",       "2fa"),
        ("two factor",       "2fa"),
        ("2fa",              "2fa"),
        ("authentication",   "2fa"),
        ("malware",          "malware"),
        ("ransomware",       "malware"),
        ("virus",            "malware"),
        ("vpn",              "vpn"),
        ("browsing",         "browsing"),
        ("browse",           "browsing"),
        ("https",            "browsing"),
        ("website",          "browsing"),
        ("internet",         "browsing"),
    };

    // ── Response pools (topic → string[]) ─────────────────────────────────
    private static readonly Dictionary<string, string[]> MainResponses = new()
    {
        ["password"] =
        [
            " Strong password tips:\n\n" +
            "• Use at least 12 characters\n" +
            "• Mix uppercase, lowercase, numbers & symbols (e.g. P@ssw0rd!23)\n" +
            "• Never use your name, birthday, or common words\n" +
            "• Use a DIFFERENT password for every account\n" +
            "• Consider a password manager like Bitwarden or 1Password!",

            " Make sure to use strong, unique passwords for each account.\n\n" +
            "Avoid using personal details — attackers research their targets!\n\n" +
            "• Passphrases work well: 'PurpleTiger$Runs@Dawn'\n" +
            "• Change passwords immediately if a site is breached\n" +
            "• Never share your password — not even with IT support",

            " Password security is your first line of defence!\n\n" +
            "• A 12-character password takes centuries to crack vs. 8-character ones\n" +
            "• Enable two-factor authentication (2FA) wherever possible\n" +
            "• Check haveibeenpwned.com to see if your email was in a breach\n" +
            "• Password managers generate AND remember strong passwords for you"
        ],

        ["phishing"] =
        [
            " Phishing is when attackers disguise themselves as trusted sources to steal your info.\n\n" +
            "• Watch for emails asking for passwords or personal details\n" +
            "• Fake links like 'amaz0n.com' or 'paypa1.com'\n" +
            "• Urgent messages like 'Act NOW or your account will be closed!'\n" +
            "• When in doubt — don't click!",

            " Be cautious of emails asking for personal information.\n\n" +
            "Scammers often disguise themselves as trusted organisations.\n\n" +
            "• Always verify the sender's actual email address\n" +
            "• Hover over links before clicking to see the real URL\n" +
            "• Legitimate companies never ask for your password via email",

            " Phishing attacks are the #1 cause of data breaches!\n\n" +
            "• Attackers craft convincing fake login pages\n" +
            "• SMS phishing ('smishing') is also very common\n" +
            "• Enable 2FA so stolen passwords alone aren't enough\n" +
            "• Report suspicious emails to your IT team or email provider"
        ],

        ["scam"] =
        [
            " Online scams come in many forms — stay alert!\n\n" +
            "• 'You've won a prize!' — if you didn't enter, it's a scam\n" +
            "• Fake tech support calls claiming your PC has a virus\n" +
            "• Romance scams that build trust before asking for money\n" +
            "• Never send money or gift cards to someone you haven't met in person",

            " Scammers are very sophisticated these days.\n\n" +
            "• Verify unexpected requests through official channels\n" +
            "• If a deal seems too good to be true — it is\n" +
            "• Urgency and pressure are classic scam tactics\n" +
            "• Report scams to SAPS or your bank immediately",

            " How to spot a scam:\n\n" +
            "• Requests for unusual payment methods (crypto, gift cards, wire transfer)\n" +
            "• Grammatical errors and strange formatting in messages\n" +
            "• Unsolicited contact asking for personal or financial info\n" +
            "• Impersonation of well-known brands or government agencies"
        ],

        ["privacy"] =
        [
            " Protecting your privacy online is crucial!\n\n" +
            "• Review app permissions — does a flashlight app need your contacts?\n" +
            "• Use a VPN on public Wi-Fi to encrypt your traffic\n" +
            "• Regularly review your social media privacy settings\n" +
            "• Limit what personal info you share publicly online",

            " Your digital footprint follows you everywhere.\n\n" +
            "• Search your own name to see what's publicly visible\n" +
            "• Use privacy-focused browsers like Firefox or Brave\n" +
            "• Consider a privacy-focused search engine like DuckDuckGo\n" +
            "• Read privacy policies before signing up for new services",

            " Data privacy tips:\n\n" +
            "• Enable end-to-end encryption for messaging (WhatsApp, Signal)\n" +
            "• Avoid oversharing on social media — location data can be dangerous\n" +
            "• Opt out of data brokers that sell your personal information\n" +
            "• Use different email addresses for different purposes"
        ],

        ["browsing"] =
        [
            " Safe browsing tips:\n\n" +
            "• Always look for HTTPS (🔒) in the address bar\n" +
            "• Avoid clicking pop-up ads — many install malware\n" +
            "• Don't download files from unknown or suspicious sites\n" +
            "• Keep your browser and its extensions updated",

            " Staying safe while browsing:\n\n" +
            "• Use an ad blocker like uBlock Origin to block malicious ads\n" +
            "• Only install browser extensions from trusted sources\n" +
            "• Clear cookies and browsing data regularly\n" +
            "• Avoid using public Wi-Fi for banking or sensitive tasks",

            " Browser security essentials:\n\n" +
            "• Enable automatic browser updates\n" +
            "• Use private/incognito mode for sensitive searches\n" +
            "• HTTPS encrypts data between you and the website\n" +
            "• Be cautious of 'certificate error' warnings — don't proceed!"
        ],

        ["malware"] =
        [
            " Malware is malicious software designed to harm your device or steal data!\n\n" +
            "• Keep your antivirus software up to date\n" +
            "• Never open email attachments from unknown senders\n" +
            "• Avoid pirated software — it often contains hidden malware\n" +
            "• Backup your data regularly in case of ransomware attacks",

            " Types of malware to know about:\n\n" +
            "• Ransomware — encrypts your files and demands payment\n" +
            "• Spyware — silently monitors your activity\n" +
            "• Trojans — disguise themselves as legitimate software\n" +
            "• Adware — floods you with unwanted ads (and worse)\n" +
            "• Run a reputable antivirus scan regularly!"
        ],

        ["2fa"] =
        [
            " Two-Factor Authentication (2FA) is one of the best things you can do!\n\n" +
            "• Even if someone steals your password, 2FA stops them\n" +
            "• Use an authenticator app (Google Authenticator, Authy) over SMS\n" +
            "• Enable 2FA on email, banking, and social media accounts first\n" +
            "• Backup your 2FA codes in a safe place",

            " Why 2FA matters:\n\n" +
            "• 99.9% of account takeovers can be blocked with 2FA (Microsoft research)\n" +
            "• SMS codes are better than nothing, but app-based codes are safer\n" +
            "• Hardware keys (YubiKey) offer the strongest protection\n" +
            "• Enable 2FA now — it only takes 2 minutes per account!"
        ],

        ["vpn"] =
        [
            " A VPN (Virtual Private Network) encrypts your internet traffic!\n\n" +
            "• Essential when using public Wi-Fi (cafés, airports, hotels)\n" +
            "• Hides your browsing activity from your internet provider\n" +
            "• Choose reputable paid VPNs — free VPNs often sell your data\n" +
            "• A VPN doesn't make you fully anonymous — practice good habits too",

            " Choosing a VPN:\n\n" +
            "• Look for a 'no-logs' policy — the VPN shouldn't store your activity\n" +
            "• Reputable options: ProtonVPN, Mullvad, ExpressVPN\n" +
            "• Free VPNs are often the product — your data gets sold\n" +
            "• A VPN is not a substitute for good security hygiene"
        ],
    };

    // Extra tip pools for follow-up requests
    private static readonly Dictionary<string, string[]> ExtraTips = new()
    {
        ["password"] =
        [
            " Extra tip: A passphrase like 'Coffee!Runs@Midnight99' is easier to remember AND more secure.",
            " Extra tip: Change important passwords (email, banking) every 6–12 months, especially after breach notifications.",
            " Extra tip: Never reuse passwords — attackers use 'credential stuffing' to try stolen passwords everywhere."
        ],
        ["phishing"] =
        [
            " Extra tip: Check the actual sender address carefully — 'PayPal Support' can hide a fake domain.",
            " Extra tip: Paste suspicious links into VirusTotal.com before clicking — it scans against dozens of security databases.",
            " Extra tip: Phishing spikes during tax season, Black Friday, and disaster relief campaigns — extra vigilance then!"
        ],
        ["scam"] =
        [
            " Extra tip: If someone contacts you unexpectedly claiming to be your bank, hang up and call the official number yourself.",
            " Extra tip: Never allow remote access to your PC unless YOU initiated contact with a verified support service.",
            " Extra tip: Social media quizzes often collect answers that match common security questions — be careful what you share!"
        ],
        ["privacy"] =
        [
            " Extra tip: Use a separate email address for newsletters and sign-ups to limit exposure.",
            " Extra tip: Disable location sharing on apps that don't genuinely need it.",
            " Extra tip: Regularly audit which apps have access to your microphone, camera, and contacts."
        ],
        ["browsing"] =
        [
            " Extra tip: Only install browser extensions with thousands of reviews from the official store.",
            " Extra tip: Always use a VPN on public Wi-Fi — coffee shop networks are unencrypted.",
            " Extra tip: Clicking the padlock icon shows you a site's certificate — no padlock is always a red flag."
        ],
        ["malware"] =
        [
            " Extra tip: Keep your operating system updated — patches close security holes malware exploits.",
            " Extra tip: Disconnect from the internet immediately if you suspect a ransomware infection to limit spread."
        ],
        ["2fa"] =
        [
            " Extra tip: If a site only offers SMS 2FA, it's still worth enabling — it's far better than nothing.",
            " Extra tip: Store your backup 2FA codes in a password manager or printed in a safe place."
        ],
        ["vpn"] =
        [
            " Extra tip: Enable the VPN kill switch feature — it cuts your internet if the VPN drops, preventing data leaks.",
            " Extra tip: Even with a VPN, sites can still track you via cookies and browser fingerprinting."
        ],
    };

    // Follow-up trigger phrases
    private static readonly string[] FollowUpTriggers =
    [
        "another tip", "give me more", "tell me more", "more info",
        "explain more", "more detail", "elaborate", "continue",
        "what else", "anything else", "keep going", "go on"
    ];

    // General conversation map (keyword fragment → response builder)
    private static readonly (string Fragment, Func<string, string> Builder)[] GeneralMap =
    [
        ("how are you",    name => $"I'm running perfectly, {name}! Always alert and ready to help! "),
        ("purpose",        name => $"My purpose, {name}, is to teach you how to stay safe online and protect your digital life!"),
        ("what do you do", name => $"My purpose, {name}, is to teach you how to stay safe online and protect your digital life!"),
        ("help",           _    => BuildHelpMessage()),
        ("what can i ask", _    => BuildHelpMessage()),
        ("topics",         _    => BuildHelpMessage()),
    ];

    // ── Public entry point ─────────────────────────────────────────────────

    public static string GetResponse(string input, UserMemory memory)
    {
        try
        {
            string name    = memory.Name;
            string empathy = SentimentDetector.GetEmpathyPrefix(
                                 SentimentDetector.Detect(input), name);

            // 1. Follow-up / conversation flow
            if (IsFollowUp(input))
                return empathy + HandleFollowUp(memory);

            // 2. User shares a personal interest
            string? interestResponse = TryHandleInterest(input, memory);
            if (interestResponse != null)
                return empathy + interestResponse;

            // 3. General conversation
            foreach (var (fragment, builder) in GeneralMap)
                if (input.Contains(fragment))
                    return empathy + builder(name);

            // 4. Cybersecurity keyword match
            string? topic = DetectTopic(input);
            if (topic != null)
            {
                memory.RecordTopic(topic);
                string topicResponse = BuildTopicResponse(topic, name, memory);
                string? nudge        = memory.GetMemoryNudge();
                return empathy + topicResponse + (nudge ?? string.Empty);
            }

            // 5. Fallback — graceful default
            return empathy + InputValidator.GetDefaultResponse(name);
        }
        catch (Exception ex)
        {
            // Safety net — never crash on unexpected input
            Console.WriteLine($"[ResponseEngine Error] {ex.Message}");
            return InputValidator.GetDefaultResponse(memory.Name);
        }
    }

    // ── Follow-up handling ─────────────────────────────────────────────────

    private static bool IsFollowUp(string input)
    {
        foreach (var trigger in FollowUpTriggers)
            if (input.Contains(trigger)) return true;
        return false;
    }

    private static string HandleFollowUp(UserMemory memory)
    {
        if (memory.LastTopic != null &&
            ExtraTips.TryGetValue(memory.LastTopic, out var tips))
            return Pick(tips);

        return $"Sure! Could you let me know which topic you'd like more on?\n\n" +
               "   Passwords     Phishing     Scams\n" +
               "   Privacy       Browsing     Malware\n" +
               "   2FA           VPNs";
    }

    // ── Interest / memory handling ─────────────────────────────────────────

    private static string? TryHandleInterest(string input, UserMemory memory)
    {
        if (!input.Contains("i'm interested in") && !input.Contains("im interested in")
            && !input.Contains("i am interested in"))
            return null;

        string? topic = DetectTopic(input);
        if (topic == null) return null;

        memory.AddFact($"you're interested in {topic}");
        memory.RecordTopic(topic);

        return $"Great! I'll remember that you're interested in {topic}, {memory.Name}. " +
               $"It's a crucial part of staying safe online. 💙\n\n" +
               BuildTopicResponse(topic, memory.Name, memory);
    }

    // ── Topic helpers ──────────────────────────────────────────────────────

    private static string? DetectTopic(string input)
    {
        foreach (var (fragment, topic) in KeywordMap)
            if (input.Contains(fragment)) return topic;
        return null;
    }

    private static string BuildTopicResponse(string topic, string name, UserMemory memory)
    {
        if (!MainResponses.TryGetValue(topic, out var pool))
            return InputValidator.GetDefaultResponse(name);

        string response = Pick(pool);
        memory.LastTopic    = topic;
        memory.LastResponse = response;
        return response;
    }

    // ── Misc helpers ───────────────────────────────────────────────────────

    private static string Pick(string[] pool) =>
        pool.Length > 0 ? pool[_rng.Next(pool.Length)] : string.Empty;

    private static string BuildHelpMessage() =>
        "You can ask me about:\n\n" +
        "    Password safety\n" +
        "    Phishing\n" +
        "    Scams\n" +
        "    Privacy\n" +
        "    Safe browsing\n" +
        "    Malware\n" +
        "    Two-factor authentication (2FA)\n" +
        "    VPNs\n\n" +
        "    After any answer, say 'give me another tip' for more!";
}
}
