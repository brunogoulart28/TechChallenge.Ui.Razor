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
            var stream = new MemoryStream();
            FileInput.CopyTo(stream);
            byte[] bytes2 = stream.ToArray();

            var url = "https://localhost:7111/api/v1/products";
            var dados = new { name = Name, description = Description, price = Price };

            // Converter os dados para JSON
            var json = JsonConvert.SerializeObject(dados);
            var conteudo = new StringContent(json, Encoding.UTF8, "application/json");
            var httpClient = new HttpClient();
            // Enviar a requisição POST
            var resposta = await httpClient.PostAsync(url, conteudo);

            if (!resposta.IsSuccessStatusCode)
            {
                // A requisição falhou, trate o erro de acordo com suas necessidades
                // Você pode acessar a mensagem de erro através de resposta.ReasonPhrase
                ModelState.AddModelError(string.Empty, "Erro ao chamar a API.");
                return Page();
            }

            var ultImputImage = "https://localhost:7111/api/v1/images/upload";

            var image = new { ProductId = "6a82fa49-c820-49a5-8055-fc8a66b78ab1", File = FileInput.OpenReadStream() };
            var jsonImage = JsonConvert.SerializeObject(image);
            var conteudoImage = new StringContent(jsonImage, Encoding.UTF8, "application/json");

            var respostaImage = await httpClient.PostAsync(ultImputImage, conteudoImage);


            return RedirectToPage("/Index");
        }
        private byte[] SerializeFile(IFormFile arquivo)
        {
            using (var memoryStream = new MemoryStream())
            {
                arquivo.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}