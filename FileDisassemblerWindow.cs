using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using Reflector.CodeModel;

namespace Reflector.FileDisassembler
{
	/// <summary>
	/// Summary description for FileDisassemblerWindow.
	/// </summary>
	public class FileDisassemblerWindow : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.LinkLabel lnkHomepage;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label outputDirectoryLabel;
		private System.Windows.Forms.TextBox outputDirectoryText;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.TextBox logTextBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cbProjectType;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private IAssemblyBrowser _assemblyBrowser;
		private IAssemblyManager _assemblyManager;
		private ILanguageManager _languageManager;
		private CheckBox checkBoxMembersBody;
		private CheckBox checkBoxDocumentation;
		private CheckBox checkBoxPublicOnly;
		private ITranslatorManager _TranslatorManager;

		public FileDisassemblerWindow()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if ( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
				_assemblyBrowser = null;
				_assemblyManager  = null;
				_languageManager = null;
				_TranslatorManager  = null;
			}
			base.Dispose( disposing );
		}

		public ILanguageManager LanguageManager
		{
			get { return _languageManager; }
			set { _languageManager = value; }
		}

		public ITranslatorManager TranslatorManager
		{
			get { return _TranslatorManager; }
			set { _TranslatorManager = value; }
		}

		public IAssemblyManager AssemblyManager
		{
			get { return _assemblyManager; }
			set { _assemblyManager = value; }
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileDisassemblerWindow));
			this.lnkHomepage = new System.Windows.Forms.LinkLabel();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cbProjectType = new System.Windows.Forms.ComboBox();
			this.outputDirectoryText = new System.Windows.Forms.TextBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.outputDirectoryLabel = new System.Windows.Forms.Label();
			this.generateButton = new System.Windows.Forms.Button();
			this.logTextBox = new System.Windows.Forms.TextBox();
			this.checkBoxPublicOnly = new System.Windows.Forms.CheckBox();
			this.checkBoxDocumentation = new System.Windows.Forms.CheckBox();
			this.checkBoxMembersBody = new System.Windows.Forms.CheckBox();
			this.groupBox4.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// lnkHomepage
			// 
			this.lnkHomepage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.lnkHomepage.Location = new System.Drawing.Point(248, 568);
			this.lnkHomepage.Name = "lnkHomepage";
			this.lnkHomepage.Size = new System.Drawing.Size(184, 23);
			this.lnkHomepage.TabIndex = 24;
			this.lnkHomepage.TabStop = true;
			this.lnkHomepage.Text = "http://www.denisbauer.com/";
			this.lnkHomepage.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHomepage_LinkClicked);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(16, 568);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(232, 23);
			this.label7.TabIndex = 23;
			this.label7.Text = "Copyright 2004-2011 by Denis Bauer, ";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.panel1);
			this.groupBox4.Controls.Add(this.groupBox1);
			this.groupBox4.Location = new System.Drawing.Point(8, 8);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(448, 552);
			this.groupBox4.TabIndex = 22;
			this.groupBox4.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.Add(this.pictureBox1);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Location = new System.Drawing.Point(4, 8);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(440, 72);
			this.panel1.TabIndex = 16;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(376, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(56, 56);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(16, 40);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(376, 23);
			this.label5.TabIndex = 1;
			this.label5.Text = "Select an assembly to dump it to the disk";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(264, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Reflector FileDisassembler";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkBoxMembersBody);
			this.groupBox1.Controls.Add(this.checkBoxDocumentation);
			this.groupBox1.Controls.Add(this.checkBoxPublicOnly);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.cbProjectType);
			this.groupBox1.Controls.Add(this.outputDirectoryText);
			this.groupBox1.Controls.Add(this.progressBar);
			this.groupBox1.Controls.Add(this.outputDirectoryLabel);
			this.groupBox1.Controls.Add(this.generateButton);
			this.groupBox1.Controls.Add(this.logTextBox);
			this.groupBox1.Location = new System.Drawing.Point(8, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(432, 456);
			this.groupBox1.TabIndex = 25;
			this.groupBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Location = new System.Drawing.Point(216, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Project Type:";
			// 
			// cbProjectType
			// 
			this.cbProjectType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbProjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbProjectType.Items.AddRange(new object[] {
            "None",
            "Class Library",
            "Windows Application",
            "Console Application"});
			this.cbProjectType.Location = new System.Drawing.Point(288, 12);
			this.cbProjectType.Name = "cbProjectType";
			this.cbProjectType.Size = new System.Drawing.Size(132, 21);
			this.cbProjectType.TabIndex = 1;
			// 
			// outputDirectoryText
			// 
			this.outputDirectoryText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.outputDirectoryText.Location = new System.Drawing.Point(8, 40);
			this.outputDirectoryText.Name = "outputDirectoryText";
			this.outputDirectoryText.Size = new System.Drawing.Size(328, 20);
			this.outputDirectoryText.TabIndex = 0;
			this.outputDirectoryText.TextChanged += new System.EventHandler(this.OutputDirectoryText_TextChanged);
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(8, 424);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(416, 23);
			this.progressBar.TabIndex = 6;
			this.progressBar.TabStop = false;
			this.progressBar.Visible = false;
			// 
			// outputDirectoryLabel
			// 
			this.outputDirectoryLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.outputDirectoryLabel.Location = new System.Drawing.Point(8, 16);
			this.outputDirectoryLabel.Name = "outputDirectoryLabel";
			this.outputDirectoryLabel.Size = new System.Drawing.Size(100, 16);
			this.outputDirectoryLabel.TabIndex = 0;
			this.outputDirectoryLabel.Text = "&Output Directory:";
			// 
			// generateButton
			// 
			this.generateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.generateButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.generateButton.Location = new System.Drawing.Point(344, 40);
			this.generateButton.Name = "generateButton";
			this.generateButton.Size = new System.Drawing.Size(75, 21);
			this.generateButton.TabIndex = 2;
			this.generateButton.Text = "&Generate";
			this.generateButton.Click += new System.EventHandler(this.GenerateButton_Click);
			// 
			// logTextBox
			// 
			this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.logTextBox.Location = new System.Drawing.Point(8, 90);
			this.logTextBox.MaxLength = 16777216;
			this.logTextBox.Multiline = true;
			this.logTextBox.Name = "logTextBox";
			this.logTextBox.ReadOnly = true;
			this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.logTextBox.Size = new System.Drawing.Size(416, 326);
			this.logTextBox.TabIndex = 5;
			this.logTextBox.Visible = false;
			this.logTextBox.WordWrap = false;
			// 
			// checkBoxPublicOnly
			// 
			this.checkBoxPublicOnly.AutoSize = true;
			this.checkBoxPublicOnly.Location = new System.Drawing.Point(8, 67);
			this.checkBoxPublicOnly.Name = "checkBoxPublicOnly";
			this.checkBoxPublicOnly.Size = new System.Drawing.Size(78, 17);
			this.checkBoxPublicOnly.TabIndex = 7;
			this.checkBoxPublicOnly.Text = "Only public";
			this.checkBoxPublicOnly.UseVisualStyleBackColor = true;
			// 
			// checkBoxDocumentation
			// 
			this.checkBoxDocumentation.AutoSize = true;
			this.checkBoxDocumentation.Checked = true;
			this.checkBoxDocumentation.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxDocumentation.Location = new System.Drawing.Point(93, 67);
			this.checkBoxDocumentation.Name = "checkBoxDocumentation";
			this.checkBoxDocumentation.Size = new System.Drawing.Size(98, 17);
			this.checkBoxDocumentation.TabIndex = 8;
			this.checkBoxDocumentation.Text = "Documentation";
			this.checkBoxDocumentation.UseVisualStyleBackColor = true;
			// 
			// checkBoxMembersBody
			// 
			this.checkBoxMembersBody.AutoSize = true;
			this.checkBoxMembersBody.Checked = true;
			this.checkBoxMembersBody.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMembersBody.Location = new System.Drawing.Point(198, 67);
			this.checkBoxMembersBody.Name = "checkBoxMembersBody";
			this.checkBoxMembersBody.Size = new System.Drawing.Size(95, 17);
			this.checkBoxMembersBody.TabIndex = 9;
			this.checkBoxMembersBody.Text = "Members body";
			this.checkBoxMembersBody.UseVisualStyleBackColor = true;
			// 
			// FileDisassemblerWindow
			// 
			this.Controls.Add(this.lnkHomepage);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.groupBox4);
			this.Name = "FileDisassemblerWindow";
			this.Size = new System.Drawing.Size(464, 592);
			this.groupBox4.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private void lnkHomepage_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			ProcessStartInfo sInfo = new ProcessStartInfo("iexplore.exe", "-new " + lnkHomepage.Text);
			Process.Start(sInfo);
		}

		public IAssemblyBrowser AssemblyBrowser
		{
			get { return _assemblyBrowser; }
			set
			{
				if (_assemblyBrowser != null)
				{
					_assemblyBrowser.ActiveItemChanged -= new EventHandler(AssemblyBrowser_ActiveItemChanged);
				}

				_assemblyBrowser = value;

				if (_assemblyBrowser != null)
				{
					_assemblyBrowser.ActiveItemChanged += new EventHandler(AssemblyBrowser_ActiveItemChanged);
				}
			}
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);

			outputDirectoryText.Enabled = true;
			generateButton.Enabled = true;
			checkBoxPublicOnly.Enabled = true;
			checkBoxMembersBody.Enabled = true;
			checkBoxDocumentation.Enabled = true;

			logTextBox.Visible = false;
			progressBar.Visible = false;

			AssemblyBrowser_ActiveItemChanged(this, EventArgs.Empty);
		}

		private void GenerateButton_Click(object sender, System.EventArgs e)
		{
			outputDirectoryText.Enabled = false;
			generateButton.Enabled = false;
			cbProjectType.Enabled = false;
			checkBoxPublicOnly.Enabled = false;
			checkBoxMembersBody.Enabled = false;
			checkBoxDocumentation.Enabled = false;

			string outputDirectory = Path.Combine(Environment.CurrentDirectory, Environment.ExpandEnvironmentVariables(outputDirectoryText.Text));

			try
			{
				if (!Directory.Exists(outputDirectory))
				{
					Directory.CreateDirectory(outputDirectory);
				}
			}
			catch(IOException)
			{
				MessageBox.Show("Invalid path. Please enter a correct path", "Invalid Path", MessageBoxButtons.OK);
			}
			catch(NotSupportedException)
			{
				MessageBox.Show("Invalid path. Please enter a correct path", "Invalid Path", MessageBoxButtons.OK);
			}

			logTextBox.Text = string.Empty;
			logTextBox.Visible = true;
			progressBar.Value = 0;
			progressBar.Visible = true;

			var helper = new FileDisassemblerHelper(
				AssemblyManager, TranslatorManager, LanguageManager, cbProjectType.SelectedIndex, outputDirectory,
				WriteLine,
				SetProgressBar,
				checkBoxDocumentation.Checked,
				checkBoxMembersBody.Checked,
				checkBoxPublicOnly.Checked);
				
			int exceptions = helper.GenerateCode(_assemblyBrowser.ActiveItem);

			WriteLine(string.Format("{0} error(s).", exceptions));
			WriteLine("Done.");

			progressBar.Value = 100;
			outputDirectoryText.Enabled = true;
			generateButton.Enabled = true;
			cbProjectType.Enabled = true;
			checkBoxPublicOnly.Enabled = true;
			checkBoxMembersBody.Enabled = true;
			checkBoxDocumentation.Enabled = true;
		}

		private void SetProgressBar(int pos)
		{
			progressBar.Value = pos;
		}

		private void WriteLine(string text)
		{
			logTextBox.Focus();
			logTextBox.AppendText(text + Environment.NewLine);
			logTextBox.ScrollToCaret();
			System.Windows.Forms.Application.DoEvents();
		}

		private void AssemblyBrowser_ActiveItemChanged(object sender, EventArgs e)
		{
			IAssembly assembly = _assemblyBrowser.ActiveItem as IAssembly;
			ITypeDeclaration typeDeclaration = _assemblyBrowser.ActiveItem as ITypeDeclaration;
			if ((assembly != null) && (assembly.Location != null) && (assembly.Location.Length != 0))
			{
				string folderName = assembly.Name + "_Source";
				outputDirectoryText.Text = Path.Combine(Path.GetDirectoryName(assembly.Location), folderName);
				outputDirectoryText.Enabled = true;
				checkBoxPublicOnly.Enabled = true;
				checkBoxMembersBody.Enabled = true;
				checkBoxDocumentation.Enabled = true;
				generateButton.Enabled = true;
				cbProjectType.SelectedIndex = 1;
				cbProjectType.Enabled = true;
			}
			else if (typeDeclaration != null)
			{
				outputDirectoryText.Text = "";
				outputDirectoryText.Enabled = true;
				checkBoxPublicOnly.Enabled = true;
				checkBoxMembersBody.Enabled = true;
				checkBoxDocumentation.Enabled = true;
				generateButton.Enabled = true;
				cbProjectType.SelectedIndex = 0;
				cbProjectType.Enabled = true;
			}
			else
			{
				outputDirectoryText.Text = "<No assembly selected>";
				outputDirectoryText.Enabled = false;
				checkBoxPublicOnly.Enabled = false;
				checkBoxMembersBody.Enabled = false;
				checkBoxDocumentation.Enabled = false;
				generateButton.Enabled = false;
				cbProjectType.SelectedIndex = -1;
				cbProjectType.Enabled = false;
			}
		}

		private void OutputDirectoryText_TextChanged(object sender, EventArgs e)
		{
			generateButton.Enabled = !string.IsNullOrEmpty(outputDirectoryText.Text);
		}
	}
}
