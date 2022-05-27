using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class Widget
    {       
        public Guid Id { get; set; }        
        public string Url { get; set; }       
        public string Name { get; set; }     
        public string Description { get; set; }      
        public string LogoUrl { get; set; }       
        public string PictogramUrl { get; set; }      
        public string Type { get; set; }      
        public bool ShowTitle { get; set; }      
        public bool SupportsMobile { get; set; }      
        public Guid ServiceId { get; set; }      
        public int SequenceNo { get; set; }        
        public string Color { get; set; }
    }
}
