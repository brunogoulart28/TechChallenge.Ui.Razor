using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Configuration;
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
        private readonly IConfiguration _configuration;
        public ProductsModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            string apiUrl = _configuration.GetSection("ApiSettings:ApiUrl").Value;

            var httpClient = new HttpClient();
            var url = apiUrl + "/products";
            var dados = new { name = Name, description = Description, price = Price };

            // Converter os dados para JSON
            var json = JsonConvert.SerializeObject(dados);
            var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
            
            var resposta = await httpClient.PostAsync(url, conteudo);

            if (!resposta.IsSuccessStatusCode)
            {
                TempData["ErroMensagem"] = resposta.RequestMessage.ToString();
                return Page() ;
            }

            if (FileInput == null)
            {
                TempData["SucessoMensagem"] = "Gravação realizada com sucesso!";
                return RedirectToPage("/Index");
            }

            var UrlImage = apiUrl + "/images/upload";
            using (var form = new MultipartFormDataContent())
            {
                using (var fileStream = FileInput.OpenReadStream())
                {
                    form.Add(new StringContent("6a82fa49-c820-49a5-8055-fc8a66b78ab1"), "ProductId");
                    form.Add(new StreamContent(fileStream), "file", FileInput.FileName);

                    using (var response = await httpClient.PostAsync(UrlImage, form))
                    {
                        // Trate a resposta da chamada REST como desejado
                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SucessoMensagem"] = "Gravação realizada com sucesso!";
                            return RedirectToPage("/Index");
                        }
                        else
                        {
                            TempData["ErroMensagem"] = response.RequestMessage.ToString();
                            return Page();
                        }
                    }
                }
            }
        }
    }
}