using System.Threading;
using Ookii.Dialogs.Wpf;

namespace RebornModManager.Utilities
{
    public static class Dialog
    {
        public enum Button
        {
            Yes, No, Cancel
        }

        private static Button Convert(ButtonType pType)
        {
            switch (pType)
            {
                case ButtonType.Yes:
                    return Button.Yes;
                case ButtonType.No:
                    return Button.No;
                case ButtonType.Cancel:
                default:
                    return Button.Cancel;
            }
        }

        public static void ShowInfo(string pTitle, string pMsg)
        {
            var td = new TaskDialog
            {
                WindowTitle = pTitle,
                Content = pMsg
            };
            td.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
            td.ShowDialog();
        }

        public static string FolderBrowser(string pTitle, string pPath)
        {
            var dlg = new VistaFolderBrowserDialog
            {
                SelectedPath = pPath,
                Description = pTitle,
                UseDescriptionForTitle = true
            };
            dlg.ShowDialog();
            return dlg.SelectedPath;
        }

        public static Button YesNoBox(string pTitle, string pQuestion)
        {
            var dlg = new TaskDialog
            {
                WindowTitle = pTitle,
                Content = pQuestion
            };
            dlg.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
            dlg.Buttons.Add(new TaskDialogButton(ButtonType.No));
            var res = dlg.ShowDialog();
            return Convert(res.ButtonType);
        }
    }

    public class UpdateDialog
    {
        private ProgressDialog _dlg;
        private int currentProgress;

        public UpdateDialog(string pTitle, string pMsg)
        {
            _dlg = new ProgressDialog
            {
                WindowTitle = pTitle,
                Text = pMsg
            };
            _dlg.DoWork += (sender, args) =>
            {
                while (true)
                {
                    if (_dlg.CancellationPending)
                        return;
                    try
                    {
                        
                        _dlg.ReportProgress(currentProgress, _dlg.Text, currentProgress + "%", currentProgress);
                    }
                    catch
                    {
                        //
                    }
                    Thread.Sleep(1000/24);
                }
            };
        }

        public void Update(int pProgress)
        {
            currentProgress = pProgress;
        }

        public void Show()
        {
            _dlg.ShowDialog();
        }

        public void Cancel()
        {
            _dlg.Dispose();
        }
    }
}
