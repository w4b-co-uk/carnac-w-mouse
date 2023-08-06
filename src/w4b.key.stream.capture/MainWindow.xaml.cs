using Carnac.Logic.KeyMonitor;
using Carnac.Tests;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;

namespace KeyStreamCapture {
    public partial class MainWindow {
        private readonly List<InterceptKeyEventArgs> keys = new();
        private readonly IDisposable subscription;
        private bool capturing;

        public MainWindow() {
            InitializeComponent();
            subscription = InterceptKeys.Current.GetKeyStream().Subscribe(value => {
                if (capturing) {
                    keys.Add(value);
                }
            });
        }

        private void StartCapture(object sender, RoutedEventArgs e) {
            keys.Clear();
            capturing = true;
        }

        private void StopCapture(object sender, RoutedEventArgs e) {
            capturing = false;
            GenerateCode();
        }

        private void GenerateCode() {
            _ = keys.ToObservable();
            CodeGeneratorOptions cgo = new() {
                BracingStyle = "C",
                BlankLinesBetweenMembers = false
            };
            using CSharpCodeProvider provider = new();
            CodeMemberMethod method = new() {
                Name = "KeyStream",
                ReturnType = new CodeTypeReference(typeof(IObservable<InterceptKeyEventArgs>))
            };

            CodeVariableDeclarationStatement player =
                new(new CodeTypeReference(typeof(KeyPlayer)), "keys",
                new CodeObjectCreateExpression(typeof(KeyPlayer)));
            _ = method.Statements.Add(player);
            foreach (InterceptKeyEventArgs interceptKeyEventArgs in keys) {
                CodeObjectCreateExpression key = new(new CodeTypeReference(typeof(InterceptKeyEventArgs)),
                new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Keys)), interceptKeyEventArgs.Key.ToString()),
                new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(KeyDirection)), interceptKeyEventArgs.KeyDirection.ToString()),
                new CodePrimitiveExpression(interceptKeyEventArgs.AltPressed),
                new CodePrimitiveExpression(interceptKeyEventArgs.ControlPressed),
                new CodePrimitiveExpression(interceptKeyEventArgs.ShiftPressed));

                CodeMethodInvokeExpression keyPress = new(new CodeVariableReferenceExpression("keys"), "Add", key);
                _ = method.Statements.Add(keyPress);
            }
            _ = method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("keys")));

            StringBuilder sb = new();
            using (StringWriter stringWriter = new(sb)) {
                provider.GenerateCodeFromMember(method, stringWriter, cgo);
            }
            textBox.Text = sb.ToString();
        }

        protected override void OnClosed(EventArgs e) {
            subscription.Dispose();
        }
    }
}
