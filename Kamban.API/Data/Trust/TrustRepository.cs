using Kamban.API.Contacts;
using Kamban.API.Data.Forms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Kamban.API.Trust
{
    public class TrustRepository : ITrustRepository
    {
        #region Fields
        TrustContext _ctx;
        #endregion

        #region Constructor
        public TrustRepository(TrustContext ctx)
        {
            _ctx = ctx;
        }
        #endregion

        #region Public Methods
        public bool AddNewTrust(Trust trust)
        {
            try
            {
                _ctx.Trusts.Add(trust);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public IQueryable<Trust> GetAllTrusts()
        {
            try
            {
                return _ctx.Trusts;
            }
            catch
            {
                return null;
            }
        }

        public IQueryable<Groups> GetAllGroups(string trustUserName)
        {
            try
            {
                return _ctx.Groups.Where(x => x.TrustUserName == trustUserName);
            }
            catch
            {
                return null;
            }
        }


        public Trust GetTrustByUserName(string userName)
        {
            try
            {
                return _ctx.Trusts
                            .Where(x => x.UserName == userName)
                            .First();
            }
            catch
            {
                return null;
            }
        }

        public bool AddNewContactToTrust(string trustUserName, Contact newContact)
        {
            try
            {
                newContact.TrustUserName = trustUserName;
                _ctx.Contacts.Add(newContact);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddNewGroup(string trustUserName, string groupName, List<int> userIds)
        {
            try
            {
                _ctx.Groups.Add(new Contacts.Groups()
                {
                    TrustUserName = trustUserName,
                    Name = groupName,
                    UserIds = userIds
                });
                return true;
            }
            catch
            {
                return false;
            }

        }
        public bool AddNewMemberToTheGroup(string trustUserName, string groupName, int userId)
        {
            try
            {
                var group = _ctx.Groups
                                .Where(x => x.TrustUserName == trustUserName)
                                .Where(x => x.Name == groupName)
                                .First();
                group.UserIds.Add(userId);
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool RemoveMemberFromTheGroup(string trustUserName, string groupName, int userId)
        {
            try
            {
                var group = _ctx.Groups
                                .Where(x => x.TrustUserName == trustUserName)
                                .Where(x => x.Name == groupName)
                                .First();
                group.UserIds.Remove(userId);
                return true;
            }
            catch
            {
                return false;
            }

        }


        #region Forms
        public bool AddNewForm(Form form)
        {
            try
            {
                _ctx.Forms.Add(form);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateForm(string userId, Form form)
        {
            try
            {
                foreach (var field in form.fields)
                {
                    if (string.IsNullOrEmpty(field.Id))
                    {
                        field.Id = Guid.NewGuid().ToString();
                        _ctx.FormFields.Add(field);
                    }
                    else
                        _ctx.Entry(field).State = System.Data.Entity.EntityState.Modified;
                }
                form.fields = null;
                _ctx.Entry(form).State = System.Data.Entity.EntityState.Modified;

                //var value = _ctx.Forms.Where(x => x.Id == form.Id && x.UserId == userId).First();
                //_ctx.Forms.Remove(value);
                //_ctx.Forms.Add(form);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Form GetFormByFormId(string userId, string id)
        {
            try
            {
                return _ctx.Forms.Where(x => x.Id == id)
                                .Where(x => x.UserId == userId)
                                .Include(x => x.fields)
                                .First();
            }
            catch
            {
                return null;
            }
        }

        public IQueryable<Form> GetAllForm(string userId)
        {
            try
            {
                return _ctx.Forms
                            .Where(x => x.UserId == userId);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool DeleteFrom(string userId, string formId)
        {
            try
            {
                Form _form = GetFormByFormId(userId, formId);

                List<FormFields> _formFields = new List<FormFields>();

                foreach (var field in _form.fields)
                {
                    _formFields.Add(field);
                }
                foreach (var field in _formFields)
                {
                    _ctx.Entry(field).State = EntityState.Deleted;
                }
                var form = _ctx.Forms.Where(x=>x.Id ==formId && x.UserId == userId).FirstOrDefault<Form>();
                
                _ctx.Entry(form).State = EntityState.Deleted;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteFormField(string userId, string formId, string formFieldId)
        {
            try
            {
                var formField = _ctx.FormFields.Where(x => x.Id == formFieldId && x.FormId == formId).FirstOrDefault<FormFields>();

                _ctx.Entry(formField).State = EntityState.Deleted;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        public bool Save()
        {
            try
            {
                _ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion
    }
}