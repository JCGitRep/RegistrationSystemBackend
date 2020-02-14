using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace RegistrationSystemBackend.Controllers
{


    [EnableCors(origins: "*", headers: "*", methods: "*")]

    public class AdminController : ApiController
    {
        private UsersEntities db = new UsersEntities();
        private List<AllUser> Users = new List<AllUser>();
        private List<string> IDList = new List<string>();
        public AdminController()
        {
            // Adding User IDs to IDList
            foreach (AllUser user in db.AllUsers)
            {
                IDList.Add(user.ID.ToString());
            }
        }

        // Check if the User is Admin or Not 
        private string UserCheckRole(string id)
        {
            var RequestUser = db.AllUsers.FirstOrDefault(e => e.ID.ToString() == id);
            return RequestUser.isAdmin.ToString();
        }

        // Check if The entered Email to Register is already present in database 
        public string checkEmail(string email)
        {
       
            string duplicate="";
            
                foreach (AllUser user in db.AllUsers)
                {
                    if (user.Email == email)
                    {
                        duplicate =  "true";
                    }
                    
                }
            return duplicate;
            }



        // function for admin User
        // retreives Users from database if they are not admin and not approved yet
        // Check for User authentication to  determin if user is Admin or Not
        // if User is admin retreive Users
        [Route("api/Admin/GetUsers")]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            var re = Request;
            var headers = re.Headers;
            var list = headers.ToList();
            try
            {
                var elem = (from item in list where item.Key == "UserAuth" select item.Value).ToList()[0].First();

                if (elem != null)
                {
                    if (IDList.Contains(elem))
                    {
                        if (UserCheckRole(elem) == "True")
                        {
                            try
                            {
                                var result = from R in db.AllUsers
                                             where R.isAdmin.ToString() == "False" & R.isApproved.ToString() == "False"
                                             select R;
                                Users = result.ToList();
                                return Ok(result);
                            }
                            catch (Exception)
                            {
                                //If any exception occurs Internal Server Error i.e. Status Code 500 will be returned  
                                return InternalServerError();
                            }

                        }
                        else
                        {
                            return ResponseMessage(Request.CreateResponse(HttpStatusCode.Forbidden));
                        }
                    }

                    else
                    {
                        return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));
                    }
                }
                else
                {
                    return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound));
                }
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            }
        }

        // function for admin to approve users or not
        // if the user is approved, the attribute isApproved will be changed to true and the user can now access his page
        // if the user is not approved, the user will be removed from database

        [HttpPatch]
        [Route("api/Admin/EditUser/{id}")]

        public IHttpActionResult EditUser(string id)
        {
            var re = Request;
            var header = re.Headers;
            var list = header.ToList();
            try
            {
                var elem = (from item in list where item.Key == "UserAuth" select item.Value).ToList()[0].First();
                var auth = elem.First();
                if (auth != null)
                {
                    if (IDList.Contains(elem))
                    {
                        if (UserCheckRole(elem) == "True")
                        {
                            var content = re.Content;
                            string jsonContent = content.ReadAsStringAsync().Result;

                            var Approve = (from item in list where item.Key == "Approve" select item.Value).ToList()[0].First();
                            var RequestToEdit = db.AllUsers.FirstOrDefault(e => e.ID.ToString() == id);
                            if (Approve == "yes")
                            {
                                RequestToEdit.isApproved = true;
                                db.SaveChangesAsync();
                                return (Ok());
                            }
                            else
                            {
                                RequestToEdit.isApproved = false;
                                db.AllUsers.Remove(RequestToEdit);
                                db.SaveChangesAsync();
                                return (Ok());
                            }
                        }
                        else
                        {
                            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Forbidden));

                        }

                    }
                    else
                    {
                        return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Unauthorized));

                    }
                }
                else
                {
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NotFound));

                }
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            }
        }


        // function for admin AND on login page to create a new Admin/User because logically only admins can create admins
        //  checks if the email entered is already used or not
        //  if an admin is creating another admin, the attributes isAdmin and isApproved are set to True
        //  if a user is creating an account, the attributes isAdmin and isApproved are set to false
        //  the function returns in addition to the added user the parameter isDuplicate because it is used in front end

        [HttpPost]
        [Route("api/Admin/addAdmin/{user}")]

        public IHttpActionResult AddUser(AllUser user)
        {
            var re = Request;
            var header = re.Headers;
            var list = header.ToList();
            if(checkEmail(user.Email) == "true")
            {
                Dictionary<string, string> ret = new Dictionary<string, string>();
                ret["isDuplicate"] = "true";
                return Ok(ret);
            }
        

            try
            {
                var elem = (from item in list where item.Key == "UserAuth" select item.Value).ToList()[0].First();
                var oid = (from item in list where item.Key == "oid" select item.Value).ToList()[0].First();

                var auth = elem.First();
                if (auth != null)
                {
                    if (IDList.Contains(elem))
                    {
                        if (UserCheckRole(elem) == "True")
                        {
                            var content = re.Content;
                            string jsonContent = content.ReadAsStringAsync().Result;

                            AllUser NewAdminUser = new AllUser();
                            NewAdminUser.ID = System.Guid.NewGuid();
                            NewAdminUser.Name = user.Name;
                            NewAdminUser.LastName = user.LastName;
                            NewAdminUser.Email = user.Email;
                            NewAdminUser.Password = user.Password;
                            NewAdminUser.isAdmin = true;
                            NewAdminUser.isApproved = true;

                            db.AllUsers.Add(NewAdminUser);
                            db.SaveChangesAsync();
                            Dictionary<string, string> ret = new Dictionary<string, string>();
                            ret["isDuplicate"] = "false";
                            return Ok(ret);

                        }
                        else
                        {
                            return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Forbidden));

                        }

                    }
                    else
                    {
                        AllUser NewAdminUser = new AllUser();
                        NewAdminUser.ID = System.Guid.NewGuid();
                        NewAdminUser.Name = user.Name;
                        NewAdminUser.LastName = user.LastName;
                        NewAdminUser.Email = user.Email;
                        NewAdminUser.Password = user.Password;
                        NewAdminUser.isAdmin = false;
                        NewAdminUser.isApproved = false;

                        db.AllUsers.Add(NewAdminUser);
                        db.SaveChangesAsync();
                        Dictionary<string, string> ret = new Dictionary<string, string>();
                        ret["isDuplicate"] = "false";
                        return Ok(ret);
                    }
                }
                else
                {
                    return ResponseMessage(new HttpResponseMessage(HttpStatusCode.NotFound));

                }
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Unauthorized));

            }
        }
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
    }
}
