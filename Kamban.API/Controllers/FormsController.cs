using Kamban.API.Data.Forms;
using Kamban.API.Trust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Kamban.API.Controllers
{
    [Authorize]
    [EnableCors(origins: "http://localhost:4200", headers: "*", methods: "*")]
    public class FormsController : ApiController
    {
        private ITrustRepository _repo;

        public FormsController(ITrustRepository repo)
        {
            _repo = repo;
        }

        #region GET Api
        public IEnumerable<Form> Get()
        {
            return _repo.GetAllForm(User.Identity.Name);
        }

        public Form Get(string id)
        {
            return _repo.GetFormByFormId(User.Identity.Name, id);
        }
        #endregion

        #region POST Api
        public IHttpActionResult Post([FromBody]Form value)
        {
            value.UserId = User.Identity.Name;
            if (value.Id == null) value.Id = Guid.NewGuid().ToString();
            if (value.fields != null)
            {
                foreach (var field in value.fields)
                {
                    if (field.Id == null) field.Id = Guid.NewGuid().ToString();
                }
            }

            if (_repo.AddNewForm(value) &&
            _repo.Save())
                return Ok();
            return null;
        }
        #endregion

        #region
        public IHttpActionResult Patch([FromBody]Form value)
        {
            if (value == null) return BadRequest("Form value cannot be null");

            foreach(var field in value.fields)
            {
                if (field.FormId == null)
                    field.FormId = value.Id;
               
            }

            if (_repo.UpdateForm(User.Identity.Name, value) &&
                _repo.Save())
                return Ok();
            return null;
        }

        #endregion

        [HttpDelete]
        public IHttpActionResult Delete(string id)
        {
            // Delete the entire Form
            if (_repo.DeleteFrom(User.Identity.Name, id) &&
            _repo.Save())
                return Ok();
            return null;
        }

        [HttpDelete]
        public IHttpActionResult Delete(string id, string formFieldId)
        {
            // Delete the specific form field in the form.
            if (_repo.DeleteFormField(User.Identity.Name, id, formFieldId) &&
            _repo.Save())
                return Ok();
            return null;
        }
    }
}
