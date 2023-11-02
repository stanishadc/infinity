namespace InfinityWeb.Helpers
{
    public class DataConversions
    {
        public DateTime ConvertUTCtoLocal(DateTime dateTime)
        {
            DateTime iKnowThisIsUtc = dateTime;
            DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                iKnowThisIsUtc,
                DateTimeKind.Utc);
            DateTime localVersion = runtimeKnowsThisIsUtc.ToLocalTime();
            return localVersion;
        }
    }
}
