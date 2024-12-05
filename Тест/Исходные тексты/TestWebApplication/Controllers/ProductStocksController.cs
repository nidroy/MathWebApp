using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Route("productStocks")]
    public class ProductStocksController : Controller
    {
        // Поле для хранения контекста базы данных
        private ApplicationContext _context;

        // Конструктор контроллера, принимающий контекст базы данных в качестве параметра
        public ProductStocksController(ApplicationContext context)
        {
            _context = context;
        }

        // Метод для отображения списка всех складских остатков
        public IActionResult Index()
        {
            try
            {
                // Получаем список всех складских остатков из базы данных
                List<product_stock> product_stocks = _context.product_stock.ToList();

                // Для каждого складского остатка находим соответствующий справочник товара 
                foreach (var product_stock in product_stocks)
                {
                    product product = _context.product.FirstOrDefault(p => p.id == product_stock.product_id);
                    product_stock.product = product;
                }

                // Возвращаем представление с переданным списком складских остатков
                return View("Index", product_stocks);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для чтения информации о конкретном складском остатке по его id
        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                // Находим складской остаток по id
                product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.id == id);
                // Находим справочник товара соответствующий складскому остатку
                product product = _context.product.FirstOrDefault(p => p.id == product_stock.product_id);
                product_stock.product = product;

                // Возвращаем представление с информацией о складском остатку
                return View("Read", product_stock);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы создания нового складского остатка
        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                // Получаем список всех справочников товаров из базы данных
                List<product> products = _context.product.ToList();
                // Создаем временную модель с дополнительными полями "справочники товаров"
                product_stock_view_model product_stock = new()
                {
                    product_stock = new(),
                    products = products
                };

                // Возвращаем представление формы создания складского остатка
                return View("Create", product_stock);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для создания нового складского остатка
        [HttpPost("create")]
        public IActionResult Create(product_stock product_stock)
        {
            try
            {
                // Проверяем корректность данных складского остатка
                if (product_stock == null)
                {
                    return BadRequest("Incorrect product stock data provided");
                }

                // Проверяем, существует ли уже складской остаток для данного справочника товара
                product_stock current_product_stock = _context.product_stock.FirstOrDefault(ps => ps.product_id == product_stock.product_id);

                if (current_product_stock != null)
                {
                    return BadRequest("Incorrect product_id data provided");
                }

                // Добавляем складской остаток в базу данных
                _context.product_stock.Add(product_stock);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы редактирования складского остатка по id
        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                // Находим складской остаток по id
                product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.id == id);
                // Находим справочник товара соответствующий складскому остатку
                product product = _context.product.FirstOrDefault(p => p.id == product_stock.product_id);
                product_stock.product = product;

                // Получаем список всех справочников товаров из базы данных
                List<product> products = _context.product.ToList();
                // Создаем временную модель с дополнительными полями "справочники товаров"
                product_stock_view_model product_stock_view_model = new()
                {
                    product_stock = product_stock,
                    products = products
                };

                // Возвращаем представление формы редактирования складского остатка
                return View("Update", product_stock_view_model);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обновления складского остатка
        [HttpPost("update/{id}")]
        public IActionResult Update(product_stock_view_model product_stock_data)
        {
            try
            {
                // Проверяем корректность данных складского остатка
                if (product_stock_data.product_stock == null)
                {
                    return BadRequest("Incorrect product stock data provided");
                }

                // Находим складской остаток по id
                product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.id == product_stock_data.product_stock.id);

                // Обновляем данные складского остатка
                product_stock.product_id = product_stock_data.product_stock.product_id;
                product_stock.actual_quantity = product_stock_data.product_stock.actual_quantity;
                product_stock.reserved_quantity = product_stock_data.product_stock.reserved_quantity;

                _context.product_stock.Update(product_stock);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для удаления складского остатка по id
        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Находим складской остаток по id
                product_stock product_stock = _context.product_stock.FirstOrDefault(ps => ps.id == id);

                // Удаляем складской остаток из базы данных
                _context.product_stock.Remove(product_stock);
                _context.SaveChanges();

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }
    }
}
