using System;
using Reflector;
using Reflector.CodeModel;

namespace Reflector.FileDisassembler
{
	internal class FileDisassemblerPackage : IPackage
	{
		private IWindowManager windowManager;
		private ICommandBarManager commandBarManager;
		private ICommandBarSeparator separator;
		private ICommandBarButton button;

		public void Load(IServiceProvider serviceProvider)
		{
			FileDisassemblerWindow fileDisassemblerWindow = new FileDisassemblerWindow();
			fileDisassemblerWindow.AssemblyBrowser = (IAssemblyBrowser) serviceProvider.GetService(typeof(IAssemblyBrowser));
			fileDisassemblerWindow.AssemblyManager = (IAssemblyManager) serviceProvider.GetService(typeof(IAssemblyManager));
			fileDisassemblerWindow.LanguageManager = (ILanguageManager) serviceProvider.GetService(typeof(ILanguageManager));
			fileDisassemblerWindow.TranslatorManager = (ITranslatorManager) serviceProvider.GetService(typeof(ITranslatorManager));

			this.windowManager = (IWindowManager) serviceProvider.GetService(typeof(IWindowManager));
			this.windowManager.Windows.Add("FileDisassemblerWindow", fileDisassemblerWindow, "File Disassembler");
	
			this.commandBarManager = (ICommandBarManager) serviceProvider.GetService(typeof(ICommandBarManager));
			this.separator = this.commandBarManager.CommandBars["Tools"].Items.AddSeparator();
			this.button = this.commandBarManager.CommandBars["Tools"].Items.AddButton("&File Disassembler", new EventHandler(this.FileDisassemblerButton_Click));
		}

		public void Unload()
		{
			this.windowManager.Windows.Remove("FileDisassemblerWindow");
			this.commandBarManager.CommandBars["Tools"].Items.Remove(this.button);
			this.commandBarManager.CommandBars["Tools"].Items.Remove(this.separator);
		}

		private void FileDisassemblerButton_Click(object sender, EventArgs e)
		{
			this.windowManager.Windows["FileDisassemblerWindow"].Visible = true;
		}
	}
}
