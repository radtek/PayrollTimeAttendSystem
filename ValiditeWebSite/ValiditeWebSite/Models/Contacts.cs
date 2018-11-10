using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ValiditeWebSite.Models
{
    public class Contact
    {
        public string ContactPerson1 { get; set; }
        public string ContactPerson1Phone { get; set; }
        public string ContactPerson2 { get; set; }
        public string ContactPerson2Phone { get; set; }
        public string ContactPerson3 { get; set; }
        public string ContactPerson3Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public string ReturnMessage { get; set; }
    }
}