using LinkAllConfig.Properties;
using LinkAllConfig.Utils;

namespace LinkAllConfig;

/// <summary> The form1. </summary>
public partial class Form1 : Form
{
	private readonly Action<string> output;
	private Helper helper;

	/// <summary> Initializes a new instance of the <see cref="Form1"/> class. </summary>
	public Form1()
	{
		InitializeComponent();
		output = s =>
		{
			Debug.WriteLine(s);

			Invoke(() =>
		{
			txtOutput.AppendText(s);
			txtOutput.AppendText(Environment.NewLine);
		});
		};
		txtFsutilPath.Text = string.IsNullOrWhiteSpace(Settings.Default.FsuitlPath) || !File.Exists(Settings.Default.FsuitlPath) ? @"C:\Windows\System32\fsutil.exe" : Settings.Default.FsuitlPath;
		helper = new Helper(txtFsutilPath.Text, output);
	}

	/// <summary>
	/// btns the create links_ click.
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The e.</param>
	private async void BtnCreateLinks_ClickAsync(object sender, EventArgs e)
	{
		await helper.MakeFileHardLinksAsync(txtLinks.Text, txtSource.Text).ConfigureAwait(false);
	}

	/// <summary>
	/// btns the select link folder_ click.
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The e.</param>
	private void BtnSelectLinkFolder_Click(object sender, EventArgs e)
	{
		folderBrowserDialog1.SelectedPath = txtLinks.Text;
		_ = folderBrowserDialog1.ShowDialog(this);
		txtLinks.Text = folderBrowserDialog1.SelectedPath;
	}

	/// <summary>
	/// btns the select tool_ click.
	/// </summary>
	/// <param name="sender">The sender.</param>
	/// <param name="e">The e.</param>
	private void BtnSelectTool_Click(object sender, EventArgs e)
	{
		openFileDialog1.InitialDirectory = Path.GetDirectoryName(txtFsutilPath.Text);
		openFileDialog1.FileName = Path.GetFileName(txtFsutilPath.Text);
		switch (openFileDialog1.ShowDialog(this))
		{
			case DialogResult.OK:
				txtFsutilPath.Text = openFileDialog1.FileName;
				helper = new Helper(txtFsutilPath.Text, output);
				break;
			case DialogResult.Abort:

			case DialogResult.Cancel:
				break;
		}
	}

	/// <summary> btns the sel targets_ click. </summary>
	private void BtnSelTargets_Click(object sender, EventArgs e)
	{
		folderBrowserDialog1.SelectedPath = txtSource.Text;
		switch (folderBrowserDialog1.ShowDialog(this))
		{
			case DialogResult.OK:
				txtSource.Text = folderBrowserDialog1.SelectedPath;

				BindFileList(folderBrowserDialog1.SelectedPath);

				break;
			case DialogResult.Abort:

			case DialogResult.Cancel:
				break;
		}
	}

	private void BindFileList(string selectedPath)
	{
		if (!Directory.Exists(selectedPath))
		{
			return;
		}

		var files = Directory.GetFiles(selectedPath);

		this.ListTargetFiles.Items.Clear();
		foreach (var file in files)
		{
			this.ListTargetFiles.Items.Add(new ListViewItem()
			{
				Name = file,
				Text = Path.GetFileName(file),
			});
		}
	}

	private void TxtFsutilPath_Validated(object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(txtFsutilPath.Text) || !File.Exists(txtFsutilPath.Text))
		{
			errorProvider1.SetError(txtFsutilPath, "请指定工具路径");
		}
		else
		{
			errorProvider1.SetError(txtFsutilPath, string.Empty);
		}
	}

	private void button1_Click(object sender, EventArgs e)
	{
		Settings.Default.SourceFolder = "xxxxx";
		Settings.Default.Save();
	}

	private void Form1_Load(object sender, EventArgs e)
	{
		txtSource.Text = Settings.Default.SourceFolder;
		txtLinks.Text = Settings.Default.LinkFolder;
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		Settings.Default.FsuitlPath = txtFsutilPath.Text;
		Settings.Default.SourceFolder = txtSource.Text;
		Settings.Default.LinkFolder = txtLinks.Text;
		Settings.Default.Save();
	}
}