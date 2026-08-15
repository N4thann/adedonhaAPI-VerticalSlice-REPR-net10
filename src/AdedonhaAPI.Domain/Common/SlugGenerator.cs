using System.Globalization;
using System.Text;

namespace AdedonhaAPI.Domain.Common
{
    /// <summary>
    /// Gera slugs e letras iniciais normalizadas (sem acento) a partir do nome de uma entidade.
    /// </summary>
    public static class SlugGenerator
    {
        /// <summary>
        /// Converte o texto em slug: minusculas, sem acento, espacos viram hifen.
        /// </summary>
        public static string Generate(string text)
        {
            var normalized = RemoveDiacritics(text.Trim()).ToLowerInvariant();
            var withHyphens = string.Join('-', normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries));
            return withHyphens;
        }

        /// <summary>
        /// Retorna a primeira letra do texto, maiuscula e sem acento.
        /// </summary>
        public static char GetInitialLetter(string text)
        {
            var normalized = RemoveDiacritics(text.Trim());
            return char.ToUpperInvariant(normalized[0]);
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            return builder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
