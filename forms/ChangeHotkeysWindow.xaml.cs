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
                Key? boundKey = JsonKeybinds.GetInstance().GetBindForHotkey(e);

                Debug.WriteLine("Found non-null keybind: " + boundKey);
                AddHotkeyRow(e, boundKey);
            }
        }

        // encapsulates each row entry
        private void AddHotkeyRow(HotkeyEnum hotkey, Key? initialBound)
        {
            var text = new TextBlock();
            text.Text = hotkey.ToString();

            var input = new KeybindTextBox(hotkey, initialBound);

            var panel = new WrapPanel();

            panel.Children.Add(text);
            panel.Children.Add(input);

            FormHotkeys_Table.Children.Add(panel);
        }

        // encapsulates our button behaviour
        class KeybindTextBox : Button
        {
            private bool isFocused = false;

            private string initialText = "";
            private Key initialKey = Key.D0;
            private HotkeyEnum pair;
            private Key selectedKey;
            
            public KeybindTextBox(HotkeyEnum pair, Key? initialKey = null)
            {

                if (initialKey == null)
                {
                    this.initialText = "Unbound";
                }
                else
                {
                    this.initialText = initialKey.ToString();
                    this.initialKey = (Key)initialKey;
                }

                this.pair = pair;

                this.Content = initialText;

            }

            private void Reset()
            {
                this.Content = initialText;
                this.isFocused = false;
            }


            private bool IsKeyValid(Key key)
            {
                // we don't want duplicate binds
                var isKeyAlreadyBound = JsonKeybinds.GetInstance().HasKeyBound(key);
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
                }
            }

            protected override void OnClick()
            {
                if (this.isFocused)
                {
                    this.Content = initialKey.ToString();
                }
                else
                {
                    this.Content = "Press any key... (ESC to Cancel)";
                }

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
