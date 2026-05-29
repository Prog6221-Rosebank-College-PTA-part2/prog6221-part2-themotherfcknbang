using System;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CyberBot
{
    public partial class MainWindow : Window
    {
        // ── State ──────────────────────────────────────────────────────────
        private readonly UserMemory _memory = new();
        private bool _nameSet = false;
        private bool _isBotTyping = false;

        // ── Brushes (defined once, reused) ────────────────────────────────
        private static readonly SolidColorBrush BrUserBubble  = new(Color.FromRgb(0x1E, 0x3A, 0x5F));
        private static readonly SolidColorBrush BrBotBubble   = new(Color.FromRgb(0x1A, 0x25, 0x35));
        private static readonly SolidColorBrush BrAccentBlue  = new(Color.FromRgb(0x4F, 0x8E, 0xF7));
        private static readonly SolidColorBrush BrAccentTeal  = new(Color.FromRgb(0x00, 0xC9, 0xA7));
        private static readonly SolidColorBrush BrAccentPurple= new(Color.FromRgb(0x9B, 0x59, 0xF5));
        private static readonly SolidColorBrush BrTextPrimary = new(Color.FromRgb(0xE8, 0xED, 0xF5));
        private static readonly SolidColorBrush BrTextMuted   = new(Color.FromRgb(0x6B, 0x7A, 0x99));

        // ── Constructor ────────────────────────────────────────────────────
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        // ── Startup ────────────────────────────────────────────────────────
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PlayGreeting();
            await Task.Delay(300);
            AddSystemMessage("CYBERBOT  ·  CYBERSECURITY AWARENESS SYSTEM  ·  v2.0");
            await TypeBotBubble("Welcome! Before we begin — what is your name? 👋");
        }

        // ── Input events ───────────────────────────────────────────────────
        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !_isBotTyping)
            {
                e.Handled = true;
                HandleInput();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isBotTyping) HandleInput();
        }

        private void InputBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Placeholder.Visibility = string.IsNullOrEmpty(InputBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // ── Core handler ───────────────────────────────────────────────────
        private async void HandleInput()
        {
            string raw = InputValidator.Sanitise(InputBox.Text);
            if (InputValidator.IsEmpty(raw)) return;

            InputBox.Clear();
            AddUserBubble(raw);

            if (InputValidator.IsTooLong(raw))
            {
                await TypeBotBubble($"That message is a bit too long, {_memory.Name}! Could you keep it shorter? ");
                return;
            }

            // First message = name
            if (!_nameSet)
            {
                _memory.Name = raw;
                _nameSet     = true;
                await TypeBotBubble(
                    $"Great to meet you, {_memory.Name}! \n\n" +
                    "I'm your Cybersecurity Awareness Bot — here to help keep you safe online.\n\n" +
                    "You can ask me about:\n" +
                    "    Passwords          Phishing\n" +
                    "    Scams              Privacy\n" +
                    "    Safe browsing      Malware\n" +
                    "    2FA / MFA          VPNs\n\n" +
                    "  Say 'give me another tip' after any answer for more!\n" +
                    "Type 'help' anytime or 'exit' to leave.");
                UpdateMemoryBadge();
                return;
            }

            string lower = raw.ToLower();

            if (lower is "exit" or "quit" or "bye")
            {
                await TypeBotBubble($"Goodbye, {_memory.Name}! Stay safe online! ");
                await Task.Delay(1400);
                Application.Current.Shutdown();
                return;
            }

            string response = ResponseEngine.GetResponse(lower, _memory);
            await TypeBotBubble(response);
            UpdateMemoryBadge();
        }

        // ── Chat bubble builders ───────────────────────────────────────────

        /// System info strip (centred, muted text)
        private void AddSystemMessage(string text)
        {
            var tb = new TextBlock
            {
                Text                = text,
                Foreground          = BrTextMuted,
                FontSize            = 10,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin              = new Thickness(0, 6, 0, 10),
                FontFamily          = new FontFamily("Segoe UI"),
                TextWrapping        = TextWrapping.Wrap
            };
            ChatPanel.Children.Add(tb);
            ScrollToBottom();
        }

        /// User bubble — right-aligned, blue tint
        private void AddUserBubble(string text)
        {
            var bubble = BuildBubble(
                text,
                BrUserBubble,
                BrTextPrimary,
                HorizontalAlignment.Right,
                isUser: true);

            ChatPanel.Children.Add(bubble);
            AnimateIn(bubble);
            ScrollToBottom();
        }

        /// Bot bubble — left-aligned with avatar dot, teal tint
        /// Returns the TextBlock inside so we can typewrite into it.
        private (Border Bubble, TextBlock Body) AddBotBubbleShell()
        {
            // Avatar dot
            var avatar = new Ellipse
            {
                Width  = 32,
                Height = 32,
                Fill   = BrAccentBlue,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 10, 0)
            };
            var avatarText = new TextBlock
            {
                Text                = "",
                FontSize            = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                IsHitTestVisible    = false
            };
            var avatarGrid = new Grid { Width = 32, Height = 32, Margin = new Thickness(0,0,10,0), VerticalAlignment = VerticalAlignment.Top };
            avatarGrid.Children.Add(avatar);
            avatarGrid.Children.Add(avatarText);

            // Message body
            var body = new TextBlock
            {
                Foreground   = BrTextPrimary,
                FontSize     = 13.5,
                FontFamily   = new FontFamily("Segoe UI"),
                TextWrapping = TextWrapping.Wrap,
                LineHeight   = 22
            };

            // "Bot" label
            var label = new TextBlock
            {
                Text       = "CyberBot",
                Foreground = BrAccentTeal,
                FontSize   = 11,
                FontWeight = FontWeights.SemiBold,
                Margin     = new Thickness(0, 0, 0, 5)
            };

            var textStack = new StackPanel();
            textStack.Children.Add(label);
            textStack.Children.Add(body);

            var bubble = new Border
            {
                Background            = BrBotBubble,
                CornerRadius          = new CornerRadius(4, 18, 18, 18),
                Padding               = new Thickness(16, 12, 16, 12),
                MaxWidth              = 580,
                Child                 = textStack,
                HorizontalAlignment   = HorizontalAlignment.Left
            };

            var row = new Grid { Margin = new Thickness(0, 6, 60, 6) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Grid.SetColumn(avatarGrid, 0);
            Grid.SetColumn(bubble, 1);
            row.Children.Add(avatarGrid);
            row.Children.Add(bubble);

            ChatPanel.Children.Add(row);
            AnimateIn(row);
            ScrollToBottom();

            return (bubble, body);
        }

        // ── Typewriter effect ──────────────────────────────────────────────
        private async Task TypeBotBubble(string message)
        {
            _isBotTyping        = true;
            SendButton.IsEnabled = false;

            var (_, body) = AddBotBubbleShell();

            foreach (char c in message)
            {
                body.Text += c;
                ScrollToBottom();
                await Task.Delay(c == '\n' ? 25 : 10);
            }

            _isBotTyping         = false;
            SendButton.IsEnabled = true;
            InputBox.Focus();
        }

        // ── Generic bubble builder (for user) ─────────────────────────────
        private static Border BuildBubble(
            string text,
            SolidColorBrush bg,
            SolidColorBrush fg,
            HorizontalAlignment align,
            bool isUser)
        {
            var body = new TextBlock
            {
                Text         = text,
                Foreground   = fg,
                FontSize     = 13.5,
                FontFamily   = new FontFamily("Segoe UI"),
                TextWrapping = TextWrapping.Wrap,
                LineHeight   = 22
            };

            CornerRadius cr = isUser
                ? new CornerRadius(18, 4, 18, 18)
                : new CornerRadius(4, 18, 18, 18);

            var bubble = new Border
            {
                Background          = bg,
                CornerRadius        = cr,
                Padding             = new Thickness(16, 11, 16, 11),
                MaxWidth            = 520,
                Child               = body,
                HorizontalAlignment = align,
                Margin              = isUser
                    ? new Thickness(60, 6, 0, 6)
                    : new Thickness(0, 6, 60, 6)
            };

            return bubble;
        }

        // ── Memory badge update ────────────────────────────────────────────
        private void UpdateMemoryBadge()
        {
            string? label = _memory.FavouriteTopic != null
                ? $"Favourite: {_memory.FavouriteTopic}"
                : _memory.LastTopic != null
                    ? $"Topic: {_memory.LastTopic}"
                    : null;

            if (label != null)
            {
                MemoryBadgeText.Text    = label;
                MemoryBadge.Visibility  = Visibility.Visible;
            }
        }

        // ── Slide-in animation ─────────────────────────────────────────────
        private static void AnimateIn(UIElement element)
        {
            element.RenderTransform = new TranslateTransform(0, 12);
            element.Opacity = 0;

            var slide = new DoubleAnimation(12, 0, TimeSpan.FromMilliseconds(220))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            var fade = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(220));

            element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, slide);
            element.BeginAnimation(OpacityProperty, fade);
        }

        // ── Scroll helper ──────────────────────────────────────────────────
        private void ScrollToBottom()
        {
            ChatScroll.ScrollToBottom();
        }

        // ── Voice greeting ─────────────────────────────────────────────────
        private static void PlayGreeting()
        {
            try { new SoundPlayer("greeting.wav").Play(); }
            catch { /* wav not present — skip silently */ }
        }
    }
}
