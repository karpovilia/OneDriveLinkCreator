using KoenZomers.OneDrive.Api;
using KoenZomers.OneDrive.Api.Entities;
using KoenZomers.OneDrive.Api.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

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
                Dictionary<String, String> ids = new Dictionary<string, string>();
                foreach (var obj in folderData.children)
                {
                    String key = obj.id.ToString();
                    String value = obj.name;
                    ids.Add(key, value);
                }
                    

                foreach (var id in ids)
                { 
                    int attempts_count = 3;
                    while (attempts_count > 0)
                    {
                        try
                        {
                            var shareJson = OneDriveApi.ShareItemModified(id.Key, OneDriveLinkType.View, OneDriveSharingScope.Anonymous).Result.OriginalJson;
                            dynamic share_data = JObject.Parse(shareJson);
                            if (share_data.error != null && share_data.error.message.ToString().Equals("Too Many Requests"))
                            {
                                Console.WriteLine(DateTime.Now.ToString() + " " + id + " Too Many Requests");
                                Thread.Sleep(5000);
                                attempts_count -= 1;
                                continue;
                            }
                            string sharingUrl = share_data.link.webUrl;
                            string fileName = id.Value;

                            string base64Value = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sharingUrl));
                            string encodedUrl = "u!" + base64Value.TrimEnd('=').Replace('/', '_').Replace('+', '-');

                            string resultUrl = string.Format("https://api.onedrive.com/v1.0/shares/{0}/root/content", encodedUrl);

                            links.Add(string.Concat("[", fileName, "](", resultUrl, ")"));
                            attempts_count = 0;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
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
