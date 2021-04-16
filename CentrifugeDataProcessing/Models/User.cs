namespace CentrifugeDataProcessing.Models
{
	public class User : ProcessingHelper
    {
	    private byte[] _dataBytes;

	    public string Name { get; set; }
	    public string Path { get; set; }
        public int Count { get; set; }
        public User(byte[] dataBytes)
        {
	        _dataBytes = dataBytes;
        }

        public void Prepare()
        {
            FindingInterval(_dataBytes, Name);
            AddPacketToIntervals(_dataBytes);
            Count = Calculate();
            _dataBytes = null;
        }

    }
}
