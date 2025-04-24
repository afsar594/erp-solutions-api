using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Helpers
{
    public static class ImageHelper
    {
        public static string? ByteArrayToBase64Image(byte[]? imgBytes)
        {
            if (imgBytes == null || imgBytes.Length == 0)
                return null;

            return Convert.ToBase64String(imgBytes);
        }

        public static byte[]? Base64ImageToByteArray(string? imgBase64)
        {
            if (string.IsNullOrWhiteSpace(imgBase64))
                return null;

            try
            {
                return Convert.FromBase64String(imgBase64);
            }
            catch (FormatException)
            {
                return null;
            }
        }

    }
}
