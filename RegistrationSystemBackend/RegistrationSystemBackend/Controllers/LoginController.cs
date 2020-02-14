using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RegistrationSystemBackend.Controllers
{
    public class LoginController : ApiController
    {
        private UsersEntities User_Entities = new UsersEntities();

        // Request to check User credentials 
        // Reads request and conversts content to json
        // returns User Name,ID,isAdmin and isApproved

        [Route("api/login/CheckCredentials")]
        [HttpPost]

        public IHttpActionResult CheckCredentials()
        {
            try
            {
                var re = Request;
                var content = re.Content;
                string jsonContent = content.ReadAsStringAsync().Result;

                   var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);
                var a = json["Email"];
                var b = json["Password"];
                var UserToBeChecked = User_Entities.AllUsers.FirstOrDefault(e => e.Email == a && e.Password == b);
                if (UserToBeChecked != null)
                {
                    var isAdmin = UserToBeChecked.isAdmin;
                    var ID = UserToBeChecked.ID;
                    var Name = UserToBeChecked.Name;
                    var Email = UserToBeChecked.Email;
                    var isApproved = UserToBeChecked.isApproved;
                    Dictionary<string, string> ret = new Dictionary<string, string>();
                    ret["isAdmin"] = isAdmin.ToString();
                    ret["ID"] = ID.ToString();
                    ret["Name"] = Name;
                    ret["Email"] = Email;
                    ret["isApproved"] = isApproved.ToString();

                    // string json = JsonConvert.SerializeObject(ret);
                    return Ok(ret);
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));
                }

            }
            catch (Exception)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest));

            }

        }

        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
    }

        }

