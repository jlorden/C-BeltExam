using System;
using System.ComponentModel.DataAnnotations;

namespace BeltExam.Models 
{

    public class RegexAttribute : ValidationAttribute 
    {
        public bool stringcheck (string input) 
        {
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach (var item in specialChar) 
            {
                for (int i = 0; i < 30; i++)
                {
                    Console.WriteLine ("this", specialChar);
                }
                if (input.Contains (item)) return true;
            }

            return false;
        }

        public bool numCheck (string input) 
        {
            string numChar = @"0123456789";
            foreach (var item in numChar) 
            {
                if (input.Contains (item)) return true;
            }

            return false;
        }
        public bool upperCheck (string input) 
        {
            string upChar = @"QWERTYUIOPASDFGHJKLZXCVBNM";
            foreach (var item in upChar) 
            {
                if (input.Contains (item)) return true;
            }
            return false;
        }
        protected override ValidationResult IsValid (object value, ValidationContext validationContext) {
            if (value is string) 
            {
                string check = (string) value;
                if (stringcheck (check) == true && numCheck(check) == true && upperCheck(check) == true) {
                    return ValidationResult.Success;
                } 
                else 
                {
                    return new ValidationResult ("Invalid password");
                }
            }
            return new ValidationResult ("Value is not a string(how did you manage that?)");
        }
    }
}