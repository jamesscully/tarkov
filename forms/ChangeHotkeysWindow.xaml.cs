using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            JsonKeybinds.GetInstance().EnableBinds = false;

            // when we close the window, enable binds again
            this.Closed += (sender, args) => JsonKeybinds.GetInstance().EnableBinds = true;
        }

        private void LoadHotkeysToForm()
        {
            FormHotkeys_Table.Children.Clear();

            HotkeyEnum[] allHotkeys = (HotkeyEnum[])Enum.GetValues(typeof(HotkeyEnum));

            // create a row for each Hotkey available
            foreach (HotkeyEnum e in allHotkeys)
            {
                if(e == HotkeyEnum.NO_OP)
                    continue;
                
                Key? boundKey = JsonKeybinds.GetInstance().GetBindForHotkey(e);

                Debug.WriteLine("Found non-null keybind: " + boundKey);

                AddHotkeyRow(e, boundKey);
            }
        }

        // encapsulates each row entry
        private void AddHotkeyRow(HotkeyEnum hotkey, Key? initialBound)
        {
            var text = new TextBlock();

            string description = Properties.Resources.ResourceManager.GetString(hotkey.ToString());
            text.Text = description;

            var input = new KeybindTextBox(hotkey, initialBound);
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
                JsonKeybinds.GetInstance().ClearBind(hotkey);
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
            private HotkeyEnum pair;
            private Key selectedKey;
            
            public KeybindTextBox(HotkeyEnum pair, Key? initialKey = null)
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
                    this.BorderBrush = new SolidColorBrush(Colors.Red);
                }
                else
                {
                    this.ClearValue(Button.BorderBrushProperty);
                }
            }

            // Determine if the key is valid (valid key, already bound, etc go here)
            private bool IsKeyValid(Key key)
            {
                // we don't want duplicate binds
                var isKeyAlreadyBound = JsonKeybinds.GetInstance().HasKeyBound(key);

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

                    selectedKey = e.Key;
                    this.Content = selectedKey.ToString();

                    JsonKeybinds.GetInstance().SetBind(this.pair, e.Key);
                    SetWarning(false);
                }

            }

            protected override void OnClick()
            {
                if (this.isFocused)
                    this.Content = initialKey.ToString();
                else
                    this.Content = "Press any key... (ESC to Cancel)";

                this.isFocused = !this.isFocused;
            }
        }

        private void FormHotkeys_SaveChangesButton_OnClick(object sender, RoutedEventArgs e)
        {
            JsonKeybinds.GetInstance().SaveBinds();
        }

        private void FormHotkeys_ResetBindingsButton_OnClick(object sender, RoutedEventArgs e)
        {
            JsonKeybinds.GetInstance().ResetToDefault();

            LoadHotkeysToForm();
        }
    }
}
