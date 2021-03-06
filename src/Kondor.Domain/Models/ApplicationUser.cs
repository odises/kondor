using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kondor.Domain.Models
{
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string TelegramUsername { get; set; }
        public int TelegramUserId { get; set; }
        //public int WelcomeMessageId { get; set; }
        public Guid LanguageId { get; set; }
        public virtual Language Language { get; set; }
        public virtual ICollection<CardState> UserCards { get; set; }
        public virtual ICollection<ExampleView> UserExampleViews { get; set; }
    }
}