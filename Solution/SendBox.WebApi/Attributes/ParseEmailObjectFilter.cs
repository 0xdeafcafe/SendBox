using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using SendBox.WebApi.Models;

namespace SendBox.WebApi.Attributes
{
	public class ParseEmailObjectFilter : ActionFilterAttribute
	{
		/// <summary>
		/// Parses the raw form-data of the POST into a snazzy email 
		/// model, and then sets it in the arguments for the POST function
		/// to read.
		/// </summary>
		/// <param name="actionContext">The HTTP action context of the POST.</param>
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var data = HttpContext.Current.Request.Form;
			var email = new Email
			{
				Dkim = data["dkim"],
				To = data["to"],
				Html = data["html"],
				From = data["from"],
				Text = data["text"],
				SenderIp = data["sender_ip"],
				SpamReport = data["spam_report"],
				Envelope = data["envelope"],
				Attachments = int.Parse(data["attachments"]),
				Subject = data["subject"],
				SpamScore = double.Parse(data["spam_score"]),
				Charsets = data["charsets"],
				Spf = data["SPF"]
			};

			actionContext.ActionArguments["email"] = email;

			base.OnActionExecuting(actionContext);
		}
	}
}
