using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Reflection;

namespace QREST
{
    public class CustomPasswordValidator : PasswordValidator
    {
        public int MaxLength { get; set; }
        public int RequiredUniqueChars { get; set; }

        public override async Task<IdentityResult> ValidateAsync(string item)
        {
            IdentityResult result = await base.ValidateAsync(item);

            var errors = result.Errors.ToList();

            //Max Length Checking
            if (string.IsNullOrEmpty(item) || item.Length > MaxLength)
            {
                errors.Add(string.Format("Password length can't exceed {0}", MaxLength));
            }


            //Required Unique Characters Checking
            if (!string.IsNullOrEmpty(item))
            {
                var countUnique = item.Distinct().Count();
                if (countUnique < RequiredUniqueChars)
                {
                    errors.Add(string.Format("Password must have {0} unique characters.", RequiredUniqueChars));
                }
            }


            //Check against common password list
            HashSet<string> hashset;
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("PasswordCommon.txt"));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                    hashset = new HashSet<string>(GetLines(reader), StringComparer.OrdinalIgnoreCase);
            }

            if (!string.IsNullOrEmpty(item) && hashset.Contains(item))
            {
                errors.Add("This password is too common. Please choose a more complex password.");
            }


            //Return any password validation errors
            return await Task.FromResult(!errors.Any() ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray()));
        }


        private static IEnumerable<string> GetLines(StreamReader reader)
        {
            while (!reader.EndOfStream)
                yield return reader.ReadLine();
        }


    }
}