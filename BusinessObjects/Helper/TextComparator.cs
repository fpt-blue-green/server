using System.Text;
using System.Globalization;

namespace BusinessObjects.Helper
{
  public static class TextComparator
  {
    public static bool ContainsIgnoreCaseAndDiacritics(string str1, string str2)
    {
      if (str1 == null || str2 == null)
        return false;

      // Chuẩn hóa hai chuỗi
      string normalizedStr1 = RemoveDiacritics(str1);
      string normalizedStr2 = RemoveDiacritics(str2);

      // Kiểm tra contains
      return normalizedStr1.Contains(normalizedStr2, StringComparison.OrdinalIgnoreCase);
    }

    private static string RemoveDiacritics(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
        return string.Empty;

      // Loại bỏ dấu
      string normalizedText = text.Normalize(NormalizationForm.FormD);
      char[] chars = normalizedText
          .ToCharArray()
          .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
          .ToArray();

      return new string(chars).Normalize(NormalizationForm.FormC);
    }
  }
}