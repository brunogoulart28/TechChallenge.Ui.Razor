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

        [BindProperty]
        public IFormFile FileInput { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var httpClient = new HttpClient();
            var url = "https://localhost:7111/api/v1/products";
            var dados = new { name = Name, description = Description, price = Price };

            // Converter os dados para JSON
            var json = JsonConvert.SerializeObject(dados);
            var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
            
            var resposta = await httpClient.PostAsync(url, conteudo);

            if (!resposta.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Erro ao chamar a API.");
                return Page();
            }

            var apiUrl = "https://localhost:7111/api/v1/images/upload";
            using (var form = new MultipartFormDataContent())
            {
                using (var fileStream = FileInput.OpenReadStream())
                {
                    form.Add(new StringContent("6a82fa49-c820-49a5-8055-fc8a66b78ab1"), "ProductId");
                    form.Add(new StreamContent(fileStream), "file", FileInput.FileName);

                    using (var response = await httpClient.PostAsync(apiUrl, form))
                    {
                        // Trate a resposta da chamada REST como desejado
                        if (response.IsSuccessStatusCode)
                        {
                            return RedirectToPage("/Index");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Erro ao chamar a API.");
                            return Page();
                        }
                    }
                }
            }
        }
    }
}