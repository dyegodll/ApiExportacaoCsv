using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace ApiExportacaoCsv.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExportController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("users-csv")]
    public async Task<IActionResult> DownloadUsersCsv()
    {
        // 1. Criar o cliente HTTP
        var client = _httpClientFactory.CreateClient();

        // 2. Buscar dados da API Externa (JSONPlaceholder)
        var url = "https://jsonplaceholder.typicode.com/users";
        var users = await client.GetFromJsonAsync<List<User>>(url);

        if (users == null || !users.Any())
        {
            return NotFound("Nenhum dado encontrado na API externa.");
        }

        // 3. Converter a lista de objetos para formato CSV (String)
        var csvContent = GenerateCsv(users);

        // 4. Transformar em bytes para download
        var fileBytes = Encoding.UTF8.GetBytes(csvContent);
        var fileName = $"usuarios_{DateTime.Now:yyyyMMdd_HHmm}.csv";

        // Retorna o arquivo (MIME type text/csv)
        return File(fileBytes, "text/csv", fileName);
    }

    // Método auxiliar simples para gerar o texto CSV
    private string GenerateCsv(List<User> users)
    {
        var builder = new StringBuilder();

        // Cabeçalho do CSV
        builder.AppendLine("Id,Nome,Email,Telefone");

        // Linhas de dados
        foreach (var user in users)
        {
            // Nota: Em cenários reais complexos, use bibliotecas como CsvHelper 
            // para tratar vírgulas dentro do texto. Aqui faremos manual para simplicidade.
            var line = $"{user.Id},{EscapeCsv(user.Name)},{EscapeCsv(user.Email)},{EscapeCsv(user.Phone)}";
            builder.AppendLine(line);
        }

        return builder.ToString();
    }

    // Função simples para evitar que vírgulas no texto quebrem o CSV
    private string EscapeCsv(string field)
    {
        if (field.Contains(","))
        {
            return $"\"{field}\""; // Envolve em aspas se tiver vírgula
        }
        return field;
    }
}