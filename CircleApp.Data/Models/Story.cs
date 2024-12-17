﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircleApp.Data.Models
{
    public class Story
    {
        public int Id { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime DateCreated { get; set; }
        public bool isDeleted { get; set; }

        //Foreign key
        public int UserId { get; set; }

        //Navigation Properties
        public User User { get; set; }
 
    }
}