using Attendance.Services.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;

namespace Attendance.Providers
{
    public static class Extensions
    {
        public static async Task<UploadFileModel> ToFileModel(this IFormFile formFile)
        {
            var model = new UploadFileModel()
            {
                FileName = formFile.FileName,
                FileType = formFile.ContentType,
            };

            using (var stream = formFile.OpenReadStream())
            {
                var bytes = new byte[stream.Length];
                await stream.ReadAsync(bytes, 0, bytes.Length);

                model.FileContent = bytes;
            }
            return model;
        }

        /// <summary>
        /// Get description for enum type
        /// </summary>
        /// <param name="enum">Enum value</param>
        /// <returns>Description</returns>
        public static string GetDescription(this Enum @enum)
        {
            var enumType = @enum.GetType();
            var memberInfo = enumType.GetMember(@enum.ToString());
            if (memberInfo.Length > 0)
            {
                var desAttr = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
                return desAttr?.Description ?? @enum.ToString();
            }
            return @enum.ToString();
        }
    }
}