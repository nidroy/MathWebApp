using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Models;

namespace TestWebApplication.Controllers
{
    [Route("products")]
    public class ProductsController : Controller
    {
        // Поле для хранения контекста базы данных
        private ApplicationContext _context;

        // Конструктор контроллера, принимающий контекст базы данных в качестве параметра
        public ProductsController(ApplicationContext context)
        {
            _context = context;
        }

        // Метод для отображения списка справочников товаров
        public IActionResult Index()
        {
            try
            {
                // Получаем список всех справочников товаров из базы данных
                List<product> products = _context.product.ToList();

                // Возвращаем представление "Index" с полученным списком справочников товаров
                return View("Index", products);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения информации о справочнике товара по его идентификатору
        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                // Ищем справочник товара по его идентификатору
                product product = _context.product.FirstOrDefault(p => p.id == id);

                // Возвращаем представление "Read" с найденным справочником товара
                return View("Read", product);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы создания нового справочника товара
        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                // Создаем новый объект справочника товара
                product product = new();
                // Возвращаем представление "Create" с созданным объектом справочника товара
                return View("Create", product);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обработки POST-запроса на создание нового справочника товара
        [HttpPost("create")]
        public IActionResult Create(product product)
        {
            try
            {
                // Проверяем, передан ли корректный объект справочника товара
                if (product == null)
                {
                    return BadRequest("Incorrect product data provided");
                }

                // Добавляем новый справочник товара в базу данных
                _context.product.Add(product);
                // Сохраняем изменения
                _context.SaveChanges();

                // Перенаправляем на метод Index для отображения списка справочников товаров
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы редактирования справочника товара по его идентификатору
        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                // Ищем справочник товара по его идентификатору
                product product = _context.product.FirstOrDefault(p => p.id == id);

                // Возвращаем представление "Update" с найденным справочником товара
                return View("Update", product);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обработки POST-запроса на обновление справочника товара
        [HttpPost("update/{id}")]
        public IActionResult Update(product product_data)
        {
            try
            {
                // Проверяем, передан ли корректный объект справочника товара
                if (product_data == null)
                {
                    return BadRequest("Incorrect product data provided");
                }

                // Ищем справочник товара по его идентификатору
                product product = _context.product.FirstOrDefault(p => p.id == product_data.id);

                // Обновляем свойства справочника товара
                product.name = product_data.name;
                product.price = product_data.price;

                // Обновляем справочник товара в базе данных
                _context.product.Update(product);
                // Сохраняем изменения
                _context.SaveChanges();

                // Перенаправляем на метод Index для отображения списка справочников товаров
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для удаления справочника товара по его идентификатору
        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Ищем справочник товара по его идентификатору
                product product = _context.product.FirstOrDefault(p => p.id == id);

                // Получаем все строки документа из базы данных
                List<document_line> document_lines = _context.document_line.ToList();

                // Удаляем все строки документа, связанные с данным справочником товара
                foreach (var document_line in document_lines)
                {
                    if (document_line.product_id == id)
                    {
                        _context.document_line.Remove(document_line);
                        _context.SaveChanges();
                    }
                }

                // Получаем все записи об остатках товара на складе из базы данных
                List<product_stock> product_stocks = _context.product_stock.ToList();

                // Удаляем все записи об остатках товара на складе, связанные с данным справочником товара
                foreach (var product_stock in product_stocks)
                {
                    if (product_stock.product_id == id)
                    {
                        _context.product_stock.Remove(product_stock);
                        _context.SaveChanges();
                    }
                }

                // Удаляем справочник товара из базы данных
                _context.product.Remove(product);
                // Сохраняем изменения
                _context.SaveChanges();

                // Перенаправляем на метод Index для отображения списка справочников товаров
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }
    }
}

