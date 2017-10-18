using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TinyPNGApplication.UserControls
{
    [Feature(typeof(DropBorderDefaults))]
    public class DropBorder : Border
    {
        public enum State
        {
            None,
            DragOver,
            Drop
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(DropBorder));

        public static readonly DependencyProperty FilenameProperty =
            DependencyProperty.Register("Filename", typeof(string), typeof(DropBorder));

        public static readonly DependencyProperty DragDropStateProperty =
            DependencyProperty.Register("DragDropState", typeof(State), typeof(DropBorder));

        public State DragDropState
        {
            get { return (State)GetValue(DragDropStateProperty); }
            set { SetValue(DragDropStateProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string Filename
        {
            get { return (string)GetValue(FilenameProperty); }
            set { SetValue(FilenameProperty, value); }
        }


        public DropBorder()
        {
            this.AllowDrop = true;
            this.Background = Brushes.Transparent;
            this.DragOver += DropBorder_DragOver;
            this.Drop += DropBorder_Drop;
        }

        private void DropBorder_DragOver(object sender, System.Windows.DragEventArgs e)
        {
            Filename = GetFilename(e);

            DragDropState = State.DragOver;

            if (Command != null && Command.CanExecute(e))
            {
                Command.Execute(e);
            }
        }

        private void DropBorder_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Filename = GetFilename(e);

            DragDropState = State.Drop;

            if (Command != null && Command.CanExecute(e))
            {
                Command.Execute(e);
            }

            Filename = null;

            DragDropState = State.None;
        }

        private string GetFilename(System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files != null && files.Any())
                    return files[0];
            }

            return null;
        }
    }

    public class DropBorderDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            base.InitializeDefaults(item);

            item.Properties["AllowDrop"].SetValue(true);
        }
    }
}
