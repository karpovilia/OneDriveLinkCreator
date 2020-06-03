using KoenZomers.OneDrive.Api;
using KoenZomers.OneDrive.Api.Entities;
using KoenZomers.OneDrive.Api.Enums;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OneDriveLinkCreator
{
    public class OneDriveGraphApiExtension : OneDriveGraphApi
    {
        public OneDriveGraphApiExtension(string applicationId, string clientSecret = null) : base(applicationId, clientSecret)
        {
        }

        public async Task<OneDrivePermission> ShareItemModified(string oneDriveRequestUrl, OneDriveLinkType linkType, OneDriveSharingScope scope)
        {
            OneDriveItem item = new OneDriveItem() { Id = oneDriveRequestUrl };
            return await ShareItem(item, linkType, scope);
        }

        public async Task<T> GetData<T>(string url) where T : OneDriveItemBase
        {
            // Construct the complete URL to call
            var completeUrl = ConstructCompleteUrl(url);

            // Call the OneDrive webservice
            var result = await SendMessageReturnOneDriveItem<T>("", HttpMethod.Get, completeUrl, HttpStatusCode.OK);
            return result;
        }
    }
}
