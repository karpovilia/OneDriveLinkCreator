using KoenZomers.OneDrive.Api;
using KoenZomers.OneDrive.Api.Entities;
using KoenZomers.OneDrive.Api.Enums;
using Newtonsoft.Json.Linq;
using System;

namespace OneDriveLinkCreator
{
    class Program
    {
        public static Options options = new Options();
        static void Main(string[] args)
        {
            Console.WriteLine("---");
            if (CommandLine.Parser.Default.ParseArgumentsStrict(args, options))
            {
                var OneDriveApi = new OneDriveGraphApiExtension(options.api_key);
                OneDriveApi.ProxyConfiguration = null;
                OneDriveApi.AuthenticateUsingRefreshToken(options.token).Wait();

                var folderJson = OneDriveApi.GetData<OneDriveItemCollection>(string.Concat("drive/root:", options.path, ":/?expand=children")).Result.OriginalJson;
                dynamic folderData = JObject.Parse(folderJson);
                System.Collections.Generic.List<String> links = new System.Collections.Generic.List<string>();
                foreach (var obj in folderData.children)
                {
                    try
                    {
                        var shareJson = OneDriveApi.ShareItemModified(obj.id.ToString(), OneDriveLinkType.View, OneDriveSharingScope.Anonymous).Result.OriginalJson;
                        dynamic share_data = JObject.Parse(shareJson);
                        string sharingUrl = share_data.link.webUrl;
                        string fileName = obj.name;

                        string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sharingUrl));
                        string encodedUrl = "u!" + base64Value.TrimEnd('=').Replace('/', '_').Replace('+', '-');

                        string resultUrl = string.Format("https://api.onedrive.com/v1.0/shares/{0}/root/content", encodedUrl);

                        links.Add(string.Concat("[", fileName, "](", resultUrl, ")"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }

                foreach (var link in links)
                {
                    Console.WriteLine(link);
                }

                Console.ReadLine();
            }
        }
    }
}
