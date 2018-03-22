using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using RestSharp;
using ToDoApp.Contract;
using ToDoApp.WebSite.Filters;

namespace ToDoApp.WebSite.Controllers
{
    [SessionAuthorize]
    public class ToDoListController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public JsonResult Get()
        {
            var request = new RestRequest("todolists", Method.GET);
            AddAuthHeaders(ref request, HttpMethod.Get.Method, "todolists");

            IRestResponse<List<ToDoListContract>> response = RestClient.Execute<List<ToDoListContract>>(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            return Json(response.Data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Post(ToDoListContract model)
        {
            var userId = (int)Session["usrid"];
            model.UserId =  userId;
            if (ModelState.IsValid)
            {
                var request = new RestRequest("todolists", Method.POST);
                AddAuthHeaders(ref request, HttpMethod.Post.Method, "todolists");
                request.AddJsonBody(model);

                IRestResponse response = RestClient.Execute(request);

                return response.StatusCode != HttpStatusCode.OK ? new HttpStatusCodeResult(HttpStatusCode.InternalServerError) : new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
    }
}