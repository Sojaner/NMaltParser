using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NMaltParser.Utilities;

namespace NMaltParser.Core.LW.Parser
{
    /// <summary>
	/// @author Johan Hall
	/// 
	/// </summary>
	public sealed class McoModel
    {
        private readonly IDictionary<string, Url> nameUrlMap;

        private readonly IDictionary<string, object> preLoadedObjects;

        private readonly IDictionary<string, string> preLoadedStrings;

        private readonly Url infoUrl;

        public McoModel(Url mcoUrl)
        {
            McoUrl = mcoUrl;

            nameUrlMap = new ConcurrentDictionary<string, Url>();

            preLoadedObjects = new ConcurrentDictionary<string, object>();

            preLoadedStrings = new ConcurrentDictionary<string, string>();

            Url tmpInfoUrl = null;

            string tmpInternalMcoName = null;

            try
            {
                ZipArchive zipArchive = new ZipArchive(McoUrl.OpenStream());

                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                {
                    string fileName = zipArchiveEntry.FullName;

                    Url entryUrl = new Url($"jar:{McoUrl}!/{fileName}\n");

                    int index = fileName.IndexOf('/');

                    if (index == -1)
                    {
                        index = fileName.IndexOf('\\');
                    }

                    nameUrlMap[fileName.Substring(index + 1)] = entryUrl;

                    if (fileName.EndsWith(".info", StringComparison.OrdinalIgnoreCase) && tmpInfoUrl == null)
                    {
                        tmpInfoUrl = entryUrl;
                    }
                    else if (fileName.EndsWith(".moo", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".map", StringComparison.OrdinalIgnoreCase))
                    {
                        preLoadedObjects[fileName.Substring(index + 1)] = PreLoadObject(entryUrl.OpenStream());
                    }
                    else if (fileName.EndsWith(".dsm", StringComparison.OrdinalIgnoreCase))
                    {
                        preLoadedStrings[fileName.Substring(index + 1)] = PreLoadString(entryUrl.OpenStream());
                    }
                    if (tmpInternalMcoName is null)
                    {
                        tmpInternalMcoName = fileName.Substring(0, index);
                    }
                }

                zipArchive.Dispose();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);

                Console.Write(e.StackTrace);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);

                Console.Write(e.StackTrace);
            }

            InternalName = tmpInternalMcoName;

            infoUrl = tmpInfoUrl;
        }

        private static object PreLoadObject(Stream stream)
        {
            object obj;

            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                obj = formatter.Deserialize(stream);
            }
            finally
            {
                stream.Close();
            }

            return obj;
        }

        private static string PreLoadString(Stream stream)
        {
            StringBuilder stringBuilder = new StringBuilder();

            using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    stringBuilder.Append(line);

                    stringBuilder.Append('\n');
                }
            }

            return stringBuilder.ToString();
        }

        public Stream GetInputStream(string fileName)
        {
            return nameUrlMap[fileName].OpenStream();
        }

        public StreamReader GetInputStreamReader(string fileName, string charSet)
        {
            return new StreamReader(GetInputStream(fileName), Encoding.GetEncoding(charSet));
        }

        public Url GetMcoEntryUrl(string fileName)
        {
            return nameUrlMap[fileName];
        }

        public Url McoUrl { get; }

        public object GetMcoEntryObject(string fileName)
        {
            return preLoadedObjects[fileName];
        }

        private Lazy<HashSet<string>> mcoEntryObjectKeys;

        public ISet<string> McoEntryObjectKeys
        {
            get
            {
                if (mcoEntryObjectKeys == null)
                {
                    mcoEntryObjectKeys = new Lazy<HashSet<string>>(() => new HashSet<string>(preLoadedObjects.Keys), true);
                }

                return mcoEntryObjectKeys.Value;
            }
        }

        public string GetMcoEntryString(string fileName)
        {
            return preLoadedStrings[fileName];
        }

        public string InternalName { get; }

        public string McoUrlString => McoUrl.ToString();

        private Lazy<string> mcoInfo;

        public string McoInfo
        {
            get
            {
                if (mcoInfo == null)
                {
                    mcoInfo = new Lazy<string>(() =>
                    {
                        StringBuilder stringBuilder = new StringBuilder();

                        using (StreamReader reader = new StreamReader(infoUrl.OpenStream(), Encoding.UTF8))
                        {
                            string line;

                            while ((line = reader.ReadLine()) != null)
                            {
                                stringBuilder.Append(line);

                                stringBuilder.Append('\n');
                            }
                        }

                        return stringBuilder.ToString();

                    }, true);
                }

                return mcoInfo.Value;
            }
        }
    }
}