namespace Flex.Entity.Api.Helpers
{
    public class AppMetadata
    {
        private Asset _assetPayload;
        private Service _servicePayload;
        private string _emailId;

        public AppMetadata(string emailid, Asset assetPayload, Service servicePayload)
        {
            _assetPayload = assetPayload;
            _servicePayload = servicePayload;
            _emailId = emailid;
        }

        public string EmailId
        {
            get { return _emailId; }
            set { _emailId = value; }
        }

        public Asset AssetPayload
        {
            get { return _assetPayload; }
            set { _assetPayload = value; }
        }

        public Service ServicePayload
        {
            get { return _servicePayload; }
            set { _servicePayload = value; }
        }
    }
}