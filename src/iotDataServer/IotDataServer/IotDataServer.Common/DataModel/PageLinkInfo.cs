namespace IotDataServer.Common.DataModel
{
    public class PageLinkInfo
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Memo { get; set; }

        public PageLinkInfo(string title, string url, string memo="�۾���")
        {
            Title = title;
            Url = url;
            Memo = memo;
        }
    }
}