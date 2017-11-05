using EmployeeDataAccess;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPIDemo.Controllers
{
    public class EmployeesController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage VracanjeSvih(string gender="All")
        {
            using (EmployeeDBEntities entites = new EmployeeDBEntities())
            {
                //postavljanje opcionalnih parametara u GET metodi
                switch (gender.ToLower())
                {
                    case "all":
                        return Request.CreateResponse(HttpStatusCode.OK, entites.Employees.ToList());
                    case "male":
                        return Request.CreateResponse(HttpStatusCode.OK, entites.Employees
                            .Where(a => a.Gender.ToLower() == "male")
                            .ToList());
                    case "female":
                        return Request.CreateResponse(HttpStatusCode.OK, entites.Employees
                            .Where(a => a.Gender.ToLower() == "female")
                            .ToList());
                    default:
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                            "Value for fender must be All, or Male or Female." + " is invalid");
                }
            }
        }

        //postavljanje GET
        [HttpGet]
        public HttpResponseMessage VracanjeJednog(int id)
        {
            using (EmployeeDBEntities entites = new EmployeeDBEntities())
            {
                var entity = entites.Employees.FirstOrDefault(e => e.ID == id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                        "Employee with Id = " + id.ToString() + " not found");
                }
            }
        }

        //postavljanje POST

        //FromBody atribut kaže da će podaci biti doveni iz request bodya
        public HttpResponseMessage Post([FromBody]Employees employee)
        {
            try
            {
                using (EmployeeDBEntities entites = new EmployeeDBEntities())
                {
                    entites.Employees.Add(employee);
                    entites.SaveChanges();

                    //da dobijemo status kod 201 za Rest kovenciju
                    var message = Request.CreateResponse(HttpStatusCode.Created, employee);
                    message.Headers.Location = new Uri(Request.RequestUri + employee.ID.ToString());

                    return message;
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        //postavljanje DELETE

        public HttpResponseMessage Delete(int id)
        {

            try
            {
                using (EmployeeDBEntities entites = new EmployeeDBEntities())
                {
                    var entity = entites.Employees.FirstOrDefault(e => e.ID == id);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                            "Employee with Id = " + id.ToString() + " not found to delete");
                    }
                    {
                        entites.Employees.Remove(entity);
                        entites.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }

                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }

        }

        //postavljanje PUT
        //trebamo staviti status code 200
        //ako nema zaposlenika ne smije biti status 500 internal server error nego 404
        public HttpResponseMessage Put(int id, [FromBody]Employees employees)
        {
            using (EmployeeDBEntities entites = new EmployeeDBEntities())
            {
                try
                {
                    var entity = entites.Employees.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with Id " + id.ToString() + " not found to update");
                    else
                    {
                        entity.FirstName = employees.FirstName;
                        entity.LastName = employees.LastName;
                        entity.Gender = employees.Gender;
                        entity.Salary = employees.Salary;

                        entites.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, entity);
                    }
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
                }

            }
        }

    }
}

