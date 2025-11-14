using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.UserDtos
{
    public class UpdateUserDto
    {
        [Required]
        public string FullName { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }
    }
}