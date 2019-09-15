using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kudvenkat.Utils {
    public class ValidEmailDomainAttribute : ValidationAttribute {
        private readonly string[] _allowedDomain;

        public ValidEmailDomainAttribute(string[] allowedDomain) {
            _allowedDomain = allowedDomain;
            ErrorMessage = "Email domain must be one of the following: " + string.Join(", ", _allowedDomain);
        }

        public override bool IsValid(object value) {
            var emailDomain = value.ToString().Split('@')[1];
            return _allowedDomain.Any(x => x.Equals(emailDomain, StringComparison.OrdinalIgnoreCase));
        }

    }
}
