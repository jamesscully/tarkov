using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TarkovAssistantWPF.enums;
using TarkovAssistantWPF.keybinding;

namespace TarkovAssistantWPF.forms
{
    /// <summary>
    /// Interaction logic for ChangeHotkeysWindow.xaml
    /// </summary>
    public partial class ChangeHotkeysWindow : Window
    {
        public ChangeHotkeysWindow()
        {
            InitializeComponent();

            LoadHotkeysToForm();

            // disable binds in this form
            KeybindManager.GetInstance().EnableBinds = false;

            // when we close the window, enable binds again
            this.Closed += (sender, args) =>
            {
                KeybindManager.GetInstance().EnableBinds = true;
                KeybindManager.GetInstance().Reload();
            };
        }

        private void LoadHotkeysToForm()
        {
            FormHotkeys_Table.Children.Clear();

            Keybind[] allHotkeys = (Keybind[])Enum.GetValues(typeof(Keybind));

            // create a row for each Keybind available
            foreach (Keybind e in allHotkeys)
            {
                if(e == Keybind.None)
                    continue;
                
                Key? boundKey = KeybindManager.GetInstance().GetKeyForKeybind(e);

                AddHotkeyRow(e, boundKey);
            }
        }

        // encapsulates each row entry
        private void AddHotkeyRow(Keybind keybind, Key? initialBound)
        {
            var text = new TextBlock();

            string description = Properties.Resources.ResourceManager.GetString(keybind.ToString());
            text.Text = description;

            var input = new KeybindTextBox(keybind, initialBound);
            var panel = new DockPanel();

            panel.Children.Add(text);
            panel.Children.Add(input);

            panel.MinWidth = 200;
            panel.Margin = new Thickness(0, 0, 0, 10);

            text.HorizontalAlignment = HorizontalAlignment.Left;
            input.HorizontalAlignment = HorizontalAlignment.Right;


            // clear button
            var clearButton = new Button();
            clearButton.Content = "X";

            clearButton.Click += (sender, args) =>
            {
                KeybindManager.GetInstance().ClearKeybind(keybind);
                input.Clear();
            };

            clearButton.MinWidth = 25;
            clearButton.HorizontalAlignment = HorizontalAlignment.Right;

            // dock controls to their needed sides
            text.SetValue(DockPanel.DockProperty, Dock.Left);
            clearButton.SetValue(DockPanel.DockProperty, Dock.Right);

            // make all text even
            text.MinWidth = 200;

            panel.Children.Add(clearButton);
            panel.LastChildFill = false;

            FormHotkeys_Table.Children.Add(panel);
        }

        // encapsulates our button behaviour
        class KeybindTextBox : Button
        {
            private bool isFocused = false;

            private string initialText = "";
            private Key initialKey;
            private Keybind pair;
            private Key selectedKey;
            
            public KeybindTextBox(Keybind pair, Key? initialKey = null)
            {

                if (initialKey == null)
                {
                    this.initialText = "Unbound";
                    SetWarning(true);
                }
                else
                {
                    this.initialText = initialKey.ToString();
                    this.initialKey = (Key) initialKey;
                }

                this.pair = pair;

                this.Content = initialText;

                this.MinWidth = 100;
            }

            public void Clear()
            {
                SetWarning(true);
            }

            // Resets to initial state
            private void Reset()
            {
                this.Content = initialText;
                this.isFocused = false;
            }

            // Highlights the button if we're unbounded, else normal
            private void SetWarning(bool warn, string message = "Unbound")
            {
                if (warn)
                {
                    this.Content = message;
                    this.Background = new SolidColorBrush(Colors.DarkSalmon);
                    this.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    this.ClearValue(Button.BorderBrushProperty);
                    this.ClearValue(Button.BackgroundProperty);
                    this.isFocused = false;
                }
            }

            // Determine if the key is valid (valid key, already bound, etc go here)
            private bool IsKeyValid(Key key)
            {
                // we don't want duplicate binds
                var isKeyAlreadyBound = KeybindManager.GetInstance().HasKeyBound(key);

                if(isKeyAlreadyBound)
                    Debug.WriteLine("Binding duplicate key? " + key);

                return !(isKeyAlreadyBound);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (e.Key == Key.Escape)
                {
                    Reset();
                    return;
                }

                if (IsKeyValid(e.Key))
                {
                    e.Handled = true;

                    var oldKey = selectedKey;

                    selectedKey = e.Key;

                    this.Content = selectedKey.ToString();

                    KeybindManager.GetInstance().SetKeybind(this.pair, e.Key);
                    KeybindManager.GetInstance().ClearKey(oldKey);

                    SetWarning(false);
                }

            }

            protected override void OnClick()
            {
                if (this.isFocused)
                    this.Content = selectedKey.ToString();
                else
                    this.Content = "Press key... (ESC to Cancel)";

                this.isFocused = !this.isFocused;
            }
        }

        private void FormHotkeys_SaveChangesButton_OnClick(object sender, RoutedEventArgs e)
        {
            KeybindManager.GetInstance().SaveToFile();
        }

        private void FormHotkeys_ResetBindingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "This will revert all of your keybinds. Are you sure?",
                "Restore default keybinds",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                KeybindManager.GetInstance().ResetToDefault();
                LoadHotkeysToForm();
            }
        }
    }
}
