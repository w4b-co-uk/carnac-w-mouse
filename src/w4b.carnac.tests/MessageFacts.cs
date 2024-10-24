﻿using Carnac.Logic.KeyMonitor;
using Carnac.Logic.Models;
using System.Windows.Forms;
using Xunit;
using Message = Carnac.Logic.Models.Message;

namespace Carnac.Tests {
    public class MessageFacts {
        private readonly ProcessInfo fakeProcess = new ProcessInfo("FakeProcess");

        [Fact]
        public void message_does_not_group_different_letters() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Back, KeyDirection.Down, false, false, false), false, new[] { "a" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Back, KeyDirection.Down, false, false, false), false, new[] { "b" })));

            // assert
            string expected = string.Join(string.Empty, result.Text);
            Assert.Equal("ab", expected);
        }

        [Fact]
        public void message_does_not_group_letter_and_backspace() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Back, KeyDirection.Down, false, false, false), false, new[] { "a" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Back, KeyDirection.Down, false, false, false), false, new[] { "Back" })));

            // assert
            Assert.Equal("aBack", string.Join(string.Empty, result.Text));
        }

        [Fact]
        public void message_groups_multiple_backspace_key_presses_together() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Back, KeyDirection.Down, false, false, false), false, new[] { "Back" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Back, KeyDirection.Down, false, false, false), false, new[] { "Back" })));

            // assert
            Assert.Equal("Back x 2 ", string.Join(string.Empty, result.Text));
        }

        [Fact]
        public void message_does_not_group_different_arrow_key_presses_together() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Up, KeyDirection.Down, false, false, false), false, new[] { "Up" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Down, KeyDirection.Down, false, false, false), false, new[] { "Down" })));

            // assert
            Assert.Equal("↑↓", string.Join(string.Empty, result.Text));
        }

        [Fact]
        public void message_groups_two_equal_arrow_key_presses_together() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Down, KeyDirection.Down, false, false, false), false, new[] { "Down" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Down, KeyDirection.Down, false, false, false), false, new[] { "Down" })));

            // assert
            Assert.Equal("↓ x 2 ", string.Join(string.Empty, result.Text));
        }

        [Fact]
        public void message_groups_three_equal_arrow_key_presses_together() {
            // arrange
            Message result = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Down, KeyDirection.Down, false, false, false), false, new[] { "Down" }))
                .Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Down, KeyDirection.Down, false, false, false), false, new[] { "Down" })))
                .Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.Down, KeyDirection.Down, false, false, false), false, new[] { "Down" })));

            // assert
            Assert.Equal("↓ x 3 ", string.Join(string.Empty, result.Text));
        }

        [Fact]
        public void multiple_shortcuts_have_comma_inserted_between_input() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.R, KeyDirection.Down, false, true, false), false, new[] { "Control", "R" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.T, KeyDirection.Down, false, true, false), false, new[] { "Control", "T" })));

            // assert
            Assert.Equal("Control + R, Control + T", string.Join(string.Empty, result.Text));
        }

        [Fact]
        public void multiple_shortcuts_duplicate_shortcuts_are_grouped() {
            // arrange
            Message message = new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.R, KeyDirection.Down, false, true, false), false, new[] { "Control", "R" }));

            // act
            Message result = message.Merge(new Message(new KeyPress(fakeProcess, new InterceptKeyEventArgs(Keys.R, KeyDirection.Down, false, true, false), false, new[] { "Control", "R" })));

            // assert
            string actual = string.Join(string.Empty, result.Text);

            Assert.Equal("Control + R x 2 ", actual);
        }
    }
}