using System;
using System.Collections.Generic;
using System.Text;

namespace DNVGL.Veracity.Services.Api.Models
{
    public class ProfilePicture
    {
        public byte[] Image { get; set; }
        public string MimeType { get; set; }

        public string AsBase64Image()
        {
            return $"data:{MimeType};base64,{Convert.ToBase64String(Image)}";
        }      
    }
}
