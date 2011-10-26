using System;
using Reflector.CodeModel;

namespace Reflector.FileDisassembler
{
	public class XmlDocLanguageWriter
	{
		private bool _injectXmlDoc;
		private ILanguage _language;
		private ILanguageConfiguration _configuration;
		private TextFormatter _formatter;
		private ILanguageWriter _writer;
		private IDocumentationLoader _docLoader;

		public XmlDocLanguageWriter(ILanguage language, IDocumentationLoader docLoader, bool injectXmlDoc)
		{
			_language = language;
			_docLoader = docLoader;
			_injectXmlDoc = injectXmlDoc;
			_configuration = new LanguageConfiguration();
			_formatter = new TextFormatter();
			_writer = language.GetWriter(_formatter, _configuration);
		}

		public void WriteAssembly(IAssembly assembly)
		{
			_writer.WriteAssembly(assembly);
			_formatter.WriteLine();
			foreach (IModule module in assembly.Modules)
			{
				_writer.WriteModule(module);
				foreach (IAssemblyName assemblyName in module.AssemblyReferences)
				{
					_writer.WriteAssemblyName(assemblyName);
				}
				foreach (IModuleName moduleName in module.ModuleReferences)
				{
					_writer.WriteModuleName(moduleName);
				}
			}
			foreach (IResource resource in assembly.Resources)
			{
				_writer.WriteResource(resource);
			}
		}

		public void WriteNamespace(INamespace namespaceDeclaration)
		{
			if (_injectXmlDoc)
			{
				// inject XML documentation into generated namespace
				string nsStr = GetNamespaceAsString(namespaceDeclaration);
				_formatter.Write(nsStr);
			}
			else 
			{
				// write it as the original implementor does it
				_writer.WriteNamespace(namespaceDeclaration);
			}
		}

		// get the namespace text with XML documentation
		private string GetNamespaceAsString(INamespace namespaceDeclaration)
		{
			int indent = 0;
			string namespaceStr = GetItemAsString(namespaceDeclaration, indent);
			if (_language.Name != "Delphi")
				indent = 1; // delphi classes are at indent 0, everybody else at 1
			foreach (ITypeDeclaration typeDeclaration in namespaceDeclaration.Types)
			{
				string oldStr = GetItemAsString(typeDeclaration, indent);
				string newStr = GetTypeAsString(typeDeclaration, indent);
				namespaceStr = namespaceStr.Replace(oldStr, newStr);
			}
			return namespaceStr;
		}

		// get the Type text with XML documentation
		private string GetTypeAsString(ITypeDeclaration typeDeclaration, int indent)
		{
			string typeStr = GetItemAsString(typeDeclaration, indent);
			string docStr = GetDocAsString(typeDeclaration, indent);
			typeStr = docStr + typeStr; // inject xml doc

			foreach (ITypeDeclaration item in typeDeclaration.NestedTypes)
			{
				typeStr = GetMemberAsString(typeStr, item, indent);
			}
			foreach (IEventDeclaration item in typeDeclaration.Events)
			{
				typeStr = GetMemberAsString(typeStr, item, indent);
			}
			foreach (IFieldDeclaration item in typeDeclaration.Fields)
			{
				typeStr = GetMemberAsString(typeStr, item, indent);
			}
			foreach (IMethodDeclaration item in typeDeclaration.Methods)
			{
				typeStr = GetMemberAsString(typeStr, item, indent);
			}
			foreach (IPropertyDeclaration item in typeDeclaration.Properties)
			{
				typeStr = GetMemberAsString(typeStr, item, indent);
			}

			return typeStr;
		}

		// get the member text with XML documentation
		private string GetMemberAsString(string typeStr, object item, int indent)
		{
			string docStr = GetDocAsString(item, indent + 1);
			if (docStr.Length > 0)
			{
				string itemStr = GetItemAsString(item, indent + 1);
				typeStr = typeStr.Replace(itemStr, docStr + itemStr); // inject xml doc
			}
			return typeStr;
		}

		// get the original text from the LanguageWriter
		private string GetItemAsString(object item, int indent)
		{
			TextFormatter formatter = new TextFormatter();
			ILanguageWriter writer = _language.GetWriter(formatter, _configuration);

			for (int i = 0; i < indent; i++)
			{
				formatter.WriteIndent();
			}

			if (item is INamespace)
				writer.WriteNamespace(item as INamespace);
			if (item is ITypeDeclaration)
				writer.WriteTypeDeclaration(item as ITypeDeclaration);
			if (item is IEventDeclaration)
				writer.WriteEventDeclaration(item as IEventDeclaration);
			if (item is IFieldDeclaration)
				writer.WriteFieldDeclaration(item as IFieldDeclaration);
			if (item is IMethodDeclaration)
				writer.WriteMethodDeclaration(item as IMethodDeclaration);
			if (item is IPropertyDeclaration)
				writer.WritePropertyDeclaration(item as IPropertyDeclaration);

			return formatter.ToString();
		}

		// get XML documentation from the doc loader
		private string GetDocAsString(object type, int indent)
		{
			string docStr = string.Empty;
			try
			{
				docStr = _docLoader.GetDocumentation(type);
			}
			catch
			{
				_injectXmlDoc = false; // getting xml doc failed, no need to try again
				return string.Empty;
			}

			if (docStr.StartsWith("<error>"))
			{
				return string.Empty; // no xml doc exists for this type
			}

			string comment = "///";
			if (_language.Name == "Visual Basic")
				comment = "'''";

			string indentStr = string.Empty;
			for (int i = 0; i < indent; i++)
			{
				indentStr += "    ";
			}

			docStr = docStr.Replace("\r\n", "\n");
			docStr = XmlCleanup(docStr);
			docStr = docStr.Replace("\n", "\r\n" + indentStr + comment);
			docStr = comment + docStr + "\r\n" + indentStr;
			
			return docStr;
		}

		// cleanup the XML a bit
		private string XmlCleanup(string xml)
		{
			// strip types
			xml = xml.Replace("=\"T:", "=\"");
			xml = xml.Replace("=\"E:", "=\"");
			xml = xml.Replace("=\"F:", "=\"");
			xml = xml.Replace("=\"M:", "=\"");
			xml = xml.Replace("=\"P:", "=\"");

			// trim white space
			string[] arr = xml.Trim().Split('\n');
			for (int i = 0; i < arr.Length; i++)
			{
				arr[i] = arr[i].Trim();
			}
			xml = string.Join("\n", arr);
			
			return xml;
		}

		public string Output
		{
			get
			{
				return _formatter.ToString().Replace("\r\n", "\n").Replace("\n", "\r\n");
			}
		}
	}


	internal class LanguageConfiguration : ILanguageConfiguration
	{
		private IVisibilityConfiguration _visibility = new VisibilityConfiguration();

		public IVisibilityConfiguration Visibility
		{
			get { return _visibility; }
		}

		public bool SortAlphabetically   { get { return true; } }
		public bool ShowCustomAttributes { get { return true; } }
		public bool ShowTypeMembers      { get { return true; } }
		public bool ShowMethodBodies     { get { return true; } }
		public bool ShowNamespaceTypes   { get { return true; } }
		public bool ShowNamespaceImports { get { return true; } }
	}


	internal class VisibilityConfiguration : IVisibilityConfiguration
	{
		public bool Public            { get { return true; } }
		public bool Private           { get { return true; } }
		public bool Family            { get { return true; } }
		public bool Assembly          { get { return true; } }
		public bool FamilyAndAssembly { get { return true; } }
		public bool FamilyOrAssembly  { get { return true; } }
	}

}
