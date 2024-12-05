using Microsoft.AspNetCore.Mvc;
using TestWebApplication.Models;

namespace TestWebApplication.Controllers
{
    [Route("counterparties")]
    public class CounterpartiesController : Controller
    {
        // Поле для хранения контекста базы данных
        private ApplicationContext _context;

        // Конструктор контроллера, принимающий контекст базы данных в качестве параметра
        public CounterpartiesController(ApplicationContext context)
        {
            _context = context;
        }

        // Метод для отображения списка справочников контрагентов
        public IActionResult Index()
        {
            try
            {
                // Получаем список всех справочников контрагентов из базы данных
                List<counterparty> counterparties = _context.counterparty.ToList();

                // Возвращаем представление "Index" с полученным списком справочников контрагентов
                return View("Index", counterparties);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения информации о справочнике контрагента по его идентификатору
        [HttpGet("read/{id}")]
        public IActionResult Read(int id)
        {
            try
            {
                // Ищем справочник контрагента по его идентификатору
                counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == id);

                // Возвращаем представление "Read" с найденным справочником контрагента
                return View("Read", counterparty);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы создания нового справочника контрагента
        [HttpGet("create")]
        public IActionResult Create()
        {
            try
            {
                // Создаем новый объект справочника контрагента
                counterparty counterparty = new();
                // Возвращаем представление "Create" с созданным объектом справочника контрагента
                return View("Create", counterparty);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обработки POST-запроса на создание нового справочника контрагента
        [HttpPost("create")]
        public IActionResult Create(counterparty counterparty)
        {
            try
            {
                // Проверяем, передан ли корректный объект справочника контрагента
                if (counterparty == null)
                {
                    return BadRequest("Incorrect counterparty data provided");
                }

                // Добавляем новый справочник контрагента в базу данных
                _context.counterparty.Add(counterparty);
                // Сохраняем изменения
                _context.SaveChanges();

                // Перенаправляем на метод Index для отображения списка справочников контрагентов
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для отображения формы редактирования справочника контрагента по его идентификатору
        [HttpGet("update/{id}")]
        public IActionResult Update(int id)
        {
            try
            {
                // Ищем справочник контрагента по его идентификатору
                counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == id);

                // Возвращаем представление "Update" с найденным справочником контрагента
                return View("Update", counterparty);
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для обработки POST-запроса на обновление справочника контрагента
        [HttpPost("update/{id}")]
        public IActionResult Update(counterparty counterparty_data)
        {
            try
            {
                // Проверяем, передан ли корректный объект справочника контрагента
                if (counterparty_data == null)
                {
                    return BadRequest("Incorrect counterparty data provided");
                }

                // Ищем справочник контрагента по его идентификатору
                counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == counterparty_data.id);

                // Обновляем свойства справочника контрагента
                counterparty.first_name = counterparty_data.first_name;
                counterparty.last_name = counterparty_data.last_name;
                counterparty.email = counterparty_data.email;
                counterparty.phone = counterparty_data.phone;

                // Обновляем справочник контрагента в базе данных
                _context.counterparty.Update(counterparty);
                // Сохраняем изменения
                _context.SaveChanges();

                // Перенаправляем на метод Index для отображения списка справочников контрагентов
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем статус 500 и сообщение об ошибке
                return StatusCode(500, string.Format("Internal server error: {0}", ex));
            }
        }

        // Метод для удаления справочника контрагента по его идентификатору
        [HttpPost("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                // Ищем справочник контрагента по его идентификатору
                counterparty counterparty = _context.counterparty.FirstOrDefault(c => c.id == id);

                // Получаем все заголовки документов из базы данных
                List<document_header> document_headers = _context.document_header.ToList();
                // Получаем все строки документов из базы данных
                List<document_line> document_lines = _context.document_line.ToList();

                // Удаляем все строки документов и заголовки документов, связанные с данным справочником контрагента
                foreach (var document_header in document_headers)
                {
                    if (document_header.counterparty_id == id)
                    {
                        foreach (var document_line in document_lines)
                        {
                            if (document_line.document_header_id == document_header.id)
                            {
                                _context.document_line.Remove(document_line);
                                _context.SaveChanges();
                            }
                        }

                        _context.document_header.Remove(document_header);
                        _context.SaveChanges();
                    }
                }

                // Удаляем справочник контрагента из базы данных
                _context.counterparty.Remove(counterparty);
                // Сохраняем изменения
                _context.SaveChanges();

                // Перенаправляем на метод Index для отображения списка справочников контрагентов
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

