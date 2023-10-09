using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace CC.Enum.Editor
{
    /// Author: LinkCC
    /// Created: 10/9/23
    /// Last Edited: 10/9/23
    /// <summary>
    /// Create a Enumerated type off inputed string names.
    /// </summary>
    /// Helpful Link : https://discussions.unity.com/t/can-i-add-an-enum-value-in-the-inspector/196544/5
    public static class EnumEditorHandler
    {
        public const string STANDARD_NAMESPACE = "CC.Enum";
        const char NAMESPACE_DELIMITER = '.';

        static readonly string path = Directory.GetParent(FileLocation.Directory).FullName + Path.AltDirectorySeparatorChar +"Enums" + Path.AltDirectorySeparatorChar;
        const string EXT = ".cs";


        /// <summary>
        /// Saves the Enum to a file within the Enum folder. NOTE: Will overwrite Enum if passed an fileName that is the same as another enumFile
        /// </summary>
        /// <param name="fileName"> The Name of the Enumerated Type to be created.</param>
        /// <param name="enumNames"> The Values of the Enumerated Type.</param>
        public static void WriteEnumFile(string fileName, ICollection<string> enumNames, string enumNamespace)
        {
            
            ValidateInput(fileName, enumNames, enumNamespace);

            string fileContent = "namespace " + enumNamespace + " {public enum " + fileName + "{ ";
            foreach (string enumName in enumNames)
            {
                fileContent += enumName + ", ";
            }
            fileContent += " };}";

            WriteFile(fileName, fileContent);
        }


        /// <summary>
        /// Write fileContent into a file with name fileName within the Enum Folder and import data. NOTE: No check to see if file exists so it will overwrite
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        public static void WriteFile(string fileName, string fileContent)
        {

            using (StreamWriter file = File.CreateText(path + fileName + EXT))
            {
                file.WriteLine(fileContent);
            }
            AssetDatabase.ImportAsset(GetRelativePath(fileName));
        }


        /// <summary>
        /// Delete Enum file/type.
        /// </summary>
        /// <param name="fileName"> The target file to destroy. Name of file without extension or directory.</param>
        public static void DeleteEnumFile(string fileName)
        {
            if (File.Exists(path + fileName + EXT))
            {
                AssetDatabase.DeleteAsset(GetRelativePath(fileName));
            }
            else
                throw new EnumEditorException("The specified file does not exist");
        }

        /// <summary>
        /// Get path relative to the Application.dataPath. AKA: the asset folder
        /// </summary>
        /// <param name="fileName">File to get path for.</param>
        /// <returns>The relative path of the file from the Asset folder</returns>
        private static string GetRelativePath(string fileName)
        {
            return Path.GetRelativePath(Directory.GetParent(Application.dataPath).FullName, path + fileName + EXT);
        }

        /// <summary>
        /// Get the names for all Enum files.
        /// </summary>
        /// <returns> Names of all the Enum files without directory or extension.</returns>
        public static string[] GetFileNames()
        {
            string[] fileNames = Directory.GetFiles(path, "*"+EXT, SearchOption.TopDirectoryOnly);
            for(int i = 0; i < fileNames.Length; i++) { fileNames[i] = Path.GetFileNameWithoutExtension(fileNames[i]); }
            return fileNames;
        }

        /// <summary>
        /// Assure User inputs are valid for file creation. NOTE: Does not catch keyword or instantiated type conflicts.
        /// </summary>
        /// <param name="fileName"> Name of Enum and File.</param>
        /// <param name="enumNames"> Name of Enumerated type values</param>
        public static void ValidateInput(string fileName, ICollection<string> enumNames, string enumNamespace)
        {

            //Validation functions
            (System.Func<string, bool>, System.Exception)[] validators = new (System.Func<string, bool>, System.Exception)[] 
            { 
                ((string input) => !string.IsNullOrWhiteSpace(input), new EnumEditorException("Names cannot be white-space or null! Make sure the namespace does not begin or end with \'.\'.")),
                ((string input) => char.IsLetter(input[0]), new EnumEditorException("Names must begin with a letter!")),
                ((string input) => input.All(char.IsLetterOrDigit), new EnumEditorException("Names must contain only alphanumeric characters!")),
            };

            //Set up input array
            string[] enumNamespaceTokens = enumNamespace.Split(NAMESPACE_DELIMITER);
            string[] inputs = new string[enumNames.Count + enumNamespaceTokens.Length + 1];//length of enumName values, length of namespace values and one more for the filename
            enumNames.CopyTo(inputs, 0);
            enumNamespaceTokens.CopyTo(inputs, enumNames.Count);
            inputs[^1] = fileName;

            //Test inputs against Validators
            foreach((System.Func<string,bool> validator, System.Exception exception) in validators)
            {
                foreach(string input in inputs)
                {
                    if (!validator(input)) throw exception;
                }
            }

            //Extra case for handling Distinct enum value names
            if (enumNames.Distinct().Count() != enumNames.Count()) throw new EnumEditorException("All Enum Value names should be unique!");
        }

        /// <summary>
        /// Standard Exception for Enum File Creation
        /// </summary>
        [System.Serializable]
        public class EnumEditorException : System.Exception
        {
            public EnumEditorException() { }
            public EnumEditorException(string message) : base(message) { }
            public EnumEditorException(string message, System.Exception inner) : base(message, inner) { }
            protected EnumEditorException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }


        /// <summary>
        /// Provides the compile time location of this File so that a reference may be used to save the Enum files
        /// </summary>
        /// Helpful Link : https://stackoverflow.com/questions/47841441/how-do-i-get-the-path-to-the-current-c-sharp-source-code-file
        private static class FileLocation
        {
            public static string Path => GetThisFilePath().Replace('\\', System.IO.Path.AltDirectorySeparatorChar);
            public static string Directory => System.IO.Path.GetDirectoryName(GetThisFilePath()).Replace('\\',System.IO.Path.AltDirectorySeparatorChar);
            private static string GetThisFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = null)
            {
                return path;
            }
        }
    }


}
