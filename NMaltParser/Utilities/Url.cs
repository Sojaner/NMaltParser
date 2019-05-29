using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace NMaltParser.Utilities
{
    public class Url:IDisposable
    {
        private readonly bool isBundle;

        private readonly string entryPath;

        private readonly Uri uri;

        private readonly FileInfo fileInfo;

        private readonly List<WebClient> webClients = new List<WebClient>();

        private readonly List<Stream> streams = new List<Stream>();

        public Url(string url)
        {
            if (url.StartsWith("zip:", StringComparison.OrdinalIgnoreCase) || url.StartsWith("jar:", StringComparison.OrdinalIgnoreCase))
            {
                isBundle = true;

                url = url.Substring(4);

                string[] parts = url.Split("!/");

                url = parts[0];

                entryPath = parts[1].Split("\n")[0];
            }

            try
            {
                uri = new Uri(url);

                if (uri.IsFile)
                {
                    fileInfo = new FileInfo(url);
                }
            }
            catch
            {
                fileInfo = new FileInfo(url);
            }

            if (fileInfo != null && !fileInfo.Exists)
            {
                throw new IOException($"Cannot find the file {fileInfo.FullName}.");
            }
        }

        public string AbsoluteUrl
        {
            get
            {
                if (fileInfo != null)
                {
                    return fileInfo.FullName;
                }
                else if(uri != null)
                {
                    return uri.AbsoluteUri;
                }
                else
                {
                    return null;
                }
            }
        }

        public Stream OpenStream()
        {
            if (fileInfo != null)
            {
                Stream stream = fileInfo.OpenRead();

                streams.Add(stream);

                if (isBundle)
                {
                    ZipArchive zipArchive = new ZipArchive(stream);

                    Stream entryStream = zipArchive.GetEntry(entryPath)?.Open();

                    streams.Add(entryStream);

                    return entryStream;
                }

                return stream;
            }
            else if (uri != null)
            {
                WebClient client = new WebClient();

                webClients.Add(client);

                Stream stream = client.OpenRead(uri);

                streams.Add(stream);

                if (isBundle && stream != null)
                {
                    ZipArchive zipArchive = new ZipArchive(stream);

                    Stream entryStream = zipArchive.GetEntry(entryPath)?.Open();

                    streams.Add(entryStream);

                    return entryStream;
                }

                return stream;
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return AbsoluteUrl;
        }

        public void Dispose()
        {
            webClients.ForEach(client => client?.Dispose());

            streams.ForEach(stream => stream?.Dispose());
        }

        public static Url ToUrl(string url)
        {
            try
            {
                return new Url(url);
            }
            catch
            {
                return null;
            }
        }
    }
}