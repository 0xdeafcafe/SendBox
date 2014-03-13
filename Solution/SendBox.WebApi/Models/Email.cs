namespace SendBox.WebApi.Models
{
	/// <summary>
	/// A model with the data format of the Inbound Parse API's POST
	/// </summary>
	public class Email
	{
		/// <summary>
		/// 
		/// </summary>
		public string Dkim { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string To { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Html { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string From { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SenderIp { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string SpamReport { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Envelope { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int Attachments { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Subject { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public double SpamScore { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Charsets { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Spf { get; set; }
	}
}