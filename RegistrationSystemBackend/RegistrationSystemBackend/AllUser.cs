//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RegistrationSystemBackend
{
    using System;
    using System.Collections.Generic;
    
    public partial class AllUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public System.DateTime BirthDate { get; set; }
        public string Password { get; set; }
        public System.Guid ID {     get; set; }
        public bool isAdmin { get; set; }
        public bool isApproved { get; set; }
    }
}
