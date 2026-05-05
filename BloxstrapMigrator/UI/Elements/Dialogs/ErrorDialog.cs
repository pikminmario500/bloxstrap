using Migrator;

using System.Drawing;
using System.Windows.Forms;
using System.Web;

public static class ErrorDialog
{
    const int MAX_GITHUB_URL_LENGTH = 8192;

    public static void Show(string message, Exception? ex = null)
    {
        string UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        var form = new Form
        {
            Text = Strings.Dialog_Title,
            StartPosition = FormStartPosition.CenterScreen,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(540, 0),
            RightToLeft = Locale.RightToLeft ? RightToLeft.Yes : RightToLeft.No
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(10),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        form.Controls.Add(layout);

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0),
            Anchor = AnchorStyles.Right
        };

        layout.Controls.Add(buttonPanel, 0, 2);

        var CloseButton = new Button { Text = Strings.Common_Close, AutoSize = true };

        CloseButton.Click += (_, _) => form.Close();

        buttonPanel.Controls.Add(CloseButton);

        if (ex is not HttpRequestException)
        {
            var ReportButton = new Button { Text = Strings.Dialog_Exception_Report, AutoSize = true };

            string template = "https://github.com/bloxstraplabs/bloxstrap/issues/new?template=bug_report.yaml";
            string title = HttpUtility.UrlEncode("[BUG] Migrator: ");
            string log = "";

            if (ex is not null)
            {
                title += HttpUtility.UrlEncode($"{ex.GetType()}: {ex.Message}".Replace(UserProfilePath, "%UserProfile%", StringComparison.InvariantCultureIgnoreCase));
                log += HttpUtility.UrlEncode(ex.ToString().Replace(UserProfilePath, "%UserProfile%", StringComparison.InvariantCultureIgnoreCase));
            }

            string issueUrl = $"{template}&title={title}&log={log}";

            if (issueUrl.Length > MAX_GITHUB_URL_LENGTH)
            {
                // url is way too long for github. remove the log parameter.
                issueUrl = $"{template}&title={title}";

                if (issueUrl.Length > MAX_GITHUB_URL_LENGTH)
                    issueUrl = template; // bruh
            }
        
            ReportButton.Click += (_, _) =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = issueUrl,
                    UseShellExecute = true
                });
            };

            buttonPanel.Controls.Add(ReportButton);
        }

        if (ex is not null)
        {
            var exception = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Font = new Font("Courier New", 9f),
                Text = ex.ToString().Replace(UserProfilePath, "%UserProfile%", StringComparison.InvariantCultureIgnoreCase),
                Dock = DockStyle.Fill
            };

            layout.Controls.Add(exception, 0, 1);
        }
        else
        {
            layout.RowStyles[1].Height = 0;
        }

        var text = new Label
        {
            Text = message,
            AutoSize = true,
            MaximumSize = new Size(500, 0),
            Padding = new Padding(0, 0, 0, 10)
        };

        layout.Controls.Add(text, 0, 0);

        form.ShowDialog();
    }
}
