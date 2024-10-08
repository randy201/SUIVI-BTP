using System.ComponentModel.DataAnnotations;

namespace temp_back
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        public AllowedExtensionsAttribute(string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var fileExtension = System.IO.Path.GetExtension(file.FileName);
                if (!_allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                {
                    return new ValidationResult($"L'extension de fichier {fileExtension} n'est pas autorisée.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
