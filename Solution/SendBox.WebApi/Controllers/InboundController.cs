using System.Net;
using System.Web.Http;
using System.Web.Mvc;
using SendBox.WebApi.Attributes;
using SendBox.WebApi.Models;

namespace SendBox.WebApi.Controllers
{
	public class InboundController : ApiController
	{
		// POST api/inbound
		[ValidateInput(false)]
		[ParseEmailObjectFilter]
		public ActionResult Post()
		{
			if (ActionContext == null || ActionContext.ActionArguments == null || ActionContext.ActionArguments["email"] == null || !ActionContext.ActionArguments.ContainsKey("email"))
				return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);

			var email = ActionContext.ActionArguments["email"] as Email;
			if (email == null) goto end;

			// Check we doin this Halo thing
			if (email.Subject.ToLowerInvariant().Trim() != "halo") goto end;

			// Send the command to the memory manager and parser
			WebApiApplication.XboxMemoryManager.ParseTextCommand(email.Text);

		end:
			return new HttpStatusCodeResult(HttpStatusCode.OK);
		}
	}
}
