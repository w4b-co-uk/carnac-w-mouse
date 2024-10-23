using Carnac.Logic.MouseMonitor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using w4b.carnac.logic;

namespace Carnac.Logic.Models {
    public sealed class Message {
        private readonly ReadOnlyCollection<KeyPress> keys;

        public Message() {
            LastMessage = DateTime.Now;
        }

        public Message(KeyPress key)
            : this() {
            ProcessName = key.Process.ProcessName;
            ProcessIcon = key.Process.ProcessIcon;
            CanBeMerged = !key.HasModifierPressed;
            IsModifier = key.HasModifierPressed;

            keys = new ReadOnlyCollection<KeyPress>(new[] { key });
            Text = new ReadOnlyCollection<string>(CreateTextSequence(key).ToArray());
        }

        public Message(IEnumerable<KeyPress> keys, KeyShortcut shortcut, bool isShortcut = false)
            : this() {
            KeyPress[] allKeys = keys.ToArray();
            string[] distinctProcessName = allKeys.Select(k => k.Process.ProcessName)
                .Distinct()
                .ToArray();
            if (distinctProcessName.Count() != 1) {
                throw new InvalidOperationException("Keys are from different processes");
            }

            ProcessName = distinctProcessName.Single();
            ProcessIcon = allKeys.First().Process.ProcessIcon;
            ShortcutName = shortcut.Name;

            IsShortcut = isShortcut;
            IsModifier = allKeys.Any(k => k.HasModifierPressed);
            CanBeMerged = false;

            this.keys = new ReadOnlyCollection<KeyPress>(allKeys);

            List<string> textSeq = CreateTextSequence(allKeys).ToList();
            if (!string.IsNullOrEmpty(ShortcutName)) {
                textSeq.Add(string.Format(" [{0}]", ShortcutName));
            }

            Text = new ReadOnlyCollection<string>(textSeq);
        }

        private Message(Message initial, Message appended)
            : this(initial.keys.Concat(appended.keys), new KeyShortcut(initial.ShortcutName)) {
            Previous = initial;
            CanBeMerged = true;
        }

        private Message(Message initial, bool isDeleting)
            : this(initial.keys, new KeyShortcut(initial.ShortcutName)) {
            IsDeleting = isDeleting;
            Previous = initial;
            LastMessage = initial.LastMessage;
        }

        private Message(Message initial, Message replacer, bool replace)
            : this(replacer.keys, new KeyShortcut(replacer.ShortcutName)) {
            Previous = initial;
        }

        public string ProcessName { get; }

        public ImageSource ProcessIcon { get; }

        public string ShortcutName { get; }

        public bool CanBeMerged { get; }

        public bool IsShortcut { get; }

        public Message Previous { get; }

        public ReadOnlyCollection<string> Text { get; }

        public DateTime LastMessage { get; }

        public bool IsDeleting { get; }

        public bool IsModifier { get; }

        public Message Merge(Message other) {
            return new Message(this, other);
        }

        public Message Replace(Message newMessage) {
            return new Message(this, newMessage, true);
        }

        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        public static Message MergeIfNeeded(Message previousMessage, Message newMessage) {
            // replace key was after standalone modifier keypress, replace by new Message
            return previousMessage.keys != null && KeyProvider.IsModifierKeyPress(previousMessage.keys[0].InterceptKeyEventArgs)
                ? previousMessage.Replace(newMessage)
                : ShouldCreateNewMessage(previousMessage, newMessage) ? newMessage : previousMessage.Merge(newMessage);
        }

        private static bool ShouldCreateNewMessage(Message previous, Message current) {
            return previous.ProcessName != current.ProcessName ||
                   current.LastMessage.Subtract(previous.LastMessage) > OneSecond ||
                   KeyProvider.IsModifierKeyPress(current.keys[0].InterceptKeyEventArgs) ||
                   // accumulate also same modifier shortcuts
                   (previous.keys[0].HasModifierPressed && !previous.keys[0].Input.SequenceEqual(current.keys[0].Input)) ||
                   !previous.CanBeMerged ||
                   !current.CanBeMerged ||
                   // new message for different mouse keys;
                   ((InterceptMouse.MouseKeys.Contains(current.keys[0].Key) ||
                   (previous.keys != null && InterceptMouse.MouseKeys.Contains(previous.keys[0].Key)))
                   && !previous.keys[0].Input.SequenceEqual(current.keys[0].Input));
        }

        public Message FadeOut() {
            return new Message(this, true);
        }

        private static IEnumerable<string> CreateTextSequence(KeyPress key) {
            return CreateTextSequence(new[] { key });
        }

        private static IEnumerable<string> CreateTextSequence(IEnumerable<KeyPress> keys) {
            return keys.Aggregate(new List<RepeatedKeyPress>(),
              (acc, curr) => {
                  if (acc.Any()) {
                      RepeatedKeyPress last = acc.Last();
                      RepeatedKeyPress secondLast = acc.Count() > 1 ? acc.SkipLast(1).Last() : null;
                      RepeatedKeyPress thirdLast = acc.Count() > 2 ? acc.SkipLast(2).Last() : null;
                      if (last.IsRepeatedBy(curr) &&
                         // not a letter or a letter with a modifier or repeated letter
                         (!(curr.InterceptKeyEventArgs.IsLetter() || curr.GetTextParts().First() == ".") || curr.HasModifierPressed || last.RepeatCount > 2)) {
                          last.IncrementRepeat();
                      } else if (last.IsRepeatedBy(curr) &&
                            // a letter is repeated 4 times now count it x times
                            (curr.InterceptKeyEventArgs.IsLetter() || curr.GetTextParts().First() == ".") && secondLast != null && secondLast.IsRepeatedBy(curr)
                            && thirdLast != null && thirdLast.IsRepeatedBy(curr)) {
                          _ = acc.Remove(last);
                          _ = acc.Remove(secondLast);
                          thirdLast.IncrementRepeat();
                          thirdLast.IncrementRepeat();
                          thirdLast.IncrementRepeat();
                      } else {
                          acc.Add(new RepeatedKeyPress(curr, last.NextRequiresSeperator));
                      }
                  } else {
                      acc.Add(new RepeatedKeyPress(curr));
                  }
                  return acc;
              })
              .SelectMany(rkp => rkp.GetTextParts());
        }

        public override string ToString() {
            return string.Format("{0} {1} {2}", ProcessName, string.Join(string.Empty, Text), ShortcutName);
        }

        private sealed class RepeatedKeyPress {
            private readonly bool requiresPrefix;
            private readonly string[] textParts;

            public RepeatedKeyPress(KeyPress keyPress, bool requiresPrefix = false) {
                NextRequiresSeperator = keyPress.HasModifierPressed;
                textParts = keyPress.GetTextParts().ToArray();
                this.requiresPrefix = requiresPrefix;
                RepeatCount = 1;
            }

            public bool NextRequiresSeperator { get; }

            public int RepeatCount { get; private set; }

            public void IncrementRepeat() {
                RepeatCount++;
            }

            public bool IsRepeatedBy(KeyPress nextKeyPress) {
                return textParts.SequenceEqual(nextKeyPress.GetTextParts());
            }

            public IEnumerable<string> GetTextParts() {
                if (requiresPrefix) {
                    yield return ", ";
                }

                foreach (string textPart in textParts) {
                    yield return textPart;
                }
                if (RepeatCount > 1) {
                    yield return string.Format(" x {0} ", RepeatCount);
                }
            }
        }

        #region Equality overrides

        private bool Equals(Message other) {
            return Text.SequenceEqual(other.Text)
                && keys.SequenceEqual(other.keys)
                && string.Equals(ProcessName, other.ProcessName)
                && Equals(ProcessIcon, other.ProcessIcon)
                && string.Equals(ShortcutName, other.ShortcutName)
                && IsShortcut.Equals(other.IsShortcut)
                && IsDeleting.Equals(other.IsDeleting)
                && LastMessage.Equals(other.LastMessage);
        }

        public override bool Equals(object obj) {
            return !(obj is null) && (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((Message)obj)));
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = Text != null ? Text.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (keys != null ? keys.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ProcessName != null ? ProcessName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ProcessIcon != null ? ProcessIcon.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ShortcutName != null ? ShortcutName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsShortcut.GetHashCode();
                hashCode = (hashCode * 397) ^ IsDeleting.GetHashCode();
                hashCode = (hashCode * 397) ^ LastMessage.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}
