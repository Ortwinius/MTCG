using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models.Users.DTOs
{
    public class UserDataDTO
    {
        public string? Name { get; set; }
        public string? Bio { get; set; }
        public string? Image { get; set; }

        public UserDataDTO(string? name, string? bio, string? image)
        {
            Name = name;
            Bio = bio;
            Image = image;
        }
    }
}
