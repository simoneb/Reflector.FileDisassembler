using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Resources;
using System.Xml;
using Reflector.CodeModel;
using Reflector.CodeModel.Memory;
using System.Linq;

namespace Reflector.FileDisassembler
{
	/// <summary>
	/// Summary description for FileDisassemblerHelper.
	/// </summary>
	public class FileDisassemblerHelper
	{
		public delegate void WriteLineDelegate(string line);
		public delegate void SetProgressBarDelegate(int pos);

		private IAssemblyManager _assemblyManager;
		private ILanguageManager _languageManager;
		private ITranslatorManager _TranslatorManager;

		private int _projectType = -1;
		private string _outputDirectory = string.Empty;
		private readonly bool documentation;
		private readonly bool membersBody;
		private readonly bool publicOnly;

		private event WriteLineDelegate WriteLine;
		private event SetProgressBarDelegate SetProgressBar;

		private StringCollection _projectFiles = new StringCollection();

		public FileDisassemblerHelper(
			IAssemblyManager assemblyManager,
			ITranslatorManager TranslatorManager,
			ILanguageManager languageManager,
			int projectType,
			string outputDirectory,
			WriteLineDelegate writeLineDelegate,
			SetProgressBarDelegate setProgressBarDelegate, bool documentation, bool membersBody, bool publicOnly)
		{
			_assemblyManager = assemblyManager;
			_TranslatorManager = TranslatorManager;
			_languageManager = languageManager;
			_projectType = projectType;
			_outputDirectory = outputDirectory;
			this.documentation = documentation;
			this.membersBody = membersBody;
			this.publicOnly = publicOnly;
			WriteLine += writeLineDelegate;
			SetProgressBar += setProgressBarDelegate;
		}

		public ILanguageManager LanguageManager
		{
			get { return _languageManager; }
		}

		public ITranslatorManager TranslatorManager
		{
			get { return _TranslatorManager; }
		}

		public IAssemblyManager AssemblyManager
		{
			get	{ return _assemblyManager; }
		}

		public int GenerateCode(object activeItem)
		{
			int exceptions = 0;

			ILanguageWriterConfiguration configuration = new LanguageWriterConfiguration(documentation, publicOnly, membersBody);
			IAssemblyResolver resolver = AssemblyManager.Resolver;
			AssemblyManager.Resolver = new AssemblyResolver(resolver);
			try
			{
				// write assembly
				IAssembly assembly = activeItem as IAssembly;
				if (assembly != null)
				{
					// calc progress bar count
					int pos = 0;
					int count = 2; // 1 for the assembly info, 1 for the project
					foreach (IModule module in assembly.Modules)
					{
						count += module.Types.Count;
					}
					count += assembly.Resources.Count;

					// write assembly info
					exceptions += WriteAssemblyInfo(assembly, configuration);
					SetProgressBar((++pos * 100) / count);

					// write modules
					string location = string.Empty;
					foreach (IModule module in assembly.Modules)
					{
						if (module.Location != location)
							location = module.Location;

						foreach (ITypeDeclaration typeDeclaration in module.Types.Cast<ITypeDeclaration>()
							.Where(d => publicOnly ? d.Visibility == TypeVisibility.Public || d.Visibility == TypeVisibility.NestedPublic : true))
						{
							if ((typeDeclaration.Namespace.Length != 0) || (
								(typeDeclaration.Name != "<Module>") &&
								(typeDeclaration.Name != "<PrivateImplementationDetails>")))
							{
								exceptions += WriteTypeDeclaration(typeDeclaration, configuration);
							}
							SetProgressBar((++pos * 100) / count);
						}
					}

					// write resources
					foreach (IResource resource in assembly.Resources)
					{
						exceptions += WriteResource(resource);
						SetProgressBar((++pos * 100) / count);
					}

					GenerateVisualStudioProject(assembly.Name, _outputDirectory, assembly.Modules[0].AssemblyReferences);
					SetProgressBar((++pos * 100) / count);
				}

					// write single item
				else if (activeItem is ITypeDeclaration)
				{
					ITypeDeclaration typeDeclaration = activeItem as ITypeDeclaration;
					if ((typeDeclaration.Namespace.Length != 0) || (
						(typeDeclaration.Name != "<Module>") &&
						(typeDeclaration.Name != "<PrivateImplementationDetails>")))
					{
						exceptions += WriteTypeDeclaration(typeDeclaration, configuration);
					}
				}
			}
			catch (Exception ex)
			{
				WriteLine("Error: " + ex.Message + Environment.NewLine + ex.StackTrace);
				exceptions++;
			}
			finally
			{
				AssemblyManager.Resolver = resolver;
			}
			return exceptions;
		}

		private int WriteAssemblyInfo(IAssembly assembly, ILanguageWriterConfiguration configuration)
		{
			ILanguage language = LanguageManager.ActiveLanguage;
			ITranslator translator = TranslatorManager.CreateDisassembler("Xml", null);

			int exceptions = 0;

			using (StreamWriter streamWriter = CreateFile(string.Empty, "AssemblyInfo"))
			{
				TextFormatter formatter = new TextFormatter();
				try
				{
					ILanguageWriter writer = language.GetWriter(formatter, configuration);
					assembly = translator.TranslateAssembly(assembly, false);
					writer.WriteAssembly(assembly);

					foreach (IModule module in assembly.Modules)
					{
						IModule visitedModule = translator.TranslateModule(module, false);
						writer.WriteModule(visitedModule);

						foreach (IAssemblyReference assemblyReference in module.AssemblyReferences)
						{
							IAssemblyReference visitedAssemblyReference = translator.TranslateAssemblyReference(assemblyReference);
							writer.WriteAssemblyReference(visitedAssemblyReference);
						}

						foreach (IModuleReference moduleReference in module.ModuleReferences)
						{
							IModuleReference visitedModuleReference = translator.TranslateModuleReference(moduleReference);
							writer.WriteModuleReference(visitedModuleReference);
						}
					}

					foreach (IResource resource in assembly.Resources)
					{
						writer.WriteResource(resource);
					}
				}
				catch (Exception exception)
				{
					streamWriter.WriteLine(exception.ToString());
					WriteLine(exception.ToString());
					exceptions++;
				}

				string output = formatter.ToString().Replace("\r\n", "\n").Replace("\n", "\r\n");
				streamWriter.WriteLine(output);
			}
			return exceptions;
		}
		
		private int WriteTypeDeclaration(ITypeDeclaration typeDeclaration, ILanguageWriterConfiguration configuration)
		{
			ILanguage language = LanguageManager.ActiveLanguage;
			ITranslator translator = TranslatorManager.CreateDisassembler("Xml", null);

			int exceptions = 0;
			using (StreamWriter streamWriter = CreateTypeDeclarationFile(typeDeclaration))
			{
				INamespace namespaceItem = new Namespace();
				namespaceItem.Name = typeDeclaration.Namespace;

				try
				{
					if (language.Translate)
					{
						typeDeclaration = translator.TranslateTypeDeclaration(typeDeclaration, true, true);
					}
					namespaceItem.Types.Add(typeDeclaration);
				}
				catch (Exception ex)
				{
					streamWriter.WriteLine(ex.ToString());
					WriteLine(ex.ToString());
					exceptions++;
				}
				
				TextFormatter formatter = new TextFormatter();
				ILanguageWriter writer = language.GetWriter(formatter, configuration);
				try
				{
					writer.WriteNamespace(namespaceItem);
				}
				catch (Exception exception)
				{
					streamWriter.WriteLine(exception.ToString());
					WriteLine(exception.ToString());
				}

				string output = formatter.ToString().Replace("\r\n", "\n").Replace("\n", "\r\n");
				streamWriter.WriteLine(output);
			}
			
			return exceptions;
		}

		private StreamWriter CreateTypeDeclarationFile(ITypeDeclaration typeDeclaration)
		{
			string directory = typeDeclaration.Namespace;

			string fileName = typeDeclaration.Name;
			if (typeDeclaration.GenericArguments.Count > 0)
			{
				fileName = fileName + "!" +	typeDeclaration.GenericArguments.Count.ToString();
			}

			return CreateFile(directory, fileName);
		}

		private StreamWriter CreateFile(string directory, string fileName)
		{
			//somehow Path.InvalidPathChars does not include all invalid chars
			ArrayList invalidPathChars = new ArrayList();
			invalidPathChars.AddRange(Path.InvalidPathChars);
			invalidPathChars.Add('^');
			invalidPathChars.Add('<');
			invalidPathChars.Add('>');
			invalidPathChars.Add(':');
			invalidPathChars.Add('|');
			invalidPathChars.Add('?');
			invalidPathChars.Add('*');

			foreach(char invalidChar in invalidPathChars)
				directory = directory.Replace(invalidChar, '_');

			directory = Path.Combine(_outputDirectory, directory);

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			ILanguage language = LanguageManager.ActiveLanguage;
			string fileExtension = language.FileExtension;

			foreach(char invalidChar in invalidPathChars)
				fileName = fileName.Replace(invalidChar, '_');

			fileName = Path.Combine(directory, fileName);
			fileName = Path.ChangeExtension(fileName, fileExtension);

			AddToProjectFiles(fileName);

			StreamWriter writer = null;
			try
			{
				writer = new StreamWriter(fileName);
			}
			catch(NotSupportedException ex)
			{
				throw new ApplicationException(string.Format("Could not create file '{0}'", fileName), ex);
			}
			return writer;
		}

		private int WriteResource(IResource resource)
		{
			IEmbeddedResource embeddedResource = resource as IEmbeddedResource;
			if (embeddedResource != null)
			{
				try 
				{
					byte[] buffer = embeddedResource.Value;
					string fileName = Path.Combine(_outputDirectory, resource.Name);
					
					if (resource.Name.EndsWith(".resources"))
					{
						fileName = Path.ChangeExtension(fileName, ".resx");
						using (MemoryStream ms = new MemoryStream(embeddedResource.Value))
						{
							ResXResourceWriter resxw = new ResXResourceWriter(fileName);
							IResourceReader reader = new ResourceReader(ms);
							IDictionaryEnumerator en = reader.GetEnumerator();
							while (en.MoveNext()) 
							{
								resxw.AddResource(en.Key.ToString(), en.Value);
							}
							reader.Close();
							resxw.Close();
						}
					}
					else // other embedded resource
					{
						if (buffer != null)
						{
							using (Stream stream = File.Create(fileName))
							{
								stream.Write(buffer, 0, buffer.Length);
							}
						}
					}
					AddToProjectFiles(fileName);
					return 0;
				}
				catch (Exception ex)
				{
					WriteLine("Error in " + resource.Name + " : " + ex.Message);
				}
			}

			return WriteOtherResource(resource);
		}

		private int WriteOtherResource(IResource resource)
		{
			byte[] buffer = null;

			IEmbeddedResource embeddedResource = resource as IEmbeddedResource;
			if (embeddedResource != null)
			{
				buffer = embeddedResource.Value;
			}

			IFileResource fileResource = resource as IFileResource;
			if (fileResource != null)
			{
				string location = Path.Combine(Path.GetDirectoryName(fileResource.Module.Location), fileResource.Location);
				location = Environment.ExpandEnvironmentVariables(location);
				if (File.Exists(location))
				{
					using (Stream stream = new FileStream(location, FileMode.Open, FileAccess.Read))
					{
						if (fileResource.Offset == 0)
						{
							buffer = new byte[stream.Length];
							stream.Read(buffer, 0, buffer.Length);
						}
						else
						{
							BinaryReader reader = new BinaryReader(stream);
							int size = reader.ReadInt32();
							buffer = new byte[size];
							stream.Read(buffer, 0, size);
						}
					}
				}
			}

			if (buffer != null)
			{
				string fileName = Path.Combine(_outputDirectory, resource.Name);
				using (Stream stream = File.Create(fileName))
				{
					stream.Write(buffer, 0, buffer.Length);
				}
				WriteLine(fileName);
			}

			return 0;
		}

		private void AddToProjectFiles(string fileName)
		{
			WriteLine(fileName);
			string relPath = fileName.Remove(0, _outputDirectory.Length + 1);
			_projectFiles.Add(relPath);
		}

		/// <summary>
		/// Generates Visual Studio project files
		/// </summary>
		/// <param name="ProjectName"></param>
		/// <param name="ProjectPath"></param>
		/// <param name="References"></param>
		/// <remarks>This code was provided by Jens Andersson, kobingo@gmail.com</remarks>
		private void GenerateVisualStudioProject(string ProjectName, string ProjectPath, IAssemblyReferenceCollection References)
		{
			if (_projectType < 1 || _projectType > 3)
				return;

			System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			string projectExt;
			string template = assembly.GetName().Name;
			ILanguage language = LanguageManager.ActiveLanguage;
			if (language.Name == "C#")
			{
				projectExt = ".csproj";
				template += ".VSProjects.CSharpProject.xml";
			}
			else if (language.Name == "Visual Basic")
			{
				projectExt = ".vbproj";
				template += ".VSProjects.VBProject.xml";
			}
			else if (language.Name == "MC++")
			{
				projectExt = ".vcproj";
				if (_projectType == 1)
					template += ".VSProjects.CppProject_dll.xml";
				else
					template += ".VSProjects.CppProject_exe.xml";
			}
			else
			{
				WriteLine("Cannot create project file, " + language.Name + " language is not supported.");
				return;
			}

			Stream stream1 = assembly.GetManifestResourceStream(template);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream1);

			if (language.Name == "MC++")
				UpdateProjectTemplateCpp(xmlDoc, ProjectName, References);
			else
				UpdateProjectTemplateCsVb(xmlDoc, ProjectName, References);

			string fileName;
			fileName = Path.Combine(ProjectPath, ProjectName + projectExt);
			xmlDoc.Save(fileName);
			WriteLine(fileName);
		}

		private void UpdateProjectTemplateCsVb(XmlDocument xmlDoc, string ProjectName, IAssemblyReferenceCollection References)
		{
			string text2 = "CSHARP";
			if (LanguageManager.ActiveLanguage.Name == "Visual Basic")
				text2 = "VisualBasic";

			XmlNode node0 = xmlDoc.SelectSingleNode("/VisualStudioProject/" + text2);
			node0.Attributes["ProjectGuid"].Value = Guid.NewGuid().ToString().ToUpper();


			string text3 = "/VisualStudioProject/" + text2 + "/Build/Settings";
			XmlNode node1 = xmlDoc.SelectSingleNode(text3);
			node1.Attributes["AssemblyName"].Value = ProjectName;
			node1.Attributes["RootNamespace"].Value = ProjectName;
			switch (_projectType)
			{
				case 1: node1.Attributes["OutputType"].Value = "Library"; break;
				case 2: node1.Attributes["OutputType"].Value = "WinExe"; break;
				case 3: node1.Attributes["OutputType"].Value = "Exe"; break;
			}
			string text4 = "/VisualStudioProject/" + text2 + "/Files/Include";
			XmlNode node2 = xmlDoc.SelectSingleNode(text4);
			node2.RemoveAll();
			foreach (string filePath in _projectFiles)
			{
				if ((filePath != null) && (filePath != string.Empty))
				{
					XmlNode node3 = xmlDoc.CreateElement("", "File", "");
					XmlAttribute attribute1 = xmlDoc.CreateAttribute("RelPath");
					XmlAttribute attribute2 = xmlDoc.CreateAttribute("SubType");
					XmlAttribute attribute3 = xmlDoc.CreateAttribute("BuildAction");
					attribute1.Value = filePath;
					node3.Attributes.Append(attribute1);
					if (filePath.EndsWith(".cs") || filePath.EndsWith(".vb"))
					{
						attribute2.Value = "Code";
						node3.Attributes.Append(attribute2);
						attribute3.Value = "Compile";
					}
					else // gotta be a resource
					{
						attribute3.Value = "EmbeddedResource";
					}
					node3.Attributes.Append(attribute3);
					node2.AppendChild(node3);
				}
			}
			string text6 = "/VisualStudioProject/" + text2 + "/Build/References";
			XmlNode node4 = xmlDoc.SelectSingleNode(text6);
			node4.RemoveAll();
			if (References != null)
			{
				StringCollection collection1 = new StringCollection();
				foreach (IAssemblyReference name1 in References)
				{
					if (!collection1.Contains(name1.Name))
					{
						XmlNode node5 = xmlDoc.CreateElement("", "Reference", "");
						XmlAttribute attribute4 = xmlDoc.CreateAttribute("Name");
						attribute4.Value = name1.Name;
						node5.Attributes.Append(attribute4);
						XmlAttribute attribute5 = xmlDoc.CreateAttribute("AssemblyName");
						attribute5.Value = name1.Name;
						node5.Attributes.Append(attribute5);
						if(name1 != null && name1.Resolve() != null)
						{
							XmlAttribute attribute6 = xmlDoc.CreateAttribute("HintPath");
							attribute6.Value = Path.GetFileName(name1.Resolve().Location);
							node5.Attributes.Append(attribute6);
						}
						node4.AppendChild(node5);
						collection1.Add(name1.Name);
					}
				}
			}
		}

		private void UpdateProjectTemplateCpp(XmlDocument xmlDoc, string ProjectName, IAssemblyReferenceCollection References)
		{
			string text1 = "/VisualStudioProject";
			XmlNode node1 = xmlDoc.SelectSingleNode(text1);
			node1.Attributes["ProjectGUID"].Value = Guid.NewGuid().ToString().ToUpper();
			node1.Attributes["Name"].Value = ProjectName;
			node1.Attributes["RootNamespace"].Value = ProjectName;

			string text2 = "/VisualStudioProject/Files";
			XmlNode node2 = xmlDoc.SelectSingleNode(text2);
			node2.RemoveAll();
			Hashtable folders = new Hashtable();
			foreach (string filePath in _projectFiles)
			{
				if ((filePath != null) && (filePath != string.Empty))
				{
					string pathName = Path.GetDirectoryName(filePath);
					if (folders[pathName] == null)
					{
						if (pathName.Length == 0)
						{
							folders[pathName] = node2;
						}
						else 
						{
							XmlNode pathNode = xmlDoc.CreateElement("", "Filter", "");
							XmlAttribute nameAttr = xmlDoc.CreateAttribute("Name");
							nameAttr.Value = pathName;
							pathNode.Attributes.Append(nameAttr);
							node2.AppendChild(pathNode);
							folders[pathName] = pathNode;
						}
					}
					XmlNode fileNode = xmlDoc.CreateElement("", "File", "");
					XmlAttribute pathAttr = xmlDoc.CreateAttribute("RelativePath");
					pathAttr.Value = filePath;
					fileNode.Attributes.Append(pathAttr);
					XmlNode node3 = (XmlNode)folders[pathName];
					node3.AppendChild(fileNode);
				}
			}

			string text4 = "/VisualStudioProject/References";
			XmlNode node4 = xmlDoc.SelectSingleNode(text4);
			node4.RemoveAll();
			if (References != null)
			{
				StringCollection collection1 = new StringCollection();
				foreach (IAssemblyReference name1 in References)
				{
					if (!collection1.Contains(name1.Name))
					{
						XmlNode refNode = xmlDoc.CreateElement("", "AssemblyReference", "");
						XmlAttribute pathAttr = xmlDoc.CreateAttribute("RelativePath");
						pathAttr.Value = Path.GetFileName(name1.Resolve().Location);
						refNode.Attributes.Append(pathAttr);
						node4.AppendChild(refNode);
						collection1.Add(name1.Name);
					}
				}
			}
		}

		private class LanguageWriterConfiguration : ILanguageWriterConfiguration
		{
			private readonly bool documentation;
			private readonly bool membersBody;

			public LanguageWriterConfiguration(bool documentation, bool publicOnly, bool membersBody)
			{
				this.documentation = documentation;
				this.membersBody = membersBody;
				Visibility = new VisibilityConfiguration(publicOnly);
			}

			public IVisibilityConfiguration Visibility { get; private set; }

			public string this[string name]
			{
				get
				{
					switch (name)
					{
						case "ShowDocumentation":
							return documentation ? "true" : "false";
						case "ShowCustomAttributes":
						case "ShowNamespaceImports":
						case "ShowNamespaceBody":
						case "ShowTypeDeclarationBody":
							return "true";
						case "ShowMethodDeclarationBody":
							return membersBody ? "true" : "false";
					}

					return "false";
				}
			}
		}

		private class VisibilityConfiguration : IVisibilityConfiguration
		{
			private readonly bool publicOnly;

			public VisibilityConfiguration(bool publicOnly)
			{
				this.publicOnly = publicOnly;
			}

			public bool Public { get { return true; } }
			public bool Private { get { return !publicOnly; } }
			public bool Family { get { return !publicOnly; } }
			public bool Assembly { get { return !publicOnly; } }
			public bool FamilyAndAssembly { get { return !publicOnly; } }
			public bool FamilyOrAssembly { get { return !publicOnly; } }
		}

		private class AssemblyResolver : IAssemblyResolver
		{
			private IDictionary _assemblyTable;
			private IAssemblyResolver _assemblyResolver;

			public AssemblyResolver(IAssemblyResolver assemblyResolver)
			{
				_assemblyTable = new Hashtable();
				_assemblyResolver = assemblyResolver;
			}

			public IAssembly Resolve(IAssemblyReference assemblyName, string localPath)
			{
				if (_assemblyTable.Contains(assemblyName))
				{
					return (IAssembly) _assemblyTable[assemblyName];
				}
				
				IAssembly assembly = _assemblyResolver.Resolve(assemblyName, localPath);

				_assemblyTable.Add(assemblyName, assembly);

				return assembly;
			}
		}

	}
}
