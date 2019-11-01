using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace YAHCMS.BlogService.Models 
{
    public class Blog 
    {
        [Required]
        public long ID {get; set;}
        [Required]
        public long UserID {get; set;}
        public string Name {get; set;}
        public string Description {get; set;}
        public DateTime Published {get; set;}
        public DateTime Updated {get; set;}
        public string Language {get; set;}
        public string Country {get; set;}
        public ICollection<Post> Posts {get; set;}

        public Blog() {}
        public Blog(long userID)
        {
            this.UserID = userID;
            Posts = new List<Post>();
            //init dates
            Published = DateTime.Now;
            Updated = DateTime.Now;
        }

        public Blog(long userID, String name, String description) : this(userID)
        {
            this.Name = name;
            this.Description = description;
        }

        public Blog(long userID, String name, String description, 
            String language, String country) : this(userID, name, description)
        {
            this.Language = language;
            this.Country = country;
        }


    }
}