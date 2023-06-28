using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace TechChallenge.Ui.Razor.Pages
{
    public class ProductsModel : PageModel
    {
        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public decimal Price { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var url = "https://localhost:7111/api/v1/products"; // Substitua pela URL da sua API
            var dados = new { name = Name, description = Description, price = Price }; // Substitua pelos dados que deseja enviar na requisição

            // Converter os dados para JSON
            var json = JsonConvert.SerializeObject(dados);
            var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            // Enviar a requisição POST
            var resposta = await httpClient.PostAsync(url, conteudo);

            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                // A requisição falhou, trate o erro de acordo com suas necessidades
                // Você pode acessar a mensagem de erro através de resposta.ReasonPhrase
                ModelState.AddModelError(string.Empty, "Erro ao chamar a API.");
                return Page();
            }
        }
    }
}
public class Product
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
}