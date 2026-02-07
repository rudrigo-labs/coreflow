using BCrypt.Net;

namespace CoreFlow.Api.Utils;

/// <summary>
/// Utilitário para gerar hashes bcrypt de ClientSecrets.
/// Use este utilitário para gerar os hashes que serão armazenados na configuração.
/// </summary>
public static class BcryptHashGenerator
{
    /// <summary>
    /// Gera um hash bcrypt para um ClientSecret.
    /// </summary>
    /// <param name="clientSecret">O ClientSecret em texto plano.</param>
    /// <param name="workFactor">Fator de trabalho (padrão: 11). Valores maiores são mais seguros mas mais lentos.</param>
    /// <returns>Hash bcrypt do ClientSecret.</returns>
    public static string GenerateHash(string clientSecret, int workFactor = 11)
    {
        if (string.IsNullOrWhiteSpace(clientSecret))
        {
            throw new ArgumentException("ClientSecret não pode ser vazio.", nameof(clientSecret));
        }

        // BCrypt.HashPassword gera um hash com salt automático
        return BCrypt.Net.BCrypt.HashPassword(clientSecret, workFactor);
    }

    /// <summary>
    /// Valida se um ClientSecret corresponde a um hash bcrypt.
    /// </summary>
    /// <param name="clientSecret">O ClientSecret em texto plano.</param>
    /// <param name="hash">O hash bcrypt armazenado.</param>
    /// <returns>True se o ClientSecret corresponde ao hash.</returns>
    public static bool Verify(string clientSecret, string hash)
    {
        if (string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(clientSecret, hash);
        }
        catch
        {
            return false;
        }
    }
}

