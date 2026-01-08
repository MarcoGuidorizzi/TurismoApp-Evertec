using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurismoApp.Data;
using TurismoApp.Models;

namespace TurismoApp.Controllers
{
    public class PontosTuristicosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PontosTuristicosController> _logger;

        public PontosTuristicosController(AppDbContext context, ILogger<PontosTuristicosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string termoBusca, int pagina = 1)
        {
            const int pageSize = 2;

            var query = _context.PontosTuristicos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(termoBusca))
            {
                termoBusca = termoBusca.Trim();
                query = query.Where(p => p.Nome.Contains(termoBusca) || p.Descricao.Contains(termoBusca) || p.Estado.Contains(termoBusca) || p.Cidade.Contains(termoBusca));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            var items = await query
                .OrderBy(p => p.Nome)
                .Skip((pagina - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.TermoBusca = termoBusca;
            ViewBag.PaginaAtual = pagina;
            ViewBag.TotalPaginas = totalPages;

            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Estado,Cidade,Descricao")] PontoTuristico pontoTuristico)
        {
            if (string.IsNullOrWhiteSpace(pontoTuristico.Estado))
            {
                var formEstado = Request.Form["Estado"].ToString();
                if (string.IsNullOrWhiteSpace(formEstado))
                {
                    formEstado = Request.Form["Estado_value"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(formEstado))
                {
                    pontoTuristico.Estado = formEstado;
                }
            }

            if (string.IsNullOrWhiteSpace(pontoTuristico.Cidade))
            {
                var formCidade = Request.Form["Cidade"].ToString();
                if (string.IsNullOrWhiteSpace(formCidade))
                {
                    formCidade = Request.Form["Cidade_value"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(formCidade))
                {
                    pontoTuristico.Cidade = formCidade;
                }
            }

            // Validação manual dos campos obrigatórios e comprimento da descrição
            var errorsList = new List<string>();
            if (string.IsNullOrWhiteSpace(pontoTuristico.Nome)) errorsList.Add("O nome é obrigatório");
            if (string.IsNullOrWhiteSpace(pontoTuristico.Estado)) errorsList.Add("O estado é obrigatório");
            if (string.IsNullOrWhiteSpace(pontoTuristico.Cidade)) errorsList.Add("A cidade é obrigatória");
            if (!string.IsNullOrEmpty(pontoTuristico.Descricao) && pontoTuristico.Descricao.Length > 100) errorsList.Add("A descrição deve ter no máximo 100 caracteres");
            if (errorsList.Count > 0)
            {
                foreach (var e in errorsList) ModelState.AddModelError(string.Empty, e);
                _logger?.LogWarning("Validação manual falhou: {Errors}", string.Join("; ", errorsList));
                return View(pontoTuristico);
            }

            pontoTuristico.DataInclusao = DateTime.Now;

            try
            {
                _context.Add(pontoTuristico);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao salvar ponto turístico");
                ModelState.AddModelError(string.Empty, "Erro ao salvar no banco de dados.");
                return View(pontoTuristico);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var pontoTuristico = await _context.PontosTuristicos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pontoTuristico == null) return NotFound();

            return View(pontoTuristico);
        }
    }
}