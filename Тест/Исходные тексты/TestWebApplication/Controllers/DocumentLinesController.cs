using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Models;
using TestWebApplication.Models.ViewModels;

namespace TestWebApplication.Controllers
{
    [Route("documentLines")]
    public class DocumentLinesController : Controller
    {
        // Поле для хранения контекста базы данных
        private ApplicationContext _context;

        // Конструктор контроллера, принимающий контекст базы данных в качестве параметра
        public DocumentLinesController(ApplicationContext context)
        {
            _context = context;
        }

        // Метод для отображения списка всех строк документов
        public IActionResult Index()
        {
            try
            {
                // Получаем список всех строк документов из базы данных
                List<document_line> document_lines = _context.document_line.ToList();

                // Для каждой строки документа находим соответствующий заголовок документа и справочник товара
                foreach (var document_line in document_lines)
                {
                    // Заголовок документа
                    document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_line.document_header_id);
                    document_line.document_header = document_header;

                    // Справочник товара
                    product product = _context.product.FirstOrDefault(p => p.id == document_line.product_id);
                    document_line.product = product;
                }

                // Возвращаем представление с переданным списком строк документов
                return View("Index", document_lines);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для чтения информации о конкретной строке документа по её id
        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                // Находим строку документа по id
                document_line document_line = _context.document_line.FirstOrDefault(dl => dl.id == id);
                // Находим заголовок документа соответствующий строке документа
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_line.document_header_id);
                // Находим справочник товара соответствующий строке документа
                product product = _context.product.FirstOrDefault(p => p.id == document_line.product_id);
                document_line.document_header = document_header;
                document_line.product = product;

                // Возвращаем представление с информацией о строке документа
                return View("Read", document_line);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы создания новой строки документа
        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                // Получаем список всех заголовков документов и справочников товаров из базы данных
                List<document_header> document_headers = _context.document_header.ToList();
                List<product> products = _context.product.ToList();
                // Создаем временную модель с дополнительными полями "заголовки документов ", "справочники товаров"
                document_line_view_model document_line = new()
                {
                    document_line = new(),
                    document_headers = document_headers,
                    products = products
                };

                // Возвращаем представление формы создания строки документа
                return View("Create", document_line);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для создания новой строки документа
        [HttpPost("create")]
        public IActionResult Create(document_line document_line)
        {
            try
            {
                // Проверяем корректность данных строки документа
                if (document_line == null)
                {
                    return BadRequest("Incorrect document line data provided");
                }

                // Добавляем строку документа в базу данных
                _context.document_line.Add(document_line);
                _context.SaveChanges();

                // Вычисляем сумму строки документа
                decimal amount = CalculateDocumentAmount(document_line);

                // Обновляем сумму заголовка документа
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_line.document_header_id);
                document_header.document_amount += amount;

                _context.document_header.Update(document_header);
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

        // Метод для отображения формы редактирования строки документа по id
        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                // Находим строку документа по id
                document_line document_line = _context.document_line.FirstOrDefault(dl => dl.id == id);
                // Находим заголовок документа соответствующий строке документа
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_line.document_header_id);
                document_line.document_header = document_header;
                // Находим справочник товара соответствующий строке документа
                product product = _context.product.FirstOrDefault(p => p.id == document_line.product_id);
                document_line.product = product;

                // Получаем список всех заголовков документов и продуктов из базы данных
                List<document_header> document_headers = _context.document_header.ToList();
                List<product> products = _context.product.ToList();
                // Создаем временную модель с дополнительными полями "заголовки документов ", "справочники товаров"
                document_line_view_model document_line_view_model = new()
                {
                    document_line = document_line,
                    document_headers = document_headers,
                    products = products
                };

                // Возвращаем представление формы редактирования строки документа
                return View("Update", document_line_view_model);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обновления строки документа
        [HttpPost("update/{id}")]
        public IActionResult Update(document_line_view_model document_line_data)
        {
            try
            {
                // Проверяем корректность данных строки документа
                if (document_line_data.document_line == null)
                {
                    return BadRequest("Incorrect document line data provided");
                }

                // Находим строку документа по id
                document_line document_line = _context.document_line.FirstOrDefault(dl => dl.id == document_line_data.document_line.id);

                // Вычисляем сумму строки документа до обновления
                decimal amount = CalculateDocumentAmount(document_line);

                // Обновляем данные строки документа
                document_line.document_header_id = document_line_data.document_line.document_header_id;
                document_line.product_id = document_line_data.document_line.product_id;
                document_line.quantity = document_line_data.document_line.quantity;
                document_line.reserved_quantity = document_line_data.document_line.reserved_quantity;
                document_line.price = document_line_data.document_line.price;
                document_line.discount = document_line_data.document_line.discount;

                _context.document_line.Update(document_line);
                _context.SaveChanges();

                // Находим заголовок документа
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_line.document_header_id);

                // Перерасчет суммы документа
                UpdateDocumentAmount(_context, document_header, document_line, amount);

                // Перенаправляем на метод Index
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 с сообщением об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для удаления строки документа по id
        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Находим строку документа по id
                document_line document_line = _context.document_line.FirstOrDefault(dl => dl.id == id);

                // Удаляем строку документа из базы данных
                _context.document_line.Remove(document_line);
                _context.SaveChanges();

                // Вычисляем сумму строки документа
                decimal amount = CalculateDocumentAmount(document_line);

                // Обновляем сумму заголовка документа
                document_header document_header = _context.document_header.FirstOrDefault(dh => dh.id == document_line.document_header_id);
                document_header.document_amount -= amount;

                _context.document_header.Update(document_header);
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

        // Метод для вычисления суммы документа
        public static decimal CalculateDocumentAmount(document_line document_line)
        {
            decimal price = document_line.price - (document_line.price * (document_line.discount / 100.0m));
            decimal amount = document_line.quantity * price;

            return amount;
        }

        // Метод для обновления суммы заголовка документа
        public static void UpdateDocumentAmount(ApplicationContext _context, document_header document_header, document_line document_line, decimal amount)
        {
            // Вычитаем сумму строки документа до обновления из суммы заголовка документа
            document_header.document_amount -= amount;

            // Вычисляем новую сумму строки документа
            amount = CalculateDocumentAmount(document_line);

            // Добавляем новую сумму строки документа к сумме заголовка документа
            document_header.document_amount += amount;

            _context.document_header.Update(document_header);
            _context.SaveChanges();
        }
    }
}
